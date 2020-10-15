using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPR2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter file path: ");
            FileReader fr = new FileReader(Console.ReadLine());
            Equation e = fr.GetEquation();
            //e.SetRow(0, new double[] {1, -3 }, true, 1);
            //e.SetRow(1, new double[] {1, 1 }, true,5);
            //e.SetRow(2, new double[] {0, 1 }, false,10);
            //e.SetRow(3, new double[] {2, 1 }, false,15);
            //e.SetFunc(new double[] { 4, 1 }, true);
            e.Print();
            e.PrintRevers();
            e.Canonize();
            Console.WriteLine("Canonize:");
            e.Print();
            e.FindDbr();
            e.PrintDBR();
            e.Simplex();
            e.PrintAnswer();
        }
    }
}
