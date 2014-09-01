using ProtoBuf;
using System.Collections.Generic;

namespace EveFortressModel
{
    [ProtoContract]
    public class Player
    {
        [ProtoMember(1)]
        public string UserName { get; set; }

        [ProtoMember(2)]
        public string Password { get; set; }

        public List<Point<long>> SubscribedChunks { get; set; }

        public Player()
        {
            SubscribedChunks = new List<Point<long>>();
        }

        public Player(string username, string password)
            : this()
        {
            UserName = username;
            Password = password;
        }
    }
}