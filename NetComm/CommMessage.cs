using System;
using System.IO;

namespace MDDNetComm
{
    [Serializable]
    public class CommMessage
    {
        public long MessageID { get; set; }
        //public long ReferencedMessageID { get; set; }
        //public bool RequireAcknowledgement { get; set; }
        public DateTime MessageSentTime { get; set; }
        public DateTime MessageReceiveTime { get; set; }
        public Guid SourceApplicationID { get; set; }
        public string SourceApplicationName { get; set; }
        public String SourceMachine { get; set; }
        //public CommMessageType MessageType { get; set; }
        public Guid? TargetID { get; set; }
        public override string ToString()
        {
            if (MessageReceiveTime != DateTime.MinValue)
                return $"Received: {MessageReceiveTime.ToString("HH:mm:ss.FFF")} Lag: {(MessageReceiveTime - MessageSentTime).TotalMilliseconds} {ToStringBasic()}";
            if (MessageSentTime != DateTime.MinValue)
                return $"Sent: {MessageSentTime.ToString("HH:mm:ss.FFF")} {ToStringBasic()}";
            return ToStringBasic();
        }
        public virtual string ToStringBasic()
        {
            return $"ID: {MessageID}  Type: {this.GetType().Name} SourceID: {SourceApplicationID}";
        }
    }
    [Serializable]
    public class GetResponseCommMessage : CommMessage
    {

    }
    [Serializable]
    public class TcpHeartbeatCommMessage : GetResponseCommMessage
    {
        public TcpHeartbeatCommMessage()
        {
            HeartbeatInterval = NetComm.HeartbeatInterval;
        }
        public TimeSpan HeartbeatInterval { get; set; }
    }
    [Serializable]
    public class LargeMessageFragmentCommMessage : CommMessage
    {
        public Guid FragmentedMessageID { get; set; }
        public int MessageIndex { get; set; }
        public int FragmentCount { get; set; }
        public byte[] Data { get; set; }
        public override string ToStringBasic()
        {
            return $"ID: {MessageID} Type: {this.GetType().Name} [{MessageIndex}/{FragmentCount}] SourceID: {SourceApplicationID}";
        }
    }
    public class LargeMessage
    {
        public Guid ID { get; set; }
        public LargeMessageFragmentCommMessage[] MessageFragments { get; set; }
        public bool IsComplete
        {
            get
            {
                for (int i = 0; i < MessageFragments.Length; i++)
                    if (MessageFragments[i] == null) return false;
                return true;
            }
        }
        public void Add(LargeMessageFragmentCommMessage inmsg)
        {
            if (ID == inmsg.FragmentedMessageID)
                MessageFragments[inmsg.MessageIndex] = inmsg;
            else
                throw new Exception("Invalid FragmentedMessageID");
        }
        public CommMessage Assemble()
        {
            using (var stream = new MemoryStream())
            {
                stream.Capacity = (MessageFragments.Length - 1) * MessageFragments[0].Data.Length + MessageFragments[MessageFragments.Length - 1].Data.Length;
                for (int i = 0; i < MessageFragments.Length; i++)
                    stream.Write(MessageFragments[i].Data, 0, MessageFragments[i].Data.Length);
                stream.Position = 0;
                return (CommMessage)NetComm.bf.Deserialize(stream);
            }
        }
        public LargeMessage(LargeMessageFragmentCommMessage inmsg)
        {
            ID = inmsg.FragmentedMessageID;
            MessageFragments = new LargeMessageFragmentCommMessage[inmsg.FragmentCount];
            Add(inmsg);
        }
    }
    [Serializable]
    public class AckCommMessage : CommMessage
    {
        public long AckMessageID { get; set; }
    }
    [Serializable]
    public class RequireAckCommMessage : CommMessage
    {
        public AckCommMessage Acknowledgement
        {
            get
            {
                return new AckCommMessage
                {
                    AckMessageID = MessageID
                };
            }
        }
    }
    [Serializable]
    public class HeartbeatCommMessage : CommMessage
    {
        public HeartbeatCommMessage()
        {
            HeartbeatInterval = NetComm.HeartbeatInterval;
            TcpListenerPort = NetComm.TcpPortNumber;
        }
        public TimeSpan HeartbeatInterval { get; set; }
        public int TcpListenerPort { get; set; }
    }
    [Serializable]
    public class ShutdownCommMessage : CommMessage
    {

    }
    [Serializable]
    public class ErrorCommMessage : CommMessage
    {
        public string ErrorMessage { get; set; }
    }
}
