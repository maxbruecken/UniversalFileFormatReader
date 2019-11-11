using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace UniversalFileFormatReader.Interpreters
{
    internal class UniversalFileDatasetNumber58BinaryBuilder : UniversalFileDatasetNumber58Builder
    {
        private const string NumberLine = "    58b";
        
        private double _currentDataIndex;
        private readonly bool _littleEndianness;
        private readonly FloatFormat _floatFormat;
        private readonly int _byteCount;

        public new static IUniversalFileDatasetBuilder ForNumberLine(string numberLine, IUniversalFileDatasetBuilder next)
        {
            if (numberLine.StartsWith(NumberLine, StringComparison.InvariantCultureIgnoreCase))
            {
                return new UniversalFileDatasetNumber58BinaryBuilder(numberLine);
            }
            return next;
        }

        private UniversalFileDatasetNumber58BinaryBuilder(string numberLine)
        {
            // Format(I6,1A1,I6,I6,I12,I12,I6,I6,I12,I12)
            _littleEndianness = int.Parse(numberLine.Substring(7, 6), NumberStyles.Integer, CultureInfo.InvariantCulture) == 1;
            _floatFormat = (FloatFormat)int.Parse(numberLine.Substring(13, 6), NumberStyles.Integer, CultureInfo.InvariantCulture);
            if (int.Parse(numberLine.Substring(19, 12), NumberStyles.Integer, CultureInfo.InvariantCulture) != 11)
            {
                throw new InvalidDataException("Unexpected header line count.");
            }
            _byteCount = int.Parse(numberLine.Substring(31, 12), NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        public override DataAdditionResult AddData(DataType dataType, object data)
        {
            if (RecordNumber < 10)
            {
                return base.AddData(dataType, data);
            }
            switch (RecordNumber)
            {
                case 10:
                    base.AddData(dataType, data);
                    return new DataAdditionResult(DataType.BinaryData, _byteCount);
                case 11:
                    var bytes = (byte[]) data;
                    AddBinaryData(bytes);
                    break;
            }
            return new DataAdditionResult(DataType.ASCIILine, -1);
        }

        private void AddBinaryData(byte[] data)
        {
            if (_floatFormat != FloatFormat.IEEE_754)
            {
                throw new NotSupportedException("Only IEEE 754 float format is supported yet.");
            }
            HandleRealNumbersWithEvenAbscissa(data);
            HandleRealNumbersWithUnevenAbscissa(data);
            HandleComplexNumbersWithEvenAbscissa(data);
            HandleComplexNumbersWithUnevenAbscissa(data);
        }

        private void HandleRealNumbersWithEvenAbscissa(byte[] data)
        {
            if (Dataset.DataType != UniversalFileDatasetNumber58DataType.RealSingle && Dataset.DataType != UniversalFileDatasetNumber58DataType.RealDouble)
            {
                return;
            }
            if (Dataset.AbscissaIsUneven)
            {
                return;
            }

            var pointValueLength = Dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 4 : 8;
            if (data.Length % pointValueLength != 0)
            {
                throw new InvalidDataException("Length of binary part is invalid.");
            }
            
            for (var pointIndex = 0; pointIndex < data.Length; pointIndex += pointValueLength)
            {
                var pointBytes = new byte[pointValueLength];
                Buffer.BlockCopy(data, pointIndex, pointBytes, 0, pointValueLength);
                if (!_littleEndianness)
                {
                    pointBytes = pointBytes.Reverse().ToArray();
                }

                var value = Dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle
                    ? BitConverter.ToSingle(pointBytes, 0)
                    : BitConverter.ToDouble(pointBytes, 0);
                Dataset.Data.Add(new UniversalFileDatasetNumber58DataPoint(_currentDataIndex, value, double.NaN));

                _currentDataIndex += Dataset.AbscissaSpacing;
            }
        }

        private void HandleRealNumbersWithUnevenAbscissa(byte[] data)
        {
            if (Dataset.DataType != UniversalFileDatasetNumber58DataType.RealSingle && Dataset.DataType != UniversalFileDatasetNumber58DataType.RealDouble)
            {
                return;
            }
            if (!Dataset.AbscissaIsUneven)
            {
                return;
            }
            
            var pointValueLength = 4 + Dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle ? 4 : 8;
            if (data.Length % pointValueLength != 0)
            {
                throw new InvalidDataException("Length of binary part is invalid.");
            }
            
            for (var pointIndex = 0; pointIndex < data.Length; pointIndex += pointValueLength)
            {
                var pointIndexBytes = new byte[4];
                var pointValueBytes = new byte[pointValueLength - 4];
                Buffer.BlockCopy(data, pointIndex, pointIndexBytes, 0, 4);
                Buffer.BlockCopy(data, pointIndex + 4, pointValueBytes, 0, pointValueLength - 4);
                if (!_littleEndianness)
                {
                    pointIndexBytes = pointIndexBytes.Reverse().ToArray();
                    pointValueBytes = pointValueBytes.Reverse().ToArray();
                }

                var index = BitConverter.ToSingle(pointIndexBytes, 0);
                var value = Dataset.DataType == UniversalFileDatasetNumber58DataType.RealSingle
                    ? BitConverter.ToSingle(pointValueBytes, 0)
                    : BitConverter.ToDouble(pointValueBytes, 0);
                Dataset.Data.Add(new UniversalFileDatasetNumber58DataPoint(index, value, double.NaN));
            }
        }

        private void HandleComplexNumbersWithEvenAbscissa(byte[] data)
        {
            if (Dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexSingle && Dataset.DataType != UniversalFileDatasetNumber58DataType.ComplexDouble)
            {
                return;
            }
            if (Dataset.AbscissaIsUneven)
            {
                return;
            }
            var pointValueLength = Dataset.DataType == UniversalFileDatasetNumber58DataType.ComplexSingle ? 4 : 8;
            if (data.Length % pointValueLength != 0)
            {
                throw new InvalidDataException("Length of binary part is invalid.");
            }
            
            for (var pointIndex = 0; pointIndex < data.Length; pointIndex += 2 * pointValueLength)
            {
                var pointRealBytes = new byte[pointValueLength];
                Buffer.BlockCopy(data, pointIndex, pointRealBytes, 0, pointValueLength);
                if (!_littleEndianness)
                {
                    pointRealBytes = pointRealBytes.Reverse().ToArray();
                }
                var pointImaginaryBytes = new byte[pointValueLength];
                Buffer.BlockCopy(data, pointIndex + pointValueLength, pointImaginaryBytes, 0, pointValueLength);
                if (!_littleEndianness)
                {
                    pointImaginaryBytes = pointImaginaryBytes.Reverse().ToArray();
                }

                var realValue = Dataset.DataType == UniversalFileDatasetNumber58DataType.ComplexSingle
                    ? BitConverter.ToSingle(pointRealBytes, 0)
                    : BitConverter.ToDouble(pointRealBytes, 0);
                var imaginaryValue = Dataset.DataType == UniversalFileDatasetNumber58DataType.ComplexSingle
                    ? BitConverter.ToSingle(pointImaginaryBytes, 0)
                    : BitConverter.ToDouble(pointImaginaryBytes, 0);
                Dataset.Data.Add(new UniversalFileDatasetNumber58DataPoint(_currentDataIndex, realValue, imaginaryValue));

                _currentDataIndex += Dataset.AbscissaSpacing;
            }
        }

        private void HandleComplexNumbersWithUnevenAbscissa(byte[] data)
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

    internal enum FloatFormat
    {
        DEC_VMS = 1, IEEE_754 = 2, IBM_5_370 = 3
    }
}