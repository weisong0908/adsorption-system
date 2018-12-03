using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    static class Report
    {
        public static double[] adsorbentMass = new double[Parameters.numOfBed];

        public static void initialise(Bed[] bed)
        {
            //calculate adsorbent mass for each bed
            for (int i = 0; i < Parameters.numOfBed; i++)
            {
                adsorbentMass[i] = bed[i].adsorbentMass;
            }
        }

        public static void publish(string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                //general section
                row = new CsvRow();
                row.Add("Parameter");
                row.Add("Value");
                writer.WriteRow(row);
                row = new CsvRow();
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("Operation Conditions");
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("Switching time (s)");
                row.Add(Parameters.switchingTime.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("Sorption time (s)");
                row.Add(Parameters.sorptionTime.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("number of cycles");
                row.Add(Parameters.numOfCycle.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("total time (s)");
                row.Add(Parameters.finalTime.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("delta T (s)");
                row.Add(Parameters.deltat.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("heating temperature (degC)");
                row.Add((Parameters.hotWaterTemperature - 273).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("cooling temperature (degC)");
                row.Add((Parameters.coldWaterTemperature - 273).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("chilled temperature (degC)");
                row.Add((Parameters.chilledWaterTemperature - 273).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                writer.WriteRow(row);

                //bed section
                row = new CsvRow();
                row.Add("Bed Specifications");
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("delta r (mm)");
                row.Add((Parameters.deltaR_bed * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("delta z (mm)");
                row.Add((Parameters.deltaZ_bed * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube ID (mm)");
                row.Add((Parameters.numOfRNodes_bedFluid * 2 * Parameters.deltaR_bed * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube OD (mm)");
                row.Add(((Parameters.numOfRNodes_bedFluid + Parameters.numOfRNodes_bedTubeWall) * 2 * Parameters.deltaR_bed * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube length (mm)");
                row.Add((((Parameters.numOfZNodes_fin + Parameters.numOfZNodes_adsorbent) * Parameters.numOfTubeSection + Parameters.numOfZNodes_fin) * Parameters.deltaZ_bed * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("number of parallel tube");
                row.Add(Parameters.numOfParallelTube_bed.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube material");
                row.Add(Parameters.bedTube.tubeMaterial.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("fin length (mm)");
                row.Add(((Parameters.numOfRNodes_adsorbent) * Parameters.deltaR_bed * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("fin spacing (mm)");
                row.Add(((Parameters.numOfZNodes_adsorbent) * Parameters.deltaZ_bed * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("fin thickness (mm)");
                row.Add(((Parameters.numOfZNodes_fin) * Parameters.deltaZ_bed * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("number of fin in one tube");
                row.Add((Parameters.numOfTubeSection + 1).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("fin material");
                row.Add(Parameters.bedTube.tubeMaterial.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("void spacing (mm)");
                row.Add(((Parameters.numOfRNodes_bedVoid) * Parameters.deltaR_bed * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("mass flow rate (kg/s)");
                row.Add((Parameters.bedFluid.density * Math.PI * Math.Pow((Parameters.numOfRNodes_bedFluid * Parameters.deltaR_bed), 2) * Parameters.fluidVelocity_bed * Parameters.numOfParallelTube_bed).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                writer.WriteRow(row);

                //condenser section
                row = new CsvRow();
                row.Add("Condenser Specifications");
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("delta r (mm)");
                row.Add((Parameters.deltaR_cond * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("delta z (mm)");
                row.Add((Parameters.deltaZ_cond * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube ID (mm)");
                row.Add((Parameters.numOfRNodes_condFluid * 2 * Parameters.deltaR_cond * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube OD (mm)");
                row.Add(((Parameters.numOfRNodes_condFluid + Parameters.numOfRNodes_condTubeWall) * 2 * Parameters.deltaR_cond * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube length (mm)");
                row.Add(((Parameters.numOfZNodes_cond) * Parameters.deltaZ_cond * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("number of parallel tube");
                row.Add(Parameters.numOfParallelTube_cond.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube material");
                row.Add(Parameters.condTube.tubeMaterial.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("void spacing (mm)");
                row.Add(((Parameters.numOfRNodes_condVoid) * Parameters.deltaR_cond * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("mass flow rate (kg/s)");
                row.Add((Parameters.condFluid.density * Math.PI * Math.Pow((Parameters.numOfRNodes_condFluid * Parameters.deltaR_cond), 2) * Parameters.fluidVelocity_cond * Parameters.numOfParallelTube_cond).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                writer.WriteRow(row);

                //evaporator section
                row = new CsvRow();
                row.Add("Evaporator Specifications");
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("delta r (mm)");
                row.Add((Parameters.deltaR_eva * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("delta z (mm)");
                row.Add((Parameters.deltaZ_eva * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube ID (mm)");
                row.Add((Parameters.numOfRNodes_evaFluid * 2 * Parameters.deltaR_eva * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube OD (mm)");
                row.Add(((Parameters.numOfRNodes_evaFluid + Parameters.numOfRNodes_evaTubeWall) * 2 * Parameters.deltaR_eva * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube length (mm)");
                row.Add(((Parameters.numOfZNodes_eva) * Parameters.deltaZ_eva * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("number of parallel tube");
                row.Add(Parameters.numOfParallelTube_eva.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("tube material");
                row.Add(Parameters.evaTube.tubeMaterial.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("void spacing (mm)");
                row.Add(((Parameters.numOfRNodes_evaVoid) * Parameters.deltaR_eva * 1000).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("mass flow rate (kg/s)");
                row.Add((Parameters.evaFluid.density * Math.PI * Math.Pow((Parameters.numOfRNodes_evaFluid * Parameters.deltaR_eva), 2) * Parameters.fluidVelocity_eva * Parameters.numOfParallelTube_eva).ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                writer.WriteRow(row);

                //adsorbent section
                row = new CsvRow();
                row.Add("Adsorbent Specifications");
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("adsorbent material");
                row.Add(Parameters.adsorbent.adsorbentMaterial.ToString());
                writer.WriteRow(row);
                for (int i = 0; i < Parameters.numOfBed; i++)
                {
                    row = new CsvRow();
                    row.Add("adsorbent mass of Bed " + (i + 1).ToString() + " (kg)");
                    row.Add(adsorbentMass[i].ToString());
                    writer.WriteRow(row);
                }
                row = new CsvRow();
                writer.WriteRow(row);

                //results
                row = new CsvRow();
                row.Add("Results");
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("Energy input (W)");
                row.Add(PerformanceIndicator.energyInputTotal.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("Energy output (W)");
                row.Add(PerformanceIndicator.energyOutput.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("COP");
                row.Add(PerformanceIndicator.COP.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("SCP");
                row.Add(PerformanceIndicator.SCP.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("Condensate collected (kg)");
                row.Add(PerformanceIndicator.condensateCollected.ToString());
                writer.WriteRow(row);
                row = new CsvRow();
                row.Add("Rate of condensation (kg/s)");
                row.Add(PerformanceIndicator.averagedRateOfCondensation.ToString());
                writer.WriteRow(row);
            }
        }
    }
}
