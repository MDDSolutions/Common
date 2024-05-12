using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MDDNetComm
{
    public delegate TcpHeartbeatCommMessage CustomHeartBeatDelegate();
    public delegate Task<CommMessage> ProvideResponseDelegate(SourceTracker FromTracker, GetResponseCommMessage inmsg);
    public delegate SourceTracker CustomSourceTrackerDelegate();
    public static partial class NetComm
    {
        #region Public Properties and fields
        public static bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (value)
                {
                    StartListener();
                    tmrMain.Change(TimerInterval, TimerInterval);
                }
                else
                {
                    tmrMain.Change(Timeout.Infinite, 0);
                    if (UdpHeartbeats)
                    {
                        SendUdpMessage(new ShutdownCommMessage());
                        UdpServer.Close();
                    }
                    else
                        foreach (var item in SourceTracker.List())
                        {
                            if (item.ApplicationID != Guid.Empty)
                                SendMessage(item, new ShutdownCommMessage());
                        }
                    Log("Listener stopping");
                }
                _enabled = value;
            }
        }
        public static Guid ApplicationID = Guid.NewGuid();
        public static bool UdpHeartbeats { get; set; }
        public static bool TcpHeartbeats { get; set; }

        public static string ApplicationName { get; set; }
        public static TimeSpan HeartbeatInterval { get; set; }
        //public static int TcpPortNumber = ;
        public static int TcpPortNumber = 0;
        public static DateTime LastHeartbeat = DateTime.MinValue;
        public static CustomHeartBeatDelegate CustomHeartBeatMethod = null;
        public static ProvideResponseDelegate ProvideResponseMethod = null;
        public static int TimerInterval
        {
            get
            {
                return _timerinterval;
            }
            set
            {
                if (Enabled)
                {
                    tmrMain.Change(value, value);
                }
                _timerinterval = value;
            }
        }
        public static Dictionary<long, CommMessage> PendingRemoteAcknowledgement = new Dictionary<long, CommMessage>();
        public static Dictionary<long, CommMessage> PendingLocalAcknowledgement = new Dictionary<long, CommMessage>();
        public static Dictionary<string, int> ReceiveMessageError = new Dictionary<string, int>();
        public static string ReceiveMessageErrorReport
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in NetComm.ReceiveMessageError)
                {
                    sb.Append($"{item.Key}: {item.Value}\r\n");
                }
                return sb.ToString();
            }
        }
        internal static BinaryFormatter bf = new BinaryFormatter();
        public static int ReadTimeout = 5000;
        public static bool Busy
        {
            get
            {
                return _busy;
            }
            set
            {
                _busy = value;
                LastActivity = DateTime.Now;
            }
        }
        private static DateTime LastActivity = DateTime.MaxValue;
        #endregion
        #region Events
        public static event EventHandler<CommMessage> MessageReceived;
        public static event EventHandler<CommMessage> MessageAcknowledged;
        public static event EventHandler<bool> SourceTrackerDead;
        public static event EventHandler ReceiveMessageErrorEvent;
        public static event EventHandler<Exception> NetCommException;
        public static event EventHandler<string> NetCommMessage;
        #endregion
        #region Public Methods
        public static CommMessage SendUdpMessage(CommMessage msg)
        {
            if (!Enabled) throw new Exception("NetComm is not enabled");
            LastActivity = DateTime.Now;
            PrepareMessage(msg);

            using (MemoryStream stream = new MemoryStream())
            {
                msg.MessageSentTime = DateTime.Now;
                bf.Serialize(stream, msg);
                byte[] msgbytes = stream.ToArray();
                UdpPublisher.Send(msgbytes, msgbytes.Length);
                //if (System.Diagnostics.Debugger.IsAttached)
                //    NetComm.Log(String.Format("Sent: {0}",msg.ToString()));
            }
            return msg;
        }
        public static CommMessage PrepareMessage(CommMessage msg)
        {
            SentMessageID++;
            msg.MessageID = SentMessageID;
            msg.SourceApplicationID = ApplicationID;
            msg.SourceApplicationName = ApplicationName;
            msg.SourceMachine = Environment.MachineName;

            if (msg is RequireAckCommMessage)
                PendingRemoteAcknowledgement.Add(msg.MessageID, msg);
            if (msg is AckCommMessage && PendingLocalAcknowledgement.ContainsKey((msg as AckCommMessage).AckMessageID))
                PendingLocalAcknowledgement.Remove((msg as AckCommMessage).AckMessageID);
            return msg;
        }
        private static bool tcpCommBusy = false;
        public static CommMessage SendMessage(SourceTracker tracker, CommMessage msg)
        {
            if (!Enabled) throw new Exception("NetComm is not enabled");
            LastActivity = DateTime.Now;
            DateTime start = DateTime.Now;
            int waits = 0;
            while (tcpCommBusy)
            {
                waits++;
                Thread.Sleep(50);
                if (waits % 10 == 0) NetCommMessage?.Invoke(null,$"Waiting for comms to be available... {waits}");
            }
            try
            {
                tcpCommBusy = true;

                //if (true || tracker.TcpClient == null || !tracker.TcpClient.Connected)
                //{
                //    tracker.TcpClient = new TcpClient(tracker.IPEndPoint.AddressFamily);
                //    tracker.TcpClient.Connect(tracker.IPEndPoint);
                //}


                var client = new TcpClient(tracker.IPEndPoint.AddressFamily);
                client.Connect(tracker.IPEndPoint);
                var netstream = client.GetStream();
                TcpMessageSend(msg, client, netstream);
                client.Close();
                return msg;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                tcpCommBusy = false;
            }
        }
        public static CommMessage GetResponse(SourceTracker tracker, GetResponseCommMessage msg)
        {
            if (!Enabled) throw new Exception("NetComm is not enabled");
            LastActivity = DateTime.Now;
            DateTime start = DateTime.Now;
            int waits = 0;
            while (tcpCommBusy)
            {
                waits++;
                Thread.Sleep(50);
                if (waits % 10 == 0) NetCommMessage?.Invoke(null, $"Waiting for comms to be available... {waits}");
            }
            try
            {
                tcpCommBusy = true;

                //if (true || tracker.TcpClient == null || !tracker.TcpClient.Connected)
                //{
                //    tracker.TcpClient = new TcpClient(tracker.IPEndPoint.AddressFamily);
                //    tracker.TcpClient.Connect(tracker.IPEndPoint);
                //}

                var client = new TcpClient(tracker.IPEndPoint.AddressFamily);
                client.Connect(tracker.IPEndPoint);

                var netstream = client.GetStream();
                TcpMessageSend(msg, client, netstream);
                var response = TcpMessageRecieve(client, netstream);
                client.Close();
                if (response == null)
                {
                    Log($"GetResponse returned a blank result for Message: {msg.ToString()}");
                    return null;
                }
                return response.Item2;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                tcpCommBusy = false;
            }

        }
        public static SourceTracker TcpConnect(string HostName)
        {
            var st = SourceTracker.Find(HostName);
            if (st == null)
            {
                var ping = new Ping();
                var options = new PingOptions();
                options.DontFragment = true;
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1000;
                PingReply reply = ping.Send(HostName, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    //Console.WriteLine("Address: {0}", reply.Address.ToString());
                    //Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                    //Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                    //Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                    //Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);

                    System.Net.IPEndPoint ep = new System.Net.IPEndPoint(reply.Address, 50282);
                    st = SourceTracker.CreateSourceTracker(
                        ep,
                        new HeartbeatCommMessage
                        {
                            SourceApplicationID = Guid.Empty,
                            SourceApplicationName = NetComm.ApplicationName,
                            SourceMachine = HostName,
                            MessageID = 0,
                            MessageReceiveTime = DateTime.Now,
                            TcpListenerPort = ep.Port,
                            HeartbeatInterval = NetComm.HeartbeatInterval
                        },
                        TimeSpan.FromSeconds(10)
                        );

                    var msg = new HeartbeatCommMessage();
                    NetComm.SendMessage(st, msg);
                }
                else
                {
                    throw new Exception("Ping was not successful");
                }
            }
            return st;
        }
        public static void TcpDisconnect(string HostName)
        {
            var st = SourceTracker.Find(HostName);
            if (st != null)
            {
                LastHeartbeat = DateTime.Now;
                var msg = new ShutdownCommMessage();
                NetComm.SendMessage(st, msg);
                SourceTracker.Delete(st);
                SourceTrackerDead?.Invoke(st, false);
            }
        }
        //public static void SynchronizedInvoke(this ISynchronizeInvoke sync, Action action)
        //{
        //    if (!sync.InvokeRequired)
        //    {
        //        action();
        //        return;
        //    }
        //    sync.Invoke(action, new object[] { });
        //}
        #endregion
        #region Private Methods
        private static void StartListener()
        {
            Log($"Listener starting - Application Name: {ApplicationName} ID: {ApplicationID}");

            TcpServer = new TcpListener(IPAddress.IPv6Any, TcpPortNumber);
            TcpServer.Server.DualMode = true;
            TcpServer.Start();
            TcpServer.BeginAcceptTcpClient(TcpServerCallback, TcpServer);
            var ep = (IPEndPoint)TcpServer.LocalEndpoint;
            TcpPortNumber = ep.Port;

            //var host = Dns.GetHostEntry(Dns.GetHostName());
            //foreach (var ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        Console.WriteLine(ip.ToString());
            //    }
            //}

            //string localIP;
            //using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            //{
            //    socket.Connect("8.8.8.8", 65530);
            //    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            //    localIP = endPoint.Address.ToString();
            //}
            //NetCommMessage?.Invoke(null, $"TCP Listener: {localIP}:{ep.Port}");


            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((item.NetworkInterfaceType == NetworkInterfaceType.Ethernet || item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            NetCommMessage?.Invoke(null, $"{item.NetworkInterfaceType.ToString()} - {item.Description} - {ip.Address.ToString()}:{ep.Port}");
                        }
                    }
                }
            }

            if (UdpHeartbeats)
            {
                UdpServer = new UdpClient();
                IPEndPoint UdpEndPoint = new IPEndPoint(IPAddress.Any, UdpPortNumber);
                UdpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                UdpServer.ExclusiveAddressUse = false;
                UdpServer.Client.Bind(UdpEndPoint);
                IPAddress multicastaddr = IPAddress.Parse(UpdIPAddress);
                UdpServer.JoinMulticastGroup(multicastaddr);
                UdpServer.BeginReceive(UdpServerCallback, UdpServer);
            }
        }
        private static void UdpServerCallback(IAsyncResult ar)
        {
            bool RunAgain = true;
            try
            {
                IPEndPoint remoteEndPoint = null;
                byte[] inbuffer = UdpServer.EndReceive(ar, ref remoteEndPoint);
                ReceiveQueue.Enqueue(new Tuple<DateTime, IPEndPoint, byte[]>(DateTime.Now, remoteEndPoint, inbuffer));
            }
            catch (ObjectDisposedException)
            {
                RunAgain = false;
                Log("Listener has successfully shut down");
            }
            catch (Exception ex)
            {
               Log(ex.ToString());
            }

            if (RunAgain)
                try
                {
                    UdpServer.BeginReceive(UdpServerCallback, UdpServer);
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                    NetCommException?.Invoke(null, ex);
                }
        }
        private static async void TcpServerCallback(IAsyncResult ar)
        {
            bool RunAgain = true;
            try
            {
                TcpClient client = TcpServer.EndAcceptTcpClient(ar);
                var netstream = client.GetStream();
                var response = TcpMessageRecieve(client, netstream);
                if (response.Item2 != null)
                {
                    if (response.Item2 is GetResponseCommMessage)
                    {
                        if (ProvideResponseMethod == null)
                        {
                            TcpMessageSend(new ErrorCommMessage { ErrorMessage = "No GetResponseMethod" }, client, netstream);
                        }
                        else
                        {
                            var responsemsg = await ProvideResponseMethod(response.Item1, response.Item2 as GetResponseCommMessage).ConfigureAwait(false);
                            TcpMessageSend(responsemsg, client, netstream);
                        }
                    }
                    else
                        MessageReceived?.Invoke(response.Item1, response.Item2);
                }
                client.Close();
            }
            catch (ObjectDisposedException)
            {
                RunAgain = false;
                Log("Listener has successfully shut down");
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                NetCommException?.Invoke(null, ex);
            }

            if (RunAgain)
                try
                {
                    TcpServer.BeginAcceptTcpClient(TcpServerCallback, TcpServer);
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                    NetCommException?.Invoke(null, ex);
                }
        }
        private static void TcpMessageSend(CommMessage msg, TcpClient client, NetworkStream netstream)
        {
            PrepareMessage(msg);
            using (MemoryStream stream = new MemoryStream())
            {
                msg.MessageSentTime = DateTime.Now;
                bf.Serialize(stream, msg);

                if (stream.Length > 65000)
                {
                    stream.Position = 0;
                    int MessageCount = Convert.ToInt32(Math.Ceiling(stream.Length / 32768.0));
                    Guid FragmentedMessageID = Guid.NewGuid();
                    for (int i = 0; i < MessageCount; i++)
                    {
                        int Length = 32768;
                        int Start = 32768 * i;
                        if (stream.Length - Start < Length)
                            Length = Convert.ToInt32(stream.Length - Start);
                        byte[] bytes = new byte[Length];
                        var n = stream.Read(bytes, 0, bytes.Length);
                        var lmf = new LargeMessageFragmentCommMessage { FragmentedMessageID = FragmentedMessageID, MessageIndex = i, FragmentCount = MessageCount, Data = bytes };
                        PrepareMessage(lmf);
                        using (var lmfstream = new MemoryStream())
                        {
                            lmf.MessageSentTime = DateTime.Now;
                            bf.Serialize(lmfstream, lmf);
                            byte[] msgbytes = lmfstream.ToArray();
                            netstream.Write(msgbytes, 0, msgbytes.Length);
                        }
                        ProcessTcpAcknowledgement(lmf.MessageID, client, netstream);
                    }
                }
                else
                {
                    byte[] msgbytes = stream.ToArray();
                    netstream.Write(msgbytes, 0, msgbytes.Length);
                    if (!(msg is AckCommMessage) && !(msg is GetResponseCommMessage))
                            ProcessTcpAcknowledgement(msg.MessageID, client, netstream);
                }
            }
        }
        private static void ProcessTcpAcknowledgement(long MessageID, TcpClient client, NetworkStream netstream)
        {
            var ack = TcpMessageRecieve(client, netstream);
            if (ack == null)
            {
                Log("Expecting acknowledgement - received no response");
            }
            else if (ack.Item2 != null)
            {
                if (ack.Item2 is AckCommMessage)
                {
                    if ((ack.Item2 as AckCommMessage).AckMessageID != MessageID)
                    {
                        throw new Exception("acknowleged message id not what was sent");
                    }
                }
                else
                {
                    Log($"Expecting Acknowledgement - received '{ack.ToString()}'");
                }
            }
        }
        private static Tuple<SourceTracker,CommMessage> TcpMessageRecieve(TcpClient client, NetworkStream netstream)
        {
            bool messagecomplete = false;
            LargeMessage lm = null;
            CommMessage final = null;
            byte[] inbuffer = null;
            SourceTracker st = null;
            while (!messagecomplete)
            {
                if (netstream.CanRead)
                {
                    inbuffer = new byte[client.ReceiveBufferSize];
                    netstream.ReadTimeout = ReadTimeout;
                    try
                    {
                        netstream.Read(inbuffer, 0, client.ReceiveBufferSize);
                    }
                    catch (Exception ex)
                    {
                        Log(ex.ToString());
                        NetCommException?.Invoke(null, ex);
                        return null;
                    }
                    
                    var resp = ReceiveMessage(DateTime.Now, (IPEndPoint)client.Client.RemoteEndPoint, inbuffer);
                    st = resp.Item1;
                    if (resp.Item2 != null)
                    {
                        if (resp.Item2 is LargeMessageFragmentCommMessage)
                        {
                            if (lm == null)
                                lm = new LargeMessage(resp.Item2 as LargeMessageFragmentCommMessage);
                            else
                                lm.Add(resp.Item2 as LargeMessageFragmentCommMessage);
                            if (lm.IsComplete)
                            {
                                final = lm.Assemble();
                                messagecomplete = true;
                            }
                        }
                        else
                        {
                            final = resp.Item2;
                            messagecomplete = true;
                        }

                        if (!(resp.Item2 is AckCommMessage) && !(resp.Item2 is GetResponseCommMessage))
                        {
                            TcpMessageSend(new AckCommMessage { AckMessageID = resp.Item2.MessageID }, client, netstream);
                        }
                    }
                    else
                    {
                        messagecomplete = true;
                    }
                    //var ack = BitConverter.GetBytes(cm.MessageID);
                    //netstream.Write(ack, 0, ack.Length);
                }
            }
            if (final == null)
            {
                string errormsg = Encoding.ASCII.GetString(inbuffer);
                throw new Exception($"Unexpected response: {errormsg}");
            }
            return new Tuple<SourceTracker, CommMessage>(st, final);
        }
        private static Tuple<SourceTracker,CommMessage> ReceiveMessage(DateTime ReceiveTime, IPEndPoint SourceEndPoint, byte[] inbuffer)
        {
            CommMessage cm = null;
            SourceTracker st = null;
            using (MemoryStream s = new MemoryStream(inbuffer))
            {
                try
                {
                    cm = (CommMessage)bf.Deserialize(s);
                }
                catch (Exception ex)
                {
                    LastReceiveMessageError = DateTime.Now;
                    if (ReceiveMessageError.ContainsKey(ex.Message))
                        ReceiveMessageError[ex.Message]++;
                    else
                        ReceiveMessageError.Add(ex.Message, 1);
                    NetCommException?.Invoke(null, ex);
                }
                if (cm != null)
                {
                    cm.MessageReceiveTime = ReceiveTime;
                    st = SourceTracker.Process(SourceEndPoint, cm);
                    if (cm is AckCommMessage)
                    {
                        if (PendingRemoteAcknowledgement.ContainsKey((cm as AckCommMessage).AckMessageID))
                            PendingRemoteAcknowledgement.Remove((cm as AckCommMessage).AckMessageID);
                        MessageAcknowledged?.Invoke(st, cm);
                    }
                    else if (cm is ShutdownCommMessage)
                    {
                        SourceTrackerDead?.Invoke(st, false);
                    }
                    else
                    {
                        if (cm is RequireAckCommMessage)
                            PendingLocalAcknowledgement.Add(cm.MessageID, cm);
                    }

                }
                else
                {
                    string errormsg = Encoding.ASCII.GetString(inbuffer);
                    Log($"Unexpected message: {errormsg}");

                }
            }
            return new Tuple<SourceTracker, CommMessage>(st, cm);
        }
        private static void Timer_Elapsed(object sender)
        {
            try
            {
                tmrMain.Change(Timeout.Infinite, 0);
                if (Busy && LastActivity.AddSeconds(5) < DateTime.Now) Busy = false;
                while (ReceiveQueue.Count > 0)
                {
                    var qitem = ReceiveQueue.Dequeue();
                    if (qitem != null)
                    {
                        var resp = ReceiveMessage(qitem.Item1, qitem.Item2, qitem.Item3);
                        if (resp.Item2 != null)
                        {
                            if (resp.Item1 != null && resp.Item2.SourceApplicationID != ApplicationID && (resp.Item2.TargetID == null || resp.Item2.TargetID == ApplicationID))
                            MessageReceived?.Invoke(resp.Item1, resp.Item2);
                        }
                    }
                }
                if (HeartbeatInterval != null && LastHeartbeat + HeartbeatInterval < DateTime.Now && (UdpHeartbeats || (TcpHeartbeats && SourceTracker.Any())))
                {
                    if (UdpHeartbeats) SendUdpMessage(new HeartbeatCommMessage());
                    if (TcpHeartbeats)
                    {
                        TcpHeartbeatCommMessage msg = null;
                        if (CustomHeartBeatMethod != null)
                            msg = CustomHeartBeatMethod();
                        else
                            msg = new TcpHeartbeatCommMessage();

                        foreach (var item in SourceTracker.List())
                        {
                            //Console.WriteLine($"{DateTime.Now.ToString("h:mm:ss")} heartbeat");
                            var resp = GetResponse(item, msg);
                            MessageReceived?.Invoke(item, resp);
                        }
                    }
                    LastHeartbeat = DateTime.Now;
                }
                if (LastPresumedDeadCheck + TimeSpan.FromSeconds(5) < DateTime.Now)
                {
                    LastPresumedDeadCheck = DateTime.Now;
                    foreach (var item in SourceTracker.PresumedDead())
                    {
                        SourceTrackerDead?.Invoke(item, true);
                    }
                }
                if (LastReceiveMessageErrorReport < DateTime.Now.Subtract(ReceiveMessageReportInterval) && LastReceiveMessageError >= DateTime.Now.Subtract(ReceiveMessageReportInterval))
                {
                    ReceiveMessageErrorEvent?.Invoke(null, null);
                    LastReceiveMessageErrorReport = DateTime.Now;
                }
                //while (MessageSendQueue.Count != 0)
                //{
                //    var qitem = MessageSendQueue.Dequeue();
                //    SendMessage(qitem.Item1, qitem.Item2);
                //}
                //foreach (var item in LargeMessageReceiver.Keys.ToList())
                //{
                //    bool complete = true;
                //    for (int i = 0; i < LargeMessageReceiver[item].Length; i++)
                //        if (LargeMessageReceiver[item][i] == null) complete = false;
                //    if (complete)
                //    {
                //        using (var stream = new MemoryStream())
                //        {
                //            int offset = 0;
                //            for (int i = 0; i < LargeMessageReceiver[item].Length; i++)
                //            {
                //                stream.Write(LargeMessageReceiver[item][i].Data, offset, LargeMessageReceiver[item][i].Data.Length);
                //                offset = offset + LargeMessageReceiver[item][i].Data.Length;
                //            }
                //            var cm = (CommMessage)bf.Deserialize(stream);
                //            var st = SourceTracker.Find(LargeMessageReceiver[item][0].SourceApplicationID);
                //            MessageReceived?.Invoke(st, cm);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                NetCommException?.Invoke(null, ex);
            }
            finally
            {
                tmrMain.Change(TimerInterval, TimerInterval);
            }
        }
        #endregion
        #region Private Fields
        private static bool _busy = false;
        private static string UpdIPAddress = "230.0.0.1";
        private static int UdpPortNumber = 11050;
        private static UdpClient UdpPublisher = new UdpClient(UpdIPAddress, UdpPortNumber);
        private static Queue<Tuple<DateTime, IPEndPoint, byte[]>> ReceiveQueue = new Queue<Tuple<DateTime, IPEndPoint, byte[]>>();
        private static Timer tmrMain = new Timer(Timer_Elapsed);
        private static bool _enabled = false;
        private static int _timerinterval = 100;
        private static long SentMessageID = 0;
        private static UdpClient UdpServer = null;
        private static TcpListener TcpServer = null;
        private static DateTime LastReceiveMessageError = default;
        private static DateTime LastReceiveMessageErrorReport = default;
        private static TimeSpan ReceiveMessageReportInterval = TimeSpan.FromSeconds(15);
        private static DateTime LastPresumedDeadCheck = default;
        #endregion
    }
}
