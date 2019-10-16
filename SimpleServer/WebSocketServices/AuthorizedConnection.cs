using Newtonsoft.Json;
using SimpleServer.Models;
using SimpleServer.WebSocketManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleServer.WebSocketServices
{
    public class AuthorizedConnection : WebSocketConnection
    {
        public WebSocketIdentity Identity { get; set; }

        public AuthorizedConnection(WebSocketHandler handler) : base(handler)
        {
        }

        public override async Task ReceiveBinaryAsync(byte[] buffer)
        {
            foreach (var c in Handler.Connections)
            {
                var a = c as AuthorizedConnection;
                if (a != null && a.Identity != Identity)
                {
                    await a.SendBinaryAsync(buffer);
                }
            }
        }

        public override async Task ReceiveTextAsync(string message)
        {
            var receiveMessage = JsonConvert.DeserializeObject<ReceiveMessage>(message);

            if (string.IsNullOrEmpty(receiveMessage.To))
            {
                foreach (var r in Handler.Connections)
                {
                    if ((r as AuthorizedConnection).Identity == Identity)
                    {
                        continue;
                    }

                    var sendMessage = JsonConvert.SerializeObject(new SendMessage
                    {
                        From = Identity.Id,
                        Msg = receiveMessage.Msg
                    });

                    await r.SendTextAsync(sendMessage);
                }
            }
            else
            {
                var receiver = Handler.Connections.FirstOrDefault(m => ((AuthorizedConnection)m).Identity.Id == receiveMessage.To);

                if (receiver != null)
                {
                    var sendMessage = JsonConvert.SerializeObject(new SendMessage
                    {
                        From = Identity.Id,
                        Msg = receiveMessage.Msg
                    });

                    await receiver.SendTextAsync(sendMessage);
                }
                else
                {
                    var sendMessage = JsonConvert.SerializeObject(new SendMessage
                    {
                        From = Identity.Id,
                        Msg = "Can not find " + receiveMessage.To
                    });

                    await SendTextAsync(sendMessage);
                }
            }
        }

        private class ReceiveMessage
        {
            public string To { get; set; }
            public Object Msg { get; set; }
        }

        private class SendMessage
        {
            public string From { get; set; }
            public Object Msg { get; set; }
        }
    }
}
