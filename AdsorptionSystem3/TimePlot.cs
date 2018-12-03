using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    class TimePlot
    {
        public double[] array_bed1, array_out, array_avg;

        public TimePlot()
        {
            array_bed1 = new double[Parameters.numOfTimeStep + 1];
            array_out = new double[Parameters.numOfTimeStep + 1];
            array_avg = new double[Parameters.numOfTimeStep + 1];
        }
    }

    class TemperaurePlot
    {
        public double[] array_bed1_inlet = new double[Parameters.numOfTimeStep + 1];
        public double[] array_bed1_outlet = new double[Parameters.numOfTimeStep + 1];
        public double[] array_bed2_inlet = new double[Parameters.numOfTimeStep + 1];
        public double[] array_bed2_outlet = new double[Parameters.numOfTimeStep + 1];
        public double[] array_cond_inlet = new double[Parameters.numOfTimeStep + 1];
        public double[] array_cond_outlet = new double[Parameters.numOfTimeStep + 1];
        public double[] array_eva_inlet = new double[Parameters.numOfTimeStep + 1];
        public double[] array_eva_outlet = new double[Parameters.numOfTimeStep + 1];

        public TemperaurePlot()
        {
            array_bed1_inlet[0] = Parameters.T0;
            array_bed1_outlet[0] = Parameters.T0;
            array_bed2_inlet[0] = Parameters.T0;
            array_bed2_outlet[0] = Parameters.T0;
            array_cond_inlet[0] = Parameters.T0;
            array_cond_outlet[0] = Parameters.T0;
            array_eva_inlet[0] = Parameters.T0;
            array_eva_outlet[0] = Parameters.T0;
        }

        public void addValue(Bed[] bed, Condenser cond, Evaporator eva, int timeStep)
        {
            double sum_in, sum_out;
            double value_in, value_out;

            //for beds
            for (int i = 0; i < Parameters.numOfBed; i++)
            {
                sum_in = 0;
                sum_out = 0;
                value_in = 0;
                value_out = 0;

                //inlet temperature
                for (int r = 0; r < bed[i].numOfRNodes_fluid; r++)
                {
                    sum_in += bed[i].T[r, 0].TP;
                }
                value_in = sum_in / bed[i].numOfRNodes_fluid;

                //outlet temperature
                for (int r = 0; r < bed[i].numOfRNodes_fluid; r++)
                {
                    sum_out += bed[i].T[r, (bed[i].numOfZNodes - 1)].TP;
                }
                value_out = sum_out / bed[i].numOfRNodes_fluid;

                switch (i)
                {
                    case 0:
                        array_bed1_inlet[timeStep] = value_in;
                        array_bed1_outlet[timeStep] = value_out;
                        break;
                    case 1:
                        array_bed2_inlet[timeStep] = value_in;
                        array_bed2_outlet[timeStep] = value_out;
                        break;
                    default:
                        Console.WriteLine("more beds are sued than declared");
                        Console.Read();
                        break;
                }
            }

            //for condenser
            sum_in = 0;
            sum_out = 0;
            value_in = 0;
            value_out = 0;

            //inlet temperature
            for (int r = 0; r < cond.numOfRNodes_fluid; r++)
            {
                sum_in += cond.T[r, 0].TP;
            }
            value_in = sum_in / cond.numOfRNodes_fluid;

            //outlet temperature
            for (int r = 0; r < cond.numOfRNodes_fluid; r++)
            {
                sum_out += cond.T[r, (cond.numOfZNodes - 1)].TP;
            }
            value_out = sum_out / cond.numOfRNodes_fluid;

            array_cond_inlet[timeStep] = value_in;
            array_cond_outlet[timeStep] = value_out;

            //for evaporator
            sum_in = 0;
            sum_out = 0;
            value_in = 0;
            value_out = 0;

            //inlet temperature
            for (int r = 0; r < eva.numOfRNodes_fluid; r++)
            {
                sum_in += eva.T[r, 0].TP;
            }
            value_in = sum_in / eva.numOfRNodes_fluid;

            //outlet temperature
            for (int r = 0; r < eva.numOfRNodes_fluid; r++)
            {
                sum_out += eva.T[r, (eva.numOfZNodes - 1)].TP;
            }
            value_out = sum_out / eva.numOfRNodes_fluid;

            array_eva_inlet[timeStep] = value_in;
            array_eva_outlet[timeStep] = value_out;

        }

        public void exportPlot(string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                row = new CsvRow();
                row.Add("Average Temperature vs Time");
                writer.WriteRow(row);

                row = new CsvRow();
                row.Add("time step");
                row.Add("time (s)");
                row.Add("bed 1 T_inlet (degC)");
                row.Add("bed 1 T_outlet (degC)");
                row.Add("bed 2 T_inlet (degC)");
                row.Add("bed 2 T_outlet (degC)");
                row.Add("cond T_inlet (degC)");
                row.Add("cond T_outlet (degC)");
                row.Add("eva T_inlet (degC)");
                row.Add("eva T_outlet (degC)");
                writer.WriteRow(row);

                for (int t = 0; t < (Parameters.numOfTimeStep + 1); t++)
                {
                    row = new CsvRow();
                    row.Add(t.ToString());
                    row.Add(((double)t / Parameters.timeStepPerSecond).ToString());
                    row.Add((array_bed1_inlet[t] - 273).ToString());
                    row.Add((array_bed1_outlet[t] - 273).ToString());
                    row.Add((array_bed2_inlet[t] - 273).ToString());
                    row.Add((array_bed2_outlet[t] - 273).ToString());
                    row.Add((array_cond_inlet[t] - 273).ToString());
                    row.Add((array_cond_outlet[t] - 273).ToString());
                    row.Add((array_eva_inlet[t] - 273).ToString());
                    row.Add((array_eva_outlet[t] - 273).ToString());
                    writer.WriteRow(row);
                }
            }
        }
    }

    class PressurePlot
    {
        double[] array_bed1_pressure = new double[Parameters.numOfTimeStep + 1];
        double[] array_bed2_pressure = new double[Parameters.numOfTimeStep + 1];
        double[] array_cond_pressure = new double[Parameters.numOfTimeStep + 1];
        double[] array_eva_pressure = new double[Parameters.numOfTimeStep + 1];
        Phase[] array_bed1_phase = new Phase[Parameters.numOfTimeStep + 1];
        Phase[] array_bed2_phase = new Phase[Parameters.numOfTimeStep + 1];

        public PressurePlot()
        {
            array_bed1_pressure[0] = Parameters.P0_bed;
            array_bed2_pressure[0] = Parameters.P0_bed;
            array_cond_pressure[0] = Parameters.P0_cond;
            array_eva_pressure[0] = Parameters.P0_eva;
            array_bed1_phase[0] = Phase.NA;
            array_bed2_phase[0] = Phase.NA;
        }

        public void addValue(Bed[] bed, Condenser cond, Evaporator eva, int timeStep)
        {
            array_bed1_pressure[timeStep] = bed[0].averagePressure;
            array_bed2_pressure[timeStep] = bed[1].averagePressure;
            array_cond_pressure[timeStep] = cond.averagePressure;
            array_eva_pressure[timeStep] = eva.averagePressure;
            array_bed1_phase[timeStep] = bed[0].phase;
            array_bed2_phase[timeStep] = bed[1].phase;
        }

        public void exportPlot(string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                row = new CsvRow();
                row.Add("time step");
                row.Add("time (s)");                
                row.Add("bed 1 P (pa)");
                row.Add("bed 2 P (pa)");
                row.Add("cond P (pa)");
                row.Add("eva P (pa)");
                row.Add("bed 1 phase");
                row.Add("bed 2 phase");
                writer.WriteRow(row);

                for (int t = 0; t < (Parameters.numOfTimeStep + 1); t++)
                {
                    row = new CsvRow();
                    row.Add(t.ToString());
                    row.Add(((double)t / Parameters.timeStepPerSecond).ToString());
                    row.Add(array_bed1_pressure[t].ToString());
                    row.Add(array_bed2_pressure[t].ToString());
                    row.Add(array_cond_pressure[t].ToString());
                    row.Add(array_eva_pressure[t].ToString());
                    row.Add(array_bed1_phase[t].ToString());
                    row.Add(array_bed2_phase[t].ToString());
                    writer.WriteRow(row);
                }
            }
        }

    }
}
