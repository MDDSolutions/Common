using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MDDNetComm
{
    public class SourceTracker
    {
        public Guid ApplicationID { get; set; }
        public IPEndPoint IPEndPoint { get; set; }
        //public TcpClient TcpClient { get; set; }
        public string ApplicationName { get; set; }
        public string SourceMachine { get; set; }
        public TimeSpan HeartbeatInterval { get; set; }
        public long LastMessageID { get; set; }
        public long MessageReceiveCount { get; set; }
        public DateTime LastMessageReceiveTime { get; set; }
        public TimeSpan MaxLagTime { get { if (lagarray.Where(x => x != TimeSpan.Zero).Any()) return lagarray.Where(x => x != TimeSpan.Zero).Max(); else return TimeSpan.Zero; } }
        public TimeSpan MinLagTime { get { if (lagarray.Where(x => x != TimeSpan.Zero).Any()) return lagarray.Where(x => x != TimeSpan.Zero).Min(); else return TimeSpan.Zero; } }
        public TimeSpan AvgLagTime 
        { 
            get 
            {
                int count = 0;
                double totalms = 0;
                foreach (var item in lagarray.Where(x => x != TimeSpan.Zero))
                {
                    if (item != null)
                    {
                        totalms += item.TotalMilliseconds;
                        count += 1;
                    }
                }
                if (count == 0)
                    return TimeSpan.Zero;
                else
                    return TimeSpan.FromMilliseconds(totalms / count);
            } 
        }
        public int GapCount { get; set; }
        public DateTime FirstMessageTime { get; set; }
        public override string ToString()
        {
            return String.Format("Machine: {8}, App ID: {0}, Message Count: {1}, Gap Count: {2}, Last MessageID: {3}, Max Lag: {4:0.0}, Min Lag: {5:0.0}, Avg Lag: {6:0.0}, Msg per minute: {7:0.00}",
                    ApplicationID,
                    MessageReceiveCount,
                    GapCount,
                    LastMessageID,
                    MaxLagTime.TotalMilliseconds,
                    MinLagTime.TotalMilliseconds,
                    AvgLagTime.TotalMilliseconds,
                    MessageReceiveCount / (DateTime.Now - FirstMessageTime).TotalMinutes,
                    SourceMachine);
        }
        public virtual string Description { get { return $"{SourceMachine} - {ApplicationName}"; } }
        public DateTime Clock
        {
            get
            {
                return DateTime.Now - Offset;
            }
        }
        public TimeSpan Offset
        {
            get
            {
                var min = MinLagTime;
                var max = MaxLagTime;
                if (min < TimeSpan.Zero && max < TimeSpan.Zero)
                    return max;
                return min;
            }

        }


        private TimeSpan[] lagarray = new TimeSpan[20];


        public static event EventHandler ListUpdated;
        public static event EventHandler<SourceTracker> ItemAdded;
        private static Dictionary<Guid, SourceTracker> allitems = new Dictionary<Guid, SourceTracker>();
        public static SourceTracker Find(Guid inApplicationID)
        {
            if (allitems.ContainsKey(inApplicationID))
                return allitems[inApplicationID];
            return null;
        }
        public static SourceTracker Find(string inSourceMachine)
        {
            return allitems.Values.Where(x => x.SourceMachine.Equals(inSourceMachine,StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        public static SourceTracker FindByAppName(string inAppName)
        {
            return allitems.Values.Where(x => x.ApplicationName == inAppName).FirstOrDefault();
        }
        public static SourceTracker Process(IPEndPoint SourceEndPoint, CommMessage cm)
        {
            if (cm.SourceApplicationID != NetComm.ApplicationID)
            {
                var Lag = (cm.MessageReceiveTime - cm.MessageSentTime);

                if (!allitems.ContainsKey(cm.SourceApplicationID))
                {
                    var st = SourceTracker.Find(cm.SourceMachine);
                    if (st != null && st.ApplicationID == Guid.Empty && st.ApplicationName == cm.SourceApplicationName)
                    {
                        allitems.Remove(Guid.Empty);
                        st.ApplicationID = cm.SourceApplicationID;
                        allitems.Add(st.ApplicationID, st);
                    }
                }
                if (allitems.ContainsKey(cm.SourceApplicationID))
                {
                    SourceTracker st = allitems[cm.SourceApplicationID];
                    if (cm is ShutdownCommMessage)
                    {
                        allitems.Remove(cm.SourceApplicationID);
                        ListUpdated?.Invoke(null, null);
                        return st;
                    }
                    st.lagarray[cm.MessageID % 20] = Lag;
                    if (st.LastMessageID != cm.MessageID - 1)
                    {
                        //NetComm.Log(String.Format("Message gap for Remote End Point: {0} - last message id: {1}, current message id: {2}",
                        //    cm.SourceApplicationID.ToString(),
                        //    st.LastMessageID,
                        //    cm.MessageID));
                        st.GapCount++;
                    }
                    try
                    {
                        if (cm is HeartbeatCommMessage)
                        {
                            var hb = cm as HeartbeatCommMessage;
                            if (st.IPEndPoint.Port != hb.TcpListenerPort)
                                st.IPEndPoint.Port = hb.TcpListenerPort;
                            if (st.HeartbeatInterval != hb.HeartbeatInterval)
                                st.HeartbeatInterval = hb.HeartbeatInterval;
                        }
                        st.LastMessageID = cm.MessageID;
                        st.LastMessageReceiveTime = cm.MessageReceiveTime;
                        st.MessageReceiveCount++;
                    }
                    catch (Exception ex)
                    {
                        NetComm.Log(ex.ToString());
                    }
                    return st;
                }
                else if (!(cm is ShutdownCommMessage))
                {
                    NetComm.Log(String.Format("Received first message from Remote End Point: {0} - message id: {1}",
                        cm.SourceApplicationID.ToString(),
                        cm.MessageID));

                    SourceTracker st;
                    st = CreateSourceTracker(SourceEndPoint, cm, Lag);

                    return st;
                }
            }
            return null;
        }

        public static SourceTracker CreateSourceTracker(IPEndPoint SourceEndPoint, CommMessage cm, TimeSpan Lag)
        {
            SourceTracker st;
            if (NewSourceTrackerMethod == null)
                st = new SourceTracker();
            else
                st = NewSourceTrackerMethod();

            st.ApplicationID = cm.SourceApplicationID;
            st.ApplicationName = cm.SourceApplicationName;
            st.SourceMachine = cm.SourceMachine;
            st.IPEndPoint = SourceEndPoint;
            st.LastMessageID = cm.MessageID;
            st.MessageReceiveCount = 1;
            st.GapCount = 0;
            st.FirstMessageTime = cm.MessageReceiveTime;
            st.LastMessageReceiveTime = cm.MessageReceiveTime;
            st.lagarray[cm.MessageID % 20] = Lag;
            allitems.Add(cm.SourceApplicationID, st);
            ItemAdded?.Invoke(null, st);
            ListUpdated?.Invoke(null, null);

            if (cm is HeartbeatCommMessage)
            {
                st.IPEndPoint.Port = (cm as HeartbeatCommMessage).TcpListenerPort;
                st.HeartbeatInterval = (cm as HeartbeatCommMessage).HeartbeatInterval;
            }
            else
            {   //The first message would generally be a Heartbeat, but if it's not
                //set the interval to a reasonably high number so it doesn't just presume
                //it's dead and delete it right away
                st.HeartbeatInterval = TimeSpan.FromSeconds(30);
            }

            return st;
        }

        public static List<SourceTracker> PresumedDead()
        {
            var pd = new List<SourceTracker>();
            foreach (var item in allitems.Values.ToList())
            {
                if (item.HeartbeatInterval != TimeSpan.Zero && item.LastMessageReceiveTime + item.HeartbeatInterval + item.HeartbeatInterval + item.HeartbeatInterval < DateTime.Now)
                {
                    pd.Add(item);
                    NetComm.Log($"Presumed Dead: {item.ToString()}");
                    allitems.Remove(item.ApplicationID);
                    ListUpdated?.Invoke(null, null);
                }
            }
            return pd;
        }
        public static void Delete(SourceTracker st)
        {
            NetComm.Log($"Explicitly Deleting: {st.ToString()}");
            allitems.Remove(st.ApplicationID);
            ListUpdated?.Invoke(null, null);
        }
        public static string Report()
        {
            StringBuilder sb = new StringBuilder("Source Tracker Report:");
            sb.Append("\r\n");
            foreach (SourceTracker st in allitems.Values)
            {
                sb.Append(st.ToString());
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        public static List<SourceTracker> List()
        {
            return allitems.Values.ToList();
        }
        public static CustomSourceTrackerDelegate NewSourceTrackerMethod = null;
        public static bool Any()
        {
            return allitems.Any();
        }
    }
}
