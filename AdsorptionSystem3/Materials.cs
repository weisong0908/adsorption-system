using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    class Material
    {
        public MaterialType materialType = MaterialType.NA;

        public double density, heatCapacity, thermalConductivity;
    }
    enum MaterialType { NA, Tube, Adsorbent, Adsorbate, GaseousFluid, HeatExchangeFluid };

    class Fluid : Material
    {
        public FluidMaterial fluidMaterial;

        public double dynamicViscosity;

        public Fluid(FluidMaterial fluidMaterial)
        {
            this.fluidMaterial = fluidMaterial;

            switch (fluidMaterial)
            {
                case FluidMaterial.Water:
                    density = 998.2;//kg/m3
                    heatCapacity = 4200;//J/kgK
                    thermalConductivity = 0.6;//W/mK
                    dynamicViscosity = 0.000798;//Ns/m2 at 30 degC
                    break;
                default:
                    Console.WriteLine("no material is chosen for heat exchange fluid");
                    Console.Read();
                    break;
            }
        }
    }
    enum FluidMaterial { NA, Water };

    class Tube : Material
    {
        public TubeMaterial tubeMaterial;

        public Tube(TubeMaterial tubeMaterial)
        {
            this.tubeMaterial = tubeMaterial;

            switch (tubeMaterial)
            {
                case TubeMaterial.Copper:
                    density = 8700;//kg/m3
                    heatCapacity = 385;//J/kgK
                    thermalConductivity = 400;//W/mK
                    break;
                default:
                    Console.WriteLine("no material is chosen for tube");
                    Console.Read();
                    break;
            }
        }

    }
    enum TubeMaterial { NA, Copper };

    class Adsorbent : Material
    {
        public AdsorbentMaterial adsorbentMaterial;

        public double porosity;
        public double permeability;
        public double adsorbentRadius;
        public double meanFreePath;

        public Adsorbent(AdsorbentMaterial adsorbentMaterial)
        {
            this.adsorbentMaterial = adsorbentMaterial;

            switch (adsorbentMaterial)
            {
                case AdsorbentMaterial.SilicaGel:
                    density = 800;//kg/m3
                    heatCapacity = 921;//J/kgK
                    thermalConductivity = 0.198;//W/mK //0.198
                    porosity = 0.5;
                    permeability = 0.2;
                    adsorbentRadius = 1e-3;// 1.7e-4;// m
                    meanFreePath = 1e-2;//m 0.01 to 0.1 miu m
                    break;
                default:
                    Console.WriteLine("no material is chosen for adsorbent");
                    Console.Read();
                    break;
            }
        }
    }
    enum AdsorbentMaterial { NA, SilicaGel };

    class Adsorbate : Material
    {
        public AdsorbateMaterial adsorbateMaterial;

        public double molarMass;
        public double molarGasConstant;
        public double specificGasConstant;
        public double latentHeat;
        public double sigma, omega;
        public double porosity;
        public double dynamicViscosity;

        public double density_condensed;
        public double thermalConductivity_condensed;
        public double heatCapacity_condensed;
        public double dynamicViscosity_condensed;

        public Adsorbate(AdsorbateMaterial adsorbateMaterial)
        {
            this.adsorbateMaterial = adsorbateMaterial;

            switch (adsorbateMaterial)
            {
                case AdsorbateMaterial.Water:
                    density = 0.03;//kg/m3 or =0.0022*P0/T0
                    heatCapacity = 1850;// 913;//J/kgK
                    thermalConductivity = 0.016;//W/mK
                    molarMass = 18.01528;//g/mol 0.01801528 kg/mol
                    molarGasConstant = 8.314;//J/molK
                    specificGasConstant = molarGasConstant / (molarMass / 1000);//J/kgK
                    latentHeat = 2264760;//2264.76 kJ/kg
                    sigma = 2.725;//Angstrom
                    omega = 1.725;
                    porosity = 1;
                    dynamicViscosity = 0.0001;// 0.00001;//kg/ms
                    density_condensed = 1000;//kg/m3
                    thermalConductivity_condensed = 0.6;//W/mK
                    heatCapacity_condensed = 4200;//J/kgK
                    dynamicViscosity_condensed = 0.000798;//Ns/m2 at 30 degC
                    break;
                default:
                    Console.WriteLine("no material is chosen for adsorbate");
                    Console.Read();
                    break;
            }
        }
    }
    enum AdsorbateMaterial { NA, Water };

    class WorkingPair : Material
    {
        public double qm;
        public double K0;
        public double Ds0;
        public double activationEnergy;
        public double heatOfAdsorption;

        public WorkingPair(AdsorbentMaterial adsorbentMaterial, AdsorbateMaterial adsorbateMaterial)
        {
            switch (adsorbateMaterial)
            {
                case AdsorbateMaterial.Water:
                    switch (adsorbentMaterial)
                    {
                        //silica gel - water
                        case AdsorbentMaterial.SilicaGel:
                            qm = 0.40;// 0.4;
                            K0 = 7.3e-13;
                            Ds0 = 2.54e-4;//m2/s
                            activationEnergy = 4.2e4;//J/mol
                            heatOfAdsorption = 2800000;//J/Kg
                            break;
                        default:
                            Console.WriteLine("no material is chosen for working pair");
                            Console.Read();
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("no material is chosen for working pair");
                    Console.Read();
                    break;
            }
        }
    }
}