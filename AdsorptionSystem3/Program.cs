using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AdsorptionSystem3
{
    class Program
    {
        static void Main(string[] args)
        {
            //define chambers
            Bed[] bed = new Bed[Parameters.numOfBed];
            for (int i = 0; i < Parameters.numOfBed; i++)
            {
                bed[i] = new Bed();
            }
            Condenser cond = new Condenser();
            Evaporator eva = new Evaporator();

            //define connecting pipes
            ConnectingPipes[] connectingPipe = new ConnectingPipes[Parameters.numOfBed + 1 + 1];
            connectingPipe[0] = new ConnectingPipes(bed[0], cond);
            connectingPipe[1] = new ConnectingPipes(bed[1], cond);
            connectingPipe[2] = new ConnectingPipes(eva, bed[0]);
            connectingPipe[3] = new ConnectingPipes(eva, bed[1]);

            //define time plots
            TemperaurePlot temperaturePlot = new TemperaurePlot();
            PressurePlot pressurePlot = new PressurePlot();

            //define performance indicators
            PerformanceIndicator.initialise();
            Report.initialise(bed);

            //starting operation
            int cycle = 1;
            for (int timeStep = 1; timeStep <= Parameters.numOfTimeStep; timeStep++)
            {
                if (timeStep > 1)
                {
                    updateNodeAtNewTimeStep(bed[0]);
                    updateq(bed[0]);
                    updateNodeAtNewTimeStep(bed[1]);
                    updateq(bed[1]);
                    updateNodeAtNewTimeStep(cond);
                    updateNodeAtNewTimeStep(eva);
                }

                if (timeStep > ((cycle - 1) * 2 * Parameters.switchingTime * Parameters.timeStepPerSecond) && timeStep <= (Parameters.switchingTime * Parameters.timeStepPerSecond + (cycle - 1) * 2 * (Parameters.switchingTime + Parameters.sorptionTime) * Parameters.timeStepPerSecond))
                {
                    //Phase
                    bed[0].phase = Phase.Preheating;
                    bed[1].phase = Phase.Precooling;
                    cond.phase = Phase.NA;
                    eva.phase = Phase.NA;

                    updateMassFlowrate(connectingPipe);

                    //switching time
                    List<Task> TaskList = new List<Task>();
                    Task task1 = new Task(bed[0].preheating);
                    task1.Start();
                    TaskList.Add(task1);
                    Task task2 = new Task(bed[1].precooling);
                    task2.Start();
                    TaskList.Add(task2);
                    Task task3 = new Task(cond.disconnected);
                    task3.Start();
                    TaskList.Add(task3);
                    Task task4 = new Task(eva.disconnected);
                    task4.Start();
                    TaskList.Add(task4);
                    Task.WaitAll(TaskList.ToArray());

                }
                else if (timeStep > ((Parameters.switchingTime + (cycle - 1) * 2 * (Parameters.switchingTime + Parameters.sorptionTime)) * Parameters.timeStepPerSecond) && timeStep <= ((Parameters.switchingTime + Parameters.sorptionTime + (cycle - 1) * 2 * (Parameters.switchingTime + Parameters.sorptionTime)) * Parameters.timeStepPerSecond))
                {
                    //Phase
                    bed[0].phase = Phase.Desorption;
                    bed[1].phase = Phase.Adsorption;
                    cond.phase = Phase.Desorption;
                    eva.phase = Phase.Adsorption;

                    updateMassFlowrate(connectingPipe);

                    //sorption time
                    List<Task> TaskList = new List<Task>();
                    Task task1 = new Task(bed[0].desorption);
                    task1.Start();
                    TaskList.Add(task1);
                    Task task2 = new Task(bed[1].adsorption);
                    task2.Start();
                    TaskList.Add(task2);
                    Task.WaitAll(TaskList.ToArray());
                    cond.condensation();
                    eva.evaporation();
                }
                else if (timeStep > ((Parameters.switchingTime + Parameters.sorptionTime + (cycle - 1) * 2 * (Parameters.switchingTime + Parameters.sorptionTime)) * Parameters.timeStepPerSecond) && timeStep <= ((2 * Parameters.switchingTime + Parameters.sorptionTime + (cycle - 1) * 2 * (Parameters.switchingTime + Parameters.sorptionTime)) * Parameters.timeStepPerSecond))
                {
                    //Phase
                    bed[0].phase = Phase.Precooling;
                    bed[1].phase = Phase.Preheating;
                    cond.phase = Phase.NA;
                    eva.phase = Phase.NA;

                    updateMassFlowrate(connectingPipe);

                    //switching time
                    List<Task> TaskList = new List<Task>();
                    Task task1 = new Task(bed[0].precooling);
                    task1.Start();
                    TaskList.Add(task1);
                    Task task2 = new Task(bed[1].preheating);
                    task2.Start();
                    TaskList.Add(task2);
                    Task task3 = new Task(cond.disconnected);
                    task3.Start();
                    TaskList.Add(task3);
                    Task task4 = new Task(eva.disconnected);
                    task4.Start();
                    TaskList.Add(task4);
                    Task.WaitAll(TaskList.ToArray());
                }
                else if (timeStep > ((2 * Parameters.switchingTime + Parameters.sorptionTime + (cycle - 1) * 2 * (Parameters.switchingTime + Parameters.sorptionTime)) * Parameters.timeStepPerSecond) && timeStep <= ((2 * Parameters.switchingTime + 2 * Parameters.sorptionTime + (cycle - 1) * 2 * (Parameters.switchingTime + Parameters.sorptionTime)) * Parameters.timeStepPerSecond))
                {
                    //Phase
                    bed[0].phase = Phase.Adsorption;
                    bed[1].phase = Phase.Desorption;
                    cond.phase = Phase.Desorption;
                    eva.phase = Phase.Adsorption;

                    updateMassFlowrate(connectingPipe);

                    //sorption time
                    List<Task> TaskList = new List<Task>();
                    Task task1 = new Task(bed[0].adsorption);
                    task1.Start();
                    TaskList.Add(task1);
                    Task task2 = new Task(bed[1].desorption);
                    task2.Start();
                    TaskList.Add(task2);
                    Task.WaitAll(TaskList.ToArray());
                    cond.condensation();
                    eva.evaporation();
                }

                if (timeStep == cycle * 2 * (Parameters.switchingTime + Parameters.sorptionTime) * Parameters.timeStepPerSecond)
                {
                    cycle++;
                }

                //reset mass flow rate to 0
                resetMassFlowRate(connectingPipe);

                //update aveage pressure
                bed[0].calculateAveragePressure();
                bed[1].calculateAveragePressure();
                cond.calculateAveragePressure();
                eva.calculateAveragePressure();

                bed[0].calculateAverageMass();
                cond.calculateAverageMass();

                //update time plots
                temperaturePlot.addValue(bed, cond, eva, timeStep);
                pressurePlot.addValue(bed, cond, eva, timeStep);

                //update performance indicators
                for (int i = 0; i < Parameters.numOfBed; i++)
                {
                    if (bed[i].phase == Phase.Preheating || bed[i].phase == Phase.Desorption)
                    {
                        //heat input
                        switch(i)
                        {
                            case 0:
                                PerformanceIndicator.addBedValue(temperaturePlot.array_bed1_inlet[timeStep], temperaturePlot.array_bed1_outlet[timeStep], timeStep);
                                break;
                            case 1:
                                PerformanceIndicator.addBedValue(temperaturePlot.array_bed2_inlet[timeStep], temperaturePlot.array_bed2_outlet[timeStep], timeStep);
                                break;
                            default:
                                Console.WriteLine("more bed than declared");
                                Console.Read();
                                break;
                        }
                    }
                }
                //heat output
                PerformanceIndicator.addEvaValue(temperaturePlot.array_eva_inlet[timeStep], temperaturePlot.array_eva_outlet[timeStep], timeStep);
                //condensatation
                PerformanceIndicator.addCondensateValue(cond.rateOfCondensation, timeStep);

                //output the current time step
                Console.WriteLine("{0} of {1} time steps completed...", timeStep, Parameters.numOfTimeStep);
                Console.WriteLine("{0}" + " " + "{1}", bed[0].averagePressure, cond.averagePressure);
                Console.WriteLine("{0}" + " " + "{1}", bed[1].averagePressure, eva.averagePressure);
            }

            string cycleTime = (Parameters.switchingTime / 60).ToString() + "m " + (Parameters.sorptionTime / 60).ToString() + "m";
            export_T(bed[0], cycleTime + " bed 1 temperature contour");
            export_T(bed[1], cycleTime + " bed 2 temperature contour");
            export_T(cond, cycleTime + " cond temperature contour");
            export_T(eva, cycleTime + " eva temperature contour");
            export_P(bed[0], cycleTime + " bed 1 pressure contour");
            export_P(bed[1], cycleTime + " bed 2 pressure contour");
            export_P(cond, cycleTime + " cond pressure contour");
            export_P(eva, cycleTime + " eva pressure contour");
            temperaturePlot.exportPlot(cycleTime + " temperature plot");
            pressurePlot.exportPlot(cycleTime + " pressure plot");

            PerformanceIndicator.calculatePerformance(bed, eva);
            Console.WriteLine("COP is {0}", PerformanceIndicator.COP);
            Report.publish(cycleTime + " report");

            Console.WriteLine("Simulation is completed, press any key to close this window");
            Console.Read();
        }

        ////Methods
        //update nodes at every time step
        public static void updateNodeAtNewTimeStep(Chamber chamber)
        {
            foreach (TNodes node in chamber.T)
            {
                node.TP0 = node.TP;
            }
            foreach (RhoNodes node in chamber.Rho)
            {
                node.RhoP0 = node.RhoP;
            }
        }
        public static void updateq(Bed bed)
        {
            foreach (RhoNodes node in bed.Rho)
            {
                if (node.q < bed.workingPair.qm && node.materialType == MaterialType.Adsorbent)
                {
                    node.q = node.q + node.dqdt * Parameters.deltat;
                    if (node.q > bed.workingPair.qm)
                    {
                        node.q = bed.workingPair.qm;
                    }
                    else if (node.q < 0)
                    {
                        node.q = 0;
                    }
                }
            }
        }
        public static void updateMassFlowrate(ConnectingPipes[] connectingPipe)
        {
            foreach (ConnectingPipes pipe in connectingPipe)
            {
                if (pipe.origin.phase == pipe.destination.phase)
                {
                    if (pipe.origin.phase == Phase.Adsorption)
                    {
                        pipe.calculateMassFlowRate(pipe.origin.phase);
                        //pipe.destination.inComingMassFlowRate = pipe.massFlowRate;
                        //pipe.origin.outGoingMassFlowRate = -pipe.massFlowRate;
                    }

                    if (pipe.origin.phase == Phase.Desorption)
                    {
                        pipe.calculateMassFlowRate(pipe.origin.phase);
                        //pipe.destination.inComingMassFlowRate = pipe.massFlowRate;
                        //pipe.origin.outGoingMassFlowRate = -pipe.massFlowRate;
                    }
                }
            }
        }
        public static void resetMassFlowRate(ConnectingPipes[] connectingPipe)
        {
            foreach (ConnectingPipes pipe in connectingPipe)
            {
                pipe.massFlowRate = 0;
                pipe.destination.inComingMassFlowRate = pipe.massFlowRate;
                pipe.origin.outGoingMassFlowRate = -pipe.massFlowRate;
            }
        }

        //export to csv
        static void export_mass(Bed bed, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (bed.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < bed.numOfZNodes; z++)
                    {
                        row.Add((bed.Rho[r, z].RhoP * bed.Rho[r, z].volume).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_k(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.T[r, z].thermalConductivity).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_aP(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.Rho[r, z].aP).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_Dw(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.T[r, z].Dw).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_uptake(Bed bed, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (bed.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < bed.numOfZNodes; z++)
                    {
                        row.Add((bed.Rho[r, z].q).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_boundary(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.T[r, z].boundary).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_T(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.T[r, z].TP - 273).ToString("0.00000"));
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_Energy(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.T[r, z].density * chamber.T[r, z].volume * chamber.T[r, z].heatCapacity * chamber.T[r, z].TP).ToString("0.00000"));
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_P(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.Rho[r, z].pressure).ToString("0.00000"));
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_Rho(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.Rho[r, z].RhoP).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_Volume(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.Rho[r, z].volume).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_Mass(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.Rho[r, z].RhoP * chamber.Rho[r, z].volume).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_Material(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.T[r, z].materialType).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_Radius(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((chamber.T[r, z].rp).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }

        static void export_coefficients(TNodes[,] T, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;
                row = new CsvRow();
                row.Add("aP");
                row.Add("aW");
                row.Add("aE");
                row.Add("aS");
                row.Add("aN");
                row.Add("Dw");
                row.Add("De");
                row.Add("Ds");
                row.Add("Dn");
                writer.WriteRow(row);

                row = new CsvRow();
                row.Add(T[1, 0].aP.ToString());
                row.Add(T[1, 0].aW.ToString());
                row.Add(T[1, 0].aE.ToString());
                row.Add(T[1, 0].aS.ToString());
                row.Add(T[1, 0].aN.ToString());
                row.Add(T[1, 0].Dw.ToString());
                row.Add(T[1, 0].De.ToString());
                row.Add(T[1, 0].Ds.ToString());
                row.Add(T[1, 0].Dn.ToString());
                writer.WriteRow(row);
            }
        }

        static void export_differenceT(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((2 * chamber.T[r, z].rp / chamber.deltaR).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }
        static void export_differenceR(Chamber chamber, string filename)
        {
            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;

                for (int r = (chamber.numOfRNodes - 1); r >= 0; r--)
                {
                    row = new CsvRow();
                    for (int z = 0; z < chamber.numOfZNodes; z++)
                    {
                        row.Add((2 * chamber.Rho[r, z].rp / chamber.deltaR).ToString());
                    }
                    writer.WriteRow(row);
                }
            }
        }

        static void export_point(Chamber chamber, string filename)
        {
            int z = 5, r = chamber.numOfRNodes - 1;

            using (CsvWriter writer = new CsvWriter(filename + ".csv"))
            {
                CsvRow row;
                row = new CsvRow();
                row.Add("r");
                row.Add("z");
                row.Add("Sc");
                row.Add("aP");
                row.Add("aP0");
                row.Add("aS");
                row.Add("aN");
                row.Add("aW");
                row.Add("aE");
                writer.WriteRow(row);

                row = new CsvRow();
                row.Add(r.ToString());
                row.Add(z.ToString());
                row.Add(chamber.Rho[r, z].Sc.ToString());
                row.Add(chamber.Rho[r, z].aP.ToString());
                row.Add(chamber.Rho[r, z].aP0.ToString());
                row.Add(chamber.Rho[r, z].aS.ToString());
                row.Add(chamber.Rho[r, z].aN.ToString());
                row.Add(chamber.Rho[r, z].aW.ToString());
                row.Add(chamber.Rho[r, z].aE.ToString());
                writer.WriteRow(row);
            }
        }

    }
}
