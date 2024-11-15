using System.Collections.Generic;
using Dalamud.Game;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using System;
using System.Globalization;
using Umbra.Common;
using Umbra.Common.Extensions;

namespace Umbra.Game.Localization;

/*
Attributive sheet:
  Japanese:
    Unknown0 = Singular Demonstrative
    Unknown1 = Plural Demonstrative
  English:
    Unknown2 = Article before a singular noun beginning with a consonant sound
    Unknown3 = Article before a generic noun beginning with a consonant sound
    Unknown4 = N/A
    Unknown5 = Article before a singular noun beginning with a vowel sound
    Unknown6 = Article before a generic noun beginning with a vowel sound
    Unknown7 = N/A
  German:
    Unknown8 = Nominative Masculine
    Unknown9 = Nominative Feminine
    Unknown10 = Nominative Neutral
    Unknown11 = Nominative Plural
    Unknown12 = Genitive Masculine
    Unknown13 = Genitive Feminine
    Unknown14 = Genitive Neutral
    Unknown15 = Genitive Plural
    Unknown16 = Dative Masculine
    Unknown17 = Dative Feminine
    Unknown18 = Dative Neutral
    Unknown19 = Dative Plural
    Unknown20 = Accusative Masculine
    Unknown21 = Accusative Feminine
    Unknown22 = Accusative Neutral
    Unknown23 = Accusative Plural
  French (unsure):
    Unknown24 = Singular Article
    Unknown25 = Singular Masculine Article
    Unknown26 = Plural Masculine Article
    Unknown27 = ?
    Unknown28 = ?
    Unknown29 = Singular Masculine/Feminine Article, before a noun beginning in a vowel or an h
    Unknown30 = Plural Masculine/Feminine Article, before a noun beginning in a vowel or an h
    Unknown31 = ?
    Unknown32 = ?
    Unknown33 = Singular Feminine Article
    Unknown34 = Plural Feminine Article
    Unknown35 = ?
    Unknown36 = ?
    Unknown37 = Singular Masculine/Feminine Article, before a noun beginning in a vowel or an h
    Unknown38 = Plural Masculine/Feminine Article, before a noun beginning in a vowel or an h
    Unknown39 = ?
    Unknown40 = ?
*/

[Service]
public class TextDecoder(IClientState clientState, IDataManager dataManager)
{
    private readonly ReadOnlySeString _empty = default;

    private readonly Dictionary<(ClientLanguage Language, string SheetName, int RowId, int Amount, int Person, int Case)
      , ReadOnlySeString> _cache = [];

