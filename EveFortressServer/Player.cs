﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EveFortressModel
{
    [ProtoContract]
    public class Player
    {
        [ProtoMember(1)]
        public string UserName { get; set; }
        [ProtoMember(2)]
        public string Password { get; set; }

        public List<Tuple<long, long>> SubscribedChunks { get; set; }

        public Player() 
        {
            SubscribedChunks = new List<Tuple<long, long>>();
        }

        public Player(string username, string password)
            : this()
        {
            UserName = username;
            Password = password;
        }
    }
}
