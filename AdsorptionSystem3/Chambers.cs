using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    class Chamber
    {
        public ChamberType chamberType;

        //Phase
        public Phase phase = Phase.NA;

        public double deltaR, deltaZ;
        public int numOfRNodes, numOfZNodes;

        public int numOfRNodes_fluid;
        public int numOfRNodes_tubeWall;
        public int numOfRNodes_void;

        public double averagePressure, averageMass;
        public double voidVolume;
        public double inComingMassFlowRate, outGoingMassFlowRate;
        public double fluidVelocity;
        public double tubeOuterSurfaceArea;
        public double voidOuterSurfaceArea;

        //materials
        public Fluid fluid;
        public Tube tube;
        public Adsorbate adsorbate;

        public TNodes[,] T;
        public RhoNodes[,] Rho;

        public void calculateAveragePressure()
        {
            double sum = 0;
            int count = 0;

            foreach (RhoNodes node in this.Rho)
            {
                if (node.materialType == MaterialType.Adsorbate)// || node.materialType == MaterialType.Adsorbent)
                {
                    sum += node.pressure;
                    count++;
                }
            }

            averagePressure = sum / count;
        }
        public void calculateAverageMass()
        {
            double sum = 0;
            int count = 0;

            foreach (RhoNodes node in this.Rho)
            {
                if (node.materialType == MaterialType.Adsorbate || node.materialType == MaterialType.Adsorbent)
                {
                    sum += node.RhoP;
                    count++;
                }
            }

            averageMass = sum / count;
        }

        public void calculateVoidVolume()
        {
            double sum = 0;
            foreach (TNodes node in this.T)
            {
                if (node.materialType == MaterialType.Adsorbate)
                {
                    sum += node.volume;
                }
            }
            voidVolume = sum;
        }
        public void calculateRadius()
        {
            for (int r = 0; r < numOfRNodes; r++)
            {
                //calculate the rs, rn, and rp for 1st column
                T[r, 0].rs = deltaR * r;
                T[r, 0].rn = deltaR * (r + 1);
                T[r, 0].rp = Helper.average(new double[] { T[r, 0].rs, T[r, 0].rn });

                for (int z = 1; z < numOfZNodes; z++)
                {
                    //copy the rs, rn, and rp for 1st column to the rest of the columns
                    T[r, z].rs = T[r, 0].rs;
                    T[r, z].rn = T[r, 0].rn;
                    T[r, z].rp = T[r, 0].rp;
                }
            }

            //copy the radius from TNodes to RhoNodes
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    Rho[r, z].rs = T[r, z].rs;
                    Rho[r, z].rn = T[r, z].rn;
                    Rho[r, z].rp = T[r, z].rp;
                }
            }
        }
        public void calculateVolume()
        {
            foreach (TNodes node in T)
            {
                node.volume = Math.PI * (Math.Pow(node.rn, 2) - Math.Pow(node.rs, 2)) * deltaZ;
            }

            //copy the volume from TNodes to RhoNodes
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    Rho[r, z].volume = T[r, z].volume;
                }
            }
        }
        public void calculateTubeOuterSurfaceArea()
        {
            int r = numOfRNodes_fluid + numOfRNodes_tubeWall;
            tubeOuterSurfaceArea = 2 * Math.PI * r * deltaR * numOfZNodes * deltaZ;
        }
        public void calculateVoidOuterSurfaceArea()
        {
            int r = numOfRNodes;
            voidOuterSurfaceArea = 2 * Math.PI * r * deltaR * numOfZNodes * deltaZ;
        }
        public void assignMaterials()
        {
            //for TNodes
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    if (r < numOfRNodes_fluid)
                    {
                        T[r, z].materialType = MaterialType.HeatExchangeFluid;
                        T[r, z].density = fluid.density;
                        T[r, z].heatCapacity = fluid.heatCapacity;
                        T[r, z].thermalConductivity = fluid.thermalConductivity;
                        T[r, z].dynamicViscosity = fluid.dynamicViscosity;
                        T[r, z].velocity = fluidVelocity;
                    }
                    else if (r < (numOfRNodes_fluid + numOfRNodes_tubeWall))
                    {
                        T[r, z].materialType = MaterialType.Tube;
                        T[r, z].density = tube.density;
                        T[r, z].heatCapacity = tube.heatCapacity;
                        T[r, z].thermalConductivity = tube.thermalConductivity;
                        T[r, z].velocity = 0;
                    }
                    else
                    {
                        T[r, z].materialType = MaterialType.Adsorbate;
                        T[r, z].density = adsorbate.density;
                        T[r, z].heatCapacity = adsorbate.heatCapacity;
                        T[r, z].thermalConductivity = adsorbate.thermalConductivity;
                        T[r, z].velocity = 0;
                    }
                }
            }

            //for RhoNodes
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    Rho[r, z].materialType = T[r, z].materialType;
                    if (Rho[r, z].materialType == MaterialType.Adsorbate)
                    {
                        Rho[r, z].porosity = adsorbate.porosity;
                        Rho[r, z].molarMass = adsorbate.molarMass;
                        Rho[r, z].molarGasConstant = adsorbate.molarGasConstant;
                        Rho[r, z].sigma = adsorbate.sigma;
                        Rho[r, z].omega = adsorbate.omega;
                        Rho[r, z].specificGasConstant = adsorbate.specificGasConstant;
                    }
                }
            }
        }
    }
    enum ChamberType { NA, Bed, Condenser, Evaporator };
    enum Phase { NA, Preheating, Precooling, Adsorption, Desorption };

    class Bed : Chamber
    {
        //Dimension
        public int numOfRNodes_adsorbent;
        public int numOfZNodes_excessTube;
        public int numOfZNodes_fin;
        public int numOfZNodes_adsorbent;
        public int numOfTubeSection;

        public double excessTubeOuterSurfaceArea;

        //Adsorbent, adsorbate, working pair
        public Adsorbent adsorbent;
        public WorkingPair workingPair;
        public double adsorbentMass;

        //PDE models
        public BedTemperaturePDE bedTemperaturePDE;
        public BedDensityPDE bedDensityPDE;

        public double rateOfCondensation1, rateOfCondensation2;
        public double rateOfEvaporation1, rateOfEvaporation2;

        public Bed()
        {
            chamberType = ChamberType.Bed;

            deltaR = Parameters.deltaR_bed;
            deltaZ = Parameters.deltaZ_bed;

            numOfRNodes_fluid = Parameters.numOfRNodes_bedFluid;
            numOfRNodes_tubeWall = Parameters.numOfRNodes_bedTubeWall;
            numOfRNodes_adsorbent = Parameters.numOfRNodes_adsorbent;
            numOfRNodes_void = Parameters.numOfRNodes_bedVoid;
            numOfZNodes_excessTube = Parameters.numOfZNodes_bedExcessTube;
            numOfZNodes_fin = Parameters.numOfZNodes_fin;
            numOfZNodes_adsorbent = Parameters.numOfZNodes_adsorbent;
            numOfTubeSection = Parameters.numOfTubeSection;

            numOfRNodes = numOfRNodes_fluid + numOfRNodes_tubeWall + numOfRNodes_adsorbent + numOfRNodes_void;
            numOfZNodes = (numOfZNodes_fin + numOfZNodes_adsorbent) * numOfTubeSection + numOfZNodes_fin + 2 * numOfZNodes_excessTube;

            fluid = Parameters.bedFluid;
            tube = Parameters.bedTube;
            adsorbent = Parameters.adsorbent;
            adsorbate = Parameters.adsorbate;
            workingPair = Parameters.workingPair;

            T = new TNodes[numOfRNodes, numOfZNodes];
            Rho = new RhoNodes[numOfRNodes, numOfZNodes];
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    T[r, z] = new TNodes();
                    Rho[r, z] = new RhoNodes();
                }
            }

            averagePressure = Parameters.P0_bed;
            fluidVelocity = Parameters.fluidVelocity_bed;
            assignMaterials(this);
            calculateRadius();
            calculateVolume();
            calculateVoidVolume();
            calculateExcessTubeOuterSurfaceArea(this);
            calculateVoidOuterSurfaceArea();
            calculateAdsorbentMass();

            bedTemperaturePDE = new BedTemperaturePDE(this);
            bedDensityPDE = new BedDensityPDE(this);
        }

        //bed specific methods
        public void calculateAdsorbentMass()
        {
            double volume, mass;
            double sumOfMass = 0;
            int count = 0;
            foreach (TNodes node in T)
            {
                if (node.materialType == MaterialType.Adsorbent)
                {
                    volume = Math.PI * (Math.Pow(node.rn, 2) - Math.Pow(node.rs, 2)) * Parameters.deltaZ_bed;
                    mass = Parameters.adsorbent.density * volume;
                    sumOfMass += mass;
                    count++;
                }
            }

            adsorbentMass = sumOfMass * Parameters.numOfParallelTube_bed;
        }
        public void assignMaterials(Bed bed)
        {
            //assign materials for TNodes
            //for first excess tube
            for (int z = 0; z < bed.numOfZNodes_excessTube; z++)
            {
                for (int r = 0; r < numOfRNodes; r++)
                {
                    if (r < bed.numOfRNodes_fluid)
                    {
                        T[r, z].materialType = MaterialType.HeatExchangeFluid;
                        T[r, z].density = bed.fluid.density;
                        T[r, z].heatCapacity = bed.fluid.heatCapacity;
                        T[r, z].thermalConductivity = bed.fluid.thermalConductivity;
                        T[r, z].dynamicViscosity = bed.fluid.dynamicViscosity;
                        T[r, z].velocity = Parameters.fluidVelocity_bed;
                    }
                    else if (r < (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall))
                    {
                        T[r, z].materialType = MaterialType.Tube;
                        T[r, z].density = bed.tube.density;
                        T[r, z].heatCapacity = bed.tube.heatCapacity;
                        T[r, z].thermalConductivity = bed.tube.thermalConductivity;
                        T[r, z].velocity = 0;
                    }
                    else
                    {
                        T[r, z].materialType = MaterialType.Adsorbate;
                        T[r, z].density = bed.adsorbate.density;
                        T[r, z].heatCapacity = bed.adsorbate.heatCapacity;
                        T[r, z].thermalConductivity = (1 - bed.adsorbent.porosity) * bed.adsorbate.thermalConductivity;
                        T[r, z].velocity = 0;
                    }
                }
            }
            //for each tube section (fin + adsorbent z nodes)
            for (int n = 0; n < bed.numOfTubeSection; n++)
            {
                for (int z = ((bed.numOfZNodes_fin + bed.numOfZNodes_adsorbent) * n + bed.numOfZNodes_excessTube); z < ((bed.numOfZNodes_fin + bed.numOfZNodes_adsorbent) * (n + 1) + bed.numOfZNodes_excessTube); z++)
                {
                    for (int r = 0; r < numOfRNodes; r++)
                    {
                        if (z >= ((bed.numOfZNodes_fin + bed.numOfZNodes_adsorbent) * n + bed.numOfZNodes_excessTube) && z < ((bed.numOfZNodes_fin + bed.numOfZNodes_adsorbent) * n + bed.numOfZNodes_fin + bed.numOfZNodes_excessTube))
                        {
                            if (r < bed.numOfRNodes_fluid)
                            {
                                T[r, z].materialType = MaterialType.HeatExchangeFluid;
                                T[r, z].density = bed.fluid.density;
                                T[r, z].heatCapacity = bed.fluid.heatCapacity;
                                T[r, z].thermalConductivity = bed.fluid.thermalConductivity;
                                T[r, z].dynamicViscosity = bed.fluid.dynamicViscosity;
                                T[r, z].velocity = Parameters.fluidVelocity_bed;
                            }
                            else if (r < (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall + bed.numOfRNodes_adsorbent))
                            {
                                T[r, z].materialType = MaterialType.Tube;
                                T[r, z].density = bed.tube.density;
                                T[r, z].heatCapacity = bed.tube.heatCapacity;
                                T[r, z].thermalConductivity = bed.tube.thermalConductivity;
                                T[r, z].velocity = 0;
                            }
                            else
                            {
                                T[r, z].materialType = MaterialType.Adsorbate;
                                T[r, z].density = bed.adsorbate.density;
                                T[r, z].heatCapacity = bed.adsorbate.heatCapacity;
                                T[r, z].thermalConductivity = bed.adsorbate.thermalConductivity;
                                T[r, z].velocity = 0;
                            }
                        }
                        else
                        {
                            if (r < bed.numOfRNodes_fluid)
                            {
                                T[r, z].materialType = MaterialType.HeatExchangeFluid;
                                T[r, z].density = bed.fluid.density;
                                T[r, z].heatCapacity = bed.fluid.heatCapacity;
                                T[r, z].thermalConductivity = bed.fluid.thermalConductivity;
                                T[r, z].dynamicViscosity = bed.fluid.dynamicViscosity;
                                T[r, z].velocity = Parameters.fluidVelocity_bed;
                            }
                            else if (r < (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall))
                            {
                                T[r, z].materialType = MaterialType.Tube;
                                T[r, z].density = bed.tube.density;
                                T[r, z].heatCapacity = bed.tube.heatCapacity;
                                T[r, z].thermalConductivity = bed.tube.thermalConductivity;
                                T[r, z].velocity = 0;
                            }
                            else if (r < (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall + bed.numOfRNodes_adsorbent))
                            {
                                T[r, z].materialType = MaterialType.Adsorbent;
                                T[r, z].density = bed.adsorbent.density;
                                T[r, z].heatCapacity = bed.adsorbent.heatCapacity;
                                T[r, z].thermalConductivity = bed.adsorbent.thermalConductivity;
                                T[r, z].velocity = 0;
                                T[r, z].porosity = bed.adsorbent.porosity;
                            }
                            else
                            {
                                T[r, z].materialType = MaterialType.Adsorbate;
                                T[r, z].density = bed.adsorbate.density;
                                T[r, z].heatCapacity = bed.adsorbate.heatCapacity;
                                T[r, z].thermalConductivity = bed.adsorbate.thermalConductivity;
                                T[r, z].velocity = 0;
                            }
                        }
                    }
                }
            }

            //for last fin
            for (int z = ((bed.numOfZNodes_fin + bed.numOfZNodes_adsorbent) * bed.numOfTubeSection + bed.numOfZNodes_excessTube); z < ((bed.numOfZNodes_fin + bed.numOfZNodes_adsorbent) * bed.numOfTubeSection + bed.numOfZNodes_excessTube + bed.numOfZNodes_fin); z++)
            {
                for (int r = 0; r < numOfRNodes; r++)
                {
                    if (r < bed.numOfRNodes_fluid)
                    {
                        T[r, z].materialType = MaterialType.HeatExchangeFluid;
                        T[r, z].density = bed.fluid.density;
                        T[r, z].heatCapacity = bed.fluid.heatCapacity;
                        T[r, z].thermalConductivity = bed.fluid.thermalConductivity;
                        T[r, z].dynamicViscosity = bed.fluid.dynamicViscosity;
                        T[r, z].velocity = Parameters.fluidVelocity_bed;
                    }
                    else if (r < (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall + bed.numOfRNodes_adsorbent))
                    {
                        T[r, z].materialType = MaterialType.Tube;
                        T[r, z].density = bed.tube.density;
                        T[r, z].heatCapacity = bed.tube.heatCapacity;
                        T[r, z].thermalConductivity = bed.tube.thermalConductivity;
                        T[r, z].velocity = 0;
                    }
                    else
                    {
                        T[r, z].materialType = MaterialType.Adsorbate;
                        T[r, z].density = bed.adsorbate.density;
                        T[r, z].heatCapacity = bed.adsorbate.heatCapacity;
                        T[r, z].thermalConductivity = bed.adsorbate.thermalConductivity;
                        T[r, z].velocity = 0;
                    }
                }
            }

            //for last excess tube
            for (int z = ((bed.numOfZNodes_fin + bed.numOfZNodes_adsorbent) * bed.numOfTubeSection + bed.numOfZNodes_excessTube + bed.numOfZNodes_fin); z < numOfZNodes; z++)
            {
                for (int r = 0; r < numOfRNodes; r++)
                {
                    if (r < bed.numOfRNodes_fluid)
                    {
                        T[r, z].materialType = MaterialType.HeatExchangeFluid;
                        T[r, z].density = bed.fluid.density;
                        T[r, z].heatCapacity = bed.fluid.heatCapacity;
                        T[r, z].thermalConductivity = bed.fluid.thermalConductivity;
                        T[r, z].dynamicViscosity = bed.fluid.dynamicViscosity;
                        T[r, z].velocity = Parameters.fluidVelocity_bed;
                    }
                    else if (r < (bed.numOfRNodes_fluid + bed.numOfRNodes_tubeWall))
                    {
                        T[r, z].materialType = MaterialType.Tube;
                        T[r, z].density = bed.tube.density;
                        T[r, z].heatCapacity = bed.tube.heatCapacity;
                        T[r, z].thermalConductivity = bed.tube.thermalConductivity;
                        T[r, z].velocity = 0;
                    }
                    else
                    {
                        T[r, z].materialType = MaterialType.Adsorbate;
                        T[r, z].density = bed.adsorbate.density;
                        T[r, z].heatCapacity = bed.adsorbate.heatCapacity;
                        T[r, z].thermalConductivity = bed.adsorbate.thermalConductivity;
                        T[r, z].velocity = 0;
                    }
                }
            }

            //assign materials for RhoNodes
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    Rho[r, z].materialType = bed.T[r, z].materialType;
                    if (Rho[r, z].materialType == MaterialType.Adsorbent)
                    {
                        Rho[r, z].porosity = bed.adsorbent.porosity;
                        Rho[r, z].molarMass = bed.adsorbate.molarMass;
                        Rho[r, z].molarGasConstant = bed.adsorbate.molarGasConstant;
                        Rho[r, z].sigma = bed.adsorbate.sigma;
                        Rho[r, z].omega = bed.adsorbate.omega;
                        Rho[r, z].specificGasConstant = bed.adsorbate.specificGasConstant;
                        Rho[r, z].volume = bed.T[r, z].volume;
                    }
                    else if (Rho[r, z].materialType == MaterialType.Adsorbate)
                    {
                        Rho[r, z].porosity = bed.adsorbate.porosity;
                        Rho[r, z].molarMass = bed.adsorbate.molarMass;
                        Rho[r, z].molarGasConstant = bed.adsorbate.molarGasConstant;
                        Rho[r, z].sigma = bed.adsorbate.sigma;
                        Rho[r, z].omega = bed.adsorbate.omega;
                        Rho[r, z].specificGasConstant = bed.adsorbate.specificGasConstant;
                        Rho[r, z].volume = bed.T[r, z].volume;
                    }
                    else
                    {
                        Rho[r, z].porosity = bed.adsorbate.porosity;
                    }
                }
            }
        }
        public void calculateExcessTubeOuterSurfaceArea(Bed bed)
        {
            int r = numOfRNodes_fluid + numOfRNodes_tubeWall;
            excessTubeOuterSurfaceArea = 2 * Math.PI * r * deltaR * bed.numOfZNodes_excessTube * deltaZ;
        }

        //bed phases
        public void preheating()
        {
            bedDensityPDE.calculateRateOfAdsorption(this);
            bedTemperaturePDE.updateCoefficients2(Parameters.hotWaterTemperature, this);
            bedTemperaturePDE.TDMA2();
            bedDensityPDE.calculateDiffusivity(this);
            bedDensityPDE.assignCoefficients2();
            bedDensityPDE.updateCoefficients2(this, 0);
            bedDensityPDE.TDMA4(this, 0);
        }

        public void desorption()
        {
            bedDensityPDE.calculateRateOfAdsorption(this);
            bedTemperaturePDE.updateCoefficients2(Parameters.hotWaterTemperature, this);
            bedTemperaturePDE.TDMA2();
            bedDensityPDE.calculateDiffusivity(this);
            bedDensityPDE.assignCoefficients2();
            bedDensityPDE.updateCoefficients2(this, outGoingMassFlowRate);
            bedDensityPDE.TDMA4(this, outGoingMassFlowRate);
        }

        public void precooling()
        {
            bedDensityPDE.calculateRateOfAdsorption(this);
            bedTemperaturePDE.updateCoefficients2(Parameters.coldWaterTemperature, this);
            bedTemperaturePDE.TDMA2();            
            bedDensityPDE.calculateDiffusivity(this);
            bedDensityPDE.assignCoefficients2();
            bedDensityPDE.updateCoefficients2(this, 0);
            bedDensityPDE.TDMA4(this, 0);
        }

        public void adsorption()
        {
            bedDensityPDE.calculateRateOfAdsorption(this);
            bedTemperaturePDE.updateCoefficients2(Parameters.coldWaterTemperature, this);
            bedTemperaturePDE.TDMA2();
            bedDensityPDE.calculateDiffusivity(this);
            bedDensityPDE.assignCoefficients2();
            bedDensityPDE.updateCoefficients2(this, inComingMassFlowRate);
            bedDensityPDE.TDMA4(this, inComingMassFlowRate);
        }
    }

    class Condenser : Chamber
    {
        //PDE models
        public CondTemperaturePDE condTemperaturePDE;
        public CondDensityPDE condDensityPDE;

        public double rateOfCondensation;

        public Condenser()
        {
            chamberType = ChamberType.Condenser;

            deltaR = Parameters.deltaR_cond;
            deltaZ = Parameters.deltaZ_cond;

            numOfRNodes_fluid = Parameters.numOfRNodes_condFluid;
            numOfRNodes_tubeWall = Parameters.numOfRNodes_condTubeWall;
            numOfRNodes_void = Parameters.numOfRNodes_condVoid;

            numOfRNodes = numOfRNodes_fluid + numOfRNodes_tubeWall + numOfRNodes_void;
            numOfZNodes = Parameters.numOfZNodes_cond;

            fluid = Parameters.condFluid;
            tube = Parameters.condTube;
            adsorbate = Parameters.adsorbate;

            T = new TNodes[numOfRNodes, numOfZNodes];
            Rho = new RhoNodes[numOfRNodes, numOfZNodes];
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    T[r, z] = new TNodes();
                    Rho[r, z] = new RhoNodes();
                }
            }

            averagePressure = Parameters.P0_cond;
            fluidVelocity = Parameters.fluidVelocity_cond;
            assignMaterials();
            calculateRadius();
            calculateVolume();
            calculateVoidVolume();
            calculateTubeOuterSurfaceArea();
            calculateVoidOuterSurfaceArea();

            condTemperaturePDE = new CondTemperaturePDE(this);
            condDensityPDE = new CondDensityPDE(this);
        }

        //condenser phases
        public void condensation()
        {
            condTemperaturePDE.updateCoefficients2(Parameters.condWaterTemperature, this);
            condTemperaturePDE.TDMA2_for_cond(this);
            condDensityPDE.calculateDiffusivity(this);
            condDensityPDE.assignCoefficients2();
            condDensityPDE.updateCoefficients2(this, inComingMassFlowRate);
            condDensityPDE.TDMA4(this, inComingMassFlowRate);//correct one is TDMA4
        }

        public void disconnected()
        {
            condTemperaturePDE.updateCoefficients2(Parameters.condWaterTemperature, this);
            condTemperaturePDE.TDMA2_for_cond(this);
            condDensityPDE.calculateDiffusivity(this);
            condDensityPDE.assignCoefficients2();
            condDensityPDE.updateCoefficients2(this, 0);
            condDensityPDE.TDMA4(this, 0);
        }
    }

    class Evaporator : Chamber
    {
        //PDE models
        public EvaTemperaturePDE evaTemperaturePDE;
        public EvaDensityPDE evaDensityPDE;

        public double rateOfEvaporation;

        public Evaporator()
        {
            chamberType = ChamberType.Evaporator;

            deltaR = Parameters.deltaR_eva;
            deltaZ = Parameters.deltaZ_eva;

            numOfRNodes_fluid = Parameters.numOfRNodes_evaFluid;
            numOfRNodes_tubeWall = Parameters.numOfRNodes_evaTubeWall;
            numOfRNodes_void = Parameters.numOfRNodes_evaVoid;

            numOfRNodes = numOfRNodes_fluid + numOfRNodes_tubeWall + numOfRNodes_void;
            numOfZNodes = Parameters.numOfZNodes_eva;

            fluid = Parameters.evaFluid;
            tube = Parameters.evaTube;
            adsorbate = Parameters.adsorbate;

            T = new TNodes[numOfRNodes, numOfZNodes];
            Rho = new RhoNodes[numOfRNodes, numOfZNodes];
            for (int r = 0; r < numOfRNodes; r++)
            {
                for (int z = 0; z < numOfZNodes; z++)
                {
                    T[r, z] = new TNodes();
                    Rho[r, z] = new RhoNodes();
                }
            }

            averagePressure = Parameters.P0_eva;
            fluidVelocity = Parameters.fluidVelocity_eva;
            assignMaterials();
            calculateRadius();
            calculateVolume();
            calculateVoidVolume();
            calculateTubeOuterSurfaceArea();
            calculateVoidOuterSurfaceArea();

            evaTemperaturePDE = new EvaTemperaturePDE(this);
            evaDensityPDE = new EvaDensityPDE(this);
        }
        public void evaporation()
        {
            evaTemperaturePDE.updateCoefficients2(Parameters.chilledWaterTemperature, this);
            evaTemperaturePDE.TDMA2_for_eva(this);
            evaDensityPDE.calculateDiffusivity(this);
            evaDensityPDE.assignCoefficients2();
            evaDensityPDE.updateCoefficients2(this, outGoingMassFlowRate);
            evaDensityPDE.TDMA4(this, outGoingMassFlowRate);
        }

        public void disconnected()
        {
            evaTemperaturePDE.updateCoefficients2(Parameters.chilledWaterTemperature, this);
            evaTemperaturePDE.TDMA2_for_eva(this);
            evaDensityPDE.calculateDiffusivity(this);
            evaDensityPDE.assignCoefficients2();
            evaDensityPDE.updateCoefficients2(this, 0);
            evaDensityPDE.TDMA4(this, 0);
        }
    }
}