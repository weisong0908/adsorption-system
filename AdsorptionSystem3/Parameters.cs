using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    static class Parameters
    {
        ////Bed
        public const int numOfBed = 2;
        //dimension
        public const double deltaR_bed = 0.001;
        public const double deltaZ_bed = 0.001;
        public const int numOfRNodes_bedFluid = 2;
        public const int numOfRNodes_bedTubeWall = 1;
        public const int numOfRNodes_adsorbent = 12;
        public const int numOfRNodes_bedVoid = 7;
        public const int numOfZNodes_bedExcessTube = 20;
        public const int numOfZNodes_fin = 1;
        public const int numOfZNodes_adsorbent = 3;
        public const int numOfTubeSection = 24;
        public const int numOfParallelTube_bed = 30;//352;
        //material
        static public Fluid bedFluid = new Fluid(FluidMaterial.Water);
        static public Tube bedTube = new Tube(TubeMaterial.Copper);
        static public Adsorbent adsorbent = new Adsorbent(AdsorbentMaterial.SilicaGel);
        static public Adsorbate adsorbate = new Adsorbate(AdsorbateMaterial.Water);
        static public WorkingPair workingPair = new WorkingPair(adsorbent.adsorbentMaterial, adsorbate.adsorbateMaterial);

        ////Condenser
        public const double deltaR_cond = 0.001;
        public const double deltaZ_cond = 0.001;
        public const int numOfRNodes_condFluid = 2;
        public const int numOfRNodes_condTubeWall = 1;
        public const int numOfRNodes_condVoid = 15;//100;
        public const int numOfZNodes_cond = 100;//50;
        public const int numOfParallelTube_cond = 20;//24;
        //material
        static public Fluid condFluid = new Fluid(FluidMaterial.Water);
        static public Tube condTube = new Tube(TubeMaterial.Copper);

        ////Evaporator
        public const double deltaR_eva = 0.001;
        public const double deltaZ_eva = 0.001;
        public const int numOfRNodes_evaFluid = 2;//2;
        public const int numOfRNodes_evaTubeWall = 1;//1;
        public const int numOfRNodes_evaVoid = 15;// 15;
        public const int numOfZNodes_eva = 100;
        public const int numOfRNodes_eva2 = 80;
        public const int numOfZNodes_eva2 = 30;
        public const int numOfParallelTube_eva = 20;//20;
        //material
        static public Fluid evaFluid = new Fluid(FluidMaterial.Water);
        static public Tube evaTube = new Tube(TubeMaterial.Copper);

        ////Connecting vacuum pipes
        public const double pipeID_connectingPipe = 16e-3;//m 16mm
        public const double pipelength_connectingPipe = 0.5;//m

        ////System
        //temperature source
        public const double hotWaterTemperature = 80 + 273;//fluid temperature in K
        public const double coldWaterTemperature = 30 + 273;//fluid temperature in K
        public const double condWaterTemperature = 30 + 273;//fluid temperature in K
        public const double chilledWaterTemperature = 15 + 273;//fluid temperature in K
        //cycle time
        public const double switchingTime = 600;//s
        public const double sorptionTime = 3000;//s
        public const double cycleTime = switchingTime + sorptionTime;
        public const int numOfCycle = 5;
        public const double initialTime = 0;
        public const double finalTime = 2 * (switchingTime + sorptionTime) * numOfCycle;
        public const int timeStepPerSecond = 10;
        public const int numOfTimeStep = (int)(finalTime * timeStepPerSecond);
        public const double deltat = (finalTime - initialTime) / numOfTimeStep;

        ////solving accuracy
        public const double tolerance_temperature = 0.00001;
        public const double tolerance_density = 0.7;
        public const double relaxationFactor_temperature = 0.5;
        public const double relaxationFactor_density = 0.7;

        ////constants
        public const double fluidVelocity_bed =  0.6 / numOfParallelTube_bed;//m/s 0.45972
        public const double pressureDropFactor_bed = 0.7;
        public const double fluidVelocity_cond = 0.2 / numOfParallelTube_cond;//m/s
        public const double fluidVelocity_eva = 0.5288 / numOfParallelTube_eva;//m/s
        public const double T0 = 30 + 273;//K
        public const double P0_bed = 2000;//4000;//Pa
        public const double P0_cond = 3600;//Pa
        public const double P0_eva = 3600;//Pa
        public const double q0 = 0.2;
    }
}