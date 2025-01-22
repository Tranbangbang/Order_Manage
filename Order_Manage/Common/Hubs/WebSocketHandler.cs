using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Order_Manage.Models;
using Order_Manage.Repository;

namespace Order_Manage.Common.Hubs
{
    public class WebSocketHandler
    {
        private static readonly List<WebSocket> _sockets = new List<WebSocket>();
        private readonly IMessageRepository _messageRepository;

        public WebSocketHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task Handle(HttpContext context, WebSocket webSocket, string userId)
        {
            _sockets.Add(webSocket);

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            try
            {
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    var message = JsonConvert.DeserializeObject<Message>(messageJson);
                    if (message == null) continue;

                    Console.WriteLine($"Received from {message.SenderId} to {message.ReceiverId}: {message.Content}");
                    try
                    {
                        _messageRepository.SaveMessageAsync(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to save message: {ex.Message}");
                    }
                    await BroadcastMessage(messageJson);
                }
                while (!result.CloseStatus.HasValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WebSocketHandler: {ex.Message}");
            }
        }


        private static async Task BroadcastMessage(string message)
        {
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var tasks = _sockets.Where(s => s.State == WebSocketState.Open)
                                .Select(socket => socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None));

            await Task.WhenAll(tasks);
        }
    }
}
