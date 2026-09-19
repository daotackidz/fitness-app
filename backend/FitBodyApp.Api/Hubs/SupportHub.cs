using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FitBodyApp.Api.Hubs;

[Authorize]
public class SupportHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var ticketId = Context.GetHttpContext()?.Request.RouteValues["ticketId"]?.ToString();
        if (!string.IsNullOrEmpty(ticketId))
            await Groups.AddToGroupAsync(Context.ConnectionId, ticketId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var ticketId = Context.GetHttpContext()?.Request.RouteValues["ticketId"]?.ToString();
        if (!string.IsNullOrEmpty(ticketId))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticketId);

        await base.OnDisconnectedAsync(exception);
    }
}
