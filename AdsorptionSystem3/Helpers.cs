using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsorptionSystem3
{
    static class Helper
    {
        //average
        public static double average(double[] value)
        {
            int length = value.Length;
            double sum = 0;
            foreach (double v in value)
            {
                sum += v;
            }

            return sum / length;
        }

        //maximum of 3 numbers
        public static double max3(double a, double b, double c)
        {
            return Math.Max(a, Math.Max(b, c));
        }

        //calculate error
        public static double calculate_error(double newNumber, double oldNumber)
        {
            double numerator = newNumber - oldNumber;
            double denominator = oldNumber;

            if (denominator == 0)
            {
                return (newNumber - oldNumber);
            }
            else
            {
                //return (newNumber - oldNumber);
                return numerator / denominator;
            }
        }
    }
}
