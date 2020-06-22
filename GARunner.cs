using System;
using System.Collections.Generic;
using System.Text;

namespace SymbolicRegression
{
    /// <summary>
    /// 遗传算法进化器
    /// </summary>
    interface IGARunner
    {
        void Evolve();
    }

    /// <summary>
    /// 默认进化算法
    /// 初始种群：随机生成
    /// 选择方式：锦标赛选择
    /// 子代构成：50％交叉、50％变异
    /// 结束条件：最优个体经过指定代数的进化后没有变化
    /// </summary>
    class GARunner : IGARunner
    {
        public List<Point> Points { get; set; }
        public FunctionSet FunctionSet { get; set; }
        public int PopSize { get; set; }
        public int GenDuration { get; set; }
        public int OptimizeGenCount { get; set; }

        public void Evolve()
        {
            // 初始化种群
            List<Individual> pop = new List<Individual>();
            for (int i = 0; i < PopSize; ++i)
            {
                Individual ind = new Individual(10, FunctionSet, -5, 5, OptimizeGenCount);
                ind.Optimize(Points);
                pop.Add(ind);
            }

            // 开始进化
            int cnt = 0;
            int iGen = 0;
            while (true)
            {
                if (cnt > GenDuration)
                {
                    break;
                }

                // 计算所有个体的误差 查找最优子代
                List<double> errs = new List<double>();
                Individual best = pop[0];
                errs.Add(best.Error(Points));
                double minErr = errs[0];
                bool findBest = false;
                for (int i = 1; i < PopSize; ++i)
                {
                    errs.Add(pop[i].Error(Points));
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
                Individual bestBackup = (Individual)best.Clone();

                // 锦标赛选择 变异
                List<Individual> newPop = new List<Individual>();
                for (int i = 0; i < PopSize; i += 2)
                {
                    if (errs[i] < errs[i + 1])
                    {
                        pop[i].Mutate();
                        pop[i].Optimize(Points);
                        newPop.Add(pop[i]);
                    }
                    else
                    {
                        pop[i + 1].Mutate();
                        pop[i + 1].Optimize(Points);
                        newPop.Add(pop[i + 1]);
                    }
                }

                // 交叉
                for (int i = 0; i < PopSize / 4; ++i)
                {
                    int r1 = RandomUtil.U(0, PopSize / 2);
                    int r2 = RandomUtil.U(0, PopSize / 2);
                    Individual p1 = (Individual)newPop[r1].Clone();
                    Individual p2 = (Individual)newPop[r2].Clone();
                    p1.Cross(p2);
                    p1.Optimize(Points);
                    p2.Optimize(Points);
                    newPop.Add(p1);
                    newPop.Add(p2);
                }

                // 保留精英
                newPop[0] = bestBackup;

                pop = newPop;

                iGen++;
            }
        }
    }
}
