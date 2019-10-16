using Newtonsoft.Json;
using SimpleServer.Models;
using SimpleServer.WebSocketManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleServer.WebSocketServices
{
    public class AudioConnection : AuthorizedConnection
    {
        public AudioConnection(WebSocketHandler handler) : base(handler)
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
    }
}
