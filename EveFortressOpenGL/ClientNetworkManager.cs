using Lidgren.Network;
using NetworkLibrary;
using System.Net;
using System.Threading.Tasks;

namespace EveFortressClient
{
    public class ClientNetworkManager : NetworkManager
    {
        private const bool DEBUG = true;

        public ClientNetworkManager()
        {
            Game.Updateables.Add(this);
            Game.Disposables.Add(this);
            var config = new NetPeerConfiguration("EveFortress");
            LidgrenPeer = new NetPeer(config);
            LidgrenPeer.Start();
        }

        public bool Connected
        {
            get
            {
                if (LidgrenPeer.Connections.Count == 1)
                {
                    var serverStatus = LidgrenPeer.Connections[0].Status;
                    if (serverStatus == NetConnectionStatus.Connected ||
                        serverStatus == NetConnectionStatus.InitiatedConnect ||
                        serverStatus == NetConnectionStatus.None)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool connecting;

        private async void Connect()
        {
            if (!connecting)
            {
                connecting = true;
                await Task.Run(() =>
                {
                    if (!Connected)
                    {
                        try
                        {
                            if (DEBUG)
                            {
                                LidgrenPeer.Connect("localhost", 19952);
                            }
                            else
                            {
                                var address = Dns.GetHostAddresses("the-simmons.dnsalias.net")[0];
                                LidgrenPeer.Connect(new IPEndPoint(address, 19952));
                            }
                        }
                        catch (NetException)
                        {
                        }
                    }
                });
                connecting = false;
            }
        }

        public override void Update()
        {
            if (Connected)
            {
                base.Update();
            }
            else
            {
                if (LidgrenPeer.Connections.Count < 1 ||
                    LidgrenPeer.Connections[0].Status != NetConnectionStatus.InitiatedConnect ||
                    LidgrenPeer.Connections[0].Status != NetConnectionStatus.RespondedConnect)
                {
                    Game.QueueReset();
                    Game.TabManager.MainSection.ReplaceTab(new ConnectingTab());
                    Connect();
                }
            }
        }

        public Task<R> SendCommand<R>(string commandName)
        {
            return SendCommand<R>(LidgrenPeer.Connections[0], commandName);
        }

        public Task<R> SendCommand<R, T1>(string commandName, T1 param1)
        {
            return SendCommand<R, T1>(LidgrenPeer.Connections[0], commandName, param1);
        }

        public Task<R> SendCommand<R, T1, T2>(string commandName, T1 param1, T2 param2)
        {
            return SendCommand<R, T1, T2>(LidgrenPeer.Connections[0], commandName, param1, param2);
        }

        public Task<R> SendCommand<R, T1, T2, T3>(string commandName, T1 param1, T2 param2, T3 param3)
        {
            return SendCommand<R, T1, T2, T3>(LidgrenPeer.Connections[0], commandName, param1, param2, param3);
        }

        public Task<R> SendCommand<R, T1, T2, T3, T4>(string commandName, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return SendCommand<R, T1, T2, T3, T4>(LidgrenPeer.Connections[0], commandName, param1, param2, param3, param4);
        }

        public Task<R> SendCommand<R, T1, T2, T3, T4, T5>(string commandName, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return SendCommand<R, T1, T2, T3, T4, T5>(LidgrenPeer.Connections[0], commandName, param1, param2, param3, param4, param5);
        }

        public override byte[] ParseMessage(string commandName, NetIncomingMessage message)
        {
            return Game.MessageParser.ParseMessage(commandName, message);
        }
    }
}