namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    /// <summary>
    /// Finds the smallest power of 2 which is greater than or equal to <paramref name="minimum"/>.
    /// </summary>
    /// <param name="minimum">The value the result should be greater than or equal to.</param>
    /// <returns>The smallest power of 2 >= <paramref name="minimum"/>.</returns>
    internal static int FindSmallestPowerOf2Above(int minimum)
    {
        return 1 << (int)Math.Ceiling(Math.Log2(minimum));
    }
}