namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Replaces all instances of one character with another in this builder.
    /// </summary>
    /// <param name="oldValue">The character to replace.</param>
    /// <param name="newValue">The character to replace <paramref name="oldValue"/> with.</param>
    public void Replace(char oldValue, char newValue) => Replace(oldValue, newValue, 0, Length);

    /// <summary>
    /// Replaces all instances of one character with another in this builder.
    /// </summary>
    /// <param name="oldValue">The character to replace.</param>
    /// <param name="newValue">The character to replace <paramref name="oldValue"/> with.</param>
    /// <param name="startIndex">The index to start in this builder.</param>
    /// <param name="count">The number of characters to read in this builder.</param>
    public void Replace(char oldValue, char newValue, int startIndex, int count)
    {
        if (startIndex < 0)
        {
            throw new ArgumentException("Start index can't be smaller than 0.", nameof(startIndex));
        }

        if (count > bufferPosition)
        {
            throw new ArgumentException($"Count: {count} is bigger than the current size {bufferPosition}.", nameof(count));
        }

        for (var i = startIndex; i < startIndex + count; i++)
        {
            if (buffer[i] == oldValue)
            {
                buffer[i] = newValue;
            }
        }
    }

    /// <summary>
    /// Replaces all instances of one string with another in this builder.
    /// </summary>
    /// <param name="oldValue">The string to replace.</param>
    /// <param name="newValue">The string to replace <paramref name="oldValue"/> with.</param>
    /// <remarks>
    /// If <paramref name="newValue"/> is <c>empty</c>, instances of <paramref name="oldValue"/>
    /// are removed from this builder.
    /// </remarks>
    public void Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue) => Replace(oldValue, newValue, 0, Length);

    /// <summary>
    /// Replaces all instances of one string with another in this builder.
    /// </summary>
    /// <param name="oldValue">The string to replace.</param>
    /// <param name="newValue">The string to replace <paramref name="oldValue"/> with.</param>
    /// <param name="startIndex">The index to start in this builder.</param>
    /// <param name="count">The number of characters to read in this builder.</param>
    /// <remarks>
    /// If <paramref name="newValue"/> is <c>empty</c>, instances of <paramref name="oldValue"/>
    /// are removed from this builder.
    /// </remarks>
    public void Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
    {
        var length = startIndex + count;
        var slice = buffer[startIndex..length];

        // We might want to check if for very small strings we go with a naive approach
        var hits = BoyerMooreSearch.FindAll(slice, oldValue);
        if (hits.IsEmpty)
        {
            return;
        }

        var delta = newValue.Length - oldValue.Length;

        for (var i = 0; i < hits.Length; i++)
        {
            var index = startIndex + hits[0] + (delta * i);
            Remove(index, oldValue.Length);
            var debug = ToString();
            Console.WriteLine(debug);
            Insert(index, newValue);
            debug = ToString();
            Console.WriteLine(debug);
        }
    }
}