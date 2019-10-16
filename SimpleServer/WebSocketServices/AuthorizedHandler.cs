using Microsoft.AspNetCore.Http;
using SimpleServer.Models;
using SimpleServer.WebSocketManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleServer.WebSocketServices
{
    public class AuthorizedHandler<T> : WebSocketHandler where T : AuthorizedConnection
    {
        protected override int BufferSize { get => 1024 * 4; }

        private WebSocketIdentity VerifyIdentity(HttpContext context)
        {
            // TODO: Authorize Token, fetch client id
            var token = context.Request.Query["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var id = token;
            var identity = MorkIdentities.Identities.FirstOrDefault(i => i.Id == id);
            return identity;
        }

        protected async virtual Task<T> GenerateConnectionAsync(HttpContext context, WebSocketIdentity identity)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            return new AuthorizedConnection(this)
            {
                Identity = identity,
                WebSocket = webSocket
            } as T;
        }

        public override async Task<WebSocketConnection> OnConnected(HttpContext context)
        {
            var identity = VerifyIdentity(context);

            if (identity != null)
            {
                var connection = Connections.FirstOrDefault(m => ((T)m).Identity == identity);

                if (connection != null)
                {
                    await OnDisconnected(connection);
                }

                connection = await GenerateConnectionAsync(context, identity);

                Connections.Add(connection);

                return connection;
            }

            return null;
        }
    }
}
