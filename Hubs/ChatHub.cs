using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Collabo_app.Hubs
{
    public class ChatHub : Hub<IChatHub>
    {
        // Send message to all clients
        //[Authorize]
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).NewuserNotification($"{Context.ConnectionId} has joined the group {groupName}.");
        }

        [Authorize]
        public async Task SendMessageToGroup(string groupName, string user, string message)
        {
            await Clients.Group(groupName).ReceiveMessage(user, message);
        }
 
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
