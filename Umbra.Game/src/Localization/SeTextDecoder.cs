using Dalamud.Game;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Umbra.Common;
using Umbra.Common.Extensions;
using LuminaSeString = Lumina.Text.SeString;

namespace Umbra.Game.Localization;

/// <summary>
/// Courtesy of Haselnussbomber.
/// https://github.com/Haselnussbomber/TextDecoderExample/blob/master/SamplePlugin/TextDecoder.cs
/// </summary>
[Service]
public unsafe class TextDecoder(IClientState clientState, IDataManager dataManager)
{
    private readonly ReadOnlySeString Empty = new();
    private readonly Dictionary<(ClientLanguage Language, string SheetName, int RowId, int Amount, int Person, int Case), ReadOnlySeString> Cache = [];

    private const int SingularColumnIdx = 0;
    private const int AdjectiveColumnIdx = 1;
    private const int PluralColumnIdx = 2;
    private const int PossessivePronounColumnIdx = 3;
    private const int StartsWithVowelColumnIdx = 4;
    private const int Unknown5ColumnIdx = 5;
    private const int PronounColumnIdx = 6;
    private const int ArticleColumnIdx = 7;

    public string ProcessNoun(string sheetName, uint rowId)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ProcessNoun(clientState.ClientLanguage, sheetName, 5, (int)rowId).ExtractText());
    }

    // <XXnoun(SheetName,Person,RowId,Amount,Case[,UnkInt5])>
    // UnkInt5 seems unused in En/Fr/De/Ja, so it's ignored for now
    public ReadOnlySeString ProcessNoun(ClientLanguage Language, string SheetName, int Person, int RowId, int Amount = 1, int Case = 1, int UnkInt5 = 1)
    {
        Case--;

        if (Case > 5 || (Language != ClientLanguage.German && Case != 0))
            return Empty;

        var key = (Language, SheetName, RowId, Amount, Person, Case);
        if (Cache.TryGetValue(key, out var value))
            return value;

        var attributiveSheet = dataManager.GameData.Excel.GetSheetRaw("Attributive", Language.ToLumina());
        if (attributiveSheet == null) {
            Logger.Warning("Sheet Attributive not found");
            return Empty;
        }

        var sheet = dataManager.GameData.Excel.GetSheetRaw(SheetName, Language.ToLumina());
        if (sheet == null) {
            Logger.Warning($"Sheet {SheetName} not found");
            return Empty;
        }

        var row = sheet.GetRow((uint)RowId);
        if (row == null) {
            Logger.Warning($"Sheet {SheetName} does not contain row #{RowId}");
            return Empty;
        }

        // see "E8 ?? ?? ?? ?? 44 8B 6B 08"
        var columnOffset = SheetName switch {
            "BeastTribe" => 10,
            "DeepDungeonItem" or "DeepDungeonEquipment" or "DeepDungeonMagicStone" or "DeepDungeonDemiclone" => 1,
            "Glasses" => 4,
            "GlassesStyle" => 15,
            "Ornament" => 8, // not part of that function, but still shifted
            _ => 0
        };

        var output = Language switch {
            ClientLanguage.Japanese => ResolveNounJa(Amount, Person, attributiveSheet, row),
            ClientLanguage.English => ResolveNounEn(Amount, Person, attributiveSheet, row, columnOffset),
            ClientLanguage.German => ResolveNounDe(Amount, Person, Case, attributiveSheet, row, columnOffset),
            ClientLanguage.French => ResolveNounFr(Amount, Person, attributiveSheet, row, columnOffset),
            _ => new ReadOnlySeString()
        };

        if (output.IsEmpty)
            return Empty;

        Cache.Add(key, output);
        return output;
    }

    // Component::Text::Localize::NounJa.Resolve
    private static ReadOnlySeString ResolveNounJa(int Amount, int Person, RawExcelSheet attributiveSheet, RowParser row)
    {
        var builder = new SeStringBuilder();

        // Ko-So-A-Do
        var ksad = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(Amount > 1 ? 1 : 0);
        if (ksad != null)
            builder.Append(ksad);

        if (Amount > 1)
            builder.ReplaceText("[n]"u8, Encoding.UTF8.GetBytes(Amount.ToString()));

        // UnkInt5 can only be 0, because the offsets array has only 1 entry, which is 0
        var text = row.ReadColumn<LuminaSeString>(0);
        if (text != null)
            builder.Append(text);

        return builder.ToReadOnlySeString();
    }

    // Component::Text::Localize::NounEn.Resolve
    private static ReadOnlySeString ResolveNounEn(int Amount, int Person, RawExcelSheet attributiveSheet, RowParser row, int columnOffset)
    {
        /*
          a1->Offsets[0] = SingularColumnIdx
          a1->Offsets[1] = PluralColumnIdx
          a1->Offsets[2] = StartsWithVowelColumnIdx
          a1->Offsets[3] = PossessivePronounColumnIdx
          a1->Offsets[4] = ArticleColumnIdx
        */

        // UnkInt5 isn't really used here. there are only 5 offsets in the array
        // offsets = &a1->Offsets[5 * a2->UnkInt5];

        var builder = new SeStringBuilder();

        var articleIndex = row.ReadColumn<sbyte>(columnOffset + ArticleColumnIdx);
        if (articleIndex == 0) {
            var v14 = row.ReadColumn<sbyte>(columnOffset + StartsWithVowelColumnIdx);
            var v17 = v14 + 2 * (v14 + 1);
            var article = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(v17 + (Amount == 1 ? 0 : 1));
            if (article != null)
                builder.Append(article);

            // skipping link marker ("//")
        }

        var text = row.ReadColumn<LuminaSeString>(columnOffset + (Amount == 1 ? SingularColumnIdx : PluralColumnIdx));
        if (text != null)
            builder.Append(text);

        builder.ReplaceText("[n]"u8, Encoding.UTF8.GetBytes(Amount.ToString()));

        return builder.ToReadOnlySeString();
    }

    // Component::Text::Localize::NounDe.Resolve
    private static ReadOnlySeString ResolveNounDe(int Amount, int Person, int Case, RawExcelSheet attributiveSheet, RowParser row, int columnOffset)
    {
        /*
             a1->Offsets[0] = SingularColumnIdx
             a1->Offsets[1] = PluralColumnIdx
             a1->Offsets[2] = PronounColumnIdx
             a1->Offsets[3] = AdjectiveColumnIdx
             a1->Offsets[4] = PossessivePronounColumnIdx
             a1->Offsets[5] = Unknown5ColumnIdx
             a1->Offsets[6] = ArticleColumnIdx
         */

        var builder = new SeStringBuilder();

        var readColumnDirectly = ((byte)(Case >> 8 & 0xFF) & 1) == 1; // BYTE2(Case) & 1

        if ((Case & 0x10000) != 0)
            Case = 0;

        // TODO: I didn't try this out yet, see if it works
        if (readColumnDirectly) {
            var v15 = row.ReadColumn<LuminaSeString>(Case - 0x10000);
            if (v15 != null)
                builder.Append(v15);

            builder.ReplaceText("[n]"u8, Encoding.UTF8.GetBytes(Amount.ToString()));

            return builder.ToReadOnlySeString();
        }

        var genderIdx = row.ReadColumn<sbyte>(columnOffset + PronounColumnIdx);
        var articleIndex = row.ReadColumn<sbyte>(columnOffset + ArticleColumnIdx);

        var caseColumnOffset = 4 * Case + 8;
        sbyte v27;
        if (Amount == 1) {
            var v26 = columnOffset + AdjectiveColumnIdx;
            v27 = (sbyte)(v26 >= 0 ? row.ReadColumn<sbyte>(v26) : ~v26);
        } else {
            var v29 = columnOffset + PossessivePronounColumnIdx;
            v27 = (sbyte)(v29 >= 0 ? row.ReadColumn<sbyte>(v29) : ~v29);
            genderIdx = 3;
        }

        var has_t = false; // v44, has article placeholder
        var text = row.ReadColumn<LuminaSeString>(columnOffset + (Amount == 1 ? SingularColumnIdx : PluralColumnIdx));
        if (text != null) {
            has_t = text.RawData.IndexOf("[t]"u8) != -1; // v34

            if (articleIndex == 0 && !has_t) {
                var v36 = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx);
                if (v36 != null)
                    builder.Append(v36);
            }

            // skipping link marker ("//")

            builder.Append(text);

            var v43 = attributiveSheet.GetRow((uint)(v27 + 26))?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx);
            if (v43 != null) {
                var has_p = builder.Contains("[p]"u8); // inverted v38
                if (has_p)
                    builder.ReplaceText("[p]"u8, v43.RawData);
                else
                    builder.Append(v43);
            }

            if (has_t) {
                var v46 = attributiveSheet.GetRow(39)?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx); // Definiter Artikel
                if (v46 != null)
                    builder.ReplaceText("[t]"u8, v46.RawData);
            }
        }

        var v50 = attributiveSheet.GetRow(24)?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx);
        if (v50 != null)
            builder.ReplaceText("[pa]"u8, v50.RawData);

        var v52 = attributiveSheet.GetRow(26); // Starke Flexion eines Artikels?!
        if (Person is 2 or 6 || has_t) // ((Person - 2) & -5) == 0
            v52 = attributiveSheet.GetRow(25); // Schwache Flexion eines Adjektivs?!
        else if (Person == 5)
            v52 = attributiveSheet.GetRow(38); // Starke Deklination
        else if (Person == 1)
            v52 = attributiveSheet.GetRow(37); // Gemischte Deklination

        var v54 = v52?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx);
        if (v54 != null)
            builder.ReplaceText("[a]"u8, v54.RawData);

        builder.ReplaceText("[n]"u8, Encoding.UTF8.GetBytes(Amount.ToString()));

        return builder.ToReadOnlySeString();
    }

    // Component::Text::Localize::NounFr.Resolve
    private static ReadOnlySeString ResolveNounFr(int Amount, int Person, RawExcelSheet attributiveSheet, RowParser row, int columnOffset)
    {
        /*
            a1->Offsets[0] = SingularColumnIdx
            a1->Offsets[1] = PluralColumnIdx
            a1->Offsets[2] = StartsWithVowelColumnIdx
            a1->Offsets[3] = PronounColumnIdx
            a1->Offsets[4] = Unknown5ColumnIdx
            a1->Offsets[5] = ArticleColumnIdx
        */

        // UnkInt5 isn't really used here either. there are only 6 offsets in the array
        // UnkInt5--;
        // offsets = &a1->Offsets[6 * a2->UnkInt5];

        var builder = new SeStringBuilder();

        var v33 = row.ReadColumn<sbyte>(columnOffset + StartsWithVowelColumnIdx);
        var v15 = row.ReadColumn<sbyte>(columnOffset + PronounColumnIdx);
        var v17 = row.ReadColumn<sbyte>(columnOffset + Unknown5ColumnIdx);
        var v19 = row.ReadColumn<sbyte>(columnOffset + ArticleColumnIdx);
        var v20 = 4 * (v33 + 6 + 2 * v15);

        if (v19 != 0) {
            var v21 = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(v20);
            if (v21 != null)
                builder.Append(v21);

            // skipping link marker ("//")

            var v30 = row.ReadColumn<LuminaSeString>(columnOffset + (Amount <= 1 ? SingularColumnIdx : PluralColumnIdx));
            if (v30 != null)
                builder.Append(v30);

            if (Amount <= 1)
                builder.ReplaceText("[n]"u8, Encoding.UTF8.GetBytes(Amount.ToString()));

            return builder.ToReadOnlySeString();
        }

        if (v17 != 0 && (Amount > 1 || v17 == 2)) {
            var v29 = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(v20 + 2);
            if (v29 != null) {
                builder.Append(v29);

                // skipping link marker ("//")

                var v30 = row.ReadColumn<LuminaSeString>(columnOffset + PluralColumnIdx);
                if (v30 != null)
                    builder.Append(v30);
            }
        } else {
            var v27 = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(v20 + (v17 != 0 ? 1 : 3));
            if (v27 != null)
                builder.Append(v27);

            // skipping link marker ("//")

            var v30 = row.ReadColumn<LuminaSeString>(columnOffset + SingularColumnIdx);
            if (v30 != null)
                builder.Append(v30);
        }

        builder.ReplaceText("[n]"u8, Encoding.UTF8.GetBytes(Amount.ToString()));

        return builder.ToReadOnlySeString();
    }
}
