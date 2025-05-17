namespace NotifyNet.Web.Service;

public class ConnectionManager
{
    public List<string> Users { get; set; } = new List<string>();
    
    public HashSet<string> ConnectionsId { get; set; } = new HashSet<string>();
}