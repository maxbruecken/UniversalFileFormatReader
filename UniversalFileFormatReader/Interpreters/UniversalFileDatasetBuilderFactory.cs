namespace UniversalFileFormatReader.Interpreters
{
    internal static class UniversalFileDatasetBuilderFactory
    {
        internal static IUniversalFileDatasetBuilder Create(string numberLine)
        {
            var builder = (IUniversalFileDatasetBuilder)new NullBuilder();
            builder = UniversalFileDatasetNumber58Builder.ForNumberLine(numberLine, builder);
            builder = UniversalFileDatasetNumber58BinaryBuilder.ForNumberLine(numberLine, builder);
            return builder;
        }

        private class NullBuilder : IUniversalFileDatasetBuilder
        {
            public DataAdditionResult AddData(DataType dataType, object data)
            {
                return new DataAdditionResult(DataType.ASCIILine, -1);
            }

            public UniversalFileDataset Build()
            {
                return null;
            }
        }
    }
}