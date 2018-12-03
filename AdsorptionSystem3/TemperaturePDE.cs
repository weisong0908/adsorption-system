using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    class TemperaturePDE
    {
        /*
         * TDMA direction:  south to north
         * sweep direction: west to east
         */

        public double deltaR, deltaZ;
        public int numOfRNodes, numOfZNodes;
        public TNodes[,] T;

        public int iterationCounter = 0;
        public int[] numberOfIterationAtEachTimeStep = new int[Parameters.numOfTimeStep + 1];
        public bool endOfIteration = true;

        public void assignBoundary()
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    T[r, z].boundary = Boundary.Center;

                    if (z == 0)
                    {
                        T[r, z].boundary = Boundary.Left;
                    }
                    else if (z == (numOfZNodes - 1))
                    {
                        T[r, z].boundary = Boundary.Right;
                    }
                    if (r == 0)
                    {
                        T[r, z].boundary = Boundary.Bottom;
                        if (z == 0)
                        {
                            T[r, z].boundary = Boundary.BottomLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            T[r, z].boundary = Boundary.BottomRight;
                        }
                    }
                    else if (r == (numOfRNodes - 1))
                    {
                        T[r, z].boundary = Boundary.Top;
                        if (z == 0)
                        {
                            T[r, z].boundary = Boundary.TopLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            T[r, z].boundary = Boundary.TopRight;
                        }
                    }
                }
            }
        }

        public void assignCoefficients2()
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    switch (T[r, z].boundary)
                    {
                        case Boundary.Left:
                            T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaZ;
                            T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z + 1].thermalConductivity) / deltaZ;
                            T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                            T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                            break;
                        case Boundary.Right:
                            T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z - 1].thermalConductivity) / deltaZ;
                            T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaZ;
                            T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                            T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                            break;
                        case Boundary.Bottom:
                            T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z - 1].thermalConductivity) / deltaZ;
                            T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z + 1].thermalConductivity) / deltaZ;
                            T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                            T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                            break;
                        case Boundary.Top:
                            T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z - 1].thermalConductivity) / deltaZ;
                            T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z + 1].thermalConductivity) / deltaZ;
                            T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                            T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                            break;
                        case Boundary.BottomLeft:
                            T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaZ;
                            T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z + 1].thermalConductivity) / deltaZ;
                            T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                            T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                            break;
                        case Boundary.BottomRight:
                            T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z - 1].thermalConductivity) / deltaZ;
                            T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaZ;
                            T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                            T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                            break;
                        case Boundary.TopLeft:
                            T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaZ;
                            T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z + 1].thermalConductivity) / deltaZ;
                            T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                            T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                            break;
                        case Boundary.TopRight:
                            T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z - 1].thermalConductivity) / deltaZ;
                            T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaZ;
                            T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                            T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                            break;
                        default:
                            T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z - 1].thermalConductivity) / deltaZ;
                            T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * 0.5 * (T[r, z].thermalConductivity + T[r, z + 1].thermalConductivity) / deltaZ;
                            T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                            T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                            break;
                    }

                   if(T[r,z].materialType == MaterialType.Adsorbent)
                    {
                        T[r, z].Dw = T[r, z].Dw * (1 - T[r, z].porosity);
                        T[r, z].De = T[r, z].De * (1 - T[r, z].porosity);
                        T[r, z].Ds = T[r, z].Ds * (1 - T[r, z].porosity);
                        T[r, z].Dn = T[r, z].Dn * (1 - T[r, z].porosity);
                    }
                }
            }

            foreach (TNodes node in T)
            {
                node.aP0 = 2 * Math.PI * node.density * node.heatCapacity * node.rp * deltaR * deltaZ / Parameters.deltat;

                node.Fw = 2 * Math.PI * node.rp * deltaR * node.velocity * node.density * node.heatCapacity;
                node.Fe = 2 * Math.PI * node.rp * deltaR * node.velocity * node.density * node.heatCapacity;
                node.Fs = 2 * Math.PI * deltaZ * node.density * node.heatCapacity * 0 * node.rs;
                node.Fn = 2 * Math.PI * deltaZ * node.density * node.heatCapacity * 0 * node.rn;
                //node.Dw = 2 * Math.PI * node.rp * deltaR * node.thermalConductivity / deltaZ;
                //node.De = 2 * Math.PI * node.rp * deltaR * node.thermalConductivity / deltaZ;
                //node.Ds = 2 * Math.PI * node.rs * deltaZ * node.thermalConductivity / deltaR;
                //node.Dn = 2 * Math.PI * node.rn * deltaZ * node.thermalConductivity / deltaR;

                node.aW = Helper.max3(node.Fw, (node.Dw + 0.5 * node.Fw), 0);
                node.aE = Helper.max3(-node.Fe, (node.De - 0.5 * node.Fe), 0);
                node.aS = Helper.max3(node.Fs, (node.Ds + 0.5 * node.Fs), 0);
                node.aN = Helper.max3(-node.Fn, (node.Dn - 0.5 * node.Fn), 0);

                node.Sc = 0;
                node.Sp = 0;

                switch (node.boundary)
                {
                    case Boundary.Left:
                        node.aW = 0;
                        break;
                    case Boundary.Right:
                        node.aE = 0;
                        break;
                    case Boundary.Bottom:
                        node.aS = 0;
                        break;
                    case Boundary.Top:
                        node.aN = 0;
                        break;
                    case Boundary.BottomLeft:
                        node.aW = 0;
                        node.aS = 0;
                        break;
                    case Boundary.BottomRight:
                        node.aE = 0;
                        node.aS = 0;
                        break;
                    case Boundary.TopLeft:
                        node.aW = 0;
                        node.aN = 0;
                        break;
                    case Boundary.TopRight:
                        node.aE = 0;
                        node.aN = 0;
                        break;
                    default:
                        break;
                }

                node.deltaF = node.Fe - node.Fw + node.Fn - node.Fs;
                node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp + node.deltaF;
            }
        }

        public void TDMA2()//solve: west to east, sweep: south to north
        {
            do
            {
                foreach (TNodes node in T)
                {
                    node.TP_prime = node.TP;
                }

                for (int r = 0; r < numOfRNodes; r++)
                {
                    //get from neighbour lines
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        if (r == 0)
                        {
                            T[r, z].TS = 0;
                            T[r, z].TN = T[r + 1, z].TP;
                        }
                        else if (r == (numOfRNodes - 1))
                        {
                            T[r, z].TS = T[r - 1, z].TP;
                            T[r, z].TN = 0;
                        }
                        else
                        {
                            T[r, z].TS = T[r - 1, z].TP;
                            T[r, z].TN = T[r + 1, z].TP;
                        }
                    }

                    //set from west to east
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        T[r, z].betaj = T[r, z].aW;
                        T[r, z].alphaj = T[r, z].aE;
                        T[r, z].Dj = T[r, z].aP;
                        T[r, z].Cj = T[r, z].aS * T[r, z].TS + T[r, z].aN * T[r, z].TN + T[r, z].aP0 * T[r, z].TP0 + T[r, z].Sc;

                        if (z == 0)
                        {
                            T[r, z].Aj = T[r, z].alphaj / (T[r, z].Dj);
                            T[r, z].Cj_prime = (T[r, z].Cj) / (T[r, z].Dj);
                        }

                        else
                        {
                            T[r, z].Aj = T[r, z].alphaj / (T[r, z].Dj - T[r, z].betaj * T[r, z - 1].Aj);
                            T[r, z].Cj_prime = (T[r, z].betaj * T[r, z - 1].Cj_prime + T[r, z].Cj) / (T[r, z].Dj - T[r, z].betaj * T[r, z - 1].Aj);
                        }
                    }

                    //solve from east to west
                    for (int z = (numOfZNodes - 1); z >= 0; z--)
                    {
                        if (z == (numOfZNodes - 1))
                        {
                            T[r, z].TP = T[r, z].Cj_prime;
                        }
                        else
                        {
                            T[r, z].TP = T[r, z].Aj * T[r, z + 1].TP + T[r, z].Cj_prime;
                        }

                        if(double.IsNaN( T[r, z].TP))
                        {
                            int test = 1;
                        }
                    }
                }

                endOfIteration = true;
                foreach(TNodes node in T)
                {
                    node.error = Helper.calculate_error(node.TP, node.TP_prime);
                    if (Math.Abs(node.error) < Parameters.tolerance_temperature)
                    {
                        node.isConverged = true;
                    }
                    else
                    {
                        node.isConverged = false;
                        endOfIteration = false;
                        break;
                    }
                }

                iterationCounter++;

            } while (endOfIteration == false);            
        }
    }

    class BedTemperaturePDE : TemperaturePDE
    {
        public BedTemperaturePDE(Bed bed)
        {
            deltaR = bed.deltaR;
            deltaZ = bed.deltaZ;
            numOfRNodes = bed.numOfRNodes;
            numOfZNodes = bed.numOfZNodes;
            T = bed.T;

            assignBoundary();

            //initial conditions
            foreach (TNodes node in T)
            {
                node.TP0 = Parameters.T0;
                node.TP = node.TP0;
            }

            assignCoefficients2();
        }

        public void updateCoefficients2(double inletTemperature, Bed bed)
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    //inlet temperature
                    if (T[r, z].boundary == Boundary.Left || T[r, z].boundary == Boundary.BottomLeft)
                    {
                        if (T[r, z].materialType == MaterialType.HeatExchangeFluid)
                        {
                            T[r, z].Sc = (2 * T[r, z].Dw + T[r, z].Fw) * inletTemperature;
                            T[r, z].Sp = -(2 * T[r, z].Dw + T[r, z].Fw);
                        }
                    }

                    //heat of adsorption
                    if (T[r, z].materialType == MaterialType.Adsorbent)
                    {
                        T[r, z].Sc = 2 * Math.PI * T[r, z].rp * deltaR * deltaZ * T[r, z].density * bed.workingPair.heatOfAdsorption * bed.Rho[r, z].dqdt;
                    }

                    T[r,z].aP = T[r,z].aW + T[r,z].aE + T[r,z].aS + T[r,z].aN + T[r,z].aP0 - T[r,z].Sp + T[r,z].deltaF;
                }
            }

            //condensation on excess tube
            //saturated temperature for adsorbate
            double A = 8.07131, B = 1730.63, C = 233.426;
            double T_sat = (B / (A - Math.Log10(bed.averagePressure * 0.00750062))) - C + 273;
            condensationOnExcessTubeSurface(T_sat, bed, 1, out bed.rateOfCondensation1);
            condensationOnExcessTubeSurface(T_sat, bed, 2, out bed.rateOfCondensation2);
        }
        public void condensationOnExcessTubeSurface(double T_sat, Bed bed, int part , out double rateOfCondensation)
        {
            int startIndex = 0, endIndex = 0;
            switch(part)
            {
                case 1:
                    startIndex = 0;
                    endIndex = bed.numOfZNodes_excessTube;
                    break;
                case 2:
                    startIndex = numOfZNodes - bed.numOfZNodes_excessTube;
                    endIndex = numOfZNodes;
                    break;
                default:
                    Console.WriteLine("wrong data for bed excess tube");
                    Console.Read();
                    break;
            }
            int r = bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall - 1;

            //calculate averaged wall temperature for excess tube
            double sum = 0; int count = 0;
            for (int z = startIndex; z < endIndex; z++)
            {
                if (T[r, z].materialType == MaterialType.Tube && T[r + 1, z].materialType == MaterialType.Adsorbate)
                {
                    sum += T[r, z].TP;
                    count++;
                }
            }
            double T_wall = sum / count;

            //calculate modified latent heat of vaporisation
            double JakobNumber = bed.adsorbate.heatCapacity_condensed * (T_sat - T_wall) / bed.adsorbate.latentHeat;
            double modifiedLatentHeat = bed.adsorbate.latentHeat * (1 + 0.68 * JakobNumber);

            //calculate heat transfer coefficient for film condensation
            double g = 9.81;//kgm/s2
            double c = 0.729;//for tube
            double numerator = g * bed.adsorbate.density_condensed * (bed.adsorbate.density_condensed - bed.adsorbate.density) * Math.Pow(bed.adsorbate.thermalConductivity_condensed, 3) * modifiedLatentHeat;
            double denominator = bed.adsorbate.dynamicViscosity_condensed * (T_sat - T_wall) * 2 * (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall) * deltaR;
            double heatTransferCoefficient_condensation = c * Math.Pow(Math.Abs(numerator / denominator), 0.25) * deltaZ * bed.numOfZNodes_excessTube;
            double heatOfCondensation = heatTransferCoefficient_condensation* bed.excessTubeOuterSurfaceArea * (T_sat - T_wall);
            rateOfCondensation = 0;

            if ((T_sat - T_wall) > 0)
            {
                //apply heat flux (ignore it first)
                for (int z = startIndex; z < endIndex; z++)
                {
                    //T[r, z].aN = 0;
                    //T[r, z].Sc = 2 * Math.PI * T[r, z].rn * deltaZ * heatTransferCoefficient_condensation * (T_sat - T_wall);
                    //T[r, z].aP = T[r, z].aW + T[r, z].aE + T[r, z].aS + T[r, z].aN + T[r, z].aP0 - T[r, z].Sp + T[r, z].deltaF;
                    rateOfCondensation = heatOfCondensation / modifiedLatentHeat;
                }
            }
        }
    }

    class CondTemperaturePDE : TemperaturePDE
    {
        public CondTemperaturePDE(Condenser cond)
        {
            deltaR = cond.deltaR;
            deltaZ = cond.deltaZ;
            numOfRNodes = cond.numOfRNodes;
            numOfZNodes = cond.numOfZNodes;
            T = cond.T;

            assignBoundary_for_cond(cond);

            //initial conditions
            foreach (TNodes node in T)
            {
                node.TP0 = Parameters.T0;
                node.TP = node.TP0;
            }

            assignCoefficients2_for_cond(cond);
        }

        public void assignBoundary_for_cond(Condenser cond)
        {
            int rtop = cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall;

            for (int r = 0; r < rtop; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    T[r, z].boundary = Boundary.Center;

                    if (z == 0)
                    {
                        T[r, z].boundary = Boundary.Left;
                    }
                    else if (z == (numOfZNodes - 1))
                    {
                        T[r, z].boundary = Boundary.Right;
                    }
                    if (r == 0)
                    {
                        T[r, z].boundary = Boundary.Bottom;
                        if (z == 0)
                        {
                            T[r, z].boundary = Boundary.BottomLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            T[r, z].boundary = Boundary.BottomRight;
                        }
                    }
                    else if (r == (rtop - 1))
                    {
                        T[r, z].boundary = Boundary.Top;
                        if (z == 0)
                        {
                            T[r, z].boundary = Boundary.TopLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            T[r, z].boundary = Boundary.TopRight;
                        }
                    }
                }
            }
        }

        public void assignCoefficients2_for_cond(Condenser cond)
        {
            int rtop = cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall;

            for (int r = 0; r < rtop; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (r == 0)
                    {
                        T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                        T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                    }
                    else if (r == (rtop - 1))
                    {
                        T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                        T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                    }
                    else
                    {
                        T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                        T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                    }
                }
            }

            foreach (TNodes node in T)
            {
                node.aP0 = 2 * Math.PI * node.density * node.heatCapacity * node.rp * deltaR * deltaZ / Parameters.deltat;

                node.Fw = 2 * Math.PI * node.rp * deltaR * node.velocity * node.density * node.heatCapacity;
                node.Fe = 2 * Math.PI * node.rp * deltaR * node.velocity * node.density * node.heatCapacity;
                node.Fs = 2 * Math.PI * deltaZ * node.density * node.heatCapacity * 0 * node.rs;
                node.Fn = 2 * Math.PI * deltaZ * node.density * node.heatCapacity * 0 * node.rn;

                node.aW = Helper.max3(node.Fw, (node.Dw + 0.5 * node.Fw), 0);
                node.aE = Helper.max3(-node.Fe, (node.De - 0.5 * node.Fe), 0);
                node.aS = Helper.max3(node.Fs, (node.Ds + 0.5 * node.Fs), 0);
                node.aN = Helper.max3(-node.Fn, (node.Dn - 0.5 * node.Fn), 0);

                node.Sc = 0;
                node.Sp = 0;

                switch (node.boundary)
                {
                    case Boundary.Left:
                        node.aW = 0;
                        break;
                    case Boundary.Right:
                        node.aE = 0;
                        break;
                    case Boundary.Bottom:
                        node.aS = 0;
                        break;
                    case Boundary.Top:
                        node.aN = 0;
                        break;
                    case Boundary.BottomLeft:
                        node.aW = 0;
                        node.aS = 0;
                        break;
                    case Boundary.BottomRight:
                        node.aE = 0;
                        node.aS = 0;
                        break;
                    case Boundary.TopLeft:
                        node.aW = 0;
                        node.aN = 0;
                        break;
                    case Boundary.TopRight:
                        node.aE = 0;
                        node.aN = 0;
                        break;
                    default:
                        break;
                }

                node.deltaF = node.Fe - node.Fw + node.Fn - node.Fs;
                node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp + node.deltaF;
            }
        }

        public void updateCoefficients2(double inletTemperature, Condenser cond)
        {
            foreach(TNodes node in T)
            {
                //inlet temperature
                if (node.boundary == Boundary.Left || node.boundary == Boundary.BottomLeft)
                {
                    if (node.materialType == MaterialType.HeatExchangeFluid)
                    {
                        node.Sc = (2 * node.Dw + node.Fw) * inletTemperature;
                        node.Sp = -(2 * node.Dw + node.Fw);
                        node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp + node.deltaF;
                    }
                }
            }

            //condensation on tube surface
            condensationOnCondenserTubeSurface(cond, out cond.rateOfCondensation);
        }
        public void condensationOnCondenserTubeSurface(Condenser cond, out double rateOfCondensation)
        {            
            int r = cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall - 1;

            //saturated temperature for adsorbate
            double A = 8.07131, B = 1730.63, C = 233.426;
            double T_sat = (B / (A - Math.Log10(cond.averagePressure * 0.00750062))) - C + 273;

            //averaged wall temperature
            double sum = 0;
            int count = 0;
            for (int z = 0; z < numOfZNodes; z++)
            {
                sum += T[r, z].TP;
                count++;
            }
            double T_wall = sum / count;

            //calculate modified latent heat of vaporisation
            double JakobNumber = cond.adsorbate.heatCapacity_condensed * (T_sat - T_wall) / cond.adsorbate.latentHeat;
            double modifiedLatentHeat = cond.adsorbate.latentHeat * (1 + 0.68 * JakobNumber);

            //calculate heat transfer coefficient for film condensation
            double g = 9.81;//kgm/s2
            double c = 0.729;//for tube
            double numerator = g * cond.adsorbate.density_condensed * (cond.adsorbate.density_condensed - cond.adsorbate.density) * Math.Pow(cond.adsorbate.thermalConductivity_condensed, 3) * modifiedLatentHeat;
            double denominator = cond.adsorbate.dynamicViscosity_condensed * (T_sat - T_wall) * 2 * (cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall) * deltaR;
            double heatTransferCoefficient_condensation = c * Math.Pow(Math.Abs(numerator / denominator), 0.25) * deltaZ * cond.numOfZNodes;
            double heatOfCondensation = 0;
            rateOfCondensation = 0;

            if ((T_sat - T_wall) > 0)
            {
                //apply heat flux
                for (int z = 0; z < numOfZNodes; z++)
                {
                    T[r, z].aN = 0;
                    T[r, z].Sc = 2 * Math.PI * T[r, z].rn * deltaZ * heatTransferCoefficient_condensation * (T_sat - T_wall);
                    T[r, z].aP = T[r, z].aW + T[r, z].aE + T[r, z].aS + T[r, z].aN + T[r, z].aP0 - T[r, z].Sp + T[r, z].deltaF;
                }

                heatOfCondensation = heatTransferCoefficient_condensation * cond.tubeOuterSurfaceArea * (T_sat - T_wall);
                rateOfCondensation = heatOfCondensation / modifiedLatentHeat;
            }
        }

        public void TDMA2_for_cond(Condenser cond)//solve: west to east, sweep: south to north
        {
            int rtop = cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall;

            do
            {
                foreach (TNodes node in T)
                {
                    node.TP_prime = node.TP;
                }

                for (int r = 0; r < rtop; r++)
                {
                    //get from neighbour lines
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        if (r == 0)
                        {
                            T[r, z].TS = 0;
                            T[r, z].TN = T[r + 1, z].TP;
                        }
                        else if (r == (rtop - 1))
                        {
                            T[r, z].TS = T[r - 1, z].TP;
                            T[r, z].TN = 0;
                        }
                        else
                        {
                            T[r, z].TS = T[r - 1, z].TP;
                            T[r, z].TN = T[r + 1, z].TP;
                        }
                    }
                }
                for (int r = 0; r < rtop; r++)
                {
                    //set from west to east
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        T[r, z].betaj = T[r, z].aW;
                        T[r, z].alphaj = T[r, z].aE;
                        T[r, z].Dj = T[r, z].aP;
                        T[r, z].Cj = T[r, z].aS * T[r, z].TS + T[r, z].aN * T[r, z].TN + T[r, z].aP0 * T[r, z].TP0 + T[r, z].Sc;

                        if (z == 0)
                        {
                            T[r, z].Aj = T[r, z].alphaj / (T[r, z].Dj);
                            T[r, z].Cj_prime = (T[r, z].Cj) / (T[r, z].Dj);
                        }

                        else
                        {
                            T[r, z].Aj = T[r, z].alphaj / (T[r, z].Dj - T[r, z].betaj * T[r, z - 1].Aj);
                            T[r, z].Cj_prime = (T[r, z].betaj * T[r, z - 1].Cj_prime + T[r, z].Cj) / (T[r, z].Dj - T[r, z].betaj * T[r, z - 1].Aj);
                        }
                    }
                }
                for (int r = 0; r < rtop; r++)
                {
                    //solve from east to west
                    for (int z = (numOfZNodes - 1); z >= 0; z--)
                    {
                        if (z == (numOfZNodes - 1))
                        {
                            T[r, z].TP = T[r, z].Cj_prime;
                        }
                        else
                        {
                            T[r, z].TP = T[r, z].Aj * T[r, z + 1].TP + T[r, z].Cj_prime;
                        }

                        T[r, z].TP = Parameters.relaxationFactor_temperature * T[r, z].TP + (1 - Parameters.relaxationFactor_temperature) * T[r, z].TP_prime;

                        if (double.IsNaN(T[r, z].TP))
                        {
                            int test = 1;
                        }
                    }
                }

                endOfIteration = true;
                foreach (TNodes node in T)
                {
                    node.error = Helper.calculate_error(node.TP, node.TP_prime);
                    if (Math.Abs(node.error) < Parameters.tolerance_temperature)
                    {
                        node.isConverged = true;
                    }
                    else
                    {
                        node.isConverged = false;
                        endOfIteration = false;
                        break;
                    }
                }

                iterationCounter++;

            } while (endOfIteration == false);
        }
    }

    class EvaTemperaturePDE : TemperaturePDE
    {
        public EvaTemperaturePDE(Evaporator eva)
        {
            deltaR = eva.deltaR;
            deltaZ = eva.deltaZ;
            numOfRNodes = eva.numOfRNodes;
            numOfZNodes = eva.numOfZNodes;
            T = eva.T;

            assignBoundary_for_eva(eva);

            //initial conditions
            foreach (TNodes node in T)
            {
                node.TP0 = Parameters.T0;
                node.TP = node.TP0;
            }

            assignCoefficients2_for_eva(eva);
        }
        
        public void assignBoundary_for_eva(Evaporator eva)
        {
            int rtop = eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall;

            for (int r = 0; r < rtop; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    T[r, z].boundary = Boundary.Center;

                    if (z == 0)
                    {
                        T[r, z].boundary = Boundary.Left;
                    }
                    else if (z == (numOfZNodes - 1))
                    {
                        T[r, z].boundary = Boundary.Right;
                    }
                    if (r == 0)
                    {
                        T[r, z].boundary = Boundary.Bottom;
                        if (z == 0)
                        {
                            T[r, z].boundary = Boundary.BottomLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            T[r, z].boundary = Boundary.BottomRight;
                        }
                    }
                    else if (r == (rtop - 1))
                    {
                        T[r, z].boundary = Boundary.Top;
                        if (z == 0)
                        {
                            T[r, z].boundary = Boundary.TopLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            T[r, z].boundary = Boundary.TopRight;
                        }
                    }
                }
            }
        }

        public void assignCoefficients2_for_eva(Evaporator eva)
        {
            int rtop = eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall;

            for (int r = 0; r < rtop; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (r == 0)
                    {
                        T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                        T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                    }
                    else if (r == (rtop - 1))
                    {
                        T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                        T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r, z].thermalConductivity) / deltaR;
                    }
                    else
                    {
                        T[r, z].Dw = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].De = 2 * Math.PI * T[r, z].rp * deltaR * T[r, z].thermalConductivity / deltaZ;
                        T[r, z].Ds = 2 * Math.PI * T[r, z].rs * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r - 1, z].thermalConductivity) / deltaR;
                        T[r, z].Dn = 2 * Math.PI * T[r, z].rn * deltaZ * 0.5 * (T[r, z].thermalConductivity + T[r + 1, z].thermalConductivity) / deltaR;
                    }
                }
            }

            foreach (TNodes node in T)
            {
                node.aP0 = 2 * Math.PI * node.density * node.heatCapacity * node.rp * deltaR * deltaZ / Parameters.deltat;

                node.Fw = 2 * Math.PI * node.rp * deltaR * node.velocity * node.density * node.heatCapacity;
                node.Fe = 2 * Math.PI * node.rp * deltaR * node.velocity * node.density * node.heatCapacity;
                node.Fs = 2 * Math.PI * deltaZ * node.density * node.heatCapacity * 0 * node.rs;
                node.Fn = 2 * Math.PI * deltaZ * node.density * node.heatCapacity * 0 * node.rn;

                node.aW = Helper.max3(node.Fw, (node.Dw + 0.5 * node.Fw), 0);
                node.aE = Helper.max3(-node.Fe, (node.De - 0.5 * node.Fe), 0);
                node.aS = Helper.max3(node.Fs, (node.Ds + 0.5 * node.Fs), 0);
                node.aN = Helper.max3(-node.Fn, (node.Dn - 0.5 * node.Fn), 0);

                node.Sc = 0;
                node.Sp = 0;

                switch (node.boundary)
                {
                    case Boundary.Left:
                        node.aW = 0;
                        break;
                    case Boundary.Right:
                        node.aE = 0;
                        break;
                    case Boundary.Bottom:
                        node.aS = 0;
                        break;
                    case Boundary.Top:
                        node.aN = 0;
                        break;
                    case Boundary.BottomLeft:
                        node.aW = 0;
                        node.aS = 0;
                        break;
                    case Boundary.BottomRight:
                        node.aE = 0;
                        node.aS = 0;
                        break;
                    case Boundary.TopLeft:
                        node.aW = 0;
                        node.aN = 0;
                        break;
                    case Boundary.TopRight:
                        node.aE = 0;
                        node.aN = 0;
                        break;
                    default:
                        break;
                }

                node.deltaF = node.Fe - node.Fw + node.Fn - node.Fs;
                node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp + node.deltaF;
            }
        }

        public void updateCoefficients2(double inletTemperature, Evaporator eva)
        {
            foreach (TNodes node in T)
            {
                //inlet temperature
                if (node.boundary == Boundary.Left || node.boundary == Boundary.BottomLeft)
                {
                    if (node.materialType == MaterialType.HeatExchangeFluid)
                    {
                        node.Sc = (2 * node.Dw + node.Fw) * inletTemperature;
                        node.Sp = -(2 * node.Dw + node.Fw);
                        node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp + node.deltaF;
                    }
                }
            }

            //pool boiling
            poolBoiling(eva, out eva.rateOfEvaporation);
        }
        public void poolBoiling(Evaporator eva, out double rateOfEvaporation)
        {
            int r = eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall - 1;

            //saturated temperature for adsorbate
            double A = 8.07131, B = 1730.63, C = 233.426;
            double T_sat = (B / (A - Math.Log10(eva.averagePressure * 0.00750062))) - C + 273;


            //averaged wall temperature
            double sum = 0;
            int count = 0;
            for (int z = 0; z < numOfZNodes; z++)
            {
                sum += T[r, z].TP;
                count++;
            }
            double T_wall = sum / count;

            //rohsenow heat transfer coefficient for pool boiling
            double surfaceTension = 0.0729;//N/m
            double g = 9.81;//kgm/s2
            double C_sf = 0.013, m = 0, n = 0.333, p = 0.293 / n;
            double deltaT = T_wall - T_sat;
            double prandtl = (eva.adsorbate.heatCapacity_condensed * eva.adsorbate.dynamicViscosity_condensed) / eva.adsorbate.thermalConductivity_condensed;
            double term1 = eva.adsorbate.dynamicViscosity_condensed * eva.adsorbate.latentHeat;
            double term2 = surfaceTension / (g * (eva.adsorbate.density_condensed - eva.adsorbate.density));
            double term3 = eva.adsorbate.heatCapacity_condensed * Math.Abs(deltaT) / (eva.adsorbate.latentHeat * C_sf * Math.Pow(prandtl, (m + 1)));
            double term4 =  eva.averagePressure / (101e3);
            double heatFlux = term1 * Math.Pow(term2, -0.5) * Math.Pow(term3, Math.Round((1 / n), 5)) * Math.Pow(term4, -p);
            //double heatTransferCoefficient_evaporation = (term1 * Math.Pow(term2, -0.5) * Math.Pow(term3, Math.Round(1 / n))) / deltaT;
            double heatOfEvaporation = 0;
            rateOfEvaporation = 0;

            if ((T_wall - T_sat) > 0)
            {
                //apply heat flux
                for (int z = 0; z < numOfZNodes; z++)
                {
                    T[r, z].aN = 0;
                    T[r, z].Sc = -2 * Math.PI * T[r, z].rn * deltaZ * heatFlux;
                    T[r, z].aP = T[r, z].aW + T[r, z].aE + T[r, z].aS + T[r, z].aN + T[r, z].aP0 - T[r, z].Sp + T[r, z].deltaF;
                }

                heatOfEvaporation = heatFlux * eva.tubeOuterSurfaceArea;
                rateOfEvaporation = heatOfEvaporation / eva.adsorbate.latentHeat;
            }
        }

        public void TDMA2_for_eva(Evaporator eva)//solve: west to east, sweep: south to north
        {
            int rtop = eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall;

            do
            {
                foreach (TNodes node in T)
                {
                    node.TP_prime = node.TP;
                }

                for (int r = 0; r < rtop; r++)
                {
                    //get from neighbour lines
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        if (r == 0)
                        {
                            T[r, z].TS = 0;
                            T[r, z].TN = T[r + 1, z].TP;
                        }
                        else if (r == (rtop - 1))
                        {
                            T[r, z].TS = T[r - 1, z].TP;
                            T[r, z].TN = 0;
                        }
                        else
                        {
                            T[r, z].TS = T[r - 1, z].TP;
                            T[r, z].TN = T[r + 1, z].TP;
                        }
                    }
                }
                for (int r = 0; r < rtop; r++)
                {
                    //set from west to east
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        T[r, z].betaj = T[r, z].aW;
                        T[r, z].alphaj = T[r, z].aE;
                        T[r, z].Dj = T[r, z].aP;
                        T[r, z].Cj = T[r, z].aS * T[r, z].TS + T[r, z].aN * T[r, z].TN + T[r, z].aP0 * T[r, z].TP0 + T[r, z].Sc;

                        if (z == 0)
                        {
                            T[r, z].Aj = T[r, z].alphaj / (T[r, z].Dj);
                            T[r, z].Cj_prime = (T[r, z].Cj) / (T[r, z].Dj);
                        }

                        else
                        {
                            T[r, z].Aj = T[r, z].alphaj / (T[r, z].Dj - T[r, z].betaj * T[r, z - 1].Aj);
                            T[r, z].Cj_prime = (T[r, z].betaj * T[r, z - 1].Cj_prime + T[r, z].Cj) / (T[r, z].Dj - T[r, z].betaj * T[r, z - 1].Aj);
                        }
                    }
                }
                for (int r = 0; r < rtop; r++)
                {
                    //solve from east to west
                    for (int z = (numOfZNodes - 1); z >= 0; z--)
                    {
                        if (z == (numOfZNodes - 1))
                        {
                            T[r, z].TP = T[r, z].Cj_prime;
                        }
                        else
                        {
                            T[r, z].TP = T[r, z].Aj * T[r, z + 1].TP + T[r, z].Cj_prime;
                        }

                        T[r, z].TP = Parameters.relaxationFactor_temperature * T[r, z].TP + (1 - Parameters.relaxationFactor_temperature) * T[r, z].TP_prime;

                        if (double.IsNaN(T[r, z].TP))
                        {
                            int test = 1;
                        }
                    }
                }

                endOfIteration = true;
                foreach (TNodes node in T)
                {
                    node.error = Helper.calculate_error(node.TP, node.TP_prime);
                    if (Math.Abs(node.error) < Parameters.tolerance_temperature)
                    {
                        node.isConverged = true;
                    }
                    else
                    {
                        node.isConverged = false;
                        endOfIteration = false;
                        break;
                    }
                }

                iterationCounter++;

            } while (endOfIteration == false);
        }
    }
}
