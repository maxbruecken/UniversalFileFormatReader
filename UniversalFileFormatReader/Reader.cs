using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UniversalFileFormatReader.Interpreters;

namespace UniversalFileFormatReader
{
    public class Reader : IDisposable
    {
        private const string DataSetDelimiter = "    -1";
        private const int BufferSize = 16384;
        private const byte LineFeed = 0x0a;
        private const byte CarriageReturn = 0x0d;

        private readonly Stream _inputStream;
        private readonly bool _leaveOpen;
        private readonly byte[] _internalBuffer = new byte[BufferSize];
        
        private int _internalPositionInBuffer;
        private int _internalBufferLength;

        public Reader(string filePath) : this(new FileStream(filePath, FileMode.Open))
        {
        }
        
        public Reader(Stream inputStream, bool leaveOpen = false)
        {
            _inputStream = inputStream;
            _leaveOpen = leaveOpen;
            
            _internalPositionInBuffer = 0;
            _internalBufferLength = 0;
        }

        public async Task<IEnumerable<UniversalFileDataset>> ReadAsync()
        {
            var datasets = new List<UniversalFileDataset>();
            var nextResult = await GetNextDatasetAsync();
            while (nextResult.NextDelimiterFound)
            {
                datasets.Add(nextResult.Dataset);
                nextResult = await GetNextDatasetAsync();
            }

            return datasets;
        }

        private async Task<(bool NextDelimiterFound, UniversalFileDataset Dataset)> GetNextDatasetAsync()
        {
            string line;
            if (!await FindNextDelimiter((t, d) => new DataAdditionResult(DataType.ASCIILine, -1)))
            {
                return (NextDelimiterFound: false, Dataset: null);
            }
            if ((line = await ReadASCIILineAsync()) == null) return (NextDelimiterFound: false, Dataset: null);
            
            var builder = UniversalFileDatasetBuilderFactory.Create(line);
            var nextDelimiterFound = await FindNextDelimiter((t, d) => builder.AddData(t, d));
            return (NextDelimiterFound: nextDelimiterFound, Dataset: builder.Build());
        }

        private async Task<string> ReadASCIILineAsync()
        {
            if (!await RefreshBufferIfNeeded())
            {
                return null;
            }
            var lineBuilder = new StringBuilder();
            byte lastByte;
            while ((lastByte = _internalBuffer[_internalPositionInBuffer]) != CarriageReturn && 
                   lastByte != LineFeed)
            {
                lineBuilder.Append(Convert.ToChar(lastByte));
                _internalPositionInBuffer++;
                if (!await RefreshBufferIfNeeded())
                {
                    break;
                }
            }

            _internalPositionInBuffer++;
            if (!await RefreshBufferIfNeeded())
            {
                return lineBuilder.ToString();
            }

            if (lastByte != CarriageReturn)
            {
                return lineBuilder.ToString();
            }

            if (_internalBuffer[_internalPositionInBuffer] == LineFeed)
            {
                _internalPositionInBuffer++;
            }

            return lineBuilder.ToString();
        }

        private async Task<bool> RefreshBufferIfNeeded()
        {
            if (_internalPositionInBuffer < _internalBufferLength)
            {
                return true;
            }

            _internalPositionInBuffer = 0;
            _internalBufferLength = await _inputStream.ReadAsync(_internalBuffer, 0, _internalBuffer.Length);
            return _internalBufferLength > 0;
        }

        private async Task<bool> FindNextDelimiter(Func<DataType, object, DataAdditionResult> dataAction)
        {
            var dataAdditionResult = new DataAdditionResult(DataType.ASCIILine, -1);
            while (true)
            {
                if (dataAdditionResult.NextDataType == DataType.ASCIILine)
                {
                    string line;
                    if ((line = await ReadASCIILineAsync()) == null)
                    {
                        return false;
                    }
                    if (line.StartsWith(DataSetDelimiter, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                    dataAdditionResult = dataAction(dataAdditionResult.NextDataType, line);
                    continue;
                }

                if (dataAdditionResult.NextDataType == DataType.BinaryData)
                {
                    dataAdditionResult = await ProcessBinaryData(dataAdditionResult, dataAction);
                    continue;
                }

                return false;
            }
        }

        private async Task<DataAdditionResult> ProcessBinaryData(DataAdditionResult previousDataAdditionResult, Func<DataType, object, DataAdditionResult> dataAction)
        {
            var byteBuffer = new byte[previousDataAdditionResult.NextDataLength];
            if (await ReadBytesAsync(byteBuffer, 0, byteBuffer.Length) < byteBuffer.Length)
            {
                throw new InvalidDataException("Stream is ended before all bytes could be read.");
            }

            return dataAction(previousDataAdditionResult.NextDataType, byteBuffer);
        }

        private async Task<int> ReadBytesAsync(byte[] buffer, int index, int length)
        {
            if (!await RefreshBufferIfNeeded())
            {
                return 0;
            }

            var readByteCount = 0;
            while (readByteCount < length)
            {
                var copyFromBufferByteCount = Math.Min(_internalBufferLength - _internalPositionInBuffer, length - readByteCount);
                Buffer.BlockCopy(_internalBuffer, _internalPositionInBuffer, buffer, index, copyFromBufferByteCount);
                _internalPositionInBuffer += copyFromBufferByteCount;
                readByteCount += copyFromBufferByteCount;
                index += copyFromBufferByteCount;
                if (!await RefreshBufferIfNeeded())
                {
                    break;
                }
            }
            return readByteCount;
        }

        public void Dispose()
        {
            if (!_leaveOpen)
            {
                _inputStream.Dispose();
            }
        }
    }
}