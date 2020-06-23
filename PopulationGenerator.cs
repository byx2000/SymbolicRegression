using System;
using System.Collections.Generic;
using System.Text;

namespace SymbolicRegression
{
    /// <summary>
    /// 种群生成器接口
    /// </summary>
    interface IPopulationGenerator
    {
        List<IIndividual> Generate();
    }

    /// <summary>
    /// GEP种群生成器
    /// </summary>
    class GEPPopulationGenerator : IPopulationGenerator
    {
        public int PopSize { get; set; }
        public int HeadLen { get; set; }
        public FunctionSet FunctionSet { get; set; }
        public List<Point> Data { get; set; }
        public double ConstMinVal { get; set; }
        public double ConstMaxVal { get; set; }
        public int OptimizeGenerationCount { get; set; }

        public List<IIndividual> Generate()
        {
            List<IIndividual> pop = new List<IIndividual>();
            for (int i = 0; i < PopSize; ++i)
            {
                GEPIndividual ind = new GEPIndividual(HeadLen, FunctionSet, ConstMinVal, ConstMaxVal, OptimizeGenerationCount);
                ind.Optimize(Data);
                pop.Add(ind);
            }
            return pop;
        }
    }
}
