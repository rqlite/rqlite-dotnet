namespace RqliteDotnet;

public enum ReadLevel
{
    Default = 1,
    Weak,
    Linearizable,
    Strong,
    None,
    Auto
}
