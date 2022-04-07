namespace LinkDotNet.StringBuilder;

/// <summary>
/// Taken from here: https://github.com/linkdotnet/StringOperations and adopted to work with ReadOnlySpan's
/// </summary>
internal static class BoyerMooreSearch
{
    private const int AlphabetSize = 256;

    /// <summary>
    /// Finds all occurence of <paramref name="word"/> in <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text to look for.</param>
    /// <param name="word">The word which should be found in <paramref name="word"/>.</param>
    /// <returns>Array of indexes where <paramref name="word"/> was found.</returns>
    public static ReadOnlySpan<int> FindAll(ReadOnlySpan<char> text, ReadOnlySpan<char> word)
    {
        if (text.IsEmpty || word.IsEmpty)
        {
            return Array.Empty<int>();
        }

        if (text.Length < word.Length)
        {
            return Array.Empty<int>();
        }

        var badCharacterTable = GetBadCharacterTable(word);
        var shift = 0;
        var hits = new TypedSpanList<int>();
        while (shift <= text.Length - word.Length)
        {
            var index = word.Length - 1;

            index = ReduceIndexWhileMatchAtShift(text, word, index, shift);

            if (index < 0)
            {
                hits.Add(shift);

                shift = ShiftPatternToNextCharacterWithLastOccurrenceOfPattern(text, shift, word.Length, badCharacterTable);
            }
            else
            {
                shift = ShiftPatternAfterBadCharacter(text, shift, index, badCharacterTable);
            }
        }

        return hits.AsSpan;
    }

    private static ReadOnlySpan<int> GetBadCharacterTable(ReadOnlySpan<char> text)
    {
        Span<int> table = new int[AlphabetSize];
        table.Fill(-1);

        for (var i = 0; i < text.Length; i++)
        {
            table[text[i]] = i;
        }

        return table;
    }

    private static int ReduceIndexWhileMatchAtShift(ReadOnlySpan<char> text, ReadOnlySpan<char> word, int index, int shift)
    {
        while (index >= 0 && text[shift + index] == word[index])
        {
            index--;
        }

        return index;
    }

    private static int ShiftPatternToNextCharacterWithLastOccurrenceOfPattern(ReadOnlySpan<char> text, int shift, int wordLength, ReadOnlySpan<int> badCharacterTable)
    {
        return shift + (shift + wordLength < text.Length
            ? wordLength - badCharacterTable[text[shift + wordLength]]
            : 1);
    }

    private static int ShiftPatternAfterBadCharacter(ReadOnlySpan<char> text, int shift, int index,  ReadOnlySpan<int> badCharacterTable)
    {
        var character = text[shift + index];
        return shift + Math.Max(1, index - badCharacterTable[character]);
    }
}