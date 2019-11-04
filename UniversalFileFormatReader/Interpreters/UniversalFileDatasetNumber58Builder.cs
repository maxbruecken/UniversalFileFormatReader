using System;
using System.Globalization;

namespace UniversalFileFormatReader.Interpreters
{
    public class UniversalFileDatasetNumber58Builder : IUniversalFileDatasetBuilder
    {
        private const string NumberLine = "    58";

        private readonly UniversalFileDatasetNumber58 _dataset;
        
        private int _recordNumber;
        private double _currentDataIndex;
        
        public static IUniversalFileDatasetBuilder ForNumberLine(string numberLine, IUniversalFileDatasetBuilder next)
        {
            return numberLine.Equals(NumberLine, StringComparison.InvariantCultureIgnoreCase) ? new UniversalFileDatasetNumber58Builder() : next;
        }

        private UniversalFileDatasetNumber58Builder()
        {
            _dataset = new UniversalFileDatasetNumber58();
        }

        public void AddLine(string line)
        {
            if (_recordNumber < 12)
            {
                _recordNumber++;
            }
            switch (_recordNumber)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    _dataset.Headers[_recordNumber - 1] = line;
                    break;
                case 7:
                    // Format(3I10,3E13.5)
                    _dataset.DataType = (UniversalFileDatasetNumber58DataType)int.Parse(line.Substring(0, 10).TrimStart(' '));
                    _dataset.DataCount = long.Parse(line.Substring(10, 10).TrimStart(' '));
                    _dataset.AbscissaIsUneven = int.Parse(line.Substring(20, 10).TrimStart(' ')) == 0;
                    _dataset.AbscissaMinimum = double.Parse(line.Substring(30, 13), NumberStyles.Float, CultureInfo.InvariantCulture);
                    _dataset.AbscissaSpacing = double.Parse(line.Substring(43, 13), NumberStyles.Float, CultureInfo.InvariantCulture);
                    _dataset.ZAxisValue = double.Parse(line.Substring(56, 13), NumberStyles.Float, CultureInfo.InvariantCulture);

                    _currentDataIndex = _dataset.AbscissaMinimum;
                    break;
                case 12:
                    AddDataLine(line);
                    break;
            }
        }

        public UniversalFileDataset Build()
        {
            return _dataset;
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
            if (_dataset.DataType != UniversalFileDatasetNumber58DataType.RealSingle && _dataset.DataType != UniversalFileDatasetNumber58DataType.RealDouble)
            {
                return;
            }
            if (_dataset.AbscissaIsUneven)
            {
                return;
            }

            var pointsPerLine = _dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 6 : 4;
            var pointValueLength = _dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 13 : 20;
            for (var pointIndex = 0; pointIndex < pointsPerLine; pointIndex++)
            {
                if (line.Length < pointIndex * pointValueLength + pointValueLength)
                {
                    break;
                }
                var value = double.Parse(line.Substring(pointIndex * pointValueLength, pointValueLength), NumberStyles.Float, CultureInfo.InvariantCulture);
                _dataset.Data.Add(new UniversalFileDatasetNumber58DataPoint(_currentDataIndex, value, double.NaN));

                _currentDataIndex += _dataset.AbscissaSpacing;
            }
        }

        private void HandleRealNumbersWithUnevenAbscissa(string line)
        {
            if (_dataset.DataType != UniversalFileDatasetNumber58DataType.RealSingle && _dataset.DataType != UniversalFileDatasetNumber58DataType.RealDouble)
            {
                return;
            }
            if (!_dataset.AbscissaIsUneven)
            {
                return;
            }
            
            var pointsPerLine = _dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 3 : 2;
            var pointValueLength = _dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 26 : 33;
            if (_dataset.DataType == UniversalFileDatasetNumber58DataType.RealDouble)
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
                _dataset.Data.Add(new UniversalFileDatasetNumber58DataPoint(index, value, double.NaN));
            }
        }

        private void HandleComplexNumbersWithEvenAbscissa(string line)
        {
            if (_dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexSingle && _dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexDouble)
            {
                return;
            }
            if (_dataset.AbscissaIsUneven)
            {
                return;
            }
            throw new NotSupportedException("Complex numbers not supported yet.");
        }

        private void HandleComplexNumbersWithUnevenAbscissa(string line)
        {
            if (_dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexSingle && _dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexDouble)
            {
                return;
            }
            if (!_dataset.AbscissaIsUneven)
            {
                return;
            }
            throw new NotSupportedException("Complex numbers not supported yet.");
        }
    }
}