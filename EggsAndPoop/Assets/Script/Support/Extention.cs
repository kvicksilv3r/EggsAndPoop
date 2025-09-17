using System;

public static class Extention
{
    public static DateTime ToDateTime(this long input)
    {
        return new DateTime(input);
    }

    public static long ToLong(this DateTime input)
    {
        return input.Ticks;
    }
}
