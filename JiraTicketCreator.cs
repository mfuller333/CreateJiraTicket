using System;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class JIRATicketCreator
{
    [SqlProcedure]
    public static void CreateTicket(SqlString jiraUrl, SqlString username, String password, SqlString projectKey, SqlString summary, String description, SqlString type, SqlString priority, String watcherAccountId, String assigneeAccountId, out SqlString issueKeyResult)
    {
        issueKeyResult = SqlString.Null; // Initialize the output parameter
                                         // Define a local variable to capture the result inside the lambda
        string localIssueKeyResult = null;

        Task.Run(async () =>
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {

                var request = new HttpRequestMessage(HttpMethod.Post, jiraUrl.Value + $"/rest/api/2/issue/") ;
                request.Headers.Add("Authorization", password.ToString());

             
                // Replace "High" with the actual priority variable value
                var content = new StringContent(
                    $"{{" +
                    $"\"fields\": {{" +
                    $"\"project\": {{\"key\": \"{projectKey.Value}\"}}," +
                    $"\"summary\": \"{summary.Value}\"," +
                    $"\"description\": \"{description.ToString()}\"," +
                    $"\"issuetype\": {{\"name\": \"{type.Value}\"}}," +
                    $"\"priority\": {{\"name\": \"{priority.Value}\"}}," +
                    $"\"assignee\": {{\"accountId\": \"{assigneeAccountId.ToString()}\"}}" + // Changed to use accountId
                    $"}}}}", Encoding.UTF8, "application/json");

                request.Content = content;

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();


                var responseBody = await response.Content.ReadAsStringAsync();

                IssueCreationResponse sresponse = JsonConvert.DeserializeObject<IssueCreationResponse>(responseBody.ToString());

                localIssueKeyResult = sresponse.Key.ToString();  // Set the output parameter with the issue key
            
                // Add the watcher
                var watcherRequest = new HttpRequestMessage(HttpMethod.Post, jiraUrl.Value + $"/rest/api/3/issue/{localIssueKeyResult}/watchers");
                watcherRequest.Headers.Add("Authorization", password.ToString());
                var watcherContent = new StringContent( $"\"{watcherAccountId.ToString()}\"", Encoding.UTF8, "application/json");
                watcherRequest.Content = watcherContent;

                var watcherResponse = await client.SendAsync(watcherRequest);
                watcherResponse.EnsureSuccessStatusCode();

            }
        }).GetAwaiter().GetResult();

        // Set the output parameter with the result after the async call
        issueKeyResult = new SqlString(localIssueKeyResult);
    }

    private static string ExtractIssueKey(string responseBody)
    {
        // Deserialize response body into strongly-typed object
        var responseJson = JsonConvert.DeserializeObject<IssueCreationResponse>(responseBody);
        return responseJson.Key;
    }
}
public class IssueCreationResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("self")]
    public string Self { get; set; }
}