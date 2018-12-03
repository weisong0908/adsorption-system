using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    class DensityPDE
    {
        /*
         * TDMA direction:  south to north
         * sweep direction: west to east
         */

        public double deltaR, deltaZ;
        public int numOfRNodes, numOfZNodes;
        public RhoNodes[,] Rho;

        public int iterationCounter = 0;
        public int[] numberOfIterationAtEachTimeStep = new int[Parameters.numOfTimeStep + 1];
        public bool endOfIteration = true;

        public void calculatePressure(Chamber chamber)
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (Rho[r, z].materialType == MaterialType.Adsorbate || Rho[r, z].materialType == MaterialType.Adsorbent)
                    {
                        Rho[r, z].pressure = Rho[r, z].RhoP * Rho[r, z].specificGasConstant * chamber.T[r, z].TP;
                    }
                    else
                    {
                        Rho[r, z].pressure = 0;
                    }
                }
            }

            //if (chamber.chamberType == ChamberType.Condenser)
            //{
            //    chamber.averagePressure = Parameters.P0_cond;
            //}
        }

        public void calculateDiffusivity(Chamber chamber)
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (Rho[r, z].materialType == MaterialType.Adsorbate)
                    {
                        //Molecular diffusion
                        Rho[r, z].diffusivity = (0.0001 * 0.00158 * Math.Sqrt(Math.Pow(chamber.T[r, z].TP, 3)) * Math.Sqrt(2 / Rho[r, z].molarMass)) / (Rho[r, z].pressure * 9.86923e-6 * Math.Pow(Rho[r, z].sigma, 2) * Rho[r, z].omega);
                    }

                    if (double.IsInfinity(Rho[r, z].diffusivity))
                    {
                        int t = 1;
                    }
                }
            }
        }

        public void assignCoefficients()
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                    {
                        Rho[r, z].aP0 = Math.PI * Math.Pow(deltaR, 2) * deltaZ * Rho[r, z].porosity / Parameters.deltat;
                        Rho[r, z].Sp = 0;
                        Rho[r, z].Sc = 0;

                        switch (Rho[r, z].boundary)
                        {
                            case Boundary.Left:
                                Rho[r, z].aW = 0;
                                Rho[r, z].aE = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aS = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rs / (Rho[r, z].rp * deltaR);
                                Rho[r, z].aN = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rn / (Rho[r, z].rp * deltaR);
                                break;
                            case Boundary.Right:
                                Rho[r, z].aW = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aE = 0;
                                Rho[r, z].aS = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rs / (Rho[r, z].rp * deltaR);
                                Rho[r, z].aN = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rn / (Rho[r, z].rp * deltaR);
                                break;
                            case Boundary.Bottom:
                                Rho[r, z].aW = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aE = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aS = 0;
                                Rho[r, z].aN = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rn / (Rho[r, z].rp * deltaR);
                                break;
                            case Boundary.Top:
                                Rho[r, z].aW = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aE = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aS = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rs / (Rho[r, z].rp * deltaR);
                                Rho[r, z].aN = 0;
                                break;
                            case Boundary.BottomLeft:
                                Rho[r, z].aW = 0;
                                Rho[r, z].aE = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aS = 0;
                                Rho[r, z].aN = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rn / (Rho[r, z].rp * deltaR);
                                break;
                            case Boundary.BottomRight:
                                Rho[r, z].aW = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aE = 0;
                                Rho[r, z].aS = 0;
                                Rho[r, z].aN = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rn / (Rho[r, z].rp * deltaR);
                                break;
                            case Boundary.TopLeft:
                                Rho[r, z].aW = 0;
                                Rho[r, z].aE = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aS = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rs / (Rho[r, z].rp * deltaR);
                                Rho[r, z].aN = 0;
                                break;
                            case Boundary.TopRight:
                                Rho[r, z].aW = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aE = 0;
                                Rho[r, z].aS = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rs / (Rho[r, z].rp * deltaR);
                                Rho[r, z].aN = 0;
                                break;
                            default:
                                Rho[r, z].aW = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aE = Math.PI * Math.Pow(deltaR, 2) * Rho[r, z].porosity * Rho[r, z].diffusivity / deltaZ;
                                Rho[r, z].aS = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rs / (Rho[r, z].rp * deltaR);
                                Rho[r, z].aN = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rn / (Rho[r, z].rp * deltaR);
                                break;
                        }

                        Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
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
                    if (Rho[r,z].materialType == MaterialType.Adsorbate || Rho[r, z].materialType == MaterialType.Adsorbent)
                    {
                        switch (Rho[r, z].boundary)
                        {
                            case Boundary.Left:
                                Rho[r, z].aW = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaZ;
                                Rho[r, z].aE = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z + 1].diffusivity) / deltaZ;
                                Rho[r, z].aS = 2 * Math.PI * Rho[r, z].rs * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r - 1, z].diffusivity) / deltaR;
                                Rho[r, z].aN = 2 * Math.PI * Rho[r, z].rn * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r + 1, z].diffusivity) / deltaR;
                                break;
                            case Boundary.Right:
                                Rho[r, z].aW = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z - 1].diffusivity) / deltaZ;
                                Rho[r, z].aE = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaZ;
                                Rho[r, z].aS = 2 * Math.PI * Rho[r, z].rs * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r - 1, z].diffusivity) / deltaR;
                                Rho[r, z].aN = 2 * Math.PI * Rho[r, z].rn * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r + 1, z].diffusivity) / deltaR;
                                break;
                            case Boundary.Bottom:
                                Rho[r, z].aW = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z - 1].diffusivity) / deltaZ;
                                Rho[r, z].aE = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z + 1].diffusivity) / deltaZ;
                                Rho[r, z].aS = 2 * Math.PI * Rho[r, z].rs * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaR;
                                Rho[r, z].aN = 2 * Math.PI * Rho[r, z].rn * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r + 1, z].diffusivity) / deltaR;
                                break;
                            case Boundary.Top:
                                Rho[r, z].aW = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z - 1].diffusivity) / deltaZ;
                                Rho[r, z].aE = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z + 1].diffusivity) / deltaZ;
                                Rho[r, z].aS = 2 * Math.PI * Rho[r, z].rs * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r - 1, z].diffusivity) / deltaR;
                                Rho[r, z].aN = 2 * Math.PI * Rho[r, z].rn * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaR;
                                break;
                            case Boundary.BottomLeft:
                                Rho[r, z].aW = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaZ;
                                Rho[r, z].aE = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z + 1].diffusivity) / deltaZ;
                                Rho[r, z].aS = 2 * Math.PI * Rho[r, z].rs * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaR;
                                Rho[r, z].aN = 2 * Math.PI * Rho[r, z].rn * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r + 1, z].diffusivity) / deltaR;
                                break;
                            case Boundary.BottomRight:
                                Rho[r, z].aW = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z - 1].diffusivity) / deltaZ;
                                Rho[r, z].aE = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaZ;
                                Rho[r, z].aS = 2 * Math.PI * Rho[r, z].rs * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaR;
                                Rho[r, z].aN = 2 * Math.PI * Rho[r, z].rn * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r + 1, z].diffusivity) / deltaR;
                                break;
                            case Boundary.TopLeft:
                                Rho[r, z].aW = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaZ;
                                Rho[r, z].aE = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z + 1].diffusivity) / deltaZ;
                                Rho[r, z].aS = 2 * Math.PI * Rho[r, z].rs * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r - 1, z].diffusivity) / deltaR;
                                Rho[r, z].aN = 2 * Math.PI * Rho[r, z].rn * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaR;
                                break;
                            case Boundary.TopRight:
                                Rho[r, z].aW = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z - 1].diffusivity) / deltaZ;
                                Rho[r, z].aE = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaZ;
                                Rho[r, z].aS = 2 * Math.PI * Rho[r, z].rs * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r - 1, z].diffusivity) / deltaR;
                                Rho[r, z].aN = 2 * Math.PI * Rho[r, z].rn * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r, z].diffusivity) / deltaR;
                                break;
                            default:
                                Rho[r, z].aW = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z - 1].diffusivity) / deltaZ;
                                Rho[r, z].aE = 2 * Math.PI * Rho[r, z].rp * deltaR * 0.5 * (Rho[r, z].diffusivity + Rho[r, z + 1].diffusivity) / deltaZ;
                                Rho[r, z].aS = 2 * Math.PI * Rho[r, z].rs * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r - 1, z].diffusivity) / deltaR;
                                Rho[r, z].aN = 2 * Math.PI * Rho[r, z].rn * deltaZ * 0.5 * (Rho[r, z].diffusivity + Rho[r + 1, z].diffusivity) / deltaR;
                                break;
                        }
                    }
                }
            }
            foreach (RhoNodes node in Rho)
            {
                if (node.materialType == MaterialType.Adsorbate || node.materialType == MaterialType.Adsorbent)
                {
                    node.aP0 = 2 * Math.PI * node.rp * deltaR * deltaZ * node.porosity;

                    //node.aW = 2 * Math.PI * deltaR * node.rp * node.porosity * node.diffusivity / deltaZ;
                    //node.aE = 2 * Math.PI * deltaR * node.rp * node.porosity * node.diffusivity / deltaZ;
                    //node.aS = 2 * Math.PI * deltaZ * node.porosity * node.rs * node.diffusivity / deltaR;
                    //node.aN = 2 * Math.PI * deltaZ * node.porosity * node.rn * node.diffusivity / deltaR;

                    node.Sc = 0;
                    node.Sp = 0;

                    //if(node.materialType == MaterialType.Tube)
                    //{
                    //    node.Sc = 1e300*0;
                    //    node.Sp = -1e300;
                    //}

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

                    node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp;
                }
            }
        }
    }

    class BedDensityPDE : DensityPDE
    {
        public BedDensityPDE(Bed bed)
        {
            deltaR = bed.deltaR;
            deltaZ = bed.deltaZ;
            numOfRNodes = bed.numOfRNodes;
            numOfZNodes = bed.numOfZNodes;
            Rho = bed.Rho;

            //assignMaterials(bed);
            assignBoundary(bed);

            //initial conditions
            foreach (RhoNodes node in Rho)
            {
                if (node.materialType == MaterialType.Adsorbent || node.materialType == MaterialType.Adsorbate)
                {
                    node.RhoP0 = Parameters.P0_bed / (bed.adsorbate.specificGasConstant * Parameters.T0);
                    node.RhoP = node.RhoP0;
                    node.pressure = node.RhoP * node.specificGasConstant * Parameters.T0;

                    if (node.materialType == MaterialType.Adsorbent)
                    {
                        node.q = Parameters.q0;
                    }
                }
                else
                {
                    node.RhoP0 = 0;
                    node.RhoP = node.RhoP0;
                }
            }

            calculateDiffusivity(bed);
            //assignCoefficients();
            assignCoefficients2();
        }

        public void assignBoundary(Bed bed)
        {
            for (int r = (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall); r < numOfRNodes; r++)
            {
                for (int z = 0; z < (numOfZNodes); z++)
                {
                    if (Rho[r, z].materialType == MaterialType.Adsorbent)
                    {
                        Rho[r, z].boundary = Boundary.Center;

                        if (Rho[r, z - 1].materialType != MaterialType.Adsorbent)
                        {
                            Rho[r, z].boundary = Boundary.Left;
                        }
                        else if (Rho[r, z + 1].materialType != MaterialType.Adsorbent)
                        {
                            Rho[r, z].boundary = Boundary.Right;
                        }
                        if (Rho[r - 1, z].materialType != MaterialType.Adsorbent)
                        {
                            Rho[r, z].boundary = Boundary.Bottom;
                            if (Rho[r, z - 1].materialType != MaterialType.Adsorbent)
                            {
                                Rho[r, z].boundary = Boundary.BottomLeft;
                            }
                            else if (Rho[r, z + 1].materialType != MaterialType.Adsorbent)
                            {
                                Rho[r, z].boundary = Boundary.BottomRight;
                            }
                        }
                        else if (r == (numOfRNodes - 1) && Rho[r, z].materialType == MaterialType.Adsorbent)
                        {
                            Rho[r, z].boundary = Boundary.Top;
                            if (Rho[r, z - 1].materialType != MaterialType.Adsorbent)
                            {
                                Rho[r, z].boundary = Boundary.TopLeft;
                            }
                            else if (Rho[r, z + 1].materialType != MaterialType.Adsorbent)
                            {
                                Rho[r, z].boundary = Boundary.TopRight;
                            }
                        }
                    }
                    else if (Rho[r, z].materialType == MaterialType.Adsorbate)
                    {
                        Rho[r, z].boundary = Boundary.Center;

                        if (z == 0)
                        {
                            Rho[r, z].boundary = Boundary.Left;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            Rho[r, z].boundary = Boundary.Right;
                        }
                        if(z==(bed.numOfZNodes_excessTube-1))
                        {
                            if(r>(bed.numOfRNodes_fluid+bed.numOfRNodes_tubeWall-1)&& r<(bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall + bed.numOfRNodes_adsorbent))
                            {
                                Rho[r, z].boundary = Boundary.Right;
                            }
                        }
                        else if(z== (numOfZNodes - bed.numOfZNodes_excessTube))
                        {
                            if (r > (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall - 1) && r < (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall + bed.numOfRNodes_adsorbent))
                            {
                                Rho[r, z].boundary = Boundary.Left;
                            }
                        }

                        if (Rho[r-1, z].materialType != MaterialType.Adsorbate && Rho[r - 1, z].materialType != MaterialType.Adsorbent)
                        {
                            Rho[r, z].boundary = Boundary.Bottom;
                            if (z == 0|| z==(numOfZNodes-bed.numOfZNodes_excessTube))
                            {
                                Rho[r, z].boundary = Boundary.BottomLeft;
                            }
                            else if (z == (numOfZNodes - 1) || z==(bed.numOfZNodes_excessTube-1))
                            {
                                Rho[r, z].boundary = Boundary.BottomRight;
                            }
                        }
                        else if (r == (numOfRNodes - 1))
                        {
                            Rho[r, z].boundary = Boundary.Top;
                            if (z == 0)
                            {
                                Rho[r, z].boundary = Boundary.TopLeft;
                            }
                            else if (z == (numOfZNodes - 1))
                            {
                                Rho[r, z].boundary = Boundary.TopRight;
                            }
                        }
                        if (Rho[r - 1, z].materialType == MaterialType.Adsorbent)
                        {
                            Rho[r, z].boundary = Boundary.Center;
                        }
                    }
                }
            }
        }

        public void calculateDiffusivity(Bed bed)
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (Rho[r, z].materialType == MaterialType.Adsorbate || Rho[r, z].materialType == MaterialType.Adsorbent)
                    {
                        //calculate critical pressure
                        double P_critical = (3 / (4 * Math.Sqrt(2))) * Math.Sqrt((Math.PI * bed.adsorbate.molarGasConstant * bed.T[r, z].TP) / bed.adsorbate.molarMass) / bed.adsorbent.meanFreePath;

                        if (Rho[r, z].pressure > (P_critical * 1.2))//if pressure is 1.2 times larger than critical pressure
                        {
                            //Molecular diffusion
                            Rho[r, z].diffusivity = (0.0001 * 0.00158 * Math.Sqrt(Math.Pow(bed.T[r, z].TP, 3)) * Math.Sqrt(2 / Rho[r, z].molarMass)) / (Rho[r, z].pressure * 9.86923e-6 * Math.Pow(Rho[r, z].sigma, 2) * Rho[r, z].omega);
                        }
                        else if (Rho[r, z].pressure < (P_critical / 1.2))//if pressure is 1.2 times smaller than critical pressure
                        {
                            Rho[r, z].diffusivity = 0.0001 * ((2 * bed.adsorbent.meanFreePath * 100) / 3) * Math.Sqrt((8 * Rho[r, z].molarGasConstant * bed.T[r, z].TP) / (Math.PI * (Rho[r, z].molarMass / 1000)));
                        }
                        else//around the critical pressure
                        {
                            //Molecular diffusion
                            double M = (0.0001 * 0.00158 * Math.Sqrt(Math.Pow(bed.T[r, z].TP, 3)) * Math.Sqrt(2 / Rho[r, z].molarMass)) / (Rho[r, z].pressure * 9.86923e-6 * Math.Pow(Rho[r, z].sigma, 2) * Rho[r, z].omega);
                            //Kundsen diffusion
                            double K = 0.0001 * ((2 * bed.adsorbent.meanFreePath * 100) / 3) * Math.Sqrt((8 * Rho[r, z].molarGasConstant * bed.T[r, z].TP) / (Math.PI * (Rho[r, z].molarMass / 1000)));

                            Rho[r, z].diffusivity = 1 / ((1 / M) + (1 / K));
                        }

                        //Rho[r, z].diffusivity = 0.006;
                    }

                    if (Rho[r, z].materialType == MaterialType.Adsorbate)
                    {
                        //Molecular diffusion
                        Rho[r, z].diffusivity = (0.0001 * 0.00158 * Math.Sqrt(Math.Pow(bed.T[r, z].TP, 3)) * Math.Sqrt(2 / Rho[r, z].molarMass)) / (Rho[r, z].pressure * 9.86923e-6 * Math.Pow(Rho[r, z].sigma, 2) * Rho[r, z].omega);

                        //Rho[r, z].diffusivity = 0.0003;
                    }
                }
            }
        }

        public void calculateRateOfAdsorption(Bed bed)
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (Rho[r, z].materialType == MaterialType.Adsorbent)
                    {
                        //toth equation
                        double K = bed.workingPair.K0 * Math.Exp((bed.workingPair.heatOfAdsorption) / (bed.adsorbate.specificGasConstant * bed.T[r, z].TP0));
                        double tothConstant = 8;
                        bed.Rho[r, z].q_eq = ((bed.workingPair.qm * K * bed.Rho[r, z].pressure) / (Math.Pow((1 + Math.Pow((K * bed.Rho[r, z].pressure), tothConstant)), (1 / tothConstant))));
                        //bed.Rho[r, z].q_eq = ((bed.workingPair.qm * K * bed.averagePressure) / (Math.Pow((1 + Math.Pow((K * bed.averagePressure), tothConstant)), (1 / tothConstant))));


                        //S-C equation
                        //saturated pressure of water vapour
                        double A = 8.07131, B = 1730.63, C = 233.426;
                        double saturatedPressure = Math.Pow(10, (A - (B / (C + bed.T[r, z].TP0 - 273)))) * 133.322;//covnert Torr to Pa
                        //the isotherm
                        double m = 1.4, alpha = 9e-7;
                        double K1 = alpha * Math.Exp((m * (4000e3 - bed.adsorbate.latentHeat)) / (bed.adsorbate.specificGasConstant * bed.T[r, z].TP0));
                        double K2 = Math.Pow((bed.Rho[r, z].pressure / saturatedPressure), m);
                        bed.Rho[r, z].q_eq = (bed.workingPair.qm * K1 * K2) / (1 + (K1 - 1) * K2);

                        //rate of desorption
                        double D0 = bed.workingPair.Ds0 * Math.Exp(-(bed.workingPair.activationEnergy / (bed.adsorbate.molarGasConstant * bed.T[r, z].TP0)));
                        bed.Rho[r, z].dqdt = ((15 * D0) / Math.Pow(bed.adsorbent.adsorbentRadius, 2)) * (bed.Rho[r, z].q_eq - bed.Rho[r, z].q);
                    }
                }
            }
        }

        public void updateCoefficients(Bed bed, double massFlowRate)
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    //flow rate to condenser and evaporator
                    if (Rho[r, z].boundary == Boundary.Top || Rho[r, z].boundary == Boundary.TopLeft || Rho[r, z].boundary == Boundary.TopRight)
                    {
                        if (massFlowRate != 0)
                        {
                            massFlowRate = -0.002;
                            Rho[r, z].aN = 0;
                            Rho[r, z].Sc = Math.PI * deltaR * deltaZ * Rho[r, z].rn * massFlowRate / (Parameters.numOfParallelTube_bed * Rho[r, z].rp * bed.voidOuterSurfaceArea);
                        }
                    }

                    //adsorption or desorption
                    if (Rho[r, z].materialType == MaterialType.Adsorbent)
                    {
                        Rho[r, z].Sc = -bed.adsorbent.density * Rho[r, z].dqdt * Math.PI * (Math.Pow(deltaR, 2)) * deltaZ;// / Rho[r, z].porosity;
                    }

                    //condensation or evaporation on excess tubes
                    if (r == (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall) && Rho[r, z].materialType == MaterialType.Adsorbate)
                    {
                        if (!double.IsNaN(bed.rateOfCondensation1) && bed.rateOfCondensation1 != 0)
                        {
                            if (z > 0 && z < bed.numOfZNodes_excessTube)
                            {
                                Rho[r, z].aS = 0;
                                Rho[r, z].Sc = -Math.PI * deltaR * deltaZ * Rho[r, z].rn * bed.rateOfCondensation1 / (Rho[r, z].rp * bed.tubeOuterSurfaceArea);
                                Rho[r, z].Sc = -deltaR * bed.rateOfCondensation1 / (2 * Rho[r, z].rp);
                                //Rho[r, z].Sc = -Math.PI * deltaR * deltaZ * Rho[r, z].rn * cond.rateOfCondensation * numOfZNodes / (numOfZNodes * Rho[r, z].rp * cond.tubeOuterSurfaceArea);
                                //Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
                            }
                            else if (z > (numOfZNodes - bed.numOfZNodes_excessTube) && z < numOfZNodes)
                            {
                                Rho[r, z].aS = 0;
                                Rho[r, z].Sc = -Math.PI * deltaR * deltaZ * Rho[r, z].rn * bed.rateOfCondensation2 / (Rho[r, z].rp * bed.tubeOuterSurfaceArea);
                                Rho[r, z].Sc = -deltaR * bed.rateOfCondensation2 / (2 * Rho[r, z].rp);
                            }
                        }

                        //else if(!double.IsNaN(bed.rateOfEvaporation1) && bed.rateOfEvaporation1 != 0)
                        //{
                        //    if (z > 0 && z < bed.numOfZNodes_tube)
                        //    {
                        //        Rho[r, z].aS = 0;
                        //        Rho[r, z].Sc = Math.PI * deltaR * deltaZ * Rho[r, z].rn * bed.rateOfEvaporation1 / (Rho[r, z].rp * bed.tubeOuterSurfaceArea);
                        //        Rho[r, z].Sc = deltaR * bed.rateOfEvaporation1 / (2 * Rho[r, z].rp);
                        //        //Rho[r, z].Sc = -Math.PI * deltaR * deltaZ * Rho[r, z].rn * cond.rateOfCondensation * numOfZNodes / (numOfZNodes * Rho[r, z].rp * cond.tubeOuterSurfaceArea);
                        //        //Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
                        //    }
                        //    else if (z > (numOfZNodes - bed.numOfZNodes_tube) && z < numOfZNodes)
                        //    {
                        //        Rho[r, z].aS = 0;
                        //        Rho[r, z].Sc = Math.PI * deltaR * deltaZ * Rho[r, z].rn * bed.rateOfEvaporation2 / (Rho[r, z].rp * bed.tubeOuterSurfaceArea);
                        //        Rho[r, z].Sc = deltaR * bed.rateOfEvaporation2 / (2 * Rho[r, z].rp);
                        //    }
                        //}
                    }

                    Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
                }
            }
        }

        public void updateCoefficients2(Bed bed, double massFlowRate)
        {
            foreach (RhoNodes node in Rho)
            {
                //flow rate to condenser and evaporator
                if (node.boundary == Boundary.Top || node.boundary == Boundary.TopLeft || node.boundary == Boundary.TopRight)
                {
                    node.aN = 0;
                    node.Sc = 2 * Math.PI * node.rn * deltaZ * node.porosity * (massFlowRate / bed.voidOuterSurfaceArea) / Parameters.numOfParallelTube_bed;
                    //node.Sc = Math.PI * deltaR * deltaZ * node.rn * massFlowRate / (Parameters.numOfParallelTube_bed * node.rp * bed.voidOuterSurfaceArea);
                }

                //adsorption or desorption
                if (node.materialType == MaterialType.Adsorbent)
                {
                    node.Sc = -2 * Math.PI * node.rp * deltaR * deltaZ * bed.adsorbent.density * node.dqdt;
                }

                node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp;
            }

            //condensation on excess tube surface
            condensationOnExcessTubeSurface(bed, 1);
            condensationOnExcessTubeSurface(bed, 2);
        }

        public void updateCoefficients3(Bed bed, double averageMass)//1st type BC
        {
            foreach (RhoNodes node in Rho)
            {
                //flow rate to condenser and evaporator
                if (node.boundary == Boundary.Top || node.boundary == Boundary.TopLeft || node.boundary == Boundary.TopRight)
                {
                    node.aN = 0;
                    if (averageMass != 0)
                    {
                        node.Sc = 2 * (2 * Math.PI * deltaZ * node.porosity * node.rn * node.diffusivity / deltaR) * (averageMass) / Parameters.numOfParallelTube_bed;
                        node.Sp = -2 * (2 * Math.PI * deltaZ * node.porosity * node.rn * node.diffusivity / deltaR);
                    }
                }

                //adsorption or desorption
                if (node.materialType == MaterialType.Adsorbent)
                {
                    node.Sc = -2 * Math.PI * node.rp * deltaR * deltaZ * bed.adsorbent.density * node.dqdt;
                }

                node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp;
            }

            ////condensation on excess tube surface
            //condensationOnExcessTubeSurface(bed, 1);
            //condensationOnExcessTubeSurface(bed, 2);
        }

        public void condensationOnExcessTubeSurface(Bed bed, int part)
        {
            double rateOfCondensation = 0;
            int startIndex = 0, endIndex = 0;
            switch (part)
            {
                case 1:
                    startIndex = 0;
                    endIndex = bed.numOfZNodes_excessTube;
                    rateOfCondensation = bed.rateOfCondensation1;
                    break;
                case 2:
                    startIndex = numOfZNodes - bed.numOfZNodes_excessTube;
                    endIndex = numOfZNodes;
                    rateOfCondensation = bed.rateOfCondensation2;
                    break;
                default:
                    Console.WriteLine("wrong data for bed excess tube");
                    Console.Read();
                    break;
            }
            int r = bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall;

            for (int z = startIndex; z < endIndex; z++)
            {
                Rho[r, z].aS = 0;
                Rho[r, z].Sc = -2 * Math.PI * Rho[r, z].rs * deltaZ * Rho[r, z].porosity * (rateOfCondensation / bed.excessTubeOuterSurfaceArea);
                Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
            }
        }

        public void TDMA(Bed bed, double massFlowRate)
        {
            do
            {
                foreach (RhoNodes node in Rho)
                {
                    if (node.materialType == MaterialType.Adsorbent || node.materialType == MaterialType.Adsorbate)
                    {
                        node.RhoP_prime = node.RhoP;
                    }
                }

                //update neighbouring pressure
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (Rho[r, z].boundary == Boundary.Left || Rho[r, z].boundary == Boundary.BottomLeft || Rho[r, z].boundary == Boundary.TopLeft)
                            {
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Right || Rho[r, z].boundary == Boundary.BottomRight || Rho[r, z].boundary == Boundary.TopRight)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Center || Rho[r, z].boundary == Boundary.Bottom || Rho[r, z].boundary == Boundary.Top)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                        }
                    }
                }

                //nonlinear terms
                calculateDiffusivity(bed);
                assignCoefficients();
                calculateRateOfAdsorption(bed);
                updateCoefficients(bed, massFlowRate);

                //calculate Aj and Cj_prime
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = (numOfRNodes - 1); r >= (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall); r--)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (r == (numOfRNodes - 1))
                            {
                                Rho[r, z].Aj = Rho[r, z].aS / (Rho[r, z].aP - Rho[r, z].aN);
                                Rho[r, z].Cj_prime = (Rho[r, z].aN + (Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc)) / (Rho[r, z].aP - Rho[r, z].aN);
                            }
                            else
                            {
                                Rho[r, z].Aj = Rho[r, z].aS / (Rho[r, z].aP - Rho[r, z].aN * Rho[r + 1, z].Aj);
                                Rho[r, z].Cj_prime = (Rho[r, z].aN * Rho[r + 1, z].Cj_prime + (Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc)) / (Rho[r, z].aP - Rho[r, z].aN * Rho[r + 1, z].Aj);
                            }
                        }
                    }
                }

                //calculate PP
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (r == (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall))
                            {
                                Rho[r, z].RhoP = Rho[r, z].Cj_prime;
                            }
                            else
                            {
                                Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r - 1, z].RhoP + Rho[r, z].Cj_prime;
                            }

                            Rho[r, z].RhoP = (1 - Parameters.relaxationFactor_density) * Rho[r, z].RhoP_prime + Parameters.relaxationFactor_density * Rho[r, z].RhoP;
                        }
                    }
                }

                foreach (RhoNodes node in Rho)
                {
                    if (node.RhoP < 0)
                    {
                        node.RhoP = 0;
                    }
                }

                //calculate error and decide if the iteration should continue
                endOfIteration = true;
                foreach (RhoNodes node in Rho)
                {
                    node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);

                    if (Math.Abs(node.error) < Parameters.relaxationFactor_density)
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

                //using (CsvWriter writer = new CsvWriter("pressure" + ".csv"))
                //{
                //    CsvRow row;

                //    for (int r = (bed.numOfRNodes - 1); r >= 0; r--)
                //    {
                //        row = new CsvRow();
                //        for (int z = 0; z < bed.numOfZNodes; z++)
                //        {
                //            row.Add((bed.Rho[r, z].pressure).ToString("0.00000"));
                //        }
                //        writer.WriteRow(row);
                //    }
                //}

                iterationCounter++;
                calculatePressure(bed);

                //using (CsvWriter writer = new CsvWriter("P old " + iterationCounter + ".csv"))
                //{
                //    CsvRow row;

                //    for (int r = (numOfRNodes - 1); r >= 0; r--)
                //    {
                //        row = new CsvRow();
                //        for (int z = 0; z < numOfZNodes; z++)
                //        {
                //            row.Add((Rho[r, z].pressure).ToString("0.00000"));
                //        }
                //        writer.WriteRow(row);
                //    }
                //}

                //int test = 1;

            } while (endOfIteration == false);

            //calculatePressure(bed);
        }

        public void TDMA2(Bed bed, double massFlowRate)//solve: west to east, sweep: south to north
        {
            do
            {
                foreach (RhoNodes node in Rho)
                {
                    node.RhoP_prime = node.RhoP;
                }

                for (int r = (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall - 1); r < numOfRNodes; r++)
                {
                    //get from neighbour lines
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            //if (r == 0)
                            //{
                            //    Rho[r, z].RhoS = 0;
                            //    Rho[r, z].RhoN = Rho[r + 1, z].RhoP;
                            //}
                            //else if (r == (numOfRNodes - 1))
                            //{
                            //    Rho[r, z].RhoS = Rho[r - 1, z].RhoP;
                            //    Rho[r, z].RhoN = 0;
                            //}
                            //else
                            //{
                            //    Rho[r, z].RhoS = Rho[r - 1, z].RhoP;
                            //    Rho[r, z].RhoN = Rho[r + 1, z].RhoP;
                            //}

                            if (Rho[r, z].boundary == Boundary.Bottom || Rho[r, z].boundary == Boundary.BottomLeft || Rho[r, z].boundary == Boundary.BottomRight)
                            {
                                Rho[r, z].RhoS = 0;
                                Rho[r, z].RhoN = Rho[r + 1, z].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Top || Rho[r, z].boundary == Boundary.TopLeft || Rho[r, z].boundary == Boundary.TopRight)
                            {
                                Rho[r, z].RhoS = Rho[r - 1, z].RhoP;
                                Rho[r, z].RhoN = 0;
                            }
                            else if (Rho[r, z].boundary == Boundary.Center || Rho[r, z].boundary == Boundary.Left || Rho[r, z].boundary == Boundary.Right)
                            {
                                Rho[r, z].RhoS = Rho[r - 1, z].RhoP;
                                Rho[r, z].RhoN = Rho[r + 1, z].RhoP;
                            }
                        }
                    }

                    //nonlinear terms
                    calculateDiffusivity(bed);
                    assignCoefficients2();
                    calculateRateOfAdsorption(bed);
                    updateCoefficients2(bed, massFlowRate);

                    //set from west to east
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            Rho[r, z].betaj = Rho[r, z].aW;
                            Rho[r, z].alphaj = Rho[r, z].aE;
                            Rho[r, z].Dj = Rho[r, z].aP;
                            Rho[r, z].Cj = Rho[r, z].aS * Rho[r, z].RhoS + Rho[r, z].aN * Rho[r, z].RhoN + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc;

                            if (z == 0)
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj);
                                Rho[r, z].Cj_prime = (Rho[r, z].Cj) / (Rho[r, z].Dj);
                            }

                            else
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r, z - 1].Aj);
                                Rho[r, z].Cj_prime = (Rho[r, z].betaj * Rho[r, z - 1].Cj_prime + Rho[r, z].Cj) / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r, z - 1].Aj);
                            }

                            //if Dj, betaj, or Aj is 0 due to the node is not adsorbent or adsorbate
                            if (double.IsNaN(Rho[r, z].Aj) || double.IsNaN(Rho[r, z].Cj_prime))
                            {
                                Rho[r, z].Aj = 0;
                                Rho[r, z].Cj_prime = 0;
                            }
                        }
                    }

                    //solve from east to west
                    for (int z = (numOfZNodes - 1); z >= 0; z--)
                    {
                        if (z == (numOfZNodes - 1))
                        {
                            Rho[r, z].RhoP = Rho[r, z].Cj_prime;
                        }
                        else
                        {
                            Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r, z + 1].RhoP + Rho[r, z].Cj_prime;
                        }
                    }
                }

                endOfIteration = true;
                foreach (RhoNodes node in Rho)
                {
                    node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);
                    if (Math.Abs(node.error) < Parameters.tolerance_density)
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
                calculatePressure(bed);

            } while (endOfIteration == false);
        }

        public void TDMA3(Bed bed, double massFlowRate)//solve: south to north, sweep: west to east
        {
            do
            {
                foreach (RhoNodes node in Rho)
                {
                    node.RhoP_prime = node.RhoP;
                }

                for (int z = 0; z < numOfZNodes; z++)
                {
                    //get from neighbour lines
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            //if (z == 0)
                            //{
                            //    Rho[r, z].RhoW = 0;
                            //    Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            //}
                            //else if (z == (numOfZNodes - 1))
                            //{
                            //    Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                            //    Rho[r, z].RhoE = 0;
                            //}
                            //else
                            //{
                            //    Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                            //    Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            //}

                            if (Rho[r, z].boundary == Boundary.Left || Rho[r, z].boundary == Boundary.BottomLeft || Rho[r, z].boundary == Boundary.TopLeft)
                            {
                                Rho[r, z].RhoW = 0;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Right || Rho[r, z].boundary == Boundary.BottomRight || Rho[r, z].boundary == Boundary.TopRight)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = 0;
                            }
                            else if (Rho[r, z].boundary == Boundary.Center || Rho[r, z].boundary == Boundary.Bottom || Rho[r, z].boundary == Boundary.Top)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                        }
                    }

                    //nonlinear terms
                    calculateDiffusivity(bed);
                    assignCoefficients2();
                    calculateRateOfAdsorption(bed);
                    updateCoefficients2(bed, massFlowRate);

                    //set from south to north
                    for (int r = (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall); r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            Rho[r, z].betaj = Rho[r, z].aS;
                            Rho[r, z].alphaj = Rho[r, z].aN;
                            Rho[r, z].Dj = Rho[r, z].aP;
                            Rho[r, z].Cj = Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc;

                            if (r == (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall))
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj);
                                Rho[r, z].Cj_prime = (Rho[r, z].Cj) / (Rho[r, z].Dj);
                            }

                            else
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r - 1, z].Aj);
                                Rho[r, z].Cj_prime = (Rho[r, z].betaj * Rho[r - 1, z].Cj_prime + Rho[r, z].Cj) / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r - 1, z].Aj);
                            }

                            //if Dj, betaj, or Aj is 0 due to the node is not adsorbent or adsorbate
                            if (double.IsNaN(Rho[r, z].Aj) || double.IsNaN(Rho[r, z].Cj_prime))
                            {
                                Rho[r, z].Aj = 0;
                                Rho[r, z].Cj_prime = 0;
                            }
                        }
                    }

                    //solve from north to south
                    for (int r = (numOfRNodes - 1); r >= (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall); r--)
                    {
                        if (r == (numOfRNodes - 1))
                        {
                            Rho[r, z].RhoP = Rho[r, z].Cj_prime;
                        }
                        else
                        {
                            Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r + 1, z].RhoP + Rho[r, z].Cj_prime;
                        }
                    }
                }

                endOfIteration = true;
                foreach (RhoNodes node in Rho)
                {
                    node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);
                    if (Math.Abs(node.error) < Parameters.tolerance_density)
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
                calculatePressure(bed);

            } while (endOfIteration == false);
        }

        public void TDMA4(Bed bed, double massFlowRate)//solve: north to south, sweep: west to east
        {
            do
            {
                foreach (RhoNodes node in Rho)
                {
                    if (node.materialType == MaterialType.Adsorbent || node.materialType == MaterialType.Adsorbate)
                    {
                        node.RhoP_prime = node.RhoP;
                    }
                }

                for (int z = 0; z < numOfZNodes; z++)
                {
                    //get from neighbour lines
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            //if (z == 0)
                            //{
                            //    Rho[r, z].RhoW = 0;
                            //    Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            //}
                            //else if (z == (numOfZNodes - 1))
                            //{
                            //    Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                            //    Rho[r, z].RhoE = 0;
                            //}
                            //else
                            //{
                            //    Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                            //    Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            //}
                            if (Rho[r, z].boundary == Boundary.Left || Rho[r, z].boundary == Boundary.BottomLeft || Rho[r, z].boundary == Boundary.TopLeft)
                            {
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Right || Rho[r, z].boundary == Boundary.BottomRight || Rho[r, z].boundary == Boundary.TopRight)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Center || Rho[r, z].boundary == Boundary.Bottom || Rho[r, z].boundary == Boundary.Top)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                        }
                    }
                }

                //nonlinear terms
                calculateDiffusivity(bed);
                assignCoefficients2();
                calculateRateOfAdsorption(bed);
                updateCoefficients2(bed, massFlowRate);

                //set from north to south
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = (numOfRNodes - 1); r >= (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall); r--)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            Rho[r, z].betaj = Rho[r, z].aN;
                            Rho[r, z].alphaj = Rho[r, z].aS;
                            Rho[r, z].Dj = Rho[r, z].aP;
                            Rho[r, z].Cj = Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc;

                            if (r == (numOfRNodes - 1))
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj);
                                Rho[r, z].Cj_prime = (Rho[r, z].Cj) / (Rho[r, z].Dj);
                            }

                            else
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r + 1, z].Aj);
                                Rho[r, z].Cj_prime = (Rho[r, z].betaj * Rho[r + 1, z].Cj_prime + Rho[r, z].Cj) / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r + 1, z].Aj);
                            }
                        }
                    }
                }

                //solve from south to north
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall); r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (r == (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall))
                            {
                                Rho[r, z].RhoP = Rho[r, z].Cj_prime;
                            }
                            else
                            {
                                Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r - 1, z].RhoP + Rho[r, z].Cj_prime;
                            }

                            Rho[r, z].RhoP = (1 - Parameters.relaxationFactor_density) * Rho[r, z].RhoP_prime + Parameters.relaxationFactor_density * Rho[r, z].RhoP;
                        }
                    }
                }

                foreach (RhoNodes node in Rho)
                {
                    if (node.RhoP < 0)
                    {
                        node.RhoP = 0;
                    }
                }

                endOfIteration = true;
                foreach (RhoNodes node in Rho)
                {
                    node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);
                    if (Math.Abs(node.error) < Parameters.tolerance_density)
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
                calculatePressure(bed);

                //using (CsvWriter writer = new CsvWriter("P new " + iterationCounter + ".csv"))
                //{
                //    CsvRow row;

                //    for (int r = (numOfRNodes - 1); r >= 0; r--)
                //    {
                //        row = new CsvRow();
                //        for (int z = 0; z < numOfZNodes; z++)
                //        {
                //            row.Add((Rho[r, z].pressure).ToString("0.00000"));
                //        }
                //        writer.WriteRow(row);
                //    }
                //}

                //int test = 1;

            } while (endOfIteration == false);
        }
    }

    //public void updateCoefficients(Bed bed, double connectingPressure)
    //{
    //    for (int r = 0; r < numOfRNodes; r++)
    //    {
    //        for (int z = 0; z < numOfZNodes; z++)
    //        {
    //            if (Rho[r, z].boundary == Boundary.Top || Rho[r, z].boundary == Boundary.TopLeft || Rho[r, z].boundary == Boundary.TopRight)
    //            {
    //                if (connectingPressure != 0)
    //                {
    //                    Rho[r, z].aN = Math.PI * deltaR * deltaZ * Rho[r, z].porosity * Rho[r, z].diffusivity * Rho[r, z].rn / (Rho[r, z].rp * deltaR);
    //                    Rho[r, z].Sc = (2 * Rho[r, z].aN) * connectingPressure;
    //                    Rho[r, z].Sp = -(2 * Rho[r, z].aN);
    //                }
    //            }

    //            if (Rho[r, z].materialType == MaterialType.Adsorbent)
    //            {
    //                //adsorption or desorption
    //                Rho[r, z].Sc = -bed.adsorbent.density * Rho[r, z].dqdt * Math.PI * (Math.Pow(deltaR, 2)) * deltaZ;
    //            }

    //            Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
    //        }
    //    }
    //}

    //public void TDMA(Bed bed, double connectingPressure)
    //{
    //    do
    //    {
    //        foreach (RhoNodes node in Rho)
    //        {
    //            if (node.materialType == MaterialType.Adsorbent || node.materialType == MaterialType.Adsorbate)
    //            {
    //                node.RhoP_prime = node.RhoP;
    //            }
    //        }

    //        //update neighbouring pressure
    //        for (int z = 0; z < numOfZNodes; z++)
    //        {
    //            for (int r = 0; r < numOfRNodes; r++)
    //            {
    //                if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
    //                {
    //                    if (Rho[r, z].boundary == Boundary.Left || Rho[r, z].boundary == Boundary.BottomLeft || Rho[r, z].boundary == Boundary.TopLeft)
    //                    {
    //                        Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
    //                    }
    //                    else if (Rho[r, z].boundary == Boundary.Right || Rho[r, z].boundary == Boundary.BottomRight || Rho[r, z].boundary == Boundary.TopRight)
    //                    {
    //                        Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
    //                    }
    //                    else if (Rho[r, z].boundary == Boundary.Center || Rho[r, z].boundary == Boundary.Bottom || Rho[r, z].boundary == Boundary.Top)
    //                    {
    //                        Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
    //                        Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
    //                    }
    //                }
    //            }
    //        }

    //        //nonlinear terms
    //        calculateDiffusivity(bed);
    //        assignCoefficients();
    //        updateCoefficients(bed, connectingPressure);

    //        //calculate Aj and Cj_prime
    //        for (int z = 0; z < numOfZNodes; z++)
    //        {
    //            for (int r = (numOfRNodes - 1); r >= (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall); r--)
    //            {
    //                if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
    //                {
    //                    if (r == (numOfRNodes - 1))
    //                    {
    //                        Rho[r, z].Aj = Rho[r, z].aS / (Rho[r, z].aP - Rho[r, z].aN);
    //                        Rho[r, z].Cj_prime = (Rho[r, z].aN + (Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc)) / (Rho[r, z].aP - Rho[r, z].aN);
    //                    }
    //                    else
    //                    {
    //                        Rho[r, z].Aj = Rho[r, z].aS / (Rho[r, z].aP - Rho[r, z].aN * Rho[r + 1, z].Aj);
    //                        Rho[r, z].Cj_prime = (Rho[r, z].aN * Rho[r + 1, z].Cj_prime + (Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc)) / (Rho[r, z].aP - Rho[r, z].aN * Rho[r + 1, z].Aj);
    //                    }
    //                }
    //            }
    //        }

    //        //calculate RhoP
    //        for (int z = 0; z < numOfZNodes; z++)
    //        {
    //            for (int r = (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall); r < numOfRNodes; r++)
    //            {
    //                if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
    //                {
    //                    if (Rho[r, z].boundary == Boundary.Bottom || Rho[r, z].boundary == Boundary.BottomLeft || Rho[r, z].boundary == Boundary.BottomRight)
    //                    {
    //                        Rho[r, z].RhoP = Rho[r, z].Cj_prime;
    //                    }
    //                    else
    //                    {
    //                        Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r - 1, z].RhoP + Rho[r, z].Cj_prime;
    //                    }

    //                    Rho[r, z].RhoP = (1 - Parameters.relaxationFactor_density) * Rho[r, z].RhoP_prime + Parameters.relaxationFactor_density * Rho[r, z].RhoP;
    //                }
    //            }
    //        }

    //        //calculate error and decide if the iteration should continue
    //        endOfIteration = true;
    //        foreach (RhoNodes node in Rho)
    //        {
    //            node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);

    //            if (Math.Abs(node.error) < Parameters.relaxationFactor_density)
    //            {
    //                node.isConverged = true;
    //            }
    //            else
    //            {
    //                node.isConverged = false;
    //                endOfIteration = false;
    //                break;
    //            }
    //        }

    //        iterationCounter++;

    //    } while (endOfIteration == false);

    //    calculatePressure(bed);
    //}

    class CondDensityPDE : DensityPDE
    {
        public CondDensityPDE(Condenser cond)
        {
            deltaR = cond.deltaR;
            deltaZ = cond.deltaZ;
            numOfRNodes = cond.numOfRNodes;
            numOfZNodes = cond.numOfZNodes;
            Rho = cond.Rho;

            assignBoundary(cond);            

            //initial conditions
            foreach (RhoNodes node in Rho)
            {
                if (node.materialType == MaterialType.Adsorbate)
                {
                    node.RhoP0 = Parameters.P0_cond / (cond.adsorbate.specificGasConstant * Parameters.T0);
                    node.RhoP = node.RhoP0;
                    node.pressure = node.RhoP * node.specificGasConstant * Parameters.T0;
                    node.pressure = Parameters.P0_cond;
                }
                else
                {
                    node.RhoP0 = 0;
                    node.RhoP = node.RhoP0;
                }
            }

            calculateDiffusivity(cond);
            //assignCoefficients();
            assignCoefficients2();
        }

        public void assignBoundary(Condenser cond)
        {
            for (int r = (cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall); r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    Rho[r, z].boundary = Boundary.Center;

                    if (z == 0)
                    {
                        Rho[r, z].boundary = Boundary.Left;
                    }
                    else if (z == (numOfZNodes - 1))
                    {
                        Rho[r, z].boundary = Boundary.Right;
                    }
                    if (r == (cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall))
                    {
                        Rho[r, z].boundary = Boundary.Bottom;
                        if (z == 0)
                        {
                            Rho[r, z].boundary = Boundary.BottomLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            Rho[r, z].boundary = Boundary.BottomRight;
                        }
                    }
                    else if (r == (numOfRNodes - 1))
                    {
                        Rho[r, z].boundary = Boundary.Top;
                        if (z == 0)
                        {
                            Rho[r, z].boundary = Boundary.TopLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            Rho[r, z].boundary = Boundary.TopRight;
                        }
                    }
                }
            }
        }

        public void updateCoefficients(Condenser cond, double massFlowRate)
        {
            int numOfParallelTubes = Parameters.numOfParallelTube_cond;

            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (Rho[r, z].boundary == Boundary.Top || Rho[r, z].boundary == Boundary.TopLeft || Rho[r, z].boundary == Boundary.TopRight)
                    {
                        if (massFlowRate != 0)
                        {
                            Rho[r, z].aN = 0;
                            Rho[r, z].Sc = Math.PI * deltaR * deltaZ * Rho[r, z].rn * massFlowRate / (numOfParallelTubes * Rho[r, z].rp * cond.voidOuterSurfaceArea);
                            //Rho[r, z].Sc = deltaR * massFlowRate / (numOfParallelTubes * 2 * Rho[r, z].rp);
                            //Rho[r, z].Sc = Math.PI * deltaR * deltaZ * Rho[r, z].rn * massFlowRate * numOfZNodes / (numOfParallelTubes * Rho[r, z].rp * cond.voidOuterSurfaceArea);
                            //Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
                        }
                    }
                    if (r == (cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall))
                    {
                        if (!double.IsNaN(cond.rateOfCondensation) && cond.rateOfCondensation != 0)
                        {
                            Rho[r, z].aS = 0;
                            Rho[r, z].Sc = -Math.PI * deltaR * deltaZ * Rho[r, z].rn * cond.rateOfCondensation / (Rho[r, z].rp * cond.tubeOuterSurfaceArea);
                            //Rho[r, z].Sc = -deltaR * cond.rateOfCondensation / (2 * Rho[r, z].rp);
                            //Rho[r, z].Sc = -Math.PI * deltaR * deltaZ * Rho[r, z].rn * cond.rateOfCondensation * numOfZNodes / (numOfZNodes * Rho[r, z].rp * cond.tubeOuterSurfaceArea);
                            //Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
                        }
                    }

                    Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
                }
            }
        }

        public void updateCoefficients2(Condenser cond, double massFlowRate)
        {
            foreach (RhoNodes node in Rho)
            {
                //mass flow rate from bed
                if (node.boundary == Boundary.Top || node.boundary == Boundary.TopLeft || node.boundary == Boundary.TopRight)
                {
                    node.aN = 0;
                    node.Sc = 2 * Math.PI * node.rn * deltaZ * node.porosity * (massFlowRate / cond.voidOuterSurfaceArea) / Parameters.numOfParallelTube_cond;
                }

                //condensation on condenser outer surface
                if (node.boundary == Boundary.Bottom || node.boundary == Boundary.BottomLeft || node.boundary == Boundary.BottomRight)
                {
                    node.aS = 0;
                    node.Sc = -2 * Math.PI * node.rs * deltaZ * node.porosity * (cond.rateOfCondensation / cond.tubeOuterSurfaceArea);
                }

                node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp;
            }
        }

        public void updateCoefficients3(Condenser cond, double averageMass)//1st type BC
        {
            foreach (RhoNodes node in Rho)
            {
                //mass flow rate from bed
                if (node.boundary == Boundary.Top || node.boundary == Boundary.TopLeft || node.boundary == Boundary.TopRight)
                {
                    node.aN = 0;
                    if (averageMass != 0)
                    {
                        node.Sc = 2 * (2 * Math.PI * deltaZ * node.porosity * node.rn * node.diffusivity / deltaR) * (averageMass);// / Parameters.numOfParallelTube_cond;
                        node.Sp = -2 * (2 * Math.PI * deltaZ * node.porosity * node.rn * node.diffusivity / deltaR);
                    }
                }

                //condensation on condenser outer surface
                if (node.boundary == Boundary.Bottom || node.boundary == Boundary.BottomLeft || node.boundary == Boundary.BottomRight)
                {
                    node.aS = 0;
                    //node.Sc = -2 * Math.PI * node.rs * deltaZ * node.porosity * (cond.rateOfCondensation / cond.tubeOuterSurfaceArea);
                }

                node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp;
            }
        }

        public void TDMA2(Condenser cond, double massFlowRate)//solve: west to east, sweep: south to north
        {
            do
            {
                foreach (RhoNodes node in Rho)
                {
                    node.RhoP_prime = node.RhoP;
                }

                for (int r = 0; r < numOfRNodes; r++)
                {
                    //get from neighbour lines
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        if (r == 0)
                        {
                            Rho[r, z].RhoS = 0;
                            Rho[r, z].RhoN = Rho[r + 1, z].RhoP;
                        }
                        else if (r == (numOfRNodes - 1))
                        {
                            Rho[r, z].RhoS = Rho[r - 1, z].RhoP;
                            Rho[r, z].RhoN = 0;
                        }
                        else
                        {
                            Rho[r, z].RhoS = Rho[r - 1, z].RhoP;
                            Rho[r, z].RhoN = Rho[r + 1, z].RhoP;
                        }
                    }

                    //nonlinear terms
                    calculateDiffusivity(cond);
                    assignCoefficients2();
                    updateCoefficients2(cond, massFlowRate);

                    //set from west to east
                    for (int z = 0; z < numOfZNodes; z++)
                    {
                        Rho[r, z].betaj = Rho[r, z].aW;
                        Rho[r, z].alphaj = Rho[r, z].aE;
                        Rho[r, z].Dj = Rho[r, z].aP;
                        Rho[r, z].Cj = Rho[r, z].aS * Rho[r, z].RhoS + Rho[r, z].aN * Rho[r, z].RhoN + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc;

                        if (z == 0)
                        {
                            Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj);
                            Rho[r, z].Cj_prime = (Rho[r, z].Cj) / (Rho[r, z].Dj);
                        }

                        else
                        {
                            Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r, z - 1].Aj);
                            Rho[r, z].Cj_prime = (Rho[r, z].betaj * Rho[r, z - 1].Cj_prime + Rho[r, z].Cj) / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r, z - 1].Aj);
                        }

                        //if Dj, betaj, or Aj is 0 due to the node is not adsorbent or adsorbate
                        if (double.IsNaN(Rho[r, z].Aj) || double.IsNaN(Rho[r, z].Cj_prime))
                        {
                            Rho[r, z].Aj = 0;
                            Rho[r, z].Cj_prime = 0;
                        }
                    }

                    //solve from east to west
                    for (int z = (numOfZNodes - 1); z >= 0; z--)
                    {
                        if (z == (numOfZNodes - 1))
                        {
                            Rho[r, z].RhoP = Rho[r, z].Cj_prime;
                        }
                        else
                        {
                            Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r, z + 1].RhoP + Rho[r, z].Cj_prime;
                        }
                    }
                }

                endOfIteration = true;
                foreach (RhoNodes node in Rho)
                {
                    node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);
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
                calculatePressure(cond);

            } while (endOfIteration == false);
        }

        public void TDMA(Condenser cond, double massFlowRate)
        {
            do
            {
                foreach (RhoNodes node in Rho)
                {
                    if (node.materialType == MaterialType.Adsorbent || node.materialType == MaterialType.Adsorbate)
                    {
                        node.RhoP_prime = node.RhoP;
                    }
                }

                //update neighbouring pressure
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (Rho[r, z].boundary == Boundary.Left || Rho[r, z].boundary == Boundary.BottomLeft || Rho[r, z].boundary == Boundary.TopLeft)
                            {
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Right || Rho[r, z].boundary == Boundary.BottomRight || Rho[r, z].boundary == Boundary.TopRight)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Center || Rho[r, z].boundary == Boundary.Bottom || Rho[r, z].boundary == Boundary.Top)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                        }
                    }
                }

                //nonlinear terms
                calculateDiffusivity(cond);
                assignCoefficients();
                updateCoefficients(cond, massFlowRate);

                //calculate Aj and Cj_prime
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = (numOfRNodes - 1); r >= (cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall); r--)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (r == (numOfRNodes - 1))
                            {
                                Rho[r, z].Aj = Rho[r, z].aS / (Rho[r, z].aP - Rho[r, z].aN);
                                Rho[r, z].Cj_prime = (Rho[r, z].aN + (Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc)) / (Rho[r, z].aP - Rho[r, z].aN);
                            }
                            else
                            {
                                Rho[r, z].Aj = Rho[r, z].aS / (Rho[r, z].aP - Rho[r, z].aN * Rho[r + 1, z].Aj);
                                Rho[r, z].Cj_prime = (Rho[r, z].aN * Rho[r + 1, z].Cj_prime + (Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc)) / (Rho[r, z].aP - Rho[r, z].aN * Rho[r + 1, z].Aj);
                            }
                        }
                    }
                }

                //calculate PP
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (r == (cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall))
                            {
                                Rho[r, z].RhoP = Rho[r, z].Cj_prime;
                            }
                            else
                            {
                                Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r - 1, z].RhoP + Rho[r, z].Cj_prime;
                            }

                            Rho[r, z].RhoP = (1 - Parameters.relaxationFactor_density) * Rho[r, z].RhoP_prime + Parameters.relaxationFactor_density * Rho[r, z].RhoP;
                        }
                    }
                }

                foreach (RhoNodes node in Rho)
                {
                    if (node.RhoP < 0)
                    {
                        node.RhoP = 0;
                    }
                }

                //calculate error and decide if the iteration should continue
                endOfIteration = true;
                foreach (RhoNodes node in Rho)
                {
                    node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);

                    if (Math.Abs(node.error) < Parameters.relaxationFactor_density)
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
                calculatePressure(cond);

            } while (endOfIteration == false);
        }//old

        public void TDMA4(Condenser cond, double massFlowRate)//solve: north to south, sweep: west to east
        {
            do
            {
                foreach (RhoNodes node in Rho)
                {
                    if (node.materialType == MaterialType.Adsorbent || node.materialType == MaterialType.Adsorbate)
                    {
                        node.RhoP_prime = node.RhoP;
                    }
                }

                for (int z = 0; z < numOfZNodes; z++)
                {
                    //get from neighbour lines
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (z == 0)
                            {
                                Rho[r, z].RhoW = 0;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                            else if (z == (numOfZNodes - 1))
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = 0;
                            }
                            else
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                        }
                    }
                }

                //nonlinear terms
                calculateDiffusivity(cond);
                assignCoefficients2();
                updateCoefficients2(cond, massFlowRate);

                //set from north to south
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = (numOfRNodes - 1); r >= (cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall); r--)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            Rho[r, z].betaj = Rho[r, z].aN;
                            Rho[r, z].alphaj = Rho[r, z].aS;
                            Rho[r, z].Dj = Rho[r, z].aP;
                            Rho[r, z].Cj = Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc;

                            if (r == (numOfRNodes - 1))
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj);
                                Rho[r, z].Cj_prime = (Rho[r, z].Cj) / (Rho[r, z].Dj);
                            }

                            else
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r + 1, z].Aj);
                                Rho[r, z].Cj_prime = (Rho[r, z].betaj * Rho[r + 1, z].Cj_prime + Rho[r, z].Cj) / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r + 1, z].Aj);
                            }
                        }
                    }
                }

                //solve from south to north
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (r == (cond.numOfRNodes_fluid + cond.numOfRNodes_tubeWall))
                            {
                                Rho[r, z].RhoP = Rho[r, z].Cj_prime;
                            }
                            else
                            {
                                Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r - 1, z].RhoP + Rho[r, z].Cj_prime;
                            }

                            Rho[r, z].RhoP = (1 - Parameters.relaxationFactor_density) * Rho[r, z].RhoP_prime + Parameters.relaxationFactor_density * Rho[r, z].RhoP;
                        }
                    }
                }

                foreach (RhoNodes node in Rho)
                {
                    if (node.RhoP < 0)
                    {
                        node.RhoP = 0;
                    }
                }

                endOfIteration = true;
                foreach (RhoNodes node in Rho)
                {
                    node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);
                    if (Math.Abs(node.error) < Parameters.tolerance_density)
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
                calculatePressure(cond);

            } while (endOfIteration == false);
        }
    }

    class EvaDensityPDE : DensityPDE
    {
        public EvaDensityPDE(Evaporator eva)
        {
            deltaR = eva.deltaR;
            deltaZ = eva.deltaZ;
            numOfRNodes = eva.numOfRNodes;
            numOfZNodes = eva.numOfZNodes;
            Rho = eva.Rho;

            assignBoundary(eva);

            //initial conditions
            foreach (RhoNodes node in Rho)
            {
                if (node.materialType == MaterialType.Adsorbate)
                {
                    node.RhoP0 = Parameters.P0_eva / (eva.adsorbate.specificGasConstant * Parameters.T0);
                    node.RhoP = node.RhoP0;
                    node.pressure = node.RhoP * node.specificGasConstant * Parameters.T0;
                    node.pressure = Parameters.P0_eva;
                }
                else
                {
                    node.RhoP0 = 0;
                    node.RhoP = node.RhoP0;
                }
            }

            calculateDiffusivity(eva);
            //assignCoefficients();
            assignCoefficients2();
        }

        public void assignBoundary(Evaporator eva)
        {
            for (int r = (eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall); r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    Rho[r, z].boundary = Boundary.Center;

                    if (z == 0)
                    {
                        Rho[r, z].boundary = Boundary.Left;
                    }
                    else if (z == (numOfZNodes - 1))
                    {
                        Rho[r, z].boundary = Boundary.Right;
                    }
                    if (r == (eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall))
                    {
                        Rho[r, z].boundary = Boundary.Bottom;
                        if (z == 0)
                        {
                            Rho[r, z].boundary = Boundary.BottomLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            Rho[r, z].boundary = Boundary.BottomRight;
                        }
                    }
                    else if (r == (numOfRNodes - 1))
                    {
                        Rho[r, z].boundary = Boundary.Top;
                        if (z == 0)
                        {
                            Rho[r, z].boundary = Boundary.TopLeft;
                        }
                        else if (z == (numOfZNodes - 1))
                        {
                            Rho[r, z].boundary = Boundary.TopRight;
                        }
                    }
                }
            }
        }

        public void updateCoefficients(Evaporator eva, double massFlowRate)
        {
            int numOfParallelTubes = Parameters.numOfParallelTube_eva;

            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (Rho[r, z].boundary == Boundary.Top || Rho[r, z].boundary == Boundary.TopLeft || Rho[r, z].boundary == Boundary.TopRight)
                    {
                        if (massFlowRate != 0)
                        {
                            //Rho[r, z].aN = 0;
                            //Rho[r, z].Sc = Math.PI * deltaR * deltaZ * Rho[r, z].rn * massFlowRate / (numOfParallelTubes * Rho[r, z].rp * eva.voidOuterSurfaceArea);
                            Rho[r, z].aN = 0;
                            Rho[r, z].Sc = Math.PI * deltaR * deltaZ * Rho[r, z].rn * massFlowRate / (numOfParallelTubes * Rho[r, z].rp * eva.voidOuterSurfaceArea);
                            //Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
                        }
                    }
                    if (r == (eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall))
                    {
                        if (!double.IsNaN(eva.rateOfEvaporation) && eva.rateOfEvaporation != 0)
                        {
                            Rho[r, z].aS = 0;
                            Rho[r, z].Sc = Math.PI * deltaR * deltaZ * Rho[r, z].rn * eva.rateOfEvaporation / (Rho[r, z].rp * eva.tubeOuterSurfaceArea);
                            //Rho[r, z].Sc = deltaR * eva.rateOfEvaporation / (2 * Rho[r, z].rp);
                            //Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
                        }
                    }

                    Rho[r, z].aP = Rho[r, z].aW + Rho[r, z].aE + Rho[r, z].aS + Rho[r, z].aN + Rho[r, z].aP0 - Rho[r, z].Sp;
                }
            }
        }

        public void updateCoefficients2(Evaporator eva, double massFlowRate)
        {
            foreach (RhoNodes node in Rho)
            {
                //mass flow rate from bed
                if (node.boundary == Boundary.Top || node.boundary == Boundary.TopLeft || node.boundary == Boundary.TopRight)
                {
                    node.aN = 0;
                    node.Sc = 2 * Math.PI * node.rn * deltaZ * node.porosity * (massFlowRate / eva.voidOuterSurfaceArea) / Parameters.numOfParallelTube_eva;
                }

                //pool boiling
                if (node.boundary == Boundary.Bottom || node.boundary == Boundary.BottomLeft || node.boundary == Boundary.BottomRight)
                {
                    node.aS = 0;
                    node.Sc = 0.3 * 2 * Math.PI * node.rs * deltaZ * node.porosity * (eva.rateOfEvaporation / eva.tubeOuterSurfaceArea);
                }

                node.aP = node.aW + node.aE + node.aS + node.aN + node.aP0 - node.Sp;
            }
        }

        public void TDMA(Evaporator eva, double massFlowRate)
        {
            do
            {
                foreach (RhoNodes node in Rho)
                {
                    if (node.materialType == MaterialType.Adsorbent || node.materialType == MaterialType.Adsorbate)
                    {
                        node.RhoP_prime = node.RhoP;
                    }
                }

                //update neighbouring pressure
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (Rho[r, z].boundary == Boundary.Left || Rho[r, z].boundary == Boundary.BottomLeft || Rho[r, z].boundary == Boundary.TopLeft)
                            {
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Right || Rho[r, z].boundary == Boundary.BottomRight || Rho[r, z].boundary == Boundary.TopRight)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                            }
                            else if (Rho[r, z].boundary == Boundary.Center || Rho[r, z].boundary == Boundary.Bottom || Rho[r, z].boundary == Boundary.Top)
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                        }
                    }
                }

                //nonlinear terms
                calculateDiffusivity(eva);
                assignCoefficients();
                updateCoefficients(eva, massFlowRate);

                //calculate Aj and Cj_prime
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = (numOfRNodes - 1); r >= (eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall); r--)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (r == (numOfRNodes - 1))
                            {
                                Rho[r, z].Aj = Rho[r, z].aS / (Rho[r, z].aP - Rho[r, z].aN);
                                Rho[r, z].Cj_prime = (Rho[r, z].aN + (Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc)) / (Rho[r, z].aP - Rho[r, z].aN);
                            }
                            else
                            {
                                Rho[r, z].Aj = Rho[r, z].aS / (Rho[r, z].aP - Rho[r, z].aN * Rho[r + 1, z].Aj);
                                Rho[r, z].Cj_prime = (Rho[r, z].aN * Rho[r + 1, z].Cj_prime + (Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc)) / (Rho[r, z].aP - Rho[r, z].aN * Rho[r + 1, z].Aj);
                            }
                        }
                    }
                }

                //calculate PP
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (r == (eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall))
                            {
                                Rho[r, z].RhoP = Rho[r, z].Cj_prime;
                            }
                            else
                            {
                                Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r - 1, z].RhoP + Rho[r, z].Cj_prime;
                            }

                            Rho[r, z].RhoP = (1 - Parameters.relaxationFactor_density) * Rho[r, z].RhoP_prime + Parameters.relaxationFactor_density * Rho[r, z].RhoP;
                        }
                    }
                }

                foreach (RhoNodes node in Rho)
                {
                    if (node.RhoP < 0)
                    {
                        node.RhoP = 0;
                    }
                }

                //calculate error and decide if the iteration should continue
                endOfIteration = true;
                foreach (RhoNodes node in Rho)
                {
                    node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);

                    if (Math.Abs(node.error) < Parameters.relaxationFactor_density)
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
                calculatePressure(eva);

            } while (endOfIteration == false);
        }//old

        public void TDMA4(Evaporator eva, double massFlowRate)//solve: north to south, sweep: west to east
        {
            do
            {
                foreach (RhoNodes node in Rho)
                {
                    if (node.materialType == MaterialType.Adsorbent || node.materialType == MaterialType.Adsorbate)
                    {
                        node.RhoP_prime = node.RhoP;
                    }
                }

                for (int z = 0; z < numOfZNodes; z++)
                {
                    //get from neighbour lines
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (z == 0)
                            {
                                Rho[r, z].RhoW = 0;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                            else if (z == (numOfZNodes - 1))
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = 0;
                            }
                            else
                            {
                                Rho[r, z].RhoW = Rho[r, z - 1].RhoP;
                                Rho[r, z].RhoE = Rho[r, z + 1].RhoP;
                            }
                        }
                    }
                }

                //nonlinear terms
                calculateDiffusivity(eva);
                assignCoefficients2();
                updateCoefficients2(eva, massFlowRate);

                //set from north to south
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = (numOfRNodes - 1); r >= (eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall); r--)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            Rho[r, z].betaj = Rho[r, z].aN;
                            Rho[r, z].alphaj = Rho[r, z].aS;
                            Rho[r, z].Dj = Rho[r, z].aP;
                            Rho[r, z].Cj = Rho[r, z].aW * Rho[r, z].RhoW + Rho[r, z].aE * Rho[r, z].RhoE + Rho[r, z].aP0 * Rho[r, z].RhoP0 + Rho[r, z].Sc;

                            if (r == (numOfRNodes - 1))
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj);
                                Rho[r, z].Cj_prime = (Rho[r, z].Cj) / (Rho[r, z].Dj);
                            }

                            else
                            {
                                Rho[r, z].Aj = Rho[r, z].alphaj / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r + 1, z].Aj);
                                Rho[r, z].Cj_prime = (Rho[r, z].betaj * Rho[r + 1, z].Cj_prime + Rho[r, z].Cj) / (Rho[r, z].Dj - Rho[r, z].betaj * Rho[r + 1, z].Aj);
                            }
                        }
                    }
                }

                //solve from south to north
                for (int z = 0; z < numOfZNodes; z++)
                {
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (Rho[r, z].materialType == MaterialType.Adsorbent || Rho[r, z].materialType == MaterialType.Adsorbate)
                        {
                            if (r == (eva.numOfRNodes_fluid + eva.numOfRNodes_tubeWall))
                            {
                                Rho[r, z].RhoP = Rho[r, z].Cj_prime;
                            }
                            else
                            {
                                Rho[r, z].RhoP = Rho[r, z].Aj * Rho[r - 1, z].RhoP + Rho[r, z].Cj_prime;
                            }

                            Rho[r, z].RhoP = (1 - Parameters.relaxationFactor_density) * Rho[r, z].RhoP_prime + Parameters.relaxationFactor_density * Rho[r, z].RhoP;
                        }
                    }
                }

                foreach (RhoNodes node in Rho)
                {
                    if (node.RhoP < 0)
                    {
                        node.RhoP = 0;
                    }
                }

                endOfIteration = true;
                foreach (RhoNodes node in Rho)
                {
                    node.error = Helper.calculate_error(node.RhoP, node.RhoP_prime);
                    if (Math.Abs(node.error) < Parameters.tolerance_density)
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
                calculatePressure(eva);

            } while (endOfIteration == false);
        }
    }
}
