using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net;

public class Script : ScriptBase
{
    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
        if (this.Context.OperationId == "RegexReplace")
        {
            return await this.PerformRegexReplace().ConfigureAwait(false);
        }

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        response.Content = CreateJsonContent($"Unknown operation ID '{this.Context.OperationId}'");
        return response;
    }

    private async Task<HttpResponseMessage> PerformRegexReplace()
    { 
        // Manipulate the request data as applicable before setting it back
        var requestContentAsString = await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);
        var requestContentAsJson = JObject.Parse(requestContentAsString);

        var input = (string)requestContentAsJson["input"];
        var pattern = (string)requestContentAsJson["pattern"];
        var replacement = (string)requestContentAsJson["replacement"];

        var regexResult = Regex.Replace(input, pattern, replacement, TimeSpan.FromMilliseconds(250));

        // Manipulate the response data as applicable before returning it
        JObject output = new JObject
        {
            ["output"] = regexResult
        };

        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = CreateJsonContent(output.ToString());
        return response;
    }
}
