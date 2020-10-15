using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPR2
{
    class Equation
    {
        List<MNum> s;
        TableDrawer td;
        public struct MNum
        {
            double num, numM;

            public MNum(double num, double numM)
            {
                this.num = num;
                this.numM = numM;
                if (Math.Abs(num) == double.MaxValue)
                {
                    this.numM = num / double.MaxValue;
                    this.num = 0;
                }
            }

            public override string ToString()
            {
                string txt = "";
                if (Math.Abs(numM) == 1)
                    txt += numM < 0 ? "-M" : "M";
                else if (numM != 0)
                    txt += numM.ToString() + "M";
                if (num != 0 && numM != 0)
                {
                    txt += num < 0 ? "" : "+";
                    txt += num.ToString();
                }
                else if (num != 0)
                {
                    txt += num.ToString();
                }
                if (numM == 0 && num == 0)
                    txt += "0";
                return txt;
            }
            public static bool operator >(MNum n1, MNum n2)
            {
                if (n1.numM == n2.numM)
                {
                    return n1.num > n2.num;
                }
                else
                    return n1.numM > n2.numM;
            }

            public static MNum operator +(MNum n1, MNum n2)
            {
                return new MNum(n1.num + n2.num, n1.numM + n2.numM);
            }
            public static MNum operator *(double n1, MNum n2)
            {
                return new MNum(n1 * n2.num, n1 * n2.numM);
            }

            public static bool operator <(MNum n1, MNum n2)
            {
                return n2 > n1;
            }
        }

        public struct Func
        {
            public bool max;
            public List<double> eq;

        }
        public struct Right { public bool Sign; public double value;

            public Right(bool sign, double value)
            {
                Sign = sign;
                this.value = value;
            }
        }
        public Func f = new Func();
        bool isCanonized = false;
        public int rows;
        public List<double>[] eq;
        public Right[] eqSign;
        Dictionary<int, double> dbrBase, dbrFree;
        Dictionary<int, MNum> dbrB;
        public Equation(int rows)
        {
            dbrBase = new Dictionary<int, double>();
            dbrFree = new Dictionary<int, double>();
            this.rows = rows;
            eq = new List<double>[rows];
            eqSign = new Right[rows];
            for (int i = 0; i < rows; i++)
                eq[i] = new List<double>();

        }
        public void FindDbr()
        {
            dbrFree.Add(1, 0);
            dbrFree.Add(2, 0);
            for (int i = 0; i < rows; i++)
                for (int j = 2; j < eq[i].Count; j++)
                    if (eq[i][j] < 0)
                        dbrFree.Add(j + 1, 0);
                    else if (eq[i][j] > 0)
                        dbrBase.Add(j + 1, eqSign[i].value);
        }

        public void PrintDBR()
        {
            Console.WriteLine("___DBR___");
            Console.WriteLine("Basis :");
            foreach (var c in dbrBase)
                Console.Write(string.Format("x{0} = {1}; ", c.Key, c.Value));
            Console.WriteLine();
            Console.WriteLine("Free :");
            foreach (var c in dbrFree)
                Console.Write(string.Format("x{0} = {1}; ", c.Key, c.Value));
            Console.WriteLine();

        }

        public void SetRow(int i, IEnumerable<double> data, bool gt, double rightSide)
        {
            eq[i] = new List<double>(data);
            eqSign[i] = new Right(gt, rightSide);
        }
        public void SetFunc(IEnumerable<double> data, bool max)
        {
            f.eq = data.ToList();
            f.max = max;
        }
        public void Print()
        {

            for (int i = 0; i < rows; i++)
            {
                Console.Write(String.Format("{0}[x1] {1}[x2]", eq[i][0], eq[i][1] > 0 ? " + " + eq[i][1].ToString() : " - " + Math.Abs(eq[i][1]).ToString()));
                for (int j = 2; j < eq[i].Count; j++)
                    Console.Write(String.Format("{0}[x{1}]", eq[i][j] >= 0 ? " + " + eq[i][j].ToString() : " - " + Math.Abs(eq[i][j]).ToString(), j + 1));
                if (!isCanonized)
                    Console.WriteLine((eqSign[i].Sign ? " >= " : " <= ") + eqSign[i].value.ToString());
                else
                    Console.WriteLine(" = " + eqSign[i].value.ToString());
            }
            Console.WriteLine("Function:");
            int it = 1;
            foreach (var d in f.eq)
                if (it != 1)
                {
                    var n = Math.Abs(d) == double.MaxValue ? "M" : Math.Abs(d).ToString();
                    Console.Write(String.Format("{0}[x{1}]", (d >= 0 ? " + " : " - ") + n, it++));
                }
                else
                    Console.Write(String.Format("F = {0}[x{1}]", d >= 0 ? "" + d.ToString() : " - " + Math.Abs(d).ToString(), it++));
            Console.WriteLine(" -> " + (f.max ? "max" : "min"));
        }


        public void Canonize()
        {
            isCanonized = true;

            for (int j = 0; j < rows; j++)
                f.eq.Add(0);
            var s = eq[0].Count;
            for (int i = 0; i < rows; i++)
            {
                if (eqSign[i].value < 0)
                {
                    eqSign[i].value *= -1;
                    for (int j = 0; j < eq[i].Count; j++)
                        eq[i][j] *= -1;
                }

                for (int j = 0; j < rows; j++)
                    eq[i].Add(0);
                eq[i][s + i] = eqSign[i].Sign ? -1 : 1;
                if (eqSign[i].Sign)
                {
                    for (int j = 0; j < rows; j++)
                        eq[j].Add(0);
                    eq[i][eq[i].Count - 1] = 1;
                    f.eq.Add(f.max ? double.MinValue : double.MaxValue);
                }

            }


        }
        Dictionary<int, MNum> bi_a;
        public void Simplex()
        {

            int row, col;
            td = new TableDrawer(f.eq);
            List<double> bi = new List<double>();
            foreach (var e in eqSign)
                bi.Add(e.value);
            dbrB = new Dictionary<int, MNum>();
            for (int i = 0; i < dbrBase.Count; i++)
                dbrB.Add(dbrBase.Keys.ToArray()[i], new MNum(f.eq[dbrBase.Keys.ToArray()[i] - 1], 0));
            do
            {
                s = Sums(dbrB, bi);
                var q = CalculateQ(s, bi);
                td.Draw(dbrB, eq, bi, q, s, f.max);
                row = findRow(q);
                col = FindCol(s, bi);
                Console.WriteLine(string.Format("Row №{0}, Column №{1}", row + 1, col + 1));
                dbrB = NewDbr(dbrB, col, row);
                bi = CalculateNewMatrix(row, col, bi);
            }
            while ((f.max && s[col] < new MNum()) || (!f.max && s[col] > new MNum()));
            bi_a = dbrB;
            for (int i = 0; i < dbrB.Count; i++)
            {
                bi_a[bi_a.Keys.ToArray()[i]] = new MNum(bi[i], 0);
            }
        }
        public void PrintRevers()
        {
            Console.WriteLine("Reversed:");
            for (int j = 0; j < eq[j].Count; j++)
            {
                Console.Write(String.Format("{0}[x{1}]", eq[0][j] >= 0 ? eq[0][j].ToString() : " - " + Math.Abs(eq[0][j]).ToString(), 1));
                for (int i = 1; i < rows; i++)
                {
                    Console.Write(String.Format("{0}[x{1}]", eq[i][j] >= 0 ? " + " + eq[i][j].ToString() : " - " + Math.Abs(eq[i][j]).ToString(), i + 1));

                }
                Console.WriteLine(" <= " + f.eq[j].ToString());
            }

        }
        public void PrintAnswer()
        {
            
            Console.WriteLine("Function:");
            int it = 1;
            foreach (var d in f.eq)
                if (it != 1)
                {
                    var n = Math.Abs(d) == double.MaxValue ? "0" : Math.Abs(d).ToString();                 
                    Console.Write(String.Format("{0}[x{1}]", (d >= 0 ? " + " : " - ") + n, it++));
                }
                else
                    Console.Write(String.Format("F = {0}[x{1}]", d >= 0 ? "" + d.ToString() : " - " + Math.Abs(d).ToString(), it++));
            Console.WriteLine(" -> " + (f.max ? "max" : "min"));
            string st = "";
            st += bi_a.ContainsKey(1) ? bi_a[1].ToString() + ", " : "0, ";
            st += bi_a.ContainsKey(2) ? bi_a[2].ToString() : "0";

            Console.WriteLine(String.Format("F{0} = F({1}) = {2}",(" -> " + (f.max ? "max" : "min")), st, s[eq[0].Count]));

        }
        List<double> CalculateNewMatrix(int row, int col, List<double> bi)
        {
            double k = 1 / eq[row][col];
            for (int i = 0; i < eq[0].Count; i++)
                eq[row][i] *= k;
            bi[row] *= k;
            for (int i = 0; i < rows; i++)
            {
                k = 1 / eq[i][col];
                if (i != row)
                {
                    for (int j = 0; j < eq[0].Count; j++)
                    {
                        eq[i][j] -= eq[row][j] / k;
                    }
                    bi[i] -= bi[row] / k;
                }
            }

            return bi;
        }
        Dictionary<int, MNum> NewDbr(Dictionary<int, MNum> dbrB, int col, int row)
        {
            Dictionary<int, MNum> ret = new Dictionary<int, MNum>();
            dbrB[dbrB.Keys.ToArray()[row]] = new MNum(f.eq[col], 0);           
            dbrB.Values.ToArray()[row] = new MNum(f.eq[col], 0);
            for (int i = 0; i < rows; i++)
            {
                if (i == row)
                    ret.Add(col+1, new MNum(f.eq[col], 0));
                else
                    ret.Add(dbrB.Keys.ToArray()[i], dbrB.Values.ToArray()[i]);
            }
            return ret;
        }
        List<MNum> Sums(Dictionary<int, MNum> dbrB, List<double> bi)
        {
            var ret = new List<MNum>();
            for (int i = 0; i < eq[0].Count; i++)
            {
                MNum s = new MNum();
                for (int j = 0; j < rows; j++)
                    s += eq[j][i] * dbrB.Values.ToArray()[j];
                s += new MNum(-f.eq[i], 0);
                ret.Add(s);
            }
            MNum bis = new MNum();
            for (int j = 0; j < rows; j++)
                bis += bi[j] * dbrB.Values.ToArray()[j];
            ret.Add(bis);
            ret.Add(new MNum());
            return ret;
        }

        int findRow(List<double> q)
        {
            for (int i = 0; i < rows; i++)
                q[i] = q[i] < 0 ? double.MaxValue : q[i];
            int indx = 0;
            for(int i = 0; i<rows;i++)
                if (q[indx] > q[i])
                    indx = i;
            return indx;
        }
        int FindCol(List<MNum> s, List<double> bi)
        {           
            int smallI = 0;
            int bigI = 0;
            for (int i = 0; i < eq[0].Count; i++)
            {
                if (s[smallI] < s[i])
                    smallI = i;
                if (s[bigI] > s[i])
                    bigI = i;
            }
            return !f.max ? smallI : bigI; 
        }
        List<double> CalculateQ(List<MNum> s, List<double> bi)
        {
            var ret = new List<double>();

            int index = FindCol(s, bi);
            for (int i = 0; i < rows; i++)
            {
                if (eq[i][index] == 0)
                    ret.Add(double.MaxValue);
                else
                    ret.Add(bi[i] / eq[i][index]);
            }
            return ret;
        }
    }
}
