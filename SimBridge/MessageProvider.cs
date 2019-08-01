using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimBridge
{
    //-----------------------------------------------------------------------------
    
    class Message
    {
        public readonly string Intent;
        public readonly object Data;

        public Message(string Intent, object Data)
        {
            this.Intent = Intent;
            this.Data = Data;
        }
    }

    //-----------------------------------------------------------------------------

    class JsonMessage
    {
        public static Message Parse(JObject json)
        {
            var data = json["data"] != null ? (json["data"].Type == JTokenType.Integer ? json["data"].ToObject<int>() : json["data"].ToObject<object>()) : null;
            return new Message(json["intent"].ToObject<string>(), data);
        }

        public static Message Parse(string json)
        {
            return Parse(JObject.Parse(json));
            //return JsonConvert.DeserializeObject<Message>(json);
        }
    }

    //-----------------------------------------------------------------------------

    abstract class MessageProvider : ObservableObject
    {
        protected ISimulator Simulator;

        protected MessageProvider(ISimulator Simulator)
        {
            this.Simulator = Simulator;
        }

        // derived class should do validation on message prior to calling this
        protected virtual void RouteMessage(Message message)
        {
            Log.Instance.Debug($"Routing Message (Intent = {message.Intent}, Data = {message.Data})");
            var intent = message.Intent.Trim().ToUpperInvariant();

            switch (intent)
            {
                case "PAUSE":
                    Simulator.SetPause((bool)message.Data);
                    break;
                case "SOUND":
                    Simulator.SetSound((bool)message.Data);
                    break;
                case "SCREENSHOT":
                    Simulator.CaptureScreenshot();
                    break;
                case "RESET":
                    Simulator.ResetFlight();
                    break;
                case "QUIT":
                    Simulator.QuitWithoutMessage();
                    break;
                case "RELOAD_AIRCRAFT":
                    Simulator.ReloadAircraft();
                    break;
                case "RELOAD_SCENERY":
                    Simulator.ReloadScenery();
                    break;
                case "VIEW_NEXT":
                    Simulator.NextViewCategory();
                    break;
                case "VIEW_BACK":
                    Simulator.PreviousViewCategory();
                    break;
                case "SUBVIEW_NEXT":
                    Simulator.NextSubView();
                    break;
                case "SUBVIEW_BACK":
                    Simulator.PreviousSubView();
                    break;
                case "VC_VIEW":
                    Simulator.SetVirtualCockpit();
                    break;
                case "XPNDR":
                    Simulator.SetTransponder((int)message.Data);
                    break;
                case "ALTIMETER":
                    Simulator.SetAltimeter((double)message.Data);
                    break;
                case "COM_1_SET":
                    Simulator.SetRadioStandby(RadioType.COM_1, (double)message.Data);
                    break;
                case "COM_1_SWAP":
                    Simulator.SwapRadio(RadioType.COM_1);
                    break;
                case "COM_2_SET":
                    Simulator.SetRadioStandby(RadioType.COM_2, (double)message.Data);
                    break;
                case "COM_2_SWAP":
                    Simulator.SwapRadio(RadioType.COM_2);
                    break;
                case "NAV_1_SET":
                    Simulator.SetRadioStandby(RadioType.NAV_1, (double)message.Data);
                    break;
                case "NAV_1_SWAP":
                    Simulator.SwapRadio(RadioType.NAV_1);
                    break;
                case "NAV_2_SET":
                    Simulator.SetRadioStandby(RadioType.NAV_2, (double)message.Data);
                    break;
                case "NAV_2_SWAP":
                    Simulator.SwapRadio(RadioType.NAV_2);
                    break;
                case "LIGHT_BEACON":
                    Simulator.SetLights(LightType.Beacon, (bool)message.Data);
                    break;
                case "LIGHT_STROBE":
                    Simulator.SetLights(LightType.Strobe, (bool)message.Data);
                    break;
                case "LIGHT_NAV":
                    Simulator.SetLights(LightType.Navigation, (bool)message.Data);
                    break;
                case "LIGHT_LANDING":
                    Simulator.SetLights(LightType.Landing, (bool)message.Data);
                    break;
                case "LIGHT_TAXI":
                    Simulator.SetLights(LightType.Taxi, (bool)message.Data);
                    break;
                case "LIGHT_PANEL":
                    Simulator.SetLights(LightType.Panel, (bool)message.Data);
                    break;
                case "PITOT_HEAT":
                    Simulator.SetPitotHeat((bool)message.Data);
                    break;
                case "FLAPS_UP":
                    Simulator.FlapsUp();
                    break;
                case "FLAPS_RAISE":
                    Simulator.RaiseFlaps();
                    break;
                case "FLAPS_LOWER":
                    Simulator.LowerFlaps();
                    break;
                case "FLAPS_FULL":
                    Simulator.FlapsFull();
                    break;
                case "GEAR":
                    Simulator.SetGear((bool)message.Data);
                    break;
                default:
                    Log.Instance.Warning($"Unknown intent {intent}");
                    break;
            }
        }
    }

    //-----------------------------------------------------------------------------
}
