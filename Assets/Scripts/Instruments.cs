using System;
using System.IO;

public enum EInstrument
{
    // respectivement : Piano, ... // a remplir
    eSaxophone,
    ePiano
}

public static class Instruments
{
    public static int GetNumber()
    {
        return Enum.GetValues(typeof(EInstrument)).Length; 
    }

    public static string GetInstrumentPath(EInstrument instrument)
    {
        string instrumentDir;
        string instrumentName;
        switch (instrument)
        {
            case EInstrument.ePiano:
                instrumentDir = "Piano";
                instrumentName = "SalamanderGrandPianoV3Retuned.sfz";
                break;
            case EInstrument.eSaxophone:
                instrumentDir = "Saxophone";
                instrumentName = "TenorSaxophone-20200717.sfz";
                break;
            default:
                return null;
        }
        string instrumentPath = Path.GetFullPath(
            Path.Combine(
                UnityEngine.Application.streamingAssetsPath,
                "Instruments", instrumentDir, instrumentName
            )
        );
        return instrumentPath;
    }
}

