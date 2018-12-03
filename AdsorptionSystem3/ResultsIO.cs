using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AdsorptionSystem3
{
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }

    //Export data into CSV file
    public class CsvWriter : StreamWriter
    {
        public CsvWriter(string filename) : base(filename) { }

        //Writes a single row to CSV file
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;

            foreach (string value in row)
            {
                if (!firstColumn)
                {
                    builder.Append(',');
                }

                //Special handling for values that contain comma or quote
                if (value.IndexOfAny(new char[] { '"', ',' }) != 1)
                {
                    //enclose value in quotes and double up any double quotes
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                }
                else
                {
                    builder.Append(value);
                }

                //set values after this as "not first column"
                firstColumn = false;
            }

            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }
}
