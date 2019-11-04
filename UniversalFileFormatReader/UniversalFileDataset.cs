namespace UniversalFileFormatReader
{
    public abstract class UniversalFileDataset
    {
        protected UniversalFileDataset(UniversalFileDatasetNumber number)
        {
            Number = number;
        }
        
        public UniversalFileDatasetNumber Number { get; }
    }
}