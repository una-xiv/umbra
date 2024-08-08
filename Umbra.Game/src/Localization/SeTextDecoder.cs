using Dalamud.Game;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using Lumina.Excel;
using Lumina.Text.ReadOnly;
using System.Collections.Generic;
using System.Globalization;
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
    private readonly Dictionary<(ClientLanguage Language, string SheetName, int RowId, int Amount, int Person, int Case), string> _cache = [];

    private const int SingularColumnIdx          = 0;
    private const int AdjectiveColumnIdx         = 1;
    private const int PluralColumnIdx            = 2;
    private const int PossessivePronounColumnIdx = 3;
    private const int StartsWithVowelColumnIdx   = 4;
    private const int Unknown5ColumnIdx          = 5;
    private const int PronounColumnIdx           = 6;
    private const int ArticleColumnIdx           = 7;

    public string ProcessNoun(string sheetName, uint rowId)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ProcessNoun(clientState.ClientLanguage, sheetName, 5, (int)rowId));
    }

    // <XXnoun(SheetName,Person,RowId,Amount,Case[,UnkInt5])>
    // UnkInt5 seems unused in En/Fr/De/Ja, so it's ignored for now
    public string ProcessNoun(
        ClientLanguage language, string sheetName, int person, int rowId, int amount = 1, int @case = 1, int unkInt5 = 1
    )
    {
        @case--;

        if (@case > 5 || (language != ClientLanguage.German && @case != 0)) return string.Empty;

        var key = (language, SheetName: sheetName, RowId: rowId, Amount: amount, Person: person, Case: @case);
        if (_cache.TryGetValue(key, out var value)) return value;

        var attributiveSheet = dataManager.GameData.Excel.GetSheetRaw("Attributive", language.ToLumina());

        if (attributiveSheet == null) {
            Logger.Warning("Sheet Attributive not found");
            return string.Empty;
        }

        var sheet = dataManager.GameData.Excel.GetSheetRaw(sheetName, language.ToLumina());

        if (sheet == null) {
            Logger.Warning($"Sheet {sheetName} not found");
            return string.Empty;
        }

        var row = sheet.GetRow((uint)rowId);

        if (row == null) {
            Logger.Warning($"Sheet {sheetName} does not contain row #{rowId}");
            return string.Empty;
        }

        // see "E8 ?? ?? ?? ?? 44 8B 6B 08"
        var columnOffset = sheetName switch {
            "BeastTribe"                                                                                     => 10,
            "DeepDungeonItem" or "DeepDungeonEquipment" or "DeepDungeonMagicStone" or "DeepDungeonDemiclone" => 1,
            "Glasses"                                                                                        => 4,
            "GlassesStyle"                                                                                   => 15,
            _                                                                                                => 0
        };

        var output = language switch {
            ClientLanguage.Japanese => ResolveNounJa(amount, person, attributiveSheet, row),
            ClientLanguage.English  => ResolveNounEn(amount, person, attributiveSheet, row, columnOffset),
            ClientLanguage.German   => ResolveNounDe(amount, person, @case, attributiveSheet, row, columnOffset),
            ClientLanguage.French   => ResolveNounFr(amount, person, attributiveSheet, row, columnOffset),
            _                       => null
        };

        if (output == null) return string.Empty;

        var str = new ReadOnlySeStringSpan(output->AsSpan()).ExtractText();
        _cache.Add(key, str);
        output->Dtor(true);
        return str;
    }

    // Component::Text::Localize::NounJa.Resolve
    private static Utf8String* ResolveNounJa(int Amount, int Person, RawExcelSheet attributiveSheet, RowParser row)
    {
        var output      = Utf8String.CreateEmpty();
        var placeholder = Utf8String.CreateEmpty();
        var temp        = Utf8String.CreateEmpty();

        // Ko-So-A-Do
        var ksad = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(Amount > 1 ? 1 : 0);
        if (ksad != null) output->SetString(ksad.RawData.WithNullTerminator());

        if (Amount > 1) {
            placeholder->SetString("[n]");
            temp->SetString(Amount.ToString());
            output->Replace(placeholder, temp);
        }

        // UnkInt5 can only be 0, because the offsets array has only 1 entry, which is 0
        var text = row.ReadColumn<LuminaSeString>(0);

        if (text != null) {
            temp->SetString(text.RawData.WithNullTerminator());
            output->Append(temp);
        }

        placeholder->Dtor(true);
        temp->Dtor(true);
        return output;
    }

    // Component::Text::Localize::NounEn.Resolve
    private static Utf8String* ResolveNounEn(
        int Amount, int Person, RawExcelSheet attributiveSheet, RowParser row, int columnOffset
    )
    {
        var output      = Utf8String.CreateEmpty();
        var placeholder = Utf8String.CreateEmpty();
        var temp        = Utf8String.CreateEmpty();

        /*
          a1->Offsets[0] = SingularColumnIdx
          a1->Offsets[1] = PluralColumnIdx
          a1->Offsets[2] = StartsWithVowelColumnIdx
          a1->Offsets[3] = PossessivePronounColumnIdx
          a1->Offsets[4] = ArticleColumnIdx
        */

        // UnkInt5 isn't really used here. there are only 5 offsets in the array
        // offsets = &a1->Offsets[5 * a2->UnkInt5];

        var articleIndex = row.ReadColumn<sbyte>(columnOffset + ArticleColumnIdx);

        if (articleIndex == 0) {
            var v14 = row.ReadColumn<sbyte>(columnOffset + StartsWithVowelColumnIdx);
            var v17 = v14 + 2 * (v14 + 1);

            var article = attributiveSheet
                .GetRow((uint)Person)
                ?.ReadColumn<LuminaSeString>(v17 + (Amount == 1 ? 0 : 1));

            if (article != null) output->SetString(article.RawData.WithNullTerminator());

            // skipping link marker ("//")
        }

        var text = row.ReadColumn<LuminaSeString>(columnOffset + (Amount == 1 ? SingularColumnIdx : PluralColumnIdx));

        if (text != null) {
            temp->SetString(text.RawData.WithNullTerminator());
            output->Append(temp);
        }

        placeholder->SetString("[n]");
        temp->SetString(Amount.ToString());
        output->Replace(placeholder, temp);

        placeholder->Dtor(true);
        temp->Dtor(true);
        return output;
    }

    // Component::Text::Localize::NounDe.Resolve
    private Utf8String* ResolveNounDe(
        int Amount, int Person, int Case, RawExcelSheet attributiveSheet, RowParser row, int columnOffset
    )
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

        var readColumnDirectly = ((byte)(Case >> 8 & 0xFF) & 1) == 1; // BYTE2(Case) & 1

        if ((Case & 0x10000) != 0) Case = 0;

        var output      = Utf8String.CreateEmpty();
        var placeholder = Utf8String.CreateEmpty();
        var temp        = Utf8String.CreateEmpty();

        // TODO: I didn't try this out yet, see if it works
        if (readColumnDirectly) {
            var v15 = row.ReadColumn<LuminaSeString>(Case - 0x10000);
            if (v15 != null) output->SetString(v15.RawData.WithNullTerminator());

            placeholder->SetString("[n]");
            temp->SetString(Amount.ToString());
            output->Replace(placeholder, temp);

            placeholder->Dtor(true);
            temp->Dtor(true);
            return output;
        }

        var genderIdx    = row.ReadColumn<sbyte>(columnOffset + PronounColumnIdx);
        var articleIndex = row.ReadColumn<sbyte>(columnOffset + ArticleColumnIdx);

        var   caseColumnOffset = 4 * Case + 8;
        sbyte v27;

        if (Amount == 1) {
            var v26 = columnOffset + AdjectiveColumnIdx;
            v27 = (sbyte)(v26 >= 0 ? row.ReadColumn<sbyte>(v26) : ~v26);
        } else {
            var v29 = columnOffset + PossessivePronounColumnIdx;
            v27       = (sbyte)(v29 >= 0 ? row.ReadColumn<sbyte>(v29) : ~v29);
            genderIdx = 3;
        }

        var has_t = false; // v44, has article placeholder
        var text  = row.ReadColumn<LuminaSeString>(columnOffset + (Amount == 1 ? SingularColumnIdx : PluralColumnIdx));

        if (text != null) {
            placeholder->SetString("[t]");
            temp->SetString(text.RawData.WithNullTerminator());
            has_t = temp->IndexOf(placeholder) != -1; // v34
            output->Clear();

            if (articleIndex == 0 && !has_t) {
                var v36 = attributiveSheet
                    .GetRow((uint)Person)
                    ?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx);

                if (v36 != null) output->SetString(v36.RawData.WithNullTerminator());
            }

            // skipping link marker ("//") (processed in "E8 ?? ?? ?? ?? 41 F6 86 ?? ?? ?? ?? ?? 74 1D")

            temp->SetString(text.RawData.WithNullTerminator());
            output->Append(temp);

            var v43 = attributiveSheet
                .GetRow((uint)(v27 + 26))
                ?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx);

            if (v43 != null) {
                placeholder->SetString("[p]");
                temp->SetString(v43.RawData.WithNullTerminator());
                var has_p = output->IndexOf(placeholder) != -1; // inverted v38

                if (has_p)
                    output->Replace(placeholder, temp);
                else
                    output->Append(temp);
            }

            if (has_t) {
                var v46 = attributiveSheet
                    .GetRow(39)
                    ?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx); // Definiter Artikel

                if (v46 != null) {
                    placeholder->SetString("[t]");
                    temp->SetString(v46.RawData.WithNullTerminator());
                    output->Replace(placeholder, temp);
                }
            }
        }

        var v50 = attributiveSheet.GetRow(24)?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx);

        if (v50 != null) {
            placeholder->SetString("[pa]");
            temp->SetString(v50.RawData.WithNullTerminator());
            output->Replace(placeholder, temp);
        }

        var v52 = attributiveSheet.GetRow(26); // Starke Flexion eines Artikels?!

        if (Person is 2 or 6 || has_t)         // ((Person - 2) & -5) == 0
            v52 = attributiveSheet.GetRow(25); // Schwache Flexion eines Adjektivs?!
        else if (Person == 5)
            v52                   = attributiveSheet.GetRow(38); // Starke Deklination
        else if (Person == 1) v52 = attributiveSheet.GetRow(37); // Gemischte Deklination

        var v54 = v52?.ReadColumn<LuminaSeString>(caseColumnOffset + genderIdx);

        if (v54 != null) {
            placeholder->SetString("[a]");
            temp->SetString(v54.RawData.WithNullTerminator());
            output->Replace(placeholder, temp);
        }

        placeholder->SetString("[n]");
        temp->SetString(Amount.ToString());
        output->Replace(placeholder, temp);

        placeholder->Dtor(true);
        temp->Dtor(true);

        return output;
    }

    // Component::Text::Localize::NounFr.Resolve
    private static Utf8String* ResolveNounFr(
        int Amount, int Person, RawExcelSheet attributiveSheet, RowParser row, int columnOffset
    )
    {
        var output      = Utf8String.CreateEmpty();
        var placeholder = Utf8String.CreateEmpty();
        var temp        = Utf8String.CreateEmpty();

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

        var v33 = row.ReadColumn<sbyte>(columnOffset + StartsWithVowelColumnIdx);
        var v15 = row.ReadColumn<sbyte>(columnOffset + PronounColumnIdx);
        var v17 = row.ReadColumn<sbyte>(columnOffset + Unknown5ColumnIdx);
        var v19 = row.ReadColumn<sbyte>(columnOffset + ArticleColumnIdx);
        var v20 = 4 * (v33 + 6 + 2 * v15);

        if (v19 != 0) {
            var v21 = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(v20);
            if (v21 != null) output->SetString(v21.RawData.WithNullTerminator());

            // skipping link marker ("//")

            var v30 = row.ReadColumn<LuminaSeString>(
                columnOffset + (Amount <= 1 ? SingularColumnIdx : PluralColumnIdx)
            );

            if (v30 != null) {
                temp->SetString(v30.RawData.WithNullTerminator());
                output->Append(temp);
            }

            if (Amount <= 1) {
                placeholder->SetString("[n]");
                temp->SetString(Amount.ToString());
                output->Replace(placeholder, temp);
            }
        } else {
            if (v17 != 0 && (Amount > 1 || v17 == 2)) {
                var v29 = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(v20 + 2);

                if (v29 != null) {
                    output->SetString(v29.RawData.WithNullTerminator());

                    // skipping link marker ("//")

                    var v30 = row.ReadColumn<LuminaSeString>(columnOffset + PluralColumnIdx);

                    if (v30 != null) {
                        temp->SetString(v30.RawData.WithNullTerminator());
                        output->Append(temp);
                    }
                }
            } else {
                var v27 = attributiveSheet.GetRow((uint)Person)?.ReadColumn<LuminaSeString>(v20 + (v17 != 0 ? 1 : 3));
                if (v27 != null) output->SetString(v27.RawData.WithNullTerminator());

                // skipping link marker ("//")

                var v30 = row.ReadColumn<LuminaSeString>(columnOffset + SingularColumnIdx);

                if (v30 != null) {
                    temp->SetString(v30.RawData.WithNullTerminator());
                    output->Append(temp);
                }
            }

            placeholder->SetString("[n]");
            temp->SetString(Amount.ToString());
            output->Replace(placeholder, temp);
        }

        placeholder->Dtor(true);
        temp->Dtor(true);
        return output;
    }
}
