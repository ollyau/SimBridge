using BeatlesBlog.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimBridge
{
    abstract class SimConnectClient
    {
        protected readonly string ApplicationName;
        protected SimConnect Client;

        public SimConnectClient(string ApplicationName)
        {
            this.ApplicationName = ApplicationName;
            Client = new SimConnect();
        }

        ~SimConnectClient() { }

        //-----------------------------------------------------------------------------

        public void Initialize()
        {
            PreInitialize();
            OpenConnection();
            PostInitialize();
        }

        public void Uninitialize()
        {
            PreUninitialize();
            CloseConnection();
            PostUninitialize();
        }

        //-----------------------------------------------------------------------------

        protected virtual void PreInitialize() { }
        protected virtual void PostInitialize() { }

        protected virtual void PreUninitialize() { }
        protected virtual void PostUninitialize() { }

        //-----------------------------------------------------------------------------

        private static bool IsLocalRunning
        {
            get { return LookupDefaultPortNumber("SimConnect_Port_IPv4") != 0 || LookupDefaultPortNumber("SimConnect_Port_IPv6") != 0; }
        }

        private static int LookupDefaultPortNumber(string ValueName)
        {
            string[] simulators = {
                                      @"HKEY_CURRENT_USER\Software\Microsoft\Microsoft Games\Flight Simulator",
                                      @"HKEY_CURRENT_USER\Software\Microsoft\Microsoft ESP",
                                      @"HKEY_CURRENT_USER\Software\LockheedMartin\Prepar3D",
                                      @"HKEY_CURRENT_USER\Software\Lockheed Martin\Prepar3D v2",
                                      @"HKEY_CURRENT_USER\Software\Lockheed Martin\Prepar3D v3",
                                      @"HKEY_CURRENT_USER\Software\Lockheed Martin\Prepar3D v4",
                                      @"HKEY_CURRENT_USER\Software\Microsoft\Microsoft Games\Flight Simulator - Steam Edition"
                                  };
            foreach (var sim in simulators)
            {
                var value = (string)Microsoft.Win32.Registry.GetValue(sim, ValueName, null);
                if (!string.IsNullOrEmpty(value))
                {
                    var port = int.Parse(value);
                    if (port != 0) { return port; }
                }
            }
            return 0;
        }

        //-----------------------------------------------------------------------------

        private void OpenConnection()
        {
            if (IsLocalRunning)
            {
                try
                {
                    Log.Instance.Info("Opening SimConnect connection.");
                    Client.Open(ApplicationName);
                }
                catch (SimConnect.SimConnectException ex)
                {
                    Log.Instance.Warning(string.Format("Local connection failed.\r\n{0}", ex.ToString()));
                    try
                    {
                        bool ipv6support = System.Net.Sockets.Socket.OSSupportsIPv6;
                        Log.Instance.Info($"Opening SimConnect connection ({(ipv6support ? "IPv6" : "IPv4")}).");
                        int scPort = LookupDefaultPortNumber(ipv6support ? "SimConnect_Port_IPv6" : "SimConnect_Port_IPv4");
                        if (scPort == 0) { throw new SimConnect.SimConnectException("Invalid port."); }
                        Client.Open(ApplicationName, null, scPort, ipv6support);
                    }
                    catch (SimConnect.SimConnectException innerEx)
                    {
                        Log.Instance.Error(string.Format("Local connection failed.\r\n{0}", innerEx.ToString()));
                    }
                }
            }
            else
            {
                Log.Instance.Warning("Flight Simulator must be running in order to connect to SimConnect.");
            }
        }

        private void CloseConnection()
        {
            Client.Close();
        }

        //-----------------------------------------------------------------------------

        // https://www.fsdeveloper.com/forum/threads/converting-adf-frequency.21264/post-444535
        // https://www.fsdeveloper.com/wiki/index.php?title=C:_Decimal_to_BCD
        protected ulong ToBCD(ulong input)
        {
            ulong a = 0UL;
            ulong result = 0UL;
            for (a = 0; input != 0; a++)
            {
                result |= (input % 10) << (int)(a * 4);
                input /= 10;
            }
            return result;
        }

        // https://www.fsdeveloper.com/wiki/index.php?title=C:_Decimal_to_BCO
        protected uint ToBCO(int xpndr)
        {
            return (uint)((xpndr % 10) + (((xpndr / 10) % 10) * 16) + (((xpndr / 100) % 10) * 256) + ((xpndr / 1000) * 4096));
        }

        // https://www.fsdeveloper.com/forum/threads/get-xpndr-code-in-external-app.8759/post-57481
        protected int FromBCO(uint xpndr)
        {            
            return (int)((((xpndr & 0xf000) >> 12) * 1000) + (((xpndr & 0x0f00) >> 8) * 100) + (((xpndr & 0x00f0) >> 4) * 10) + (xpndr & 0x000f));
        }

        //-----------------------------------------------------------------------------
    }
}
