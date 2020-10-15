using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPR2
{
    class FileReader
    {

        string path;
        public FileReader(string path)
        {
            this.path = path;
        }


        public Equation GetEquation()
        {
            var content = File.ReadAllLines(path, Encoding.UTF8);
            Equation e = new Equation(content.Count() -1 );
            var f = content[0].Split("->".ToCharArray());
            f[0] = f[0].Substring(f[0].IndexOf('=')+1);
            var f1 = f[0].Split(' ');
            List<double> fList = new List<double>();
            foreach (var s in f1)
            {
                double t = 0;
                if (double.TryParse(s, out t))
                    fList.Add(t);
            }

            e.SetFunc(fList, f[2].Trim() == "max");
            for (int i = 1; i < content.Count(); i++)
            {
                bool gt = false;
                double r = 0;
                var eq = content[i].Split(' ');
                fList.Clear();
                foreach (var s in eq)
                {
                    if (s == ">=")
                    {
                        gt = true;
                        r = double.MaxValue;
                    }
                    else if (s == "<=")
                    {
                        gt = false;
                        r = double.MaxValue;
                    }
                    else if (r == double.MaxValue)
                    {
                        double t = 0;
                        if (double.TryParse(s, out t))
                            r = t;
                    }
                    else
                    {
                        double t = 0;
                        if (double.TryParse(s, out t))
                            fList.Add(t);

                    }
                }
                e.SetRow(i - 1, fList, gt, r);
            }

            

            return e;
        }

    }
}
