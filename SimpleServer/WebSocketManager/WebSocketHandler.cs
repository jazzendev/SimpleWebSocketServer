using Microsoft.AspNetCore.Http;
using SimpleServer.WebSocketServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleServer.WebSocketManager
{
    public abstract class WebSocketHandler
    {
        protected abstract int BufferSize { get; }

        private List<WebSocketConnection> _connections = new List<WebSocketConnection>();
        public List<WebSocketConnection> Connections { get => _connections; }

        private TimeSpan PingInterval { get; set; }

        public async Task ListenPing(WebSocketConnection connection)
        {
            //default ping interval in SignalR is 300000 ticks, 5min
            var ticks = 3000;
            PingInterval = new TimeSpan(ticks * TimeSpan.TicksPerMillisecond);
            connection.LastLiveTime = DateTime.UtcNow;

            while (connection.WebSocket.State == WebSocketState.Open)
            {
                var interval = DateTime.UtcNow - connection.LastLiveTime;
                if (interval > PingInterval * 2)
                {
                    // cloase by Timeout
                    await OnDisconnected(connection);
                }

                await connection.SendTextAsync("ping");
                await Task.Delay(ticks);
            }
        }

        public async Task ListenConnection(WebSocketConnection connection)
        {
            var buffer = new byte[BufferSize];
            var offset = 0;
            var free = buffer.Length;

            while (connection.WebSocket.State == WebSocketState.Open)
            {
                var result = await connection.WebSocket.ReceiveAsync(
                    buffer: new ArraySegment<byte>(buffer, offset, free),
                    cancellationToken: CancellationToken.None);

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        if (message.ToLower() == "pong")
                        {
                            connection.LastLiveTime = DateTime.UtcNow;
                        }
                        else
                        {
                            await connection.ReceiveTextAsync(message);
                        }
                        offset = 0;
                        buffer = new byte[BufferSize];
                        break;
                    case WebSocketMessageType.Binary:

                        offset += result.Count;
                        free -= result.Count;

                        if (result.EndOfMessage)
                        {                            
                            await connection.ReceiveBinaryAsync(buffer);
                            offset = 0;
                            buffer = new byte[BufferSize];
                        }

                        if (free == 0)
                        {
                            // No free space
                            // Resize the outgoing buffer
                            var newSize = buffer.Length + BufferSize;
                            // Check if the new size exceeds a limit
                            // It should suit the data it receives
                            // This limit however has a max value of 2 billion bytes (2 GB)
                            //if (newSize > maxFrameSize)
                            //{
                            //    throw new Exception("Maximum size exceeded");
                            //}
                            var newBuffer = new byte[newSize];
                            Array.Copy(buffer, 0, newBuffer, 0, offset);
                            buffer = newBuffer;
                            free = buffer.Length - offset;
                        }
                        
                        break;
                    case WebSocketMessageType.Close:
                        await OnDisconnected(connection);
                        break;
                }
            }
        }

        public virtual async Task OnDisconnected(WebSocketConnection connection)
        {
            if (connection != null)
            {
                _connections.Remove(connection);

                await connection.WebSocket.CloseAsync(
                    closeStatus: WebSocketCloseStatus.NormalClosure,
                    statusDescription: "Closed by the WebSocketHandler",
                    cancellationToken: CancellationToken.None);
            }
        }

        public abstract Task<WebSocketConnection> OnConnected(HttpContext context);
    }
}
