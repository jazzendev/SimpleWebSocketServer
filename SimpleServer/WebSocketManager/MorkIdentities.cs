using SimpleServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleServer.WebSocketManager
{
    public class MorkIdentities
    {
        public static List<WebSocketIdentity> Identities;
        static MorkIdentities()
        {
            Identities = new List<WebSocketIdentity>();
            Identities.Add(new WebSocketIdentity() { Id = "1", Username = "Jazzen" });
            Identities.Add(new WebSocketIdentity() { Id = "2", Username = "Bin" });
            Identities.Add(new WebSocketIdentity() { Id = "3", Username = "Richard" });
            Identities.Add(new WebSocketIdentity() { Id = "4", Username = "John" });
            Identities.Add(new WebSocketIdentity() { Id = "5", Username = "Somebody" });
        }
    }
}
