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
}