using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EveFortressModel;
using Lidgren.Network;
using ProtoBuf;
using System.IO;

namespace EveFortressServer
{
    public class PlayerManager : IDisposeNeeded
    {
        public Dictionary<string, Player> Players { get; set; }
        public Dictionary<string, NetConnection> Connections { get; set; }
        public Dictionary<NetConnection, string> ConnectionNames { get; set; }

        public PlayerManager()
        {
            Players = Utils.SerializationUtils.DeserializeFileOrValue("Players.bin", new Dictionary<string,Player>());
            Connections = new Dictionary<string, NetConnection>();
            ConnectionNames = new Dictionary<NetConnection, string>();

            Program.Disposables.Add(this);
        }

        public LoginInformation LoginAttempt(LoginInformation loginInfo, NetConnection connection)
        {
            if (string.IsNullOrWhiteSpace(loginInfo.UserName))
            {
                loginInfo.LoginResponse = LoginResponse.UsernameEmpty;
            }
            else if (string.IsNullOrWhiteSpace(loginInfo.Password))
            {
                loginInfo.LoginResponse = LoginResponse.PasswordEmpty;
            }
            else
            {
                Player player;
                var userExists = Players.TryGetValue(loginInfo.UserName, out player);
                if (userExists)
                {
                    if (loginInfo.Password == player.Password)
                    {
                        if (!Connections.ContainsKey(loginInfo.UserName))
                        {
                            Connections.Add(loginInfo.UserName, connection);
                            ConnectionNames.Add(connection, loginInfo.UserName);
                            loginInfo.LoginResponse = LoginResponse.Success;
                        }
                        else
                        {
                            loginInfo.LoginResponse = LoginResponse.AlreadyLoggedIn;
                        }
                    }
                    else
                    {
                        loginInfo.LoginResponse = LoginResponse.PasswordWrong;
                    }
                }
                else
                {
                    loginInfo.LoginResponse = LoginResponse.UserDoesNotExist;
                }
            }

            return loginInfo;
        }

        public LoginInformation RegisterUser(LoginInformation loginInfo, NetConnection connection)
        {
            if (Players.ContainsKey(loginInfo.UserName))
            {
                loginInfo.LoginResponse = LoginResponse.AlreadyLoggedIn;
            }
            else if (string.IsNullOrWhiteSpace(loginInfo.UserName))
            {
                loginInfo.LoginResponse = LoginResponse.UsernameEmpty;
            }
            else if (string.IsNullOrWhiteSpace(loginInfo.Password))
            {
                loginInfo.LoginResponse = LoginResponse.PasswordEmpty;
            }
            else
            {
                Players[loginInfo.UserName] = new Player(loginInfo.UserName, loginInfo.Password);
                loginInfo = LoginAttempt(loginInfo, connection);
            }
            return loginInfo;
        }

        public void DisconnectPlayer(NetConnection connection)
        {
            if (ConnectionNames.ContainsKey(connection))
            {
                var name = ConnectionNames[connection];
                Connections.Remove(name);
                ConnectionNames.Remove(connection);
            }
        }

        public void Dispose()
        {
            Utils.SerializationUtils.SerializeToFile("Players.bin", Players);
        }
    }
}
