namespace UniversalFileFormatReader.Interpreters
{
    public static class UniversalFileDatasetBuilderFactory
    {
        public static IUniversalFileDatasetBuilder Create(string numberLine)
        {
            var builder = (IUniversalFileDatasetBuilder)new NullBuilder();
            builder = UniversalFileDatasetNumber58Builder.ForNumberLine(numberLine, builder);
            return builder;
        }

        private class NullBuilder : IUniversalFileDatasetBuilder
        {
            public void AddLine(string line)
            {
            }

            public UniversalFileDataset Build()
            {
                return null;
            }
        }
    }
}