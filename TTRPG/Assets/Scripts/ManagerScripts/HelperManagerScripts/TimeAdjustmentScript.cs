using System;

public static class TimeAdjustmentScript
{
    public static float Strength { get; set; } = 1f;

    public static double LogReduce(double value, float? strengthOverride = null)
    {
        if (value <= 0) return 0;

        float strength = strengthOverride ?? Strength;

        double reduction = Math.Log(value + 1) * strength;
        double reducedValue = value - reduction;

        return Math.Max(0, reducedValue);
    }

    public static float LogReduce(float value)
        => (float)LogReduce((double)value);

    public static int LogReduce(int value)
        => (int)Math.Round(LogReduce((double)value));

    public static long LogReduce(long value)
        => (long)Math.Round(LogReduce((double)value));
}