    // column names from ExdSchema
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
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
            ProcessNoun(clientState.ClientLanguage, sheetName, 5, (int)rowId).ExtractText()
        );
    }

    // Placeholders:
    // [t] = article or grammatical gender (EN: the, DE: der, die, das)
    // [n] = amount (number)
    // [a] = declension
    // [p] = plural
    // [pa] = ?

    // SeString macro parameters:
    // <XXnoun(SheetName,Person,RowId,Amount,Case[,UnkInt5])>
    // UnkInt5 used in EN/FR/CH* (technically only in chinese, maybe for script variant?)
    // made linkMarker from the noun ctors optional, since it's processed in the pronoun module
    public ReadOnlySeString ProcessNoun(
        ClientLanguage language,    string sheetName, int person, int rowId, int amount = 1, int grammaticalCase = 1,
        int            unkInt5 = 1, ReadOnlySeString linkMarker = default
    )
    {
        grammaticalCase--;

        if (grammaticalCase > 5 || (language != ClientLanguage.German && grammaticalCase != 0)) return _empty;

        var key = (language, sheetName, rowId, amount, person, grammaticalCase);
        if (_cache.TryGetValue(key, out var value)) return value;

        var attributiveSheet = dataManager.GameData.Excel.GetSheet<RawRow>(language.ToLumina(), "Attributive");
        var sheet            = dataManager.GameData.Excel.GetSheet<RawRow>(language.ToLumina(), sheetName);

        if (!sheet.HasRow((uint)rowId)) {
            Logger.Warning($"Sheet {sheetName} does not contain row #{rowId}");
            return _empty;
        }

        var row = sheet.GetRow((uint)rowId);

        // see "E8 ?? ?? ?? ?? 44 8B 6B 08"
        var columnOffset = sheetName switch {
            "BeastTribe" => 10,
            "DeepDungeonItem" or "DeepDungeonEquipment" or "DeepDungeonMagicStone" or "DeepDungeonDemiclone" => 1,
            "Glasses" => 4,
            "GlassesStyle" => 15,
            "Ornament" => 8, // not part of that function, but still shifted
            _ => 0
        };

        var output = language switch {
            ClientLanguage.Japanese => ResolveNounJa(amount, person, attributiveSheet, row, linkMarker),
            ClientLanguage.English  => ResolveNounEn(amount, person, attributiveSheet, row, columnOffset, linkMarker),
            ClientLanguage.German => ResolveNounDe(
                amount,
                person,
                grammaticalCase,
                attributiveSheet,
                row,
                columnOffset,
                linkMarker
            ),
            ClientLanguage.French => ResolveNounFr(amount, person, attributiveSheet, row, columnOffset, linkMarker),
            _                     => _empty
        };

        _cache.Add(key, output);
        return output;
    }

    // Component::Text::Localize::NounJa.Resolve
    private static ReadOnlySeString ResolveNounJa(
        int amount, int person, ExcelSheet<RawRow> attributiveSheet, RawRow row, ReadOnlySeString linkMarker
    )
    {
        var builder = SeStringBuilder.SharedPool.Get();

        // Ko-So-A-Do
        var ksad = attributiveSheet.GetRow((uint)person).ReadStringColumn(amount > 1 ? 1 : 0);

        if (!ksad.IsEmpty) {
            builder.Append(ksad);

            if (amount > 1) builder.ReplaceText("[n]"u8, ReadOnlySeString.FromText(amount.ToString()));
        }

        if (!linkMarker.IsEmpty) builder.Append(linkMarker);

        var text = row.ReadStringColumn(0);
        if (!text.IsEmpty) builder.Append(text);

        var ross = builder.ToReadOnlySeString();
        SeStringBuilder.SharedPool.Return(builder);
        return ross;
    }

    // Component::Text::Localize::NounEn.Resolve
    private static ReadOnlySeString ResolveNounEn(
        int              amount, int person, ExcelSheet<RawRow> attributiveSheet, RawRow row, int columnOffset,
        ReadOnlySeString linkMarker
    )
    {
        /*
          a1->Offsets[0] = SingularColumnIdx
          a1->Offsets[1] = PluralColumnIdx
          a1->Offsets[2] = StartsWithVowelColumnIdx
          a1->Offsets[3] = PossessivePronounColumnIdx
          a1->Offsets[4] = ArticleColumnIdx
        */

        var builder = SeStringBuilder.SharedPool.Get();

        var isProperNounColumn = columnOffset + ArticleColumnIdx;
        var isProperNoun       = isProperNounColumn >= 0 ? row.ReadInt8Column(isProperNounColumn) : ~isProperNounColumn;

        if (isProperNoun == 0) {
            var startsWithVowelColumn = columnOffset + StartsWithVowelColumnIdx;

            var startsWithVowel = startsWithVowelColumn >= 0
                ? row.ReadInt8Column(startsWithVowelColumn)
                : ~startsWithVowelColumn;

            var articleColumn                 = startsWithVowel + 2 * (startsWithVowel + 1);
            var grammaticalNumberColumnOffset = amount == 1 ? SingularColumnIdx : PluralColumnIdx;

            var article = attributiveSheet
                .GetRow((uint)person)
                .ReadStringColumn(articleColumn + grammaticalNumberColumnOffset);

            if (!article.IsEmpty) builder.Append(article);

            if (!linkMarker.IsEmpty) builder.Append(linkMarker);
        }

        var text = row.ReadStringColumn(columnOffset + (amount == 1 ? SingularColumnIdx : PluralColumnIdx));
        if (!text.IsEmpty) builder.Append(text);

        builder.ReplaceText("[n]"u8, ReadOnlySeString.FromText(amount.ToString()));

        var ross = builder.ToReadOnlySeString();
        SeStringBuilder.SharedPool.Return(builder);
        return ross;
    }

    // Component::Text::Localize::NounDe.Resolve
    private static ReadOnlySeString ResolveNounDe(
        int amount, int person, int grammaticalCase, ExcelSheet<RawRow> attributiveSheet, RawRow row, int columnOffset,
        ReadOnlySeString linkMarker
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

        var              builder = SeStringBuilder.SharedPool.Get();
        ReadOnlySeString ross;

        var readColumnDirectly = (byte)(grammaticalCase >> 8 & 0xFF) & 1; // BYTE2(Case) & 1

        if ((grammaticalCase & 0x10000) != 0) grammaticalCase = 0;

        if (readColumnDirectly != 0) {
            builder.Append(row.ReadStringColumn(grammaticalCase - 0x10000));
            builder.ReplaceText("[n]"u8, ReadOnlySeString.FromText(amount.ToString()));

            ross = builder.ToReadOnlySeString();
            SeStringBuilder.SharedPool.Return(builder);
            return ross;
        }

        var genderIndexColumn = columnOffset + PronounColumnIdx;
        var genderIndex       = genderIndexColumn >= 0 ? row.ReadInt8Column(genderIndexColumn) : ~genderIndexColumn;

        var articleIndexColumn = columnOffset + ArticleColumnIdx;
        var articleIndex       = articleIndexColumn >= 0 ? row.ReadInt8Column(articleIndexColumn) : ~articleIndexColumn;

        var caseColumnOffset = 4 * grammaticalCase + 8;

        var caseRowOffsetColumn = columnOffset + (amount == 1 ? AdjectiveColumnIdx : PossessivePronounColumnIdx);

        var caseRowOffset = caseRowOffsetColumn >= 0
            ? row.ReadInt8Column(caseRowOffsetColumn)
            : (sbyte)~caseRowOffsetColumn;

        if (amount != 1) genderIndex = 3;

        var hasT = false;
        var text  = row.ReadStringColumn(columnOffset + (amount == 1 ? SingularColumnIdx : PluralColumnIdx));

        if (!text.IsEmpty) {
            hasT = text.Contains("[t]"u8);

            if (articleIndex == 0 && !hasT) {
                var grammaticalGender =
                    attributiveSheet.GetRow((uint)person).ReadStringColumn(caseColumnOffset + genderIndex); // Genus

                if (!grammaticalGender.IsEmpty) builder.Append(grammaticalGender);
            }

            if (!linkMarker.IsEmpty) builder.Append(linkMarker);

            builder.Append(text);

            var plural = attributiveSheet
                .GetRow((uint)(caseRowOffset + 26))
                .ReadStringColumn(caseColumnOffset + genderIndex);

            if (builder.Contains("[p]"u8))
                builder.ReplaceText("[p]"u8, plural);
            else
                builder.Append(plural);

            if (hasT) {
                var article =
                    attributiveSheet.GetRow(39).ReadStringColumn(caseColumnOffset + genderIndex); // Definiter Artikel

                builder.ReplaceText("[t]"u8, article);
            }
        }

        var pa = attributiveSheet.GetRow(24).ReadStringColumn(caseColumnOffset + genderIndex);
        builder.ReplaceText("[pa]"u8, pa);

        RawRow declensionRow;

        if (person is 2 or 6 || hasT)
            declensionRow = attributiveSheet.GetRow(25); // Schwache Flexion eines Adjektivs?!
        else if (person == 5)
            declensionRow = attributiveSheet.GetRow(38); // Starke Deklination
        else if (person == 1)
            declensionRow = attributiveSheet.GetRow(37); // Gemischte Deklination
        else
            declensionRow = attributiveSheet.GetRow(26); // Starke Flexion eines Artikels?!

        var declension = declensionRow.ReadStringColumn(caseColumnOffset + genderIndex);
        builder.ReplaceText("[a]"u8, declension);

        builder.ReplaceText("[n]"u8, ReadOnlySeString.FromText(amount.ToString()));

        ross = builder.ToReadOnlySeString();
        SeStringBuilder.SharedPool.Return(builder);
        return ross;
    }

    // Component::Text::Localize::NounFr.Resolve
    private static ReadOnlySeString ResolveNounFr(
        int              amount, int person, ExcelSheet<RawRow> attributiveSheet, RawRow row, int columnOffset,
        ReadOnlySeString linkMarker
    )
    {
        /*
            a1->Offsets[0] = SingularColumnIdx
            a1->Offsets[1] = PluralColumnIdx
            a1->Offsets[2] = StartsWithVowelColumnIdx
            a1->Offsets[3] = PronounColumnIdx
            a1->Offsets[4] = Unknown5ColumnIdx
            a1->Offsets[5] = ArticleColumnIdx
        */

        var              builder = SeStringBuilder.SharedPool.Get();
        ReadOnlySeString ross;

        var startsWithVowelColumn = columnOffset + StartsWithVowelColumnIdx;

        var startsWithVowel = startsWithVowelColumn >= 0
            ? row.ReadInt8Column(startsWithVowelColumn)
            : ~startsWithVowelColumn;

        var pronounColumn = columnOffset + PronounColumnIdx;
        var pronoun       = pronounColumn >= 0 ? row.ReadInt8Column(pronounColumn) : ~pronounColumn;

        var articleColumn = columnOffset + ArticleColumnIdx;
        var article       = articleColumn >= 0 ? row.ReadInt8Column(articleColumn) : ~articleColumn;

        var v20 = 4 * (startsWithVowel + 6 + 2 * pronoun);

        if (article != 0) {
            var v21 = attributiveSheet.GetRow((uint)person).ReadStringColumn(v20);
            if (!v21.IsEmpty) builder.Append(v21);

            if (!linkMarker.IsEmpty) builder.Append(linkMarker);

            var text = row.ReadStringColumn(columnOffset + (amount <= 1 ? SingularColumnIdx : PluralColumnIdx));
            if (!text.IsEmpty) builder.Append(text);

            if (amount <= 1) builder.ReplaceText("[n]"u8, ReadOnlySeString.FromText(amount.ToString()));

            ross = builder.ToReadOnlySeString();
            SeStringBuilder.SharedPool.Return(builder);
            return ross;
        }

        var v17 = row.ReadInt8Column(columnOffset + Unknown5ColumnIdx);

        if (v17 != 0 && (amount > 1 || v17 == 2)) {
            var v29 = attributiveSheet.GetRow((uint)person).ReadStringColumn(v20 + 2);

            if (!v29.IsEmpty) {
                builder.Append(v29);

                if (!linkMarker.IsEmpty) builder.Append(linkMarker);

                var text = row.ReadStringColumn(columnOffset + PluralColumnIdx);
                if (!text.IsEmpty) builder.Append(text);
            }
        } else {
            var v27 = attributiveSheet.GetRow((uint)person).ReadStringColumn(v20 + (v17 != 0 ? 1 : 3));
            if (!v27.IsEmpty) builder.Append(v27);

            if (!linkMarker.IsEmpty) builder.Append(linkMarker);

            var text = row.ReadStringColumn(columnOffset + SingularColumnIdx);
            if (!text.IsEmpty) builder.Append(text);
        }

        builder.ReplaceText("[n]"u8, ReadOnlySeString.FromText(amount.ToString()));

        ross = builder.ToReadOnlySeString();
        SeStringBuilder.SharedPool.Return(builder);
        return ross;
    }
}

public static class ReadOnlySeStringExtensions
{
    public static bool Contains(this ReadOnlySeString ross, ReadOnlySpan<byte> needle)
    {
        return ross.Data.Span.IndexOf(needle) != -1;
    }
}
