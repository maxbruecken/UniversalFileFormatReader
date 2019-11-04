using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace UniversalFileFormatReader
{
    public class UniversalFileDatasetNumber58 : UniversalFileDataset
    {
        public UniversalFileDatasetNumber58() : base(UniversalFileDatasetNumber.Number58)
        {
        }
        
        public string[] Headers { get; } = new string[5];
        
        public FunctionIdentification FunctionIdentification { get; } = new FunctionIdentification();
        
        public UniversalFileDatasetNumber58DataType DataType { get; set; }
        
        public long DataCount { get; set; }
        
        public bool AbscissaIsUneven { get; set; }
        
        public double AbscissaMinimum { get; set; }
        
        public double AbscissaSpacing { get; set; }
        
        public double ZAxisValue { get; set; }

        public AxisDataCharacteristics AbscissaDataCharacteristics { get; } = new AxisDataCharacteristics();

        public AxisDataCharacteristics OrdinateDataCharacteristics { get; } = new AxisDataCharacteristics();
        
        public AxisDataCharacteristics OrdinateDenominatorDataCharacteristics { get; } = new AxisDataCharacteristics();
        
        public AxisDataCharacteristics ZAxisDataCharacteristics { get; } = new AxisDataCharacteristics();
        
        public ICollection<UniversalFileDatasetNumber58DataPoint> Data { get; } = new List<UniversalFileDatasetNumber58DataPoint>();
    }

    public class FunctionIdentification
    {
        public FunctionIdentificationType Type { get; set; }
        
        public int Number { get; set; }
        
        public int VersionOrSequenceNumber { get; set; }
        
        public string ResponseEntityName { get; set; }
        
        public int ResponseNode { get; set; }
        
        public int ResponseDirection { get; set; }
        
        public string ReferenceEntityName { get; set; }
        
        public int ReferenceNode { get; set; }
        
        public int ReferenceDirection { get; set; }
    }

    public class AxisDataCharacteristics
    {
        public AxisDataType DataType { get; set; }
        
        public double LengthUnitExponent { get; set; }
        
        public double ForceUnitExponent { get; set; }
        
        public double TemperatureUnitExponent { get; set; }
        
        public string Label { get; set; }
        
        public string Unit { get; set; }
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

    public enum AxisDataType
    {
        Unknown = 0, General = 1, Stress = 2, Strain = 3, Temperature = 5, HeatFlux = 6, Displacement = 8, ReactionForce = 9, Velocity = 11, Acceleration = 12, 
        ExcitationForce = 13, Pressure = 15, Mass = 16, Time = 17, Frequency = 18, Rpm = 19, Order = 20, SoundPressure = 21, SoundIntensity = 22, SoundPower = 23
    }

    public enum FunctionIdentificationType
    {
        GeneralOrUnknown = 0, TimeResponse = 1, AutoSpectrum = 2, CrossSpectrum = 3, FrequencyResponseFunction = 4, Transmissibility = 5, Coherence = 6, AutoCorrelation = 7,
        CrossCorrelation = 8, PowerSpectralDensity = 9, EnergySpectralDensity = 10, ProbabilityDensityFunction = 11, Spectrum = 12, CumulativeFrequencyDistribution = 13,
        PeaksValley = 14, StressCycles = 15, StrainCycles = 16, Orbit = 17, ModeIndicatorFunction = 18, ForcePattern = 19, PartialPower = 20, PartialCoherence = 21, Eigenvalue = 22,
        Eigenvector = 23, ShockResponseSpectrum = 24, FiniteImpulseResponseFilter = 25, MultipleCoherence = 26, OrderFunction = 27, PhaseCompensation = 28
    }
}