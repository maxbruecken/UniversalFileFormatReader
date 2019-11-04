namespace UniversalFileFormatReader.Interpreters
{
    public interface IUniversalFileDatasetBuilder
    {   
        void AddLine(string line);

        UniversalFileDataset Build();
    }
}