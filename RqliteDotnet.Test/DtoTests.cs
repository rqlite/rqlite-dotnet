using NUnit.Framework;
using RqliteDotnet.Dto;

namespace RqliteDotnet.Test;

public class DtoTests
{
    [Test]
    public void QueryParameterSerialization_Works()
    {
        var qNum = new QueryParameter() { Value = 10, ParamType = QueryParamType.Number };
        var qpStr = new QueryParameter() { Value = "Alexander", ParamType = QueryParamType.String };
        
        Assert.That(qNum.ToParamString(), Is.EqualTo("10"));
        Assert.That(qpStr.ToParamString(), Is.EqualTo("\"Alexander\""));
    }

    [Test]
    public void NamedQueryParameterSerialization_Works()
    {
        var qNum = new NamedQueryParameter() {Name = "age", Value = 10, ParamType = QueryParamType.Number };
        var qStr = new NamedQueryParameter() {Name = "name", Value = "Alexander", ParamType = QueryParamType.String };
        
        Assert.That(qNum.ToParamString(), Is.EqualTo("\"age\":10"));
        Assert.That(qStr.ToParamString(), Is.EqualTo("\"name\":\"Alexander\""));
    }
}