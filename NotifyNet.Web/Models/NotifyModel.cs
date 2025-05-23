namespace NotifyNet.Web.Models;

public class NotifyModel
{
    public string Token { get; set; }
    
    public Notification Notification { get; set; }
    
    public Dictionary<string, string> Data { get; set; } 
}