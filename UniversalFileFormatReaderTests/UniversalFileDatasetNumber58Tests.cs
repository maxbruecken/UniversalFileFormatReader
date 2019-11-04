using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using UniversalFileFormatReader;

namespace UniversalFileFormatReaderTests
{
    public class UniversalFileDatasetNumber58Tests
    {
        #region TestData
        private const string FileContent = @"    -1
    58
vx;435465
RMX-gfdghfh-565676-xxx-788799
19-09-25T07:13:58Z
3465678-XXX
PwrAvg;kW
    0         0    0         0 AP                 0   0 NONE               0   0
         4         1         0  0.00000E+00  0.00000E+00  0.00000E+00
        17    0    0    0 Time                 s                   
         1    0    0    0 PwrAvg               kW                  
         0    0    0    0 NONE                 NONE                
         0    0    0    0 NONE                 NONE                
  0.000000000000E+00  2.137199951172E+03
    -1
    -1
    58
vx;435465
RMX-gfdghfh-565676-xxx-788799
19-09-25T07:13:58Z
3465678-XXX
GnSpdAvg;Hz
    0         0    0         0 CALC               0   0 NONE               0   0
         4         1         0  0.00000E+00  0.00000E+00  0.00000E+00
        17    0    0    0 Time                 s                   
         1    0    0    0 GnSpdAvg             Hz                  
         0    0    0    0 NONE                 NONE                
         0    0    0    0 NONE                 NONE                
  0.000000000000E+00  2.396208953857E+01
    -1
    -1
    58
vx;435465
RMX-gfdghfh-565676-xxx-788799
19-09-25T07:13:58Z
3465678-XXX
GnDe;0,0102;m/s2
    0         0    0         0 Channel_00         0   0 NONE               0   0
         4        40         1  0.00000E+00  3.90625E-05  0.00000E+00
        17    0    0    0 Time                 s                   
         1    0    0    0 Generator Drive End  m/s2                
         0    0    0    0 NONE                 NONE                
         0    0    0    0 NONE                 NONE                
  1.258055743049E+03  1.258701997645E+03  1.254622515510E+03  1.263993207146E+03
  1.257611443015E+03  1.264316334444E+03  1.249452478745E+03  1.256076588350E+03
  1.253572351792E+03  1.260277243222E+03  1.259832943187E+03  1.264397116268E+03
  1.254784079159E+03  1.256440106560E+03  1.254178215476E+03  1.260519588695E+03
  1.255753461052E+03  1.269244025735E+03  1.253693524529E+03  1.255268770106E+03
  1.252764533548E+03  1.259752161363E+03  1.254662906422E+03  1.264841416303E+03
  1.263185388902E+03  1.254864860983E+03  1.260640761432E+03  1.258176915786E+03
  1.258903952206E+03  1.258701997645E+03  1.261933270623E+03  1.255228379193E+03
  1.250866160673E+03  1.263791252585E+03  1.251068115234E+03  1.265245325425E+03
  1.256965188419E+03  1.261771706974E+03  1.247958014993E+03  1.262741088867E+03
    -1
    -1
    58
vx;435465
RMX-gfdghfh-565676-xxx-788799
19-09-25T07:13:58Z
3465678-XXX
GnDe1;0,0102;m/s2
    0         0    0         0 Channel_01         0   0 NONE               0   0
         4        20         0  0.00000E+00  0.00000E+00  0.00000E+00
        17    0    0    0 Time                 s                   
         1    0    0    0 Generator Drive End1 m/s2                
         0    0    0    0 NONE                 NONE                
         0    0    0    0 NONE                 NONE                
  0.00000E+00  1.258055743049E+03  1.00000E+00  1.258701997645E+03
  2.00000E+00  1.257611443015E+03  3.00000E+00  1.264316334444E+03
  4.00000E+00  1.253572351792E+03  5.00000E+00  1.260277243222E+03
  6.00000E+00  1.254784079159E+03  7.00000E+00  1.256440106560E+03
  8.00000E+00  1.255753461052E+03  9.00000E+00  1.269244025735E+03
  1.00000E+01  1.252764533548E+03  1.10000E+01  1.259752161363E+03
  1.20000E+01  1.263185388902E+03  1.30000E+01  1.254864860983E+03
  1.40000E+01  1.258903952206E+03  1.50000E+01  1.258701997645E+03
  1.60000E+01  1.250866160673E+03  1.70000E+01  1.263791252585E+03
  1.80000E+01  1.256965188419E+03  1.90000E+01  1.261771706974E+03
    -1";
        #endregion

        [Test]
        public async Task CanReadAllDatasetsFromFile()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = await reader.ReadAsync();

            datasets.Should().HaveCount(4);
        }

        [Test]
        public async Task AllReadDatasetsAreOfTypeNumber58()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = await reader.ReadAsync();

            datasets.Should().AllBeOfType<UniversalFileDatasetNumber58>();
        }

        [Test]
        public async Task HeadersAreReadCorrectly()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.Headers.Length == 5 && x.Headers.All(h => !string.IsNullOrWhiteSpace(h)));
            datasets.ElementAt(0).Headers.Should().BeEquivalentTo(new[] {"vx;435465", "RMX-gfdghfh-565676-xxx-788799", "19-09-25T07:13:58Z", "3465678-XXX", "PwrAvg;kW"});
        }

        [Test]
        public async Task FunctionIdentificationIsReadCorrectly()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.ElementAt(0).FunctionIdentification.Type.Should().Be(FunctionIdentificationType.GeneralOrUnknown);
            datasets.ElementAt(0).FunctionIdentification.Number.Should().Be(0);
            datasets.ElementAt(0).FunctionIdentification.VersionOrSequenceNumber.Should().Be(0);
            datasets.ElementAt(0).FunctionIdentification.ResponseEntityName.Should().Be("AP");
            datasets.ElementAt(0).FunctionIdentification.ReferenceEntityName.Should().Be("NONE");
        }

        [Test]
        public async Task DataTypeIsReadCorrectly()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.ElementAt(0).DataType.Should().Be(UniversalFileDatasetNumber58DataType.RealDouble);
            datasets.ElementAt(0).AbscissaIsUneven.Should().BeTrue();
            datasets.ElementAt(1).DataType.Should().Be(UniversalFileDatasetNumber58DataType.RealDouble);
            datasets.ElementAt(1).AbscissaIsUneven.Should().BeTrue();
            datasets.ElementAt(2).DataType.Should().Be(UniversalFileDatasetNumber58DataType.RealDouble);
            datasets.ElementAt(2).AbscissaIsUneven.Should().BeFalse();
            datasets.ElementAt(3).DataType.Should().Be(UniversalFileDatasetNumber58DataType.RealDouble);
            datasets.ElementAt(3).AbscissaIsUneven.Should().BeTrue();
        }

        [Test]
        public async Task DataCountIsReadCorrectly()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.ElementAt(0).DataCount.Should().Be(1);
            datasets.ElementAt(1).DataCount.Should().Be(1);
            datasets.ElementAt(2).DataCount.Should().Be(40);
            datasets.ElementAt(3).DataCount.Should().Be(20);
        }

        [Test]
        public async Task AbscissaDataCharacteristicsAreReadCorrectly()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.AbscissaDataCharacteristics.DataType == AxisDataType.Time);
            datasets.Should().OnlyContain(x => x.AbscissaDataCharacteristics.Label == "Time");
            datasets.Should().OnlyContain(x => x.AbscissaDataCharacteristics.Unit == "s");
        }

        [Test]
        public async Task OrdinateDataCharacteristicsAreReadCorrectly()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.OrdinateDataCharacteristics.DataType == AxisDataType.General && x.OrdinateDataCharacteristics.LengthUnitExponent == 0 &&
                                               x.OrdinateDataCharacteristics.ForceUnitExponent == 0 && x.OrdinateDataCharacteristics.TemperatureUnitExponent == 0);
            datasets.ElementAt(0).OrdinateDataCharacteristics.Label.Should().Be("PwrAvg");
            datasets.ElementAt(3).OrdinateDataCharacteristics.Label.Should().Be("Generator Drive End1");
        }

        [Test]
        public async Task OrdinateDenominatorDataCharacteristicsAreReadCorrectly()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.OrdinateDenominatorDataCharacteristics.DataType == AxisDataType.Unknown);
            datasets.Should().OnlyContain(x => x.OrdinateDenominatorDataCharacteristics.Label == "NONE");
            datasets.Should().OnlyContain(x => x.OrdinateDenominatorDataCharacteristics.Unit == "NONE");
        }

        [Test]
        public async Task ZAxisDataCharacteristicsAreReadCorrectly()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.ZAxisDataCharacteristics.DataType == AxisDataType.Unknown);
            datasets.Should().OnlyContain(x => x.ZAxisDataCharacteristics.Label == "NONE");
            datasets.Should().OnlyContain(x => x.ZAxisDataCharacteristics.Unit == "NONE");
        }

        [Test]
        public async Task RealDataIsReadCorrectly()
        {
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            var data = datasets.ElementAt(2).Data;
            data.Should().HaveCount(40);
            data.First().Index.Should().Be(0);
            data.First().RealPart.Should().BeApproximately(1.258055743049e3, 1e-5);
            data.First().ImaginaryPart.Should().Be(double.NaN);
            data.Last().Index.Should().BeApproximately(0.0015234375, 1e-5);
            data.Last().RealPart.Should().BeApproximately(1.262741088867e3, 1e-5);
            data.Last().ImaginaryPart.Should().Be(double.NaN);
            
            data = datasets.ElementAt(3).Data;
            data.Should().HaveCount(20);
            data.First().Index.Should().Be(0);
            data.First().RealPart.Should().BeApproximately(1.258055743049e3, 1e-5);
            data.First().ImaginaryPart.Should().Be(double.NaN);
            data.Last().Index.Should().BeApproximately(19, 1e-5);
            data.Last().RealPart.Should().BeApproximately(1.261771706974e3, 1e-5);
            data.Last().ImaginaryPart.Should().Be(double.NaN);
        }
    }
}