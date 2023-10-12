using System.Collections.Generic;
public enum DamageType
{
    None = 0,
    Fire = 1,
    Ice = 2,
    Earth = 3,
    Wind = 4,
    Holy = 5,
    Dark = 6,
}

public enum ImpulseMode
{
    DamageToTarget = 0,
    DamageLocal = 1,
    Global = 2,
    PlayerToTarget = 3,
    PlayerLocal = 4,
}

public class GlobalName
{
    public static List<string> DamageTypeNames = new List<string>()
    {
        "None",
        "Fire",
        "Ice",
        "Earth",
        "Wind",
        "Holy",
        "Dark",
    };

    public static List<string> ImpulseModeNames = new List<string>()
    {
        "LookAtTarget",
        "Local",
        "Global",
    };

}
