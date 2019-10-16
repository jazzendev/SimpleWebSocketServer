using Microsoft.AspNetCore.Http;
using SimpleServer.Models;
using SimpleServer.WebSocketManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleServer.WebSocketServices
{
    public class AudioHandler<T> : AuthorizedHandler<T> where T : AudioConnection
    {
        protected async override Task<T> GenerateConnectionAsync(HttpContext context, WebSocketIdentity identity)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            return new AudioConnection(this)
            {
                Identity = identity,
                WebSocket = webSocket
            } as T;
        }
    }
}
