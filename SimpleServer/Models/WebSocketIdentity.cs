using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleServer.Models
{
    public class WebSocketIdentity
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public override bool Equals(object obj)
        {
            return (obj as WebSocketIdentity).Id == Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
