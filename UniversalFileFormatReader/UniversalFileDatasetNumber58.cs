using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversalFileFormatReader
{
    public class UniversalFileDatasetNumber58 : UniversalFileDataset
    {
        public UniversalFileDatasetNumber58() : base(UniversalFileDatasetNumber.Number58)
        {
        }
        
        public string[] Headers { get; set; } = new string[5];
        
        public UniversalFileDatasetNumber58DataType DataType { get; set; }
        
        public long DataCount { get; set; }
        
        public bool AbscissaIsUneven { get; set; }
        
        public double AbscissaMinimum { get; set; }
        
        public double AbscissaSpacing { get; set; }
        
        public double ZAxisValue { get; set; }
        
        public ICollection<UniversalFileDatasetNumber58DataPoint> Data { get; } = new List<UniversalFileDatasetNumber58DataPoint>();
    }

    public class UniversalFileDatasetNumber58DataPoint
    {
        public UniversalFileDatasetNumber58DataPoint(double index, double realPart, double imaginaryPart)
        {
            Index = index;
            RealPart = realPart;
            ImaginaryPart = imaginaryPart;
        }
        
        public double Index { get; }
        
        public double RealPart { get; }
        
        public double ImaginaryPart { get; }
    }

    public enum UniversalFileDatasetNumber58DataType
    {
        RealSingle = 2, RealDouble = 4, ComplexSingle = 5, ComplexDouble = 6
    }
}