using System;

public class RandomNum
{
    public static Random randomNum = new Random();

    public static int Rand(int lowerBound, int upperBound)
    {
        return randomNum.Next(lowerBound, upperBound);
    }
}
