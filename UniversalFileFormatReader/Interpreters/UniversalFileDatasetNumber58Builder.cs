using System;
using System.Globalization;

namespace UniversalFileFormatReader.Interpreters
{
    internal class UniversalFileDatasetNumber58Builder : IUniversalFileDatasetBuilder
    {
        private const string NumberLine = "    58";

        protected readonly UniversalFileDatasetNumber58 Dataset;
        
        protected int RecordNumber;
        private double _currentDataIndex;
        
        public static IUniversalFileDatasetBuilder ForNumberLine(string numberLine, IUniversalFileDatasetBuilder next)
        {
            return numberLine.Equals(NumberLine, StringComparison.InvariantCultureIgnoreCase) ? new UniversalFileDatasetNumber58Builder() : next;
        }

        protected UniversalFileDatasetNumber58Builder()
        {
            Dataset = new UniversalFileDatasetNumber58();
        }

        public virtual DataAdditionResult AddData(DataType dataType, object data)
        {
            if (dataType != DataType.ASCIILine)
            {
                throw new NotSupportedException("Only ASCII data type is supported");
            }
            var line = (string) data;
            if (RecordNumber < 12)
            {
                RecordNumber++;
            }
            switch (RecordNumber)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    Dataset.Headers[RecordNumber - 1] = line;
                    break;
                case 6:
                    //Format(2(I5,I10),2(1X,10A1,I10,I4))
                    Dataset.FunctionIdentification.Type = (FunctionIdentificationType)int.Parse(line.Substring(0, 5), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    Dataset.FunctionIdentification.Number = int.Parse(line.Substring(5, 10), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    Dataset.FunctionIdentification.VersionOrSequenceNumber = int.Parse(line.Substring(15, 5), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    Dataset.FunctionIdentification.ResponseEntityName = line.Substring(31, 10).TrimEnd(' ');
                    Dataset.FunctionIdentification.ResponseNode = int.Parse(line.Substring(41, 10), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    Dataset.FunctionIdentification.ResponseDirection = int.Parse(line.Substring(51, 4), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    Dataset.FunctionIdentification.ReferenceEntityName = line.Substring(56, 10).TrimEnd(' ');
                    Dataset.FunctionIdentification.ReferenceNode = int.Parse(line.Substring(66, 10), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    Dataset.FunctionIdentification.ReferenceDirection = int.Parse(line.Substring(76, 4), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    break;
                case 7:
                    // Format(3I10,3E13.5)
                    Dataset.DataType = (UniversalFileDatasetNumber58DataType)int.Parse(line.Substring(0, 10), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    Dataset.DataCount = long.Parse(line.Substring(10, 10), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    Dataset.AbscissaIsUneven = int.Parse(line.Substring(20, 10), NumberStyles.Integer, CultureInfo.InvariantCulture) == 0;
                    Dataset.AbscissaMinimum = double.Parse(line.Substring(30, 13), NumberStyles.Float, CultureInfo.InvariantCulture);
                    Dataset.AbscissaSpacing = double.Parse(line.Substring(43, 13), NumberStyles.Float, CultureInfo.InvariantCulture);
                    Dataset.ZAxisValue = double.Parse(line.Substring(56, 13), NumberStyles.Float, CultureInfo.InvariantCulture);

                    _currentDataIndex = Dataset.AbscissaMinimum;
                    break;
                case 8:
                    ExtractAxisDataCharacteristics(line, Dataset.AbscissaDataCharacteristics);
                    break;
                case 9:
                    ExtractAxisDataCharacteristics(line, Dataset.OrdinateDataCharacteristics);
                    break;
                case 10:
                    ExtractAxisDataCharacteristics(line, Dataset.OrdinateDenominatorDataCharacteristics);
                    break;
                case 11:
                    ExtractAxisDataCharacteristics(line, Dataset.ZAxisDataCharacteristics);
                    break;
                case 12:
                    AddDataLine(line);
                    break;
            }
            return new DataAdditionResult(DataType.ASCIILine, -1);
        }

        private static void ExtractAxisDataCharacteristics(string line, AxisDataCharacteristics axisDataCharacteristics)
        {
            // Format(I10,3I5,2(1X,20A1))
            axisDataCharacteristics.DataType = (AxisDataType) int.Parse(line.Substring(0, 10).TrimStart(' '));
            axisDataCharacteristics.Label = line.Substring(26, 20).TrimEnd();
            axisDataCharacteristics.Unit = line.Substring(47, Math.Min(20, line.Length - 47)).TrimEnd();
        }

        public UniversalFileDataset Build()
        {
            return Dataset;
        }

        private void AddDataLine(string line)
        {
            HandleRealNumbersWithEvenAbscissa(line);
            HandleRealNumbersWithUnevenAbscissa(line);
            HandleComplexNumbersWithEvenAbscissa(line);
            HandleComplexNumbersWithUnevenAbscissa(line);
        }

        private void HandleRealNumbersWithEvenAbscissa(string line)
        {
            if (Dataset.DataType != UniversalFileDatasetNumber58DataType.RealSingle && Dataset.DataType != UniversalFileDatasetNumber58DataType.RealDouble)
            {
                return;
            }
            if (Dataset.AbscissaIsUneven)
            {
                return;
            }

            var pointsPerLine = Dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 6 : 4;
            var pointValueLength = Dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 13 : 20;
            for (var pointIndex = 0; pointIndex < pointsPerLine; pointIndex++)
            {
                if (line.Length < pointIndex * pointValueLength + pointValueLength)
                {
                    break;
                }
                var value = double.Parse(line.Substring(pointIndex * pointValueLength, pointValueLength), NumberStyles.Float, CultureInfo.InvariantCulture);
                Dataset.Data.Add(new UniversalFileDatasetNumber58DataPoint(_currentDataIndex, value, double.NaN));

                _currentDataIndex += Dataset.AbscissaSpacing;
            }
        }

        private void HandleRealNumbersWithUnevenAbscissa(string line)
        {
            if (Dataset.DataType != UniversalFileDatasetNumber58DataType.RealSingle && Dataset.DataType != UniversalFileDatasetNumber58DataType.RealDouble)
            {
                return;
            }
            if (!Dataset.AbscissaIsUneven)
            {
                return;
            }
            
            var pointsPerLine = Dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 3 : 2;
            var pointValueLength = Dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 26 : 33;
            if (Dataset.DataType == UniversalFileDatasetNumber58DataType.RealDouble)
            {
                // allow abscissa values to have format E20.12
                if (line.Length > pointValueLength && line.Length % pointValueLength != 0)
                {
                    pointValueLength = 40;
                }
            }
            if (line.Length > pointValueLength && line.Length % pointsPerLine != 0)
            {
                throw new InvalidOperationException("Invalid data line length.");
            }

            for (var pointIndex = 0; pointIndex < pointsPerLine; pointIndex++)
            {
                if (line.Length < pointIndex * pointValueLength + pointValueLength)
                {
                    break;
                }
                var pairString = line.Substring(pointIndex * pointValueLength, pointValueLength);
                var indexLength = pairString.Length - 20;
                var index = double.Parse(pairString.Substring(0, indexLength), NumberStyles.Float, CultureInfo.InvariantCulture);
                var value = double.Parse(pairString.Substring(indexLength), NumberStyles.Float, CultureInfo.InvariantCulture);
                Dataset.Data.Add(new UniversalFileDatasetNumber58DataPoint(index, value, double.NaN));
            }
        }

        private void HandleComplexNumbersWithEvenAbscissa(string line)
        {
            if (Dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexSingle && Dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexDouble)
            {
                return;
            }
            if (Dataset.AbscissaIsUneven)
            {
                return;
            }
            
            var pointsPerLine = Dataset.DataType == UniversalFileDatasetNumber58DataType.ComplexSingle ? 3 : 2;
            var pointValueLength = Dataset.DataType == UniversalFileDatasetNumber58DataType.ComplexSingle ? 13 : 20;
            for (var pointIndex = 0; pointIndex < pointsPerLine; pointIndex++)
            {
                if (line.Length < 2 * (pointIndex * pointValueLength + pointValueLength))
                {
                    break;
                }
                var realValue = double.Parse(line.Substring(2 * pointIndex * pointValueLength, pointValueLength), NumberStyles.Float, CultureInfo.InvariantCulture);
                var imaginaryValue = double.Parse(line.Substring(2 * pointIndex * pointValueLength + pointValueLength, pointValueLength), NumberStyles.Float, CultureInfo.InvariantCulture);
                Dataset.Data.Add(new UniversalFileDatasetNumber58DataPoint(_currentDataIndex, realValue, imaginaryValue));

                _currentDataIndex += Dataset.AbscissaSpacing;
            }
        }

        private void HandleComplexNumbersWithUnevenAbscissa(string line)
        {
            if (Dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexSingle && Dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexDouble)
            {
                return;
            }
            if (!Dataset.AbscissaIsUneven)
            {
                return;
            }
            throw new NotSupportedException("Complex numbers not supported yet.");
        }
    }
}