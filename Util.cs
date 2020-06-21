using System;
using System.Collections.Generic;
using System.Text;

namespace SymbolicRegression
{
    /// <summary>
    /// 随机数工具类
    /// </summary>
    static class RandomUtil
    {
        private static readonly Random rand = new Random();

        // 整数均匀分布
        public static int U(int minValue, int maxValue)
        {
            return rand.Next(minValue, maxValue);
        }

        // 实数均匀分布
        public static double U(double minValue, double maxValue)
        {
            return rand.NextDouble() * (maxValue - minValue) - maxValue;
        }

        // 正态分布
        public static double N(double u, double d)
        {
            double u1, u2, z, x;
            if (d <= 0)
            {
                return u;
            }
            u1 = rand.NextDouble();
            u2 = rand.NextDouble();
            z = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);
            x = u + d * z;
            return x;
        }

        // 随机取列表的一个元素
        public static T RandomElement<T>(List<T> elems)
        {
            return elems[U(0, elems.Count)];
        }

        // 测试概率
        public static bool Test(double probability)
        {
            return rand.NextDouble() < probability;
        }
    }
}
