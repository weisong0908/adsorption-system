using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    class ConnectingPipes
    {
        public Chamber origin, destination;
        public double innerRadius, pipeLength;
        public double finalPressure, inletPressure, outletPressure;
        public double massFlowRate;

        public ConnectingPipes(Chamber from, Chamber to)
        {
            origin = from;
            destination = to;

            innerRadius = Parameters.pipeID_connectingPipe / 2;
            pipeLength = Parameters.pipelength_connectingPipe;
        }

        public void calculateMassFlowRate(Phase phase)
        {
            calculateFinalPressure(phase);
            calculatePipeFlowPressure();
            massFlowRate = -0.8*(origin.adsorbate.density * Math.PI * Math.Pow(innerRadius, 4) * (destination.averagePressure - origin.averagePressure)) / (8 * origin.adsorbate.dynamicViscosity * pipeLength);
            //massFlowRate = (origin.adsorbate.density * Math.PI * Math.Pow(innerRadius, 4) * (outletPressure - inletPressure)) / (8 * origin.adsorbate.dynamicViscosity * pipeLength);
            //origin.outGoingMassFlowRate = (origin.adsorbate.density * Math.PI * Math.Pow(innerRadius, 4) * (finalPressure - origin.averagePressure)) / (8 * origin.adsorbate.dynamicViscosity * pipeLength);
            //destination.inComingMassFlowRate = (origin.adsorbate.density * Math.PI * Math.Pow(innerRadius, 4) * (finalPressure - destination.averagePressure)) / (8 * origin.adsorbate.dynamicViscosity * pipeLength);
            //if(origin.averagePressure<finalPressure)
            //{
            //    origin.outGoingMassFlowRate = massFlowRate;
            //    destination.inComingMassFlowRate = -massFlowRate;
            //}
            //else
            //{
                origin.outGoingMassFlowRate = -massFlowRate;
                destination.inComingMassFlowRate = massFlowRate;
            //}
        }

        public void calculateFinalPressure(Phase phase)
        {
            double p1v1, p2v2, finalVoidVolume;

            if(phase == Phase.Adsorption)
            {
                p1v1 = origin.averagePressure * origin.voidVolume * Parameters.numOfParallelTube_eva;
                p2v2 = destination.averagePressure * destination.voidVolume * Parameters.numOfParallelTube_bed;
                finalVoidVolume = origin.voidVolume * Parameters.numOfParallelTube_eva + destination.voidVolume * Parameters.numOfParallelTube_bed;
            }
            else if (phase == Phase.Desorption)
            {
                p1v1 = origin.averagePressure * origin.voidVolume * Parameters.numOfParallelTube_bed;
                p2v2 = destination.averagePressure * destination.voidVolume * Parameters.numOfParallelTube_cond;
                finalVoidVolume = origin.voidVolume * Parameters.numOfParallelTube_bed + destination.voidVolume * Parameters.numOfParallelTube_cond;
            }
            else
            {
                p1v1 = 0;
                p2v2 = 0;
                finalVoidVolume = 1;
            }

            finalPressure = (p1v1 + p2v2) / finalVoidVolume;
        }

        public void calculatePipeFlowPressure()
        {
            inletPressure = origin.averagePressure - finalPressure;
            outletPressure = finalPressure - destination.averagePressure;
        }
    }
}
