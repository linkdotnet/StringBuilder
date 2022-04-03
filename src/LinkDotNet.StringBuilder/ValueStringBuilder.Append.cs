namespace LinkDotNet.StringBuilder;

public ref partial struct ValueStringBuilder
{
    public void Append(sbyte value) => AppendSpanFormattable(value);

    public void Append(byte value) => AppendSpanFormattable(value);

    public void Append(short value) => AppendSpanFormattable(value);

    public void Append(int value) => AppendSpanFormattable(value);

    public void Append(long value) => AppendSpanFormattable(value);

    public void Append(float value) => AppendSpanFormattable(value);

    public void Append(double value) => AppendSpanFormattable(value);

    public void Append(decimal value) => AppendSpanFormattable(value);

    private void AppendSpanFormattable<T>(T value)
        where T : ISpanFormattable
    {
        Span<char> tempBuffer = stackalloc char[32];
        if (value.TryFormat(tempBuffer, out var written, default, null))
        {
            var newSize = written + bufferPosition;
            if (newSize > buffer.Length)
            {
                Grow();
            }

            tempBuffer.CopyTo(buffer);
            bufferPosition = newSize;
        }
    }
}