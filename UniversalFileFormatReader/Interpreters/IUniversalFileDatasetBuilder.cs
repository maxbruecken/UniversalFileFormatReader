using System.Data.Common;

namespace UniversalFileFormatReader.Interpreters
{
    internal interface IUniversalFileDatasetBuilder
    {   
        DataAdditionResult AddData(DataType dataType, object data);

        UniversalFileDataset Build();
    }
}