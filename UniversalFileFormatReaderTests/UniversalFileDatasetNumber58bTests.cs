using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using UniversalFileFormatReader;

namespace UniversalFileFormatReaderTests
{
    public class UniversalFileDatasetNumber58bTests
    {
        [Test]
        public async Task CanReadDatasetFromFile()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = await reader.ReadAsync();

            datasets.Should().HaveCount(1);
        }

        private static FileStream GetTestDataStream(string testDataFileName = "uff_58b.uff")
        {
            return new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", testDataFileName), FileMode.Open);
        }

        [Test]
        public async Task AllReadDatasetsAreOfTypeNumber58()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = await reader.ReadAsync();

            datasets.Should().AllBeOfType<UniversalFileDatasetNumber58>();
        }

        [Test]
        public async Task HeadersAreReadCorrectly()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.Headers.Length == 5 && x.Headers.All(h => !string.IsNullOrWhiteSpace(h)));
            datasets.ElementAt(0).Headers.Should().BeEquivalentTo(new[] {"Mic 01.0Scalar", "NONE", "18-Apr-16 13:49:58", "NONE", "NONE"});
        }

        [Test]
        public async Task FunctionIdentificationIsReadCorrectly()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.ElementAt(0).FunctionIdentification.Type.Should().Be(FunctionIdentificationType.TimeResponse);
            datasets.ElementAt(0).FunctionIdentification.Number.Should().Be(0);
            datasets.ElementAt(0).FunctionIdentification.VersionOrSequenceNumber.Should().Be(0);
            datasets.ElementAt(0).FunctionIdentification.ResponseEntityName.Should().Be("Mic 01");
            datasets.ElementAt(0).FunctionIdentification.ReferenceEntityName.Should().Be("NONE");
        }

        [Test]
        public async Task DataTypeIsReadCorrectly()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.ElementAt(0).DataType.Should().Be(UniversalFileDatasetNumber58DataType.RealSingle);
            datasets.ElementAt(0).AbscissaIsUneven.Should().BeFalse();
        }

        [Test]
        public async Task DataCountIsReadCorrectly()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.ElementAt(0).DataCount.Should().Be(79292);
        }

        [Test]
        public async Task AbscissaDataCharacteristicsAreReadCorrectly()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.AbscissaDataCharacteristics.DataType == AxisDataType.Time);
            datasets.Should().OnlyContain(x => x.AbscissaDataCharacteristics.Label == "time");
            datasets.Should().OnlyContain(x => x.AbscissaDataCharacteristics.Unit == "s");
        }

        [Test]
        public async Task OrdinateDataCharacteristicsAreReadCorrectly()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.OrdinateDataCharacteristics.DataType == AxisDataType.SoundPressure && x.OrdinateDataCharacteristics.LengthUnitExponent == 0 &&
                                               x.OrdinateDataCharacteristics.ForceUnitExponent == 0 && x.OrdinateDataCharacteristics.TemperatureUnitExponent == 0);
            datasets.ElementAt(0).OrdinateDataCharacteristics.Label.Should().Be("Pressure");
        }

        [Test]
        public async Task OrdinateDenominatorDataCharacteristicsAreReadCorrectly()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.OrdinateDenominatorDataCharacteristics.DataType == AxisDataType.Unknown);
            datasets.Should().OnlyContain(x => x.OrdinateDenominatorDataCharacteristics.Label == "NONE");
            datasets.Should().OnlyContain(x => x.OrdinateDenominatorDataCharacteristics.Unit == "NONE");
        }

        [Test]
        public async Task ZAxisDataCharacteristicsAreReadCorrectly()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.ZAxisDataCharacteristics.DataType == AxisDataType.Unknown);
            datasets.Should().OnlyContain(x => x.ZAxisDataCharacteristics.Label == "NONE");
            datasets.Should().OnlyContain(x => x.ZAxisDataCharacteristics.Unit == "NONE");
        }

        [Test]
        public async Task RealDataIsReadCorrectly()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            var data = datasets.ElementAt(0).Data;
            data.Should().HaveCount(79292);
            data.First().Index.Should().Be(0);
            data.First().RealPart.Should().BeApproximately(-0.0147552601993084, 1e-8);
            data.First().ImaginaryPart.Should().Be(double.NaN);
            data.Last().Index.Should().BeApproximately(1.2098855108, 1e-8);
            data.Last().RealPart.Should().BeApproximately(-0.00431468896567822, 1e-8);
            data.Last().ImaginaryPart.Should().Be(double.NaN);
        }

        [Test]
        public async Task RealDataIsReadCorrectlyForRealDoublePrecision()
        {
            await using var stream = GetTestDataStream("uff_58b_real_double_even.uff");
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            var data = datasets.ElementAt(0).Data;
            data.Should().HaveCount(250);
            data.First().Index.Should().Be(0);
            data.First().RealPart.Should().BeApproximately(0, 1e-8);
            data.First().ImaginaryPart.Should().Be(double.NaN);
            data.Last().Index.Should().BeApproximately(2.49, 1e-8);
            data.Last().RealPart.Should().BeApproximately(0.309019356966019, 1e-8);
            data.Last().ImaginaryPart.Should().Be(double.NaN);
        }

        [Test]
        public async Task ComplexDataIsReadCorrectly()
        {
            await using var stream = GetTestDataStream("uff_58b_complex_single_even.uff");
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.ElementAt(0).DataType.Should().Be(UniversalFileDatasetNumber58DataType.ComplexSingle);
            var data = datasets.ElementAt(0).Data;
            data.Should().HaveCount(1601);
            data.First().Index.Should().Be(0);
            data.First().RealPart.Should().BeApproximately(0.41495406627655, 1e-8);
            data.First().ImaginaryPart.Should().Be(0);
            data.Last().Index.Should().BeApproximately(20000, 1e-8);
            data.Last().RealPart.Should().BeApproximately(-0.331138014793396, 1e-8);
            data.Last().ImaginaryPart.Should().BeApproximately(-0.48037052154541, 1e-8);
        }
    }
}