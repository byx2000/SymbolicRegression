using System;
using System.Collections.Generic;

namespace SymbolicRegression
{
    class Program
    {
        static void Main()
        {
            // 数据点
            List<Point> points = new List<Point>
            {
                // minErr: 0.90
                //new Point(-5.25, -2.26),
                //new Point(-4.22, 1.19),
                //new Point(-3.55, 2.72),
                //new Point(-0.73, 2.74),
                //new Point(0.45, 0.78),
                //new Point(1.07, -1.26),
                //new Point(-4.95, -1.12),
                //new Point(-2.09, 3.6),

                // minErr: 0.90
                //new Point(-7.46, -1.42),
                //new Point(-5.42, -0.68),
                //new Point(-2.92, 0.6),
                //new Point(-0.86, 2.98),
                //new Point(0.66, 5.62),
                //new Point(1.34, 7.48),
                //new Point(1.72, 9.6),
                //new Point(1.92, 10.86),

                // minErr: 0.30
                //new Point(-4.7, -1.14),
                //new Point(-4.54, -0.35),
                //new Point(-4.28, 0.44),
                //new Point(-3.86, 1.06),
                //new Point(-3.32, 1.2),
                //new Point(-2.44, 0.83),
                //new Point(-0.92, -0.58),
                //new Point(-1.73, 0.27),

                // minErr: 1.42
                //new Point(-2.1, 4.38),
                //new Point(-1.86, 2.7),
                //new Point(-1.34, 1.16),
                //new Point(0.92, 0.36),
                //new Point(2.82, 0.82),
                //new Point(4.62, 1.62),
                //new Point(5.9, 2.82),
                //new Point(6.82, 4),

                // minErr: 0.60
                //new Point(-2.92, 4.65),
                //new Point(-2, 2),
                //new Point(-1.02, 0.4),
                //new Point(0, -1),
                //new Point(1.54, -1.83),
                //new Point(4.25, -2.54),
                //new Point(6.51, -2.93),
                //new Point(8.77, -3.07),

                // minErr: 0.39
                //new Point(-1.82, -6.62),
                //new Point(-0.44, -5.08),
                //new Point(2, -3),
                //new Point(3.36, -1.56),
                //new Point(5.52, 0.66),
                //new Point(7.1, 2.44),
                //new Point(7.9, 3.28),
                //new Point(8.58, 4),

                // minErr: 0.48
                //new Point(1.9, 0.58),
                //new Point(2.38, 1.68),
                //new Point(2.78, 2.92),
                //new Point(3.42, 4.52),
                //new Point(4, 6.16),
                //new Point(4.52, 7.34),
                //new Point(4.98, 8.76),
                //new Point(5.36, 9.76),

                // minErr: 0.01
                //new Point(-4.09, 0.81),
                //new Point(-3.02, -0.12),
                //new Point(-1.68, -0.99),
                //new Point(-0.5, -0.48),
                //new Point(0.43, 0.42),
                //new Point(1.5, 1),
                //new Point(2.51, 0.59),
                //new Point(3.91, -0.69),

                // minErr: 0.21
                new Point(-3.33, 0.14),
                new Point(-2.62, 0.23),
                new Point(-1.73, 0.41),
                new Point(-0.73, 0.74),
                new Point(0.28, 1.23),
                new Point(1.15, 2.09),
                new Point(1.61, 3.23),
                new Point(1.94, 4.19),
            };

            // 函数集
            FunctionSet functionSet = new FunctionSet();
            functionSet
                .AddTerminator(new Var())
                .AddTerminator(new Const())
                .AddNonterminator(new Add())
                .AddNonterminator(new Sub())
                .AddNonterminator(new Mul())
                .AddNonterminator(new Div())
                .AddNonterminator(new Sin())
                .AddNonterminator(new Exp())
                ;

            GARunner runner = new GARunner
            {
                Points = points,
                FunctionSet = functionSet,
                PopSize = 200,
                GenDuration = 100,
                OptimizeGenCount = 500,
            };

            runner.Evolve();

            // 测试代码
            //Individual ind = new Individual(new List<Function> 
            //{
            //    new Add(),
            //    new Add(),
            //    new Mul(),
            //    new Mul(),
            //    new Const(),
            //    new Var(),
            //    new Var(),
            //    new Mul(),
            //    new Const(),
            //    new Var(),
            //    new Const(),
            //}, new List<double> 
            //{
            //    RandomUtil.U(-5.0, 5.0),
            //    RandomUtil.U(-5.0, 5.0),
            //    RandomUtil.U(-5.0, 5.0),
            //});
            //Console.WriteLine(ind.ExprString);
            //ind.Optimize(points);
            //Console.WriteLine(ind.ExprString);
        }
    }
}
