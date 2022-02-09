namespace RqliteDotnet.Dto;

public class NamedQueryParameter : QueryParameter
{
    public string Name { get; set; }

    public override string ToParamString()
    {
        return $"\"{Name}\":" + (ParamType == QueryParamType.Number ? Value.ToString() : $"\"{Value}\"");
    }
}