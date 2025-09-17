using System;
using System.Numerics;

public static class TimeAdjustmentScript
{
    public static float Strength { get; set; } = 1f;
    public static Vector2 Power { get; set; } = new Vector2(1.8f, 2.3f);

    private static double a = 20;
    private static double b = 3;
    private static double c = 0.2;
    private static double d = 9;

    public static double ReduceTime(double value)
    {
        double g = b * (1 - Math.Pow(1 - (value / a), d));
        double h = c * value;

        double finalVal = value < a ? g + h : b * (1 - Math.Pow(1 - 1, d)) + h;

        return Math.Round(Math.Max(0, finalVal), 2);
    }

    public static float LogReduce(float value)
        => (float)ReduceTime((double)value);

    public static int LogReduce(int value)
        => (int)Math.Round(ReduceTime((double)value));

    public static long LogReduce(long value)
        => (long)Math.Round(ReduceTime((double)value));
}
