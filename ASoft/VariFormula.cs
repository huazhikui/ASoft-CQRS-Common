using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASoft
{
    public class VariFormula
    {
        #region Variables
        private string vars = "", zc;
        private const string allowed = "abcdefgxyz";
        private double[] values = new double[10];
        private int pos, clip, level;
        private double res;
        private bool error = false;
        #endregion
       
        private bool TryVariable(string Op, out double worth)
        {
            int i = vars.IndexOf(Op);
            if (i != -1)
            {
                worth = values[i];
                return true;
            }
            else
            {
                worth = 0.0;
                return false;
            }
        }
        
        private bool varAllowed(string var)
        {
            return (allowed.IndexOf(var) != -1 && vars.IndexOf(var) == -1);
        } 

        private int GetClip(string Op, int start)
        {
            int res = start;
            for (int i = start; i < Op.Length; i++)
            {
                switch (Op.Substring(i, 1))
                {
                    case "(": clip++; break;
                    case ")": clip--; break;
                }
                if (clip == 0) { res = i; break; }
            }
            return res;
        }
        private double Faculty(double number)
        {
            if (double.IsInfinity(number) ||
                double.IsNaN(number) ||
                number < 0.0 ||
                number % 1.0 != 0) return double.NaN;
            double res = 1.0;
            for (int i = 0; i < number; i++)
                res *= (double)(i + 1);
            return res;
        }
       
        private double Allocation(string Op)
        {
            if (Op.Length == 0) goto fehl;
            if (Op.StartsWith("(") && GetClip(Op, 0) == Op.Length - 1) Op = Op.Substring(1, Op.Length - 2);
            if (double.TryParse(Op, System.Globalization.NumberStyles.Float,
                null, out res)) return res;
            if (Op.Length == 2 && Op == "PI") return Math.PI;
            if (Op.Length == 1 && TryVariable(Op, out res)) return res;
            if (Op.Length > 4 && Op.Substring(3, 1) == "(")
            {
                pos = GetClip(Op, 3);
                if (pos != Op.Length - 1) goto next;
                zc = Op.Substring(4, pos - 4);
                switch (Op.Substring(0, 3))
                {
                    case "sqr": return Math.Sqrt(Allocation(zc));
                    case "sin": return Math.Sin(Math.PI * Allocation(zc) / 180);
                    case "cos": return Math.Cos(Math.PI * Allocation(zc) / 180);
                    case "tan": return Math.Tan(Math.PI * Allocation(zc) / 180);
                    case "log": return Math.Log10(Allocation(zc));
                    case "abs": return Math.Abs(Allocation(zc));
                    case "fac": return Faculty(Allocation(zc));
                }
            }
        next:
            pos = 0; level = 6; clip = 0;
            for (int i = Op.Length - 1; i > -1; i--)
            {
                switch (Op.Substring(i, 1))
                {
                    case "(": clip++; break;
                    case ")": clip--; break;
                    case "+": if (clip == 0 && level > 0) { pos = i; level = 0; } break;
                    case "-": if (clip == 0 && level > 1) { pos = i; level = 1; } break;
                    case "*": if (clip == 0 && level > 2) { pos = i; level = 2; } break;
                    case "%": if (clip == 0 && level > 3) { pos = i; level = 3; } break;
                    case "/": if (clip == 0 && level > 4) { pos = i; level = 4; } break;
                    case "^": if (clip == 0 && level > 5) { pos = i; level = 5; } break;
                }
            }
            if (pos == 0 || pos == Op.Length - 1) goto fehl;
            zc = Op.Substring(pos, 1);
            string t1, t2;
            t1 = Op.Substring(0, pos);
            t2 = Op.Substring(pos + 1, Op.Length - (pos + 1));
            switch (zc)
            {
                case "+": return Allocation(t1) + Allocation(t2);
                case "-": return Allocation(t1) - Allocation(t2);
                case "*": return Allocation(t1) * Allocation(t2);
                case "/": return Allocation(t1) / Allocation(t2);
                case "%": return Math.IEEERemainder(Allocation(t1), Allocation(t2));
                case "^": return Math.Pow(Allocation(t1), Allocation(t2));
            }
        fehl:
            error = true;
            return 0.0;
        }
       
        public bool New(string formula, out double result)
        {
            if (formula == "") { result = 0.0; return false; }
            error = false;
            result = Allocation(formula);
            return !error;
        }

        public bool NewV(Variable var)
        {
            if (var.name == "" || var.name.Length != 1
                || vars.Length >= values.Length - 1 || !varAllowed(var.name)) return false;
            vars = string.Concat(vars, var.name);
            values[vars.Length - 1] = var.worth;
            return true;
        }
      
        public bool HoleV()
        {
            vars = "";
            return true;
        }
     
        public bool HoleV(Variable var)
        {
            if (var.name == "" || var.name.Length != 1) return false;
            int pos = vars.IndexOf(var.name);
            if (pos != -1)
            {
                vars = vars.Remove(pos, 1);
                for (int i = pos; i < values.Length - 1; i++)
                    values[i] = values[i + 1];
                return true;
            }
            else
                return false;
        }
      
        public void WorthCountriesUS(double worth)
        {
            values[0] = worth;
        }
       
        public bool WorthCountries(Variable var)
        {
            if (var.name == "" || var.name.Length != 1) return false;
            int pos = vars.IndexOf(var.name);
            if (pos != -1)
            {
                values[pos] = var.worth;
                return true;
            }
            else
                return false;
        }
      
        public Variable[] Variables
        {
            get
            {
                Variable[] va = new Variable[vars.Length];
                for (int i = 0; i < vars.Length; i++)
                    va[i] = new Variable(vars.Substring(i, 1), values[i]);
                return va;
              
            }
        }

       
    }



    public struct Variable
    {
        public Variable(string name, double worth)
        {
            this.name = name;
            this.worth = worth;
        }
        public string name;
        public double worth;
    }
}
