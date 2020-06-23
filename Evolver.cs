using System;
using System.Collections.Generic;
using System.Text;

namespace SymbolicRegression
{
    /// <summary>
    /// 种群进化器接口
    /// </summary>
    interface IEvolver
    {
        List<IIndividual> Evolve(List<IIndividual> initPop);
    }

    /// <summary>
    /// 进化器1
    /// 选择方式：锦标赛选择（锦标赛规模=2）
    /// 子代构成：50％交叉、50％变异
    /// 结束条件：最优个体经过MaxCount代的进化后没有变化
    /// </summary>
    class Evolver1 : IEvolver
    {
        public List<Point> Data { get; set; }
        public int MaxCount { get; set; }

        public List<IIndividual> Evolve(List<IIndividual> initPop)
        {
            int popSize = initPop.Count;
            List<IIndividual> pop = initPop;

            // 开始进化
            int cnt = 0;
            int iGen = 0;
            while (true)
            {
                if (cnt > MaxCount)
                {
                    break;
                }

                // 计算所有个体的误差 查找最优子代
                List<double> errs = new List<double>();
                IIndividual best = pop[0];
                errs.Add(best.Error(Data));
                double minErr = errs[0];
                bool findBest = false;
                for (int i = 1; i < popSize; ++i)
                {
                    errs.Add(pop[i].Error(Data));
                    if (errs[i] < minErr)
                    {
                        best = pop[i];
                        minErr = errs[i];
                        findBest = true;
                    }
                }

                if (findBest)
                {
                    cnt = 0;
                }
                else
                {
                    cnt++;
                }

                // 输出最优个体
                Console.WriteLine(iGen);
                Console.WriteLine(best.ExprString);
                Console.WriteLine("error: " + minErr);
                Console.WriteLine();

                // 备份最好个体
                GEPIndividual bestBackup = (GEPIndividual)best.Clone();

                // 锦标赛选择 变异
                List<IIndividual> newPop = new List<IIndividual>();
                for (int i = 0; i < popSize; i += 2)
                {
                    if (errs[i] < errs[i + 1])
                    {
                        pop[i].Mutate();
                        pop[i].Optimize(Data);
                        newPop.Add(pop[i]);
                    }
                    else
                    {
                        pop[i + 1].Mutate();
                        pop[i + 1].Optimize(Data);
                        newPop.Add(pop[i + 1]);
                    }
                }

                // 交叉
                for (int i = 0; i < popSize / 4; ++i)
                {
                    int r1 = RandomUtil.U(0, popSize / 2);
                    int r2 = RandomUtil.U(0, popSize / 2);
                    GEPIndividual p1 = (GEPIndividual)newPop[r1].Clone();
                    GEPIndividual p2 = (GEPIndividual)newPop[r2].Clone();
                    p1.Cross(p2);
                    p1.Optimize(Data);
                    p2.Optimize(Data);
                    newPop.Add(p1);
                    newPop.Add(p2);
                }

                // 保留精英
                newPop[0] = bestBackup;

                pop = newPop;

                iGen++;
            }

            return pop;
        }
    }
}
