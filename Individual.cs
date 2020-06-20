using System;
using System.Collections.Generic;
using System.Text;

namespace SymbolicRegression
{
    class Individual : ICloneable
    {
        private readonly List<Function> gene = new List<Function>();
        private readonly List<double> coeff = new List<double>();
        private  FunctionSet functionSet;

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

        public Individual()
        {

        }

        public Individual(List<Function> gene, List<double> coeff, FunctionSet functionSet)
        {
            this.gene = gene;
            this.coeff = coeff;
            this.functionSet = functionSet;
        }

        public Individual(int headLen, FunctionSet functionSet, double minVal, double maxVal)
        {
            this.functionSet = functionSet;

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
            return GetValue(ref iGene, ref iCoeff, x);
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

        public void Cross(Individual ind)
        {
            int r1 = RandomUtil.U(0, gene.Count);
            int r2 = RandomUtil.U(r1, gene.Count);
            for (int i = r1; i <= r2; ++i)
            {
                Function t = gene[i];
                gene[i] = ind.gene[i];
                ind.gene[i] = t;
            }
        }

        public object Clone()
        {
            Individual ind = new Individual();
            for (int i = 0; i < gene.Count; ++i)
            {
                ind.gene.Add(gene[i]);
            }
            for (int i = 0; i < coeff.Count; ++i)
            {
                ind.coeff.Add(coeff[i]);
            }
            ind.functionSet = functionSet;
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

        // 比较两组系数谁更优
        private bool Better(List<double> c1, List<double> c2, List<Point> points)
        {
            for (int i = 0; i < c1.Count; ++i)
            {
                coeff[i] = c1[i];
            }
            double e1 = Error(points);
            for (int i = 0; i < c2.Count; ++i)
            {
                coeff[i] = c2[i];
            }
            double e2 = Error(points);
            return e1 < e2;
        }

        public void Optimize(List<Point> points, int maxGeneration)
        {
            //int popSize = 10;
            //double sigma = 1;
            //int geneLen = CoeffCount;

            //// 初始化种群
            //List<List<double>> pop = new List<List<double>>();
            //for (int i = 0; i < popSize; ++i)
            //{
            //    List<double> ind = new List<double>();
            //    for (int j = 0; j < geneLen; ++j)
            //    {
            //        ind.Add(RandomUtil.N(coeff[j], sigma));
            //    }
            //    pop.Add(ind);
            //}

            //// 最好个体
            //List<double> best = new List<double>();
            //for (int i = 0; i < geneLen; ++i)
            //{
            //    best.Add(coeff[i]);
            //}

            //for (int iGen = 0; iGen < maxGeneration; ++iGen)
            //{
            //    // 查找最优个体
            //    for (int i = 0; i < popSize; ++i)
            //    {
            //        if (Better(pop[i], best, points))
            //        {
            //            best = pop[i];
            //        }
            //    }

            //    // 生成子代
            //    for (int i = 0; i < popSize; ++i)
            //    {
            //        List<double> child = new List<double>();
            //        for (int j = 0; j < geneLen; ++j)
            //        {
            //            child.Add(RandomUtil.N(pop[i][j], sigma));
            //        }
            //        if (Better(child, pop[i], points))
            //        {
            //            pop[i] = child;
            //        }
            //    }
            //}

            //// 更新系数
            //for (int i = 0; i < geneLen; ++i)
            //{
            //    coeff[i] = best[i];
            //}

            double sigma = 1;
            int coeffCount = CoeffCount;

            double minErr = Error(points);
            for (int iGen = 0; iGen < maxGeneration; ++iGen)
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
