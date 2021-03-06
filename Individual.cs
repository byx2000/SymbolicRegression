﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SymbolicRegression
{
    /// <summary>
    /// 个体接口
    /// </summary>
    interface IIndividual : ICloneable
    {
        string ExprString { get; }
        double Eval(double x);
        double Error(List<Point> points);
        void Mutate();
        void Cross(IIndividual ind);
        void Optimize(List<Point> points);
    }

    /// <summary>
    /// GEP个体实现
    /// </summary>
    class GEPIndividual : IIndividual
    {
        private readonly List<Function> gene = new List<Function>();
        private readonly List<double> coeff = new List<double>();
        private FunctionSet functionSet;
        private int optimizeGenerationCount;

        public string GeneString
        {
            get
            {
                string s = "";
                for (int i = 0; i < gene.Count; ++i)
                {
                    s += gene[i].Symbol;
                }
                return s;
            }
        }

        public string ExprString
        {
            get
            {
                int iGene = 0;
                int iCoeff = 0;
                return GetExprString(ref iGene, ref iCoeff);
            }
        }

        public string CoeffString
        {
            get
            {
                string s = "";
                for (int i = 0; i < coeff.Count; ++i)
                {
                    s += coeff[i].ToString();
                    s += " ";
                }
                return s;
            }
        }

        public int CoeffCount
        {
            get
            {
                int iGene = 0;
                return GetCoeffCount(ref iGene);
            }
        }

        public GEPIndividual()
        {

        }

        public GEPIndividual(List<Function> gene, List<double> coeff, FunctionSet functionSet, int optimizeGenerationCount)
        {
            this.gene = gene;
            this.coeff = coeff;
            this.functionSet = functionSet;
            this.optimizeGenerationCount = optimizeGenerationCount;
        }

        public GEPIndividual(int headLen, FunctionSet functionSet, double minVal, double maxVal, int optimizeGenerationCount)
        {
            this.functionSet = functionSet;
            this.optimizeGenerationCount = optimizeGenerationCount;

            // 计算尾部长度
            int tailLen = headLen * (functionSet.MaxParamCount - 1) + 1;

            // 随机生成头部
            gene.Add(functionSet.RandomNonterminator());
            for (int i = 1; i < headLen; ++i)
            {
                gene.Add(functionSet.RandomFunction());
            }

            // 随机生成尾部
            for (int i = 0; i < tailLen; ++i)
            {
                gene.Add(functionSet.RandomTerminator());
            }

            // 随机生成系数
            for (int i = 0; i < tailLen; ++i)
            {
                coeff.Add(RandomUtil.U(minVal, maxVal));
            }
        }

        private string GetExprString(ref int iGene, ref int iCoeff)
        {
            FunctionType type = gene[iGene].Type;
            
            // 常数
            if (type == FunctionType.Constant)
            {
                iGene++;
                return "(" + coeff[iCoeff++].ToString() + ")";
            }
            // 终结符
            else if (gene[iGene].ParamCount == 0)
            {
                return gene[iGene++].Symbol;
            }
            // 一元运算符 op(...)
            else if (gene[iGene].ParamCount == 1)
            {
                string s = gene[iGene++].Symbol;
                s += "(";
                s += GetExprString(ref iGene, ref iCoeff);
                s += ")";
                return s;
            }
            // 多元运算符 a op b op c...
            else
            {
                string op = gene[iGene].Symbol;
                int cnt = gene[iGene++].ParamCount;
                string s = GetExprString(ref iGene, ref iCoeff);
                for (int i = 1; i < cnt; ++i)
                {
                    s += op;
                    s += GetExprString(ref iGene, ref iCoeff);
                }
                s = "(" + s + ")";
                return s;
            }
        }

        private double GetValue(ref int iGene, ref int iCoeff, double x)
        {
            FunctionType type = gene[iGene].Type;
            if (type == FunctionType.Constant)
            {
                iGene++;
                return coeff[iCoeff++];
            }
            else if (type == FunctionType.Variable)
            {
                iGene++;
                return x;
            }
            else
            {
                Function f = gene[iGene++];
                int cnt = f.ParamCount;
                List<double> p = new List<double>();
                for (int i = 0; i < cnt; ++i)
                {
                    p.Add(GetValue(ref iGene, ref iCoeff, x));
                }
                return f.Eval(p.ToArray());
            }
        }

        public double Error(List<Point> points)
        {
            double err = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                err += Math.Abs(Eval(points[i].X) - points[i].Y);
            }
            return err;
        }

        public double Eval(double x)
        {
            int iGene = 0;
            int iCoeff = 0;
            double val = GetValue(ref iGene, ref iCoeff, x);
            if (double.IsNaN(val))
            {
                return double.PositiveInfinity;
            }
            return val;
        }

        public void Mutate()
        {
            double p = 0.1;
            int headLen = (gene.Count - 1) / functionSet.MaxParamCount;
            for (int i = 0; i < headLen; ++i)
            {
                if (RandomUtil.Test(p))
                {
                    gene[i] = functionSet.RandomFunction();
                }
            }
            for (int i = headLen; i < gene.Count; ++i)
            {
                if (RandomUtil.Test(p))
                {
                    gene[i] = functionSet.RandomTerminator();
                }
            }
        }

        public void Cross(IIndividual ind)
        {
            if (ind is GEPIndividual)
            {
                GEPIndividual gep = (GEPIndividual)ind;
                int r1 = RandomUtil.U(0, gene.Count);
                int r2 = RandomUtil.U(r1, gene.Count);
                for (int i = r1; i <= r2; ++i)
                {
                    Function t = gene[i];
                    gene[i] = gep.gene[i];
                    gep.gene[i] = t;
                }
            }
        }

        public object Clone()
        {
            GEPIndividual ind = new GEPIndividual();
            for (int i = 0; i < gene.Count; ++i)
            {
                ind.gene.Add(gene[i]);
            }
            for (int i = 0; i < coeff.Count; ++i)
            {
                ind.coeff.Add(coeff[i]);
            }
            ind.functionSet = functionSet;
            ind.optimizeGenerationCount = optimizeGenerationCount;
            return ind;
        }

        private int GetCoeffCount(ref int iGene)
        {
            if (gene[iGene].Type == FunctionType.Constant)
            {
                iGene++;
                return 1;
            }
            else if (gene[iGene].ParamCount == 0)
            {
                iGene++;
                return 0;
            }
            else
            {
                int cnt = gene[iGene++].ParamCount;
                int r = 0;
                for (int i = 0; i < cnt; ++i)
                {
                    r += GetCoeffCount(ref iGene);
                }
                return r;
            }
        }

        public void Optimize(List<Point> points)
        {
            double sigma = 1;
            int coeffCount = CoeffCount;

            double minErr = Error(points);
            for (int iGen = 0; iGen < optimizeGenerationCount; ++iGen)
            {
                // 备份
                List<double> backup = new List<double>();
                for (int i = 0; i < coeffCount; ++i)
                {
                    backup.Add(coeff[i]);
                }

                // 更新系数
                for (int i = 0; i < coeffCount; ++i)
                {
                    coeff[i] = RandomUtil.N(coeff[i], sigma);
                }

                double err = Error(points);
                if (err >= minErr)
                {
                    for (int i = 0; i < coeffCount; ++i)
                    {
                        coeff[i] = backup[i];
                    }
                }
                else
                {
                    minErr = err;
                }
            }
        }
    }
}
