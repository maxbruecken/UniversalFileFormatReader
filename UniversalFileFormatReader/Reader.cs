using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalFileFormatReader.Interpreters;

namespace UniversalFileFormatReader
{
    public class Reader : IDisposable
    {
        private const string DataSetDelimiter = "    -1"; 
        
        private readonly StreamReader _inputReader;
        private readonly bool _leaveOpen;

        public Reader(string filePath) : this(new FileStream(filePath, FileMode.Open))
        {
        }
        
        public Reader(Stream inputStream, bool leaveOpen = false) 
            : this(new StreamReader(inputStream, Encoding.UTF8, true, 16384, leaveOpen))
        {
        }

        public Reader(StreamReader inputReader, bool leaveOpen = false)
        {
            _inputReader = inputReader;
            _leaveOpen = leaveOpen;
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
            if (!await FindNextDelimiter(l => {}))
            {
                return (NextDelimiterFound: false, Dataset: null);
            }
            if ((line = await _inputReader.ReadLineAsync()) == null) return (NextDelimiterFound: false, Dataset: null);
            
            var builder = UniversalFileDatasetBuilderFactory.Create(line);
            var nextDelimiterFound = await FindNextDelimiter(l => builder.AddLine(l));
            return (NextDelimiterFound: nextDelimiterFound, Dataset: builder.Build());
        }

        private async Task<bool> FindNextDelimiter(Action<string> lineAction)
        {
            string line;
            while ((line = await _inputReader.ReadLineAsync()) != null)
            {
                if (line.Equals(DataSetDelimiter, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                lineAction(line);
            }

            return false;
        }

        public void Dispose()
        {
            if (!_leaveOpen)
            {
                _inputReader.Dispose();
            }
        }
    }
}