using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NotifyNet.Core.Models;
using NotifyNet.Web.Models;

namespace NotifyNet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public TestController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost]
    [Route("Post")]
    public async Task<ActionResult> PostAsync(Order model)
    {
        //206364511873-gh73elvv6qj7g49dugsf543gd1p5d56r.apps.googleusercontent.com
        var token = "206364511873-gh73elvv6qj7g49dugsf543gd1p5d56r.apps.googleusercontent.com";
        
        var credential = GoogleCredential.FromFile(@"C:\Users\katana\Downloads\NotifyServer\NotifyServer\LocalNotificationsDemo\Platforms\Android\Resources\metal-ranger-379519-firebase-adminsdk-pufly-b376aa5169.json")
            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        var json2 = """
                    {
                        "message": {
                            "topic": "order_created",
                            "notification": {
                                "title": "Order is created",
                                "body": "Pleasant with clouds and sun"
                            },
                            "data": {
                                "line": "string 111",
                            }
                        }
                    }
                    """;
        
        var payload = new
        {
            message = new
            {
                topic = "order_created",
                data = new Dictionary<string, string>
                {
                    { "order", JsonConvert.SerializeObject(model) }
                }
            }
        };

        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var responseMessage = await _httpClient.PostAsync($"https://fcm.googleapis.com/v1/projects/metal-ranger-379519/messages:send", content);
        
        var contentString = await responseMessage.Content.ReadAsStringAsync();
        
        return Ok(contentString);
    }
}