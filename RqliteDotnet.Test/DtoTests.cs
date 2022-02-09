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
        
        Assert.AreEqual("10", qNum.ToParamString());
        Assert.AreEqual("\"Alexander\"", qpStr.ToParamString());
    }

    [Test]
    public void NamedQueryParameterSerialization_Works()
    {
        var qNum = new NamedQueryParameter() {Name = "age", Value = 10, ParamType = QueryParamType.Number };
        var qStr = new NamedQueryParameter() {Name = "name", Value = "Alexander", ParamType = QueryParamType.String };
        
        Assert.AreEqual("\"age\":10", qNum.ToParamString());
        Assert.AreEqual("\"name\":\"Alexander\"", qStr.ToParamString());
    }
}