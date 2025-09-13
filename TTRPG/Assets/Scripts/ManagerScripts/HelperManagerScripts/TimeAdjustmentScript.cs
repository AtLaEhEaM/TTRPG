using System;
using System.Numerics;

public static class TimeAdjustmentScript
{
    public static float Strength { get; set; } = 1f;
    public static Vector2 Power { get; set; } = new Vector2(1.8f, 2.3f);

    public static double LogReduce(double value, float? strengthOverride = null)
    {
        if (value <= 0) return 0;

        float strength = strengthOverride ?? Strength;

        float reduction = MathF.Pow((float)value, Power.X / Power.Y);
        //double reducedValue = value - reduction;

        return Math.Max(0, (double)reduction);
    }

    public static float LogReduce(float value)
        => (float)LogReduce((double)value);

    public static int LogReduce(int value)
        => (int)Math.Round(LogReduce((double)value));

    public static long LogReduce(long value)
        => (long)Math.Round(LogReduce((double)value));
}
