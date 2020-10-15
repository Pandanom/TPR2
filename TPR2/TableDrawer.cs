using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPR2
{
    class TableDrawer
    {
        static int tableWidth = 110;
        int col;
        string[] firstRow, secondRow;
        public TableDrawer(List<double> values)
        {
            col = 5 + values.Count;
            firstRow = new string[col];
            secondRow = new string[col];
            for (int i = 0; i < 5 + values.Count; i++)
            {
                if (i < 3)
                    firstRow[i] = "";
                else if (i < 3 + values.Count)
                {
                    if (values[i - 3] == double.MaxValue)
                        firstRow[i] = "M";
                    else if(values[i - 3] == double.MinValue)
                        firstRow[i] = "-M";
                    else
                        firstRow[i] = values[i-3].ToString();
                }
                else
                    firstRow[i] = "";
            }

            secondRow[0] = "I";
            secondRow[1] = "Xb";
            secondRow[2] = "Cb";
            secondRow[4 + values.Count] = "Q";
            secondRow[3 + values.Count] = "Bi";
            for (int i = 3; i < 3 + values.Count; i++)
                secondRow[i] = "X" + (i - 2).ToString();

        }
        public void Draw(Dictionary<int, Equation.MNum> dbrBase, List<double>[] a, List<double> Bi, List<double>Q, List<Equation.MNum> last, bool max)
        {
            PrintLine();
            PrintRow(firstRow);
            PrintRow(secondRow);
            PrintLine();
            for (int i = 0; i < dbrBase.Count; i++)
            {
                var c = new string[col];
                c[0] = (i + 1).ToString();
                c[1] = "x" + dbrBase.ToList()[i].Key.ToString();
                c[2] = dbrBase.ToList()[i].Value.ToString();
                for (int j = 0; j < col - 5; j++)
                    c[j + 3] = a[i][j].ToString();
                c[col - 2] = Bi[i].ToString();
                c[col - 1] = Q[i].ToString();
                PrintRow(c);
            }
            PrintLine();

            string[] t = new string[col];
            t[0] = "";
            t[1] = max ? "max" : "min";
            t[2] = "";
            for (int i = 0; i < col - 3; i++)
                t[i + 3] = last[i].ToString();
            PrintRow(t);
            PrintLine();

        }


    

        void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

    }
}
