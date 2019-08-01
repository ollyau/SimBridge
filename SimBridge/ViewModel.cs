using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimBridge
{
    class ViewModel : ObservableObject
    {
        ISimulator Simulator;
        MessageProvider MessageSource;

        private string _messageStatus;
        public string MessageStatus
        {
            get { return _messageStatus; }
            private set { SetField(ref _messageStatus, value); }
        }

        private string _simulatorStatus;
        public string SimulatorStatus
        {
            get { return _simulatorStatus; }
            private set { SetField(ref _simulatorStatus, value); }
        }

        public ViewModel()
        {
            var cfg = new Settings("settings.json");

            //Simulator = new MockSimulatorImpl();
            Simulator = new FlightSimulator();
            if (Simulator is FlightSimulator fs)
            {
                SimulatorStatus = "Not connected to flight simulator.  Please restart the application to attempt connecting again.";
                fs.PropertyChanged += (s, e) => {
                    if (fs.Connected)
                    {
                        SimulatorStatus = $"Connected to {fs.SimulatorName}.";
                    }
                    else
                    {
                        SimulatorStatus = "Disconnected from flight simulator.  Please restart the application to connect again.";
                    }
                };

                fs.Initialize();
            }

            MessageSource = new MQTTMessageProvider(Simulator);
            if (MessageSource is MQTTMessageProvider mqtt)
            {
                mqtt.PropertyChanged += (s, e) => {
                    if (e.PropertyName == "State")
                    {
                        switch (mqtt.State)
                        {
                            case MQTTMessageState.Offline:
                                MessageStatus = "MQTT connection offline.";
                                break;
                            case MQTTMessageState.Pairing:
                                MessageStatus = $"Tell Alexa, \"ask Virtual Copilot to pair with {NNumber.Phonetic(mqtt.PairingKey)}.\"";
                                break;
                            case MQTTMessageState.Listening:
                                MessageStatus = "Listening for commands.";
                                break;
                        }
                    }
                };

                Task.Run(() => mqtt.Initialize(cfg));
            }
        }
    }
}
