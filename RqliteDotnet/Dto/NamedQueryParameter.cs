namespace RqliteDotnet.Dto;

public class NamedQueryParameter : QueryParameter
{
    public string? Name { get; set; }

    public override string ToParamString()
    {
        return $"\"{Name}\":" + PrintValue();
    }

    private string PrintValue()
    {
        if (Value == null)
        {
            return "null";
        }
        return (ParamType == QueryParamType.Number ? FormattableString.Invariant($"{Value}") : $"\"{Value}\"");
    }
}