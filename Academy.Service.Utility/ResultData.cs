using AutoWrapper;

namespace Academy.Service.Utility;

public class ResultData<T>
{
    public string Information { get; set; }
    public T Response { get; set; }
}
/// <summary>
/// API Result in order filter based on success/failure
/// </summary>
public class MapResponseObject  
{
    /// <summary>
    /// Is Error property indicates the request is success/failed
    /// </summary>
    [AutoWrapperPropertyMap(Prop.IsError)]
    public bool IsError { get; set; }
    [AutoWrapperPropertyMap(Prop.Result)]
    public object Data { get; set; }
    
    [AutoWrapperPropertyMap(Prop.ResponseException)]
    public object Error { get; set; }
}


public class Error  
{
    public string Message { get; set; }

    public string Code { get; set; }
    public InnerError InnerError { get; set; }

    public Error(string message, string code, InnerError inner)
    {
        this.Message = message;
        this.Code = code;
        this.InnerError = inner;
    }

}

public class InnerError  
{
    public string RequestId { get; set; }
    public string Date { get; set; }

    public InnerError(string reqId, string reqDate)
    {
        this.RequestId = reqId;
        this.Date = reqDate;
    }
}