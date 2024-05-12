using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MDDNetComm
{
    public static partial class NetComm
    {
        internal static void Log(string LogStr)
        {
            DirectoryInfo logfiledir = (new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)).Directory;
            string CurLogFile = Path.Combine(logfiledir.FullName, "NetCommLog.txt");
            bool Finished = false;
            bool WriteHeader = !File.Exists(CurLogFile) || LogStr == "";
            while (!Finished)
            {
                try
                {
                    using (StreamWriter writer = File.AppendText(CurLogFile))
                    {
                        if (WriteHeader)
                            writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF") + " -- NetComm Log File - Build Time: " + BuildTime().ToString("yyyy-MM-dd H:mm"));
                        if (LogStr != "")
                            writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF") + " -- " + LogStr);
                    }
                    Finished = true;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("because it is being used by another process"))
                        Thread.Sleep(50);
                    else
                    {
                        Finished = true;
                        throw;
                    }
                }

            }
        }
        public static DateTime BuildTime()
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildtime = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            if (buildtime.IsDaylightSavingTime())
                buildtime = buildtime.AddHours(1);
            return buildtime;
        }
        //private static Dictionary<Guid, LargeMessageFragmentCommMessage[]> LargeMessageReceiver = new Dictionary<Guid, LargeMessageFragmentCommMessage[]>();
        //private static Queue<Tuple<IPEndPoint,LargeMessageFragmentCommMessage>> MessageSendQueue = new Queue<Tuple<IPEndPoint,LargeMessageFragmentCommMessage>>();

    }
}
