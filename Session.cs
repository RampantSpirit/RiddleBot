using System.Collections.Generic;

public class Session
{
    public int CurrentRiddle { get; set; }
    public List<int> PreviousRiddles { get; set; }
    public int ClueNo { get; set; }

    public Session(int currentRiddle, List<int> previousRiddles, int clueNo)
    {
        CurrentRiddle = currentRiddle;
        PreviousRiddles = previousRiddles;
        ClueNo = clueNo;
    }
}