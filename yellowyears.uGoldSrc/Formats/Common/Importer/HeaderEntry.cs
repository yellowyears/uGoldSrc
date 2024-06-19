namespace yellowyears.uGoldSrc.Formats.Common.Importer
{
    public struct HeaderEntry
    {
        /// <summary>
        /// The offset of the lump from the start of the file
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// The amount of bytes that the lump takes up in the file
        /// </summary>
        public int Length { get; private set; }

        public HeaderEntry(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }
    }
}