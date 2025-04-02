using BusinessLogic.ViewModels;
using Microsoft.AspNetCore.SignalR;
namespace Application.Hubs
{
    public class ReviewHub : Hub
    {
        public async Task SendReview(ReviewViewModel review)
        {
            await Clients.All.SendAsync("ReceiveReview", review);
        }
    }
}
