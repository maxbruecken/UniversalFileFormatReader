﻿using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using UniversalFileFormatReader;

namespace UniversalFileFormatReaderTests
{
    public class UniversalFileDatasetNumber58Tests
    {
        [Test]
        public async Task CanReadAllDatasetsFromFile()
        {
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = await reader.ReadAsync();

            datasets.Should().HaveCount(4);
        }

        private static FileStream GetTestDataStream(string testDataFileName = "uff_58.uff")
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
            datasets.ElementAt(0).Headers.Should().BeEquivalentTo(new[] {"vx;435465", "RMX-gfdghfh-565676-xxx-788799", "19-09-25T07:13:58Z", "3465678-XXX", "PwrAvg;kW"});
        }

        [Test]
        public async Task FunctionIdentificationIsReadCorrectly()
        {
            await using var stream = GetTestDataStream();
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
            await using var stream = GetTestDataStream();
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
            await using var stream = GetTestDataStream();
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
            await using var stream = GetTestDataStream();
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.Should().OnlyContain(x => x.AbscissaDataCharacteristics.DataType == AxisDataType.Time);
            datasets.Should().OnlyContain(x => x.AbscissaDataCharacteristics.Label == "Time");
            datasets.Should().OnlyContain(x => x.AbscissaDataCharacteristics.Unit == "s");
        }

        [Test]
        public async Task OrdinateDataCharacteristicsAreReadCorrectly()
        {
            await using var stream = GetTestDataStream();
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
        
        [Test]
        public async Task CanReadAllDatasetsFromFileWithLineFeedsOnly()
        {
            await using var stream = GetTestDataStream("uff_58_lf_only.uff");
            var reader = new Reader(stream);

            var datasets = await reader.ReadAsync();

            datasets.Should().HaveCount(4);
        }
        
        [Test]
        public async Task CanReadAllDatasetsFromFileWithCarriageReturnsOnly()
        {
            await using var stream = GetTestDataStream("uff_58_cr_only.uff");
            var reader = new Reader(stream);

            var datasets = await reader.ReadAsync();

            datasets.Should().HaveCount(4);
        }

        [Test]
        public async Task ComplexDataIsReadCorrectly()
        {
            await using var stream = GetTestDataStream("uff_58_complex_single_even.uff");
            var reader = new Reader(stream);

            var datasets = (await reader.ReadAsync()).OfType<UniversalFileDatasetNumber58>();

            datasets.ElementAt(0).DataType.Should().Be(UniversalFileDatasetNumber58DataType.ComplexSingle);
            var data = datasets.ElementAt(0).Data;
            data.Should().HaveCount(401);
            data.First().Index.Should().Be(0);
            data.First().RealPart.Should().BeApproximately(-6.36716e-4, 1e-8);
            data.First().ImaginaryPart.Should().Be(0);
            data.Last().Index.Should().BeApproximately(12.5, 1e-5);
            data.Last().RealPart.Should().BeApproximately(1.40075e-6, 1e-8);
            data.Last().ImaginaryPart.Should().BeApproximately(1.43056e-6, 1e-8);
        }
    }
}