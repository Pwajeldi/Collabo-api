namespace Collabo_app.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage(string user, string message);
        Task NewuserNotification(string message);
    }
}
