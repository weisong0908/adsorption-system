using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    class BaseNodes
    {
        //boundary
        public Boundary boundary;

        //radius
        public double rp, rs, rn;

        //properties
        public MaterialType materialType;
        public double volume;

        //for discretisation
        public double betaj, alphaj, Dj, Cj;
        public double aP, aW, aE, aS, aN, Sc, Sp, aP0;

        //for TDMA
        public double Aj, Cj_prime;
        public double error;
        public bool isConverged = false;
    }
    enum Boundary
    {
        NA, Center, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight
    };

    //Temperature nodes
    class TNodes : BaseNodes
    {
        //temperature
        public double density, heatCapacity, thermalConductivity, porosity;

        //for fluid
        public double dynamicViscosity, velocity;

        //condensation on tube surfaces
        public double rateOfCondensation;

        //for hybrid differencing scheme
        public double Fe, Fw, Fs, Fn, De, Dw, Ds, Dn, deltaF;

        //for TDMA
        public double TW, TE, TS, TN, TP, TP0, TP_prime = 0;//no need TS and TN because sweep from left to right
    }

    ////Pressure nodes
    //class PNodes : BaseNodes
    //{
    //    //pressure
    //    public double diffusivity, sigma, omega;

    //    //for adsorbent
    //    public double q, q_eq, dqdt, porosity, molarMass, molarGasConstant;

    //    //for TDMA
    //    public double PW, PE, PP, PP0, PP_prime;
    //}

    //Density nodes
    class RhoNodes : BaseNodes
    {
        //density
        public double diffusivity, sigma, omega;

        //for adsorbent
        public double q, q_eq, dqdt, porosity, molarMass, molarGasConstant, specificGasConstant;

        //for adsorbate
        public double pressure;

        //for TDMA
        public double RhoW, RhoE, RhoS, RhoN, RhoP, RhoP0, RhoP_prime;
    }
}
