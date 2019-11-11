namespace UniversalFileFormatReader.Interpreters
{
    internal class DataAdditionResult
    {
        internal DataAdditionResult(DataType nextDataType, int nextDataLength)
        {
            NextDataType = nextDataType;
            NextDataLength = nextDataLength;
        }
        
        internal DataType NextDataType { get; }
        
        internal int NextDataLength { get; }
    }

    internal enum DataType
    {
        ASCIILine, BinaryData
    }
}