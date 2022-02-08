namespace RqliteDotnet.Dto;

public class QueryParameter
{
    public QueryParameter(object value)
    {
        ParamType = QueryParamType.String;
        Value = value;
    }
    
    public QueryParamType ParamType { get; set; }
    
    public object Value { get; set; }
}

public enum QueryParamType
{
    String,
    Number
}