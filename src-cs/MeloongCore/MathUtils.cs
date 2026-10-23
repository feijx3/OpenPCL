using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using MeloongCore.Extensions;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class MathUtils
    {
        public static string ConvertRadix(this string input, int fromRadix, int toRadix)
        {
            if (fromRadix < 2 || fromRadix > "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/+=!?@#$%^&*()[]{}<>;:',".Length)
            {
                throw new ArgumentOutOfRangeException("fromRadix", "fromRadix must be between 2 and 86.");
            }
            if (string.IsNullOrEmpty(input))
            {
                return "0";
            }
            bool flag = input.StartsWithF("-", false);
            if (flag)
            {
                input = input.TrimStart(new char[] { '-' });
            }
            BigInteger bigInteger = 0;
            foreach (char c in input)
            {
                int num = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/+=!?@#$%^&*()[]{}<>;:',".IndexOf(c);
                if (num == -1 || num >= fromRadix)
                {
                    throw new ArgumentException(string.Format("Character '{0}' in input '{1}' is not a valid digit for radix {2}.", c, input, fromRadix));
                }
                bigInteger = bigInteger * fromRadix + num;
            }
            return (bigInteger * (flag ? (-1) : 1)).ConvertRadix(toRadix);
        }

        public static string ConvertRadix(this long input, int toRadix)
        {
            return input.ConvertRadix(toRadix);
        }

        public static string ConvertRadix(this ulong input, int toRadix)
        {
            return input.ConvertRadix(toRadix);
        }

        public static string ConvertRadix(this int input, int toRadix)
        {
            return input.ConvertRadix(toRadix);
        }

        public static string ConvertRadix(this byte input, int toRadix)
        {
            return input.ConvertRadix(toRadix);
        }

        public static string ConvertRadix(this BigInteger input, int toRadix)
        {
            if (toRadix < 2 || toRadix > "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/+=!?@#$%^&*()[]{}<>;:',".Length)
            {
                throw new ArgumentOutOfRangeException("toRadix", "toRadix must be between 2 and 86.");
            }
            if (input == 0L)
            {
                return "0";
            }
            bool flag = input < 0L;
            List<char> list = new List<char>();
            BigInteger bigInteger = BigInteger.Abs(input);
            BigInteger bigInteger2 = toRadix;
            while (bigInteger > 0L)
            {
                BigInteger bigInteger3;
                bigInteger = BigInteger.DivRem(bigInteger, bigInteger2, out bigInteger3);
                list.Add("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/+=!?@#$%^&*()[]{}<>;:',"[(int)bigInteger3]);
            }
            if (flag)
            {
                list.Add('-');
            }
            list.Reverse();
            return new string(list.ToArray());
        }

        public static T Clamp<[Nullable(0)] T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            if (value.CompareTo(max) > 0)
            {
                return max;
            }
            return value;
        }

        public static double Lerp(double start, double end, double percentage)
        {
            return Math.Round(start + (end - start) * percentage, 7);
        }

        public static double Bezier(double t, double x1, double y1, double x2, double y2, double accuracy = 0.01)
        {
            if (t < 1E-07 || double.IsNaN(t))
            {
                return 0.0;
            }
            if (t > 0.9999999)
            {
                return 1.0;
            }
            double num = t;
            int num2 = 1000;
            double num3;
            do
            {
                num3 = 3.0 * num * ((0.33333333 + x1 - x2) * num * num + (x2 - 2.0 * x1) * num + x1);
                num += (t - num3) * 0.5;
                num2--;
            }
            while (Math.Abs(num3 - t) >= accuracy && num2 > 0);
            return 3.0 * num * ((0.33333333 + y1 - y2) * num * num + (y2 - 2.0 * y1) * num + y1);
        }

        public static bool IsNear(this double value, double target, double tolerance = 1E-06)
        {
            return Math.Abs(value - target) < tolerance;
        }

        public static IEnumerable<int> GetPrimeFactors(int num)
        {
            MathUtils.<GetPrimeFactors>d__10 <GetPrimeFactors>d__ = new MathUtils.<GetPrimeFactors>d__10(-2);
            <GetPrimeFactors>d__.<>3__num = num;
            return <GetPrimeFactors>d__;
        }

        public static int Gcd(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b != 0)
            {
                int num = b;
                b = a % b;
                a = num;
            }
            return a;
        }

        public static int Gcd(double a, int b)
        {
            return MathUtils.Gcd((int)Math.Round(a), b);
        }
    }
}
