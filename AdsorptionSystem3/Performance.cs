using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    static class PerformanceIndicator
    {
        static double[] bedInletTemperature = new double[Parameters.numOfTimeStep + 1];
        static double[] bedOutletTemperature = new double[Parameters.numOfTimeStep + 1];
        static double[] evaInletTemperature = new double[Parameters.numOfTimeStep + 1];
        static double[] evaOutletTemperature = new double[Parameters.numOfTimeStep + 1];
        static double[] rateOfCondensation = new double[Parameters.numOfTimeStep + 1];

        public static double energyInputPerBed;
        public static double energyInputTotal;
        public static double energyOutput;

        public static double COP, SCP, condensateCollected, averagedRateOfCondensation;

        public static void initialise()
        {
            bedInletTemperature[0] = Parameters.T0;
            bedOutletTemperature[0] = Parameters.T0;
            evaInletTemperature[0] = Parameters.T0;
            evaOutletTemperature[0] = Parameters.T0;            
        }

        //storing data
        public static void addBedValue(double inletTemp, double outletTemp, int timeStep)
        {
            bedInletTemperature[timeStep] = inletTemp;
            bedOutletTemperature[timeStep] = outletTemp;
        }

        public static void addEvaValue(double inletTemp, double outletTemp, int timeStep)
        {
            evaInletTemperature[timeStep] = inletTemp;
            evaOutletTemperature[timeStep] = outletTemp;
        }

        public static void addCondensateValue(double currentRateOfCondensation, int timeStep)
        {
            if(double.IsNaN(currentRateOfCondensation))
            {
                currentRateOfCondensation = 0;
            }

            rateOfCondensation[timeStep] = currentRateOfCondensation;
        }

        //calculate performance indicators
        public static void calculatePerformance(Bed[] bed, Evaporator eva)
        {
            calculateEnergyInput(bed[0].fluid);
            calculateEnergyInput(bed[1].fluid);
            calculateEnergyOutput(eva.fluid);

            calculateCOP();
            calculateSCP(bed);
            calculateCondensateCollected();
            calculateAveragedRateOfCondensation();
        }

        static void calculateEnergyInput(Fluid fluid)
        {
            double massFlowRate = fluid.density * Math.PI * Math.Pow((Parameters.numOfRNodes_bedFluid * Parameters.deltaR_bed), 2) * Parameters.fluidVelocity_bed;
            double deltaTemperature;
            double sumOfQ = 0;
            double sumOfDeltat = 0;
            double pressureDropFactor = Parameters.pressureDropFactor_bed;
            for (int i = 0; i < Parameters.numOfTimeStep; i++)
            {
                deltaTemperature = bedOutletTemperature[i] - bedInletTemperature[i];
                sumOfQ += pressureDropFactor * massFlowRate * fluid.heatCapacity * deltaTemperature * Parameters.deltat;
                sumOfDeltat += Parameters.deltat;
            }

            energyInputPerBed = (sumOfQ * Parameters.numOfParallelTube_bed) / sumOfDeltat;
            energyInputTotal += energyInputPerBed;
        }

        static void calculateEnergyOutput(Fluid fluid)
        {
            double massFlowRate = fluid.density * Math.PI * Math.Pow((Parameters.numOfRNodes_evaFluid * Parameters.deltaR_eva), 2) * Parameters.fluidVelocity_eva;
            double deltaTemperature;
            double sumOfQ = 0;
            double sumOfDeltat = 0;
            for (int i = 0; i < Parameters.numOfTimeStep; i++)
            {
                deltaTemperature = evaOutletTemperature[i] - evaInletTemperature[i];
                sumOfQ += massFlowRate * fluid.heatCapacity * deltaTemperature * Parameters.deltat;
                sumOfDeltat += Parameters.deltat;
            }

            energyOutput = (sumOfQ * Parameters.numOfParallelTube_eva) / sumOfDeltat;
        }

        static void calculateCOP()
        {
            Fluid fluid = new Fluid(FluidMaterial.Water);

            //calculateEnergyInput(fluid);
            //calculateEnergyOutput(fluid);

            COP = Math.Abs(energyOutput / energyInputTotal);
        }

        static void calculateSCP(Bed[] bed)
        {
            double totalAdsorbentMass = 0;
            for (int i = 0; i < Parameters.numOfBed; i++)
            {
                totalAdsorbentMass += bed[i].adsorbentMass;
            }

            SCP = Math.Abs(energyOutput) / (totalAdsorbentMass);
        }

        static void calculateCondensateCollected()
        {
            double sum = 0;
            for(int i = 0; i < Parameters.numOfTimeStep; i++)
            {
                sum += Parameters.numOfParallelTube_cond * rateOfCondensation[i] * Parameters.deltat;
            }

            condensateCollected = sum;
        }

        static void calculateAveragedRateOfCondensation()
        {
            double sumOfRateOfCondensation = 0;
            double sumOfDeltat = 0;

            for (int i = 0; i < Parameters.numOfTimeStep; i++)
            {
                sumOfRateOfCondensation += rateOfCondensation[i] * Parameters.deltat;
                sumOfDeltat += Parameters.deltat;
            }

            averagedRateOfCondensation = sumOfRateOfCondensation / sumOfDeltat;
        }

        public static void exportPlot(string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                row = new CsvRow();
                row.Add("Average Pressure vs Time");
                writer.WriteRow(row);

                row = new CsvRow();
                row.Add("time step");
                row.Add("delta t");
                row.Add("rate of condensation");
                writer.WriteRow(row);

                for (int t = 0; t < rateOfCondensation.Length; t++)
                {
                    row = new CsvRow();
                    row.Add(t.ToString());
                    row.Add((Parameters.deltat).ToString());
                    row.Add(rateOfCondensation[t].ToString());
                    writer.WriteRow(row);
                }
            }
        }
    }
}
