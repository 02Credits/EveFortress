using EveFortressModel;
using Lidgren.Network;
using System;
using System.Collections.Generic;

namespace EveFortressServer
{
    public class PlayerManager : IDisposeNeeded
    {
        public Dictionary<string, Player> Players { get; set; }

        public Dictionary<string, NetConnection> Connections { get; set; }

        public Dictionary<NetConnection, string> ConnectionNames { get; set; }

        public PlayerManager()
        {
            Players = Utils.SerializationUtils.DeserializeFileOrValue("Players.bin", new Dictionary<string, Player>());
            Connections = new Dictionary<string, NetConnection>();
            ConnectionNames = new Dictionary<NetConnection, string>();
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
                            Console.WriteLine(loginInfo.UserName + " has logged in");
                        }
                        else
                        {
                            loginInfo.LoginResponse = LoginResponse.AlreadyLoggedIn;
                            Console.WriteLine("User tried to login to already connected player " + loginInfo.UserName);
                        }
                    }
                    else
                    {
                        loginInfo.LoginResponse = LoginResponse.PasswordWrong;
                        Console.WriteLine("User tried to login with the wrong password to " + loginInfo.UserName);
                    }
                }
                else
                {
                    loginInfo.LoginResponse = LoginResponse.UserDoesNotExist;
                    Console.WriteLine("User tried to login with non-existent account " + loginInfo.UserName);
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
                Console.WriteLine("New user " + loginInfo.UserName);
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