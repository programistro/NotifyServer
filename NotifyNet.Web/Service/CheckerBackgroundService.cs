using System.Net.Http.Headers;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Models;
using NotifyNet.Infrastructure.Data;
using NotifyNet.Web.Hubs;
using NotifyNet.Web.Models;

namespace NotifyNet.Web.Service;

public class CheckerBackgroundService : BackgroundService
{
    private List<Order> Orders { get; set; } = new();
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHubContext<OrderHub> _orderHub;
    private readonly ILogger<CheckerBackgroundService> _logger;
    private readonly ConnectionManager _connectionManager;
    private readonly HttpClient _httpClient;

    public CheckerBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<CheckerBackgroundService> logger,
        ConnectionManager connectionManager, IHubContext<OrderHub> orderHub)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _connectionManager = connectionManager;
        _orderHub = orderHub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                
                    if (Orders.Count == 0)
                    {
                        var list = await _orderService.GetAllAsync();

                        foreach (var item in list)
                        {
                            Orders.Add(item);
                        }
                    }

                    var newList = await _orderService.GetAllAsync();
                    newList.ToList();

                    var onlyInFirst = newList.Except(Orders).ToList();
                    var onlyInSecond = Orders.Except(newList).ToList();

                    if (onlyInFirst.Count < newList.Count())
                    {
                        foreach (var item in onlyInFirst)
                        {
                            Orders.Add(item);

                            // await _orderHub.Clients.All.SendAsync("OrderCreated", item);
                            var token = "206364511873-gh73elvv6qj7g49dugsf543gd1p5d56r.apps.googleusercontent.com";
        
                            var credential = GoogleCredential.FromFile(@"LocalNotificationsDemo/Platforms/Android/Resources/metal-ranger-379519-firebase-adminsdk-pufly-b376aa5169.json")
                                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                            var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
                            var payload = new
                            {
                                message = new
                                {
                                    topic = "order_created",
                                    data = new Dictionary<string, string>
                                    {
                                        { "order", JsonConvert.SerializeObject(item) }
                                    }
                                }
                            };

                            var json = JsonConvert.SerializeObject(payload);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");
                            var responseMessage = await _httpClient.PostAsync($"https://fcm.googleapis.com/v1/projects/metal-ranger-379519/messages:send", content);
        
                            var contentString = await responseMessage.Content.ReadAsStringAsync();
                        }
                    }
                    if(onlyInSecond.Count < newList.Count())
                    {
                        foreach (var item in onlyInSecond)
                        {
                            Orders.Remove(item);
                        }
                    }
                }
            }
            catch (Exception e)
            { 
                Console.WriteLine(e);
            }
        
            foreach (var item in _connectionManager.Users)
            {
                _logger.LogInformation($"user: {item}");
            }
            foreach (var item in _connectionManager.ConnectionsId)
            {
                _logger.LogInformation($"user connect id: {item}");
            }
            _logger.LogInformation($"orders count {Orders.Count}");

            await Task.Delay(10000, stoppingToken);
        }
    }
}