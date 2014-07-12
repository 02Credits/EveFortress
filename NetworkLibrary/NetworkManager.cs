using EveFortressModel;
using Lidgren.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace NetworkLibrary
{
    public abstract class NetworkManager : IUpdateNeeded, IDisposeNeeded
    {
        public NetPeer LidgrenPeer;

        IDManager AckIDManager = new IDManager();
        IDManager ConvoIDManager = new IDManager();

        Dictionary<long, Tuple<DateTime, NetConnection, NetOutgoingMessage>> nonAckedMessages = new Dictionary<long, Tuple<DateTime, NetConnection, NetOutgoingMessage>>();
        Dictionary<long, Action<NetIncomingMessage>> callbackActions = new Dictionary<long, Action<NetIncomingMessage>>();

        ConcurrentQueue<NetIncomingMessage> Messages = new ConcurrentQueue<NetIncomingMessage>();

        Dictionary<NetConnection, List<long>> SeenAckIDs = new Dictionary<NetConnection, List<long>>();
        Dictionary<long, Action<NetIncomingMessage>> ConversationSubscriptions = new Dictionary<long, Action<NetIncomingMessage>>();

        public abstract byte[] ParseMessage(string commandName, NetIncomingMessage message);
        public virtual void ConnectionDisconnected(NetConnection connection) { }

        public NetworkManager()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (LidgrenPeer != null)
                    {
                        NetIncomingMessage msg = LidgrenPeer.ReadMessage();
                        if (msg != null)
                        {
                            Messages.Enqueue(msg);
                            continue;
                        }
                        Thread.Sleep(20);
                    }
                }
            });
        }

        public virtual void Update()
        {
            NetIncomingMessage msg;
            while (Messages.TryDequeue(out msg))
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        var ackID = msg.ReadInt64();
                        ParseOrHandleAck(msg, ackID);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        HandleStatusChange(msg);
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(msg.ReadString());
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }
                LidgrenPeer.Recycle(msg);
            }

            ResendMessages();
        }

        private void ResendMessages()
        {
            foreach (var id in nonAckedMessages.Keys.ToList())
            {
                var data = nonAckedMessages[id];
                var sentTime = data.Item1;
                var recipient = data.Item2;
                var nonAckedMessage = data.Item3;

                if ((DateTime.Now - sentTime).TotalSeconds > 1)
                {
                    if (nonAckedMessage.LengthBits != 0)
                    {
                        var ackMessage = LidgrenPeer.CreateMessage();
                        ackMessage.Write(id);
                        ackMessage.Write(nonAckedMessage.PeekDataBuffer());
                        LidgrenPeer.SendMessage(ackMessage, recipient, NetDeliveryMethod.Unreliable);
                        nonAckedMessages[id] = Tuple.Create(DateTime.Now, recipient, nonAckedMessage);
                    }
                    else
                    {
                        nonAckedMessages.Remove(id);
                    }
                }
            }
        }

        private void HandleStatusChange(NetIncomingMessage msg)
        {
            var status = (NetConnectionStatus)msg.ReadByte();
            switch (status)
            {
                case NetConnectionStatus.Disconnected:
                    ConnectionDisconnected(msg.SenderConnection);
                    break;
            }
        }

        private void ParseOrHandleAck(NetIncomingMessage msg, long ackID)
        {
            var command = msg.ReadString();
            if (command == "ack")
            {
                nonAckedMessages.Remove(ackID);
            }
            else
            {
                var ackMessage = LidgrenPeer.CreateMessage();
                ackMessage.Write(ackID);
                ackMessage.Write("ack");
                LidgrenPeer.SendMessage(ackMessage, msg.SenderConnection, NetDeliveryMethod.Unreliable);
                if (!SeenAckIDs.ContainsKey(msg.SenderConnection))
                {
                    SeenAckIDs[msg.SenderConnection] = new List<long>();
                }

                if (!SeenAckIDs[msg.SenderConnection].Contains(ackID))
                {
                    SeenAckIDs[msg.SenderConnection].Add(ackID);
                    var convoID = msg.ReadInt64();
                    if (command == "response")
                    {
                        ConversationSubscriptions[convoID](msg);
                    }
                    else
                    {
                        var data = ParseMessage(command, msg);
                        var responseMessage = GetAckedMessage(msg.SenderConnection);
                        responseMessage.Write("response");
                        responseMessage.Write(convoID);
                        responseMessage.Write(data.Length);
                        responseMessage.Write(data);
                        LidgrenPeer.SendMessage(responseMessage, msg.SenderConnection, NetDeliveryMethod.Unreliable);
                    }
                }
            }
        }

        public NetOutgoingMessage GetAckedMessage(NetConnection target)
        {
            var msg = LidgrenPeer.CreateMessage();
            var ackID = AckIDManager.GetNextID();
            msg.Write(ackID);
            nonAckedMessages[ackID] = Tuple.Create(DateTime.Now, target, msg);
            return msg;
        }

        public Task<R> SubscribeToResponse<R>(long conversationID)
        {
            var completionSource = new TaskCompletionSource<R>();
            ConversationSubscriptions[conversationID] = (msg) =>
            {
                var dataLength = msg.ReadInt32();
                if (dataLength > 0)
                {
                    var data = msg.ReadBytes(dataLength);
                    var obj = SerializationUtils.Deserialize<R>(data);
                    completionSource.SetResult(obj);
                }
                else
                {
                    completionSource.SetResult(default(R));
                }
            };
            return completionSource.Task;
        }

        public void WriteMessageParam<T1>(NetOutgoingMessage msg, T1 param)
        {
            var data = SerializationUtils.Serialize(param);
            msg.Write(data.Length);
            msg.Write(data);
        }

        public long WriteCommand(NetOutgoingMessage msg, string commandName)
        {
            msg.Write(commandName);
            var convoID = ConvoIDManager.GetNextID();
            msg.Write(convoID);
            return convoID;
        }

        public Task<R> SendCommand<R>(NetConnection connection, string commandName)
        {
            var msg = GetAckedMessage(connection);
            var id = WriteCommand(msg, commandName);
            LidgrenPeer.SendMessage(msg, connection, NetDeliveryMethod.Unreliable);
            return SubscribeToResponse<R>(id);
        }

        public long WriteCommand<T1>(NetOutgoingMessage msg, string commandName, T1 param1)
        {
            var id = WriteCommand(msg, commandName);
            WriteMessageParam(msg, param1);
            return id;
        }

        public Task<R> SendCommand<R, T1>(NetConnection connection, string commandName, T1 param1)
        {
            var msg = GetAckedMessage(connection);
            var id = WriteCommand(msg, commandName, param1);
            LidgrenPeer.SendMessage(msg, connection, NetDeliveryMethod.Unreliable);
            return SubscribeToResponse<R>(id);
        }

        public long WriteCommand<T1, T2>(NetOutgoingMessage msg, string commandName, T1 param1, T2 param2)
        {
            var id = WriteCommand(msg, commandName, param1);
            WriteMessageParam(msg, param2);
            return id;
        }

        public Task<R> SendCommand<R, T1, T2>(NetConnection connection, string commandName, T1 param1, T2 param2)
        {
            var msg = GetAckedMessage(connection);
            var id = WriteCommand(msg, commandName, param1, param2);
            LidgrenPeer.SendMessage(msg, connection, NetDeliveryMethod.Unreliable);
            return SubscribeToResponse<R>(id);
        }

        public long WriteCommand<T1, T2, T3>(NetOutgoingMessage msg, string commandName, T1 param1, T2 param2, T3 param3)
        {
            var id = WriteCommand(msg, commandName, param1, param2);
            WriteMessageParam(msg, param3);
            return id;
        }

        public Task<R> SendCommand<R, T1, T2, T3>(NetConnection connection, string commandName, T1 param1, T2 param2, T3 param3)
        {
            var msg = GetAckedMessage(connection);
            var id = WriteCommand(msg, commandName, param1, param2, param3);
            LidgrenPeer.SendMessage(msg, connection, NetDeliveryMethod.Unreliable);
            return SubscribeToResponse<R>(id);
        }

        public long WriteCommand<T1, T2, T3, T4>(NetOutgoingMessage msg, string commandName, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var id = WriteCommand(msg, commandName, param1, param2, param3);
            WriteMessageParam(msg, param4);
            return id;
        }

        public Task<R> SendCommand<R, T1, T2, T3, T4>(NetConnection connection, string commandName, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var msg = GetAckedMessage(connection);
            var id = WriteCommand(msg, commandName, param1, param2, param3, param4);
            LidgrenPeer.SendMessage(msg, connection, NetDeliveryMethod.Unreliable);
            return SubscribeToResponse<R>(id);
        }

        public long WriteCommand<T1, T2, T3, T4, T5>(NetOutgoingMessage msg, string commandName, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            var id = WriteCommand(msg, commandName, param1, param2, param3, param4);
            WriteMessageParam(msg, param5);
            return id;
        }

        public Task<R> SendCommand<R, T1, T2, T3, T4, T5>(NetConnection connection, string commandName, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            var msg = GetAckedMessage(connection);
            var id = WriteCommand(msg, commandName, param1, param2, param3, param4, param5);
            LidgrenPeer.SendMessage(msg, connection, NetDeliveryMethod.Unreliable);
            return SubscribeToResponse<R>(id);
        }
        public byte[] ExecuteMethodFromMessage(NetIncomingMessage message, Action method)
        {
            method();
            return new byte[] { };
        }

        public byte[] ExecuteMethodFromMessage<R>(NetIncomingMessage message, Func<R> method)
        {
            var returnVal = method();
            return SerializationUtils.Serialize(returnVal);
        }

        public T ParseParameter<T>(NetIncomingMessage message)
        {
            var paramDataLength = message.ReadInt32();
            var paramData = message.ReadBytes(paramDataLength);
            return SerializationUtils.Deserialize<T>(paramData);
        }

        public byte[] ExecuteMethodFromMessage<T1>(NetIncomingMessage message, Action<T1> method)
        {
            var param1 = ParseParameter<T1>(message);

            method(param1);
            return new byte[] { };
        }

        public byte[] ExecuteMethodFromMessage<T1, R>(NetIncomingMessage message, Func<T1, R> method)
        {
            var param1 = ParseParameter<T1>(message);

            var returnVal = method(param1);
            return SerializationUtils.Serialize(returnVal);
        }

        public byte[] ExecuteMethodFromMessage<T1, T2>(NetIncomingMessage message, Action<T1, T2> method)
        {
            var param1 = ParseParameter<T1>(message);
            var param2 = ParseParameter<T2>(message);

            method(param1, param2);
            return new byte[] { };
        }

        public byte[] ExecuteMethodFromMessage<T1, T2, R>(NetIncomingMessage message, Func<T1, T2, R> method)
        {
            var param1 = ParseParameter<T1>(message);
            var param2 = ParseParameter<T2>(message);

            var returnVal = method(param1, param2);
            return SerializationUtils.Serialize(returnVal);
        }

        public byte[] ExecuteMethodFromMessage<T1, T2, T3>(NetIncomingMessage message, Action<T1, T2, T3> method)
        {
            var param1 = ParseParameter<T1>(message);
            var param2 = ParseParameter<T2>(message);
            var param3 = ParseParameter<T3>(message);

            method(param1, param2, param3);
            return new byte[] { };
        }

        public byte[] ExecuteMethodFromMessage<T1, T2, T3, R>(NetIncomingMessage message, Func<T1, T2, T3, R> method)
        {
            var param1 = ParseParameter<T1>(message);
            var param2 = ParseParameter<T2>(message);
            var param3 = ParseParameter<T3>(message);

            var returnVal = method(param1, param2, param3);
            return SerializationUtils.Serialize(returnVal);
        }

        public byte[] ExecuteMethodFromMessage<T1, T2, T3, T4>(NetIncomingMessage message, Action<T1, T2, T3, T4> method)
        {
            var param1 = ParseParameter<T1>(message);
            var param2 = ParseParameter<T2>(message);
            var param3 = ParseParameter<T3>(message);
            var param4 = ParseParameter<T4>(message);

            method(param1, param2, param3, param4);
            return new byte[] { };
        }

        public byte[] ExecuteMethodFromMessage<T1, T2, T3, T4, R>(NetIncomingMessage message, Func<T1, T2, T3, T4, R> method)
        {
            var param1 = ParseParameter<T1>(message);
            var param2 = ParseParameter<T2>(message);
            var param3 = ParseParameter<T3>(message);
            var param4 = ParseParameter<T4>(message);

            var returnVal = method(param1, param2, param3, param4);
            return SerializationUtils.Serialize(returnVal);
        }

        public byte[] ExecuteMethodFromMessage<T1, T2, T3, T4, T5>(NetIncomingMessage message, Action<T1, T2, T3, T4, T5> method)
        {
            var param1 = ParseParameter<T1>(message);
            var param2 = ParseParameter<T2>(message);
            var param3 = ParseParameter<T3>(message);
            var param4 = ParseParameter<T4>(message);
            var param5 = ParseParameter<T5>(message);

            method(param1, param2, param3, param4, param5);
            return new byte[] { };
        }

        public byte[] ExecuteMethodFromMessage<T1, T2, T3, T4, T5, R>(NetIncomingMessage message, Func<T1, T2, T3, T4, T5, R> method)
        {
            var param1 = ParseParameter<T1>(message);
            var param2 = ParseParameter<T2>(message);
            var param3 = ParseParameter<T3>(message);
            var param4 = ParseParameter<T4>(message);
            var param5 = ParseParameter<T5>(message);

            var returnVal = method(param1, param2, param3, param4, param5);
            return SerializationUtils.Serialize(returnVal);
        }

        public void Dispose()
        {
            foreach (var connection in LidgrenPeer.Connections)
            {
                connection.Disconnect("Disconnected by User");
            }
        }
    }
}

