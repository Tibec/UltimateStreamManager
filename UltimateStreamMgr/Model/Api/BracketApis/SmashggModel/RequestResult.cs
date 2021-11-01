using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api.BracketApis.SmashggModel
{
    public class RequestResult
    {
        [JsonProperty("data")] public RequestResultData Data { get; set; }
        [JsonProperty("extensions")] public RequestResultExtension Extension { get; set; }
        //[JsonProperty("actionRecords")] public RequestResultActionRecord ActionRecords { get; set; }
        [JsonProperty("errors")] public RequestError Error { get; set; }
    }

    public class RequestResultData
    {
        [JsonProperty("tournament")] public Tournament Tournament { get; set; }
        // [JsonProperty("extensions")] public object Extension { get; set; }
        // [JsonProperty("actionRecords")] public object Data { get; set; }
    }

    public class RequestError
    {
        [JsonProperty("message")] public string Message { get; set; }
    }

    public class RequestResultExtension
    {

    }

    public class RequestResultActionRecord
    {

    }
}
