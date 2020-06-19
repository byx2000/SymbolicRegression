using System;
using System.Collections.Generic;
using System.Text;

namespace SymbolicRegression
{
    enum FunctionType
    {
        Constant, Variable, Function
    }

    abstract class Function
    {
        public abstract int ParamCount { get; }
        public abstract string Symbol { get; }
        public virtual FunctionType Type { get; } = FunctionType.Function;
        public abstract double Eval(params double[] p);
    }

    class Const : Function
    {
        public override int ParamCount { get; } = 0;
        public override string Symbol { get; } = "c";
        public override FunctionType Type { get; } = FunctionType.Constant;

        public override double Eval(params double[] p)
        {
            throw new NotImplementedException();
        }
    }

    class Var : Function
    {
        public override int ParamCount { get; } = 0;
        public override string Symbol { get; } = "x";
        public override FunctionType Type { get; } = FunctionType.Variable;

        public override double Eval(params double[] p)
        {
            throw new NotImplementedException();
        }
    }

    class Add : Function
    {
        public override int ParamCount { get; } = 2;
        public override string Symbol { get; } = "+";

        public override double Eval(params double[] p)
        {
            return p[0] + p[1];
        }
    }

    class Sub : Function
    {
        public override int ParamCount { get; } = 2;
        public override string Symbol { get; } = "-";

        public override double Eval(params double[] p)
        {
            return p[0] - p[1];
        }
    }

    class Mul : Function
    {
        public override int ParamCount { get; } = 2;
        public override string Symbol { get; } = "*";

        public override double Eval(params double[] p)
        {
            return p[0] * p[1];
        }
    }

    class Div : Function
    {
        public override int ParamCount { get; } = 2;
        public override string Symbol { get; } = "/";

        public override double Eval(params double[] p)
        {
            return p[0] / p[1];
        }
    }

    class Sin : Function
    {
        public override int ParamCount { get; } = 1;

        public override string Symbol { get; } = "sin";

        public override double Eval(params double[] p)
        {
            return Math.Sin(p[0]);
        }
    }

    class Exp : Function
    {
        public override int ParamCount { get; } = 1;

        public override string Symbol { get; } = "exp";

        public override double Eval(params double[] p)
        {
            return Math.Exp(p[0]);
        }
    }

    class FunctionSet
    {
        private readonly List<Function> terminator = new List<Function>();
        private readonly List<Function> nonterminator = new List<Function>();

        public int MaxParamCount
        {
            get
            {
                int r = 0;
                for (int i = 0; i < nonterminator.Count; ++i)
                {
                    r = Math.Max(r, nonterminator[i].ParamCount);
                }
                return r;
            }
        }

        public FunctionSet AddTerminator(Function f)
        {
            if (f.ParamCount == 0)
            {
                terminator.Add(f);
            }
            return this;
        }

        public FunctionSet AddNonterminator(Function f)
        {
            if (f.ParamCount > 0)
            {
                nonterminator.Add(f);
            }
            return this;
        }

        public Function RandomTerminator()
        {
            return RandomUtil.RandomElement(terminator);
        }

        public Function RandomNonterminator()
        {
            return RandomUtil.RandomElement(nonterminator);
        }

        public Function RandomFunction()
        {
            if (RandomUtil.Test(0.5))
            {
                return RandomTerminator();
            }
            else
            {
                return RandomNonterminator();
            }
        }
    }
}
