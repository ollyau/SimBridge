using BeatlesBlog.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimBridge
{
    class FlightSimulator : SimConnectClient, ISimulator
    {
        //-----------------------------------------------------------------------------

        enum Requests
        {
            DisplayText
        }

        enum Events
        {
            PAUSE_SET,
            SOUND_SET,
            CAPTURE_SCREENSHOT,
            SITUATION_RESET,
            ABORT,
            RELOAD_USER_AIRCRAFT,
            REFRESH_SCENERY,
            VIEW_MODE,
            NEXT_SUB_VIEW,
            VIEW_MODE_REV,
            PREV_SUB_VIEW,
            VIEW_CAMERA_SELECT_1,
            XPNDR_SET,
            KOHLSMAN_SET,
            PITOT_HEAT_SET,
            COM_STBY_RADIO_SET,
            COM2_STBY_RADIO_SET,
            NAV1_STBY_SET,
            NAV2_STBY_SET,
            COM_STBY_RADIO_SWAP,
            COM2_RADIO_SWAP,
            NAV1_RADIO_SWAP,
            NAV2_RADIO_SWAP,
            LANDING_LIGHTS_SET,
            PANEL_LIGHTS_SET,
            STROBES_SET,
            FLAPS_UP,
            FLAPS_DECR,
            FLAPS_INCR,
            FLAPS_DOWN,
            GEAR_SET
        }

        //-----------------------------------------------------------------------------

        interface ICommand
        {
            void Execute();
        }

        class Event : ICommand
        {
            private readonly SimConnect Client;
            public readonly Events Target;

            public Event(SimConnect Client, Events Target)
            {
                this.Client = Client;
                this.Target = Target;
            }

            public void Execute()
            {
                Log.Instance.Debug($"TransmitClientEventToUser: {Enum.GetName(typeof(Events), Target)}");
                Client.TransmitClientEventToUser(Target, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
            }
        }

        class EventWithData : ICommand
        {
            private readonly SimConnect Client;
            public readonly Events Target;
            public readonly uint Data;

            public EventWithData(SimConnect Client, Events Target, uint Data)
            {
                this.Client = Client;
                this.Target = Target;
                this.Data = Data;
            }

            public void Execute()
            {
                Log.Instance.Debug($"TransmitClientEventToUser: {Enum.GetName(typeof(Events), Target)} -> {Data}");
                Client.TransmitClientEventToUser(Target, Data, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
            }
        }

        //-----------------------------------------------------------------------------

        public FlightSimulator() : base("SimBridge")
        {
            Client.OnRecvOpen += OnRecvOpen;
            Client.OnRecvQuit += OnRecvQuit;
            Client.OnRecvException += OnRecvException;
        }

        private void OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            var simVersion = $"{data.dwApplicationVersionMajor}.{data.dwApplicationVersionMinor}.{data.dwApplicationBuildMajor}.{data.dwApplicationBuildMinor}";
            var scVersion = $"{data.dwSimConnectVersionMajor}.{data.dwSimConnectVersionMinor}.{data.dwSimConnectBuildMajor}.{data.dwSimConnectBuildMinor}";
            Log.Instance.Info($"Connected to {data.szApplicationName}\r\n    Simulator Version:\t{simVersion}\r\n    SimConnect Version:\t{scVersion}");

            // this requires all Events enum entries to match their appropriate SimConnect event ID name
            var eventNames = Enum.GetNames(typeof(Events));
            var eventValues = (Events[])Enum.GetValues(typeof(Events));
            for (var i = 0; i < eventNames.Length; i++)
            {
                Log.Instance.Debug($"Mapping event: {eventNames[i]}");
                Client.MapClientEventToSimEvent(eventValues[i], eventNames[i]);
            }
        }

        private void OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Log.Instance.Info("Flight Simulator disconnected.");
        }

        private void OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            var exceptionName = Enum.GetName(typeof(SIMCONNECT_EXCEPTION), data.dwException);
            var message = $"{exceptionName} (Exception = {data.dwException}, SendID = {data.dwSendID}, Index = {data.dwIndex})";
            Log.Instance.Warning(message);
            Client.Text(SIMCONNECT_TEXT_TYPE.PRINT_WHITE, 10.0f, Requests.DisplayText, $"{ApplicationName} SimConnect Exception: {message}");
        }

        //-----------------------------------------------------------------------------

        public void SetPause(bool paused)
        {
            Client.TransmitClientEventToUser(Events.PAUSE_SET, (uint)(paused ? 1 : 0), SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void SetSound(bool on)
        {
            Client.TransmitClientEventToUser(Events.SOUND_SET, (uint)(on ? 1 : 0), SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void CaptureScreenshot()
        {
            Client.TransmitClientEventToUser(Events.CAPTURE_SCREENSHOT, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void ResetFlight()
        {
            Client.TransmitClientEventToUser(Events.SITUATION_RESET, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void QuitWithoutMessage()
        {
            Client.TransmitClientEventToUser(Events.ABORT, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void ReloadAircraft()
        {
            Client.TransmitClientEventToUser(Events.RELOAD_USER_AIRCRAFT, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void ReloadScenery()
        {
            Client.TransmitClientEventToUser(Events.REFRESH_SCENERY, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void NextViewCategory()
        {
            Client.TransmitClientEventToUser(Events.VIEW_MODE, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void NextSubView()
        {
            Client.TransmitClientEventToUser(Events.NEXT_SUB_VIEW, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void PreviousViewCategory()
        {
            Client.TransmitClientEventToUser(Events.VIEW_MODE_REV, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void PreviousSubView()
        {
            Client.TransmitClientEventToUser(Events.PREV_SUB_VIEW, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void SetVirtualCockpit()
        {
            // VIEW_VIRTUAL_COCKPIT_FORWARD maybe?
            Client.TransmitClientEventToUser(Events.VIEW_CAMERA_SELECT_1, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void SetTransponder(int code)
        {
            if (code > 7777)
            {
                throw new ArgumentOutOfRangeException("Transponder values must be <= 7777");
            }
            Client.TransmitClientEventToUser(Events.XPNDR_SET, (uint)ToBCO(code), SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void SetAltimeter(double inHg)
        {
            // https://www.nist.gov/physical-measurement-laboratory/nist-guide-si-appendix-b8
            var millibars = inHg * 33.86389;
            Client.TransmitClientEventToUser(Events.KOHLSMAN_SET, (uint)(millibars * 16), SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void SetRadioStandby(RadioType radio, double MHz)
        {
            var bcd = ToBCD((uint)(MHz * 100));
            switch (radio)
            {
                case RadioType.COM_1:
                    Client.TransmitClientEventToUser(Events.COM_STBY_RADIO_SET, (uint)bcd, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                case RadioType.COM_2:
                    Client.TransmitClientEventToUser(Events.COM2_STBY_RADIO_SET, (uint)bcd, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                case RadioType.NAV_1:
                    Client.TransmitClientEventToUser(Events.NAV1_STBY_SET, (uint)bcd, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                case RadioType.NAV_2:
                    Client.TransmitClientEventToUser(Events.NAV2_STBY_SET, (uint)bcd, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                default:
                    Log.Instance.Warning("Unsupported radio selected.");
                    break;
            }
        }

        public void SwapRadio(RadioType radio)
        {
            switch (radio)
            {
                case RadioType.COM_1:
                    Client.TransmitClientEventToUser(Events.COM_STBY_RADIO_SWAP, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                case RadioType.COM_2:
                    Client.TransmitClientEventToUser(Events.COM2_RADIO_SWAP, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                case RadioType.NAV_1:
                    Client.TransmitClientEventToUser(Events.NAV1_RADIO_SWAP, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                case RadioType.NAV_2:
                    Client.TransmitClientEventToUser(Events.NAV2_RADIO_SWAP, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                default:
                    Log.Instance.Warning("Unsupported radio swap selected.");
                    break;
            }
        }

        public void SetLights(LightType light, bool on)
        {
            switch (light)
            {
                case LightType.Beacon:
                    throw new NotImplementedException();
                    break;
                case LightType.Strobe:
                    Client.TransmitClientEventToUser(Events.STROBES_SET, (uint)(on ? 1 : 0), SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                case LightType.Navigation:
                    throw new NotImplementedException();
                    break;
                case LightType.Landing:
                    Client.TransmitClientEventToUser(Events.LANDING_LIGHTS_SET, (uint)(on ? 1 : 0), SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                case LightType.Taxi:
                    throw new NotImplementedException();
                    break;
                case LightType.Panel:
                    Client.TransmitClientEventToUser(Events.PANEL_LIGHTS_SET, (uint)(on ? 1 : 0), SIMCONNECT_GROUP_PRIORITY.HIGHEST);
                    break;
                default:
                    var message = "Unsupported light selected.";
                    Log.Instance.Warning(message);
                    Client.Text(SIMCONNECT_TEXT_TYPE.PRINT_WHITE, 10.0f, Requests.DisplayText, message);
                    break;
            }
        }

        public void SetPitotHeat(bool on)
        {
            Client.TransmitClientEventToUser(Events.PITOT_HEAT_SET, (uint)(on ? 1 : 0), SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void FlapsUp()
        {
            Client.TransmitClientEventToUser(Events.FLAPS_UP, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void RaiseFlaps()
        {
            Client.TransmitClientEventToUser(Events.FLAPS_DECR, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void LowerFlaps()
        {
            Client.TransmitClientEventToUser(Events.FLAPS_INCR, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void FlapsFull()
        {
            Client.TransmitClientEventToUser(Events.FLAPS_DOWN, SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        public void SetGear(bool down)
        {
            Client.TransmitClientEventToUser(Events.GEAR_SET, (uint)(down ? 1 : 0), SIMCONNECT_GROUP_PRIORITY.HIGHEST);
        }

        //-----------------------------------------------------------------------------
    }
}
