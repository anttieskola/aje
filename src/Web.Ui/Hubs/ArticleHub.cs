using AJE.Application.Commands;

namespace AJE.Web.Ui.Hubs
{
    public class ArticleHub : Hub
    {
        private readonly ISender _sender;
        public ArticleHub(ISender sender)
        {
            _sender = sender;
        }

        public async Task SendMessage(string user, string message)
        {
            var sendEvent = await _sender.Send(new SendChatMessageCommand { User = user, Message = message });
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
