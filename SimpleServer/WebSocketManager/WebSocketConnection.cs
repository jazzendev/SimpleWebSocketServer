﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleServer.WebSocketManager
{
    public abstract class WebSocketConnection
    {
        public WebSocketHandler Handler { get; }

        public WebSocket WebSocket { get; set; }
        public DateTime LastLiveTime { get; set; }

        public WebSocketConnection(WebSocketHandler handler)
        {
            Handler = handler;
            LastLiveTime = new DateTime(1970, 1, 1);
        }

        public virtual async Task SendTextAsync(string message)
        {
            if (WebSocket.State != WebSocketState.Open) return;
            var arr = Encoding.UTF8.GetBytes(message);

            var buffer = new ArraySegment<byte>(
                    array: arr,
                    offset: 0,
                    count: arr.Length);

            await WebSocket.SendAsync(
                buffer: buffer,
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None
                );
        }

        public virtual async Task SendBinaryAsync(byte[] buffer)
        {
            await WebSocket.SendAsync(
                buffer: buffer,
                messageType: WebSocketMessageType.Binary,
                endOfMessage: true,
                cancellationToken: CancellationToken.None
                );
        }

        public abstract Task ReceiveTextAsync(string message);
        public abstract Task ReceiveBinaryAsync(byte[] buffer);
    }
}
