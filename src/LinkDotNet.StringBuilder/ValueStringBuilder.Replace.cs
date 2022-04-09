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
    public void Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
        => Replace(oldValue, newValue, 0, Length);

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

        if (oldValue == newValue)
        {
            return;
        }

        // We might want to check whether or not we want to introduce different
        // string search algorithms for longer strings.
        // I had checked initially with Boyer-Moore but it didn't make that much sense as we
        // don't expect very long strings and then the performance is literally the same. So I went with the easier solution.
        var hits = NaiveSearch.FindAll(slice, oldValue);

        if (hits.IsEmpty)
        {
            return;
        }

        var delta = newValue.Length - oldValue.Length;

        for (var i = 0; i < hits.Length; i++)
        {
            var index = startIndex + hits[i] + (delta * i);
            Remove(index, oldValue.Length);
            Insert(index, newValue);
        }
    }
}