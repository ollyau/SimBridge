using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimBridge
{

    //-----------------------------------------------------------------------------

    enum MQTTMessageState
    {
        Offline,
        Pairing,
        Listening
    }

    //-----------------------------------------------------------------------------

    class MQTTMessageProvider : MessageProvider
    {
        public readonly string PairingKey;
        private string UserIdentifier;
        public MQTTMessageState State { get; private set; }

        Settings Configuration;
        IMqttClient MQTTClient;

        public MQTTMessageProvider(ISimulator Simulator) : base(Simulator)
        {
            State = MQTTMessageState.Offline;
            PairingKey = NNumber.Generate();
        }

        public async void Initialize(Settings Configuration)
        {
            this.Configuration = Configuration;

            UserIdentifier = Configuration.Get<string>("user_identifier", null);

            var server = Configuration.Get<string>("mqtt_server");
            var username = Configuration.Get<string>("mqtt_username", null);
            var password = Configuration.Get<string>("mqtt_password", null);

            await Task.Run( () => Initialize(server, username, password) );
        }

        // presently this is only configured to connect to MQTT over secure websockets
        public async void Initialize(string server, string username = null, string password = null)
        {
            // configure client
            var factory = new MqttFactory();
            MQTTClient = factory.CreateMqttClient();

            // load server config -- currently exoects MQTT over secure websockets
            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(server)
                .WithTls();

            // add authentication options if applicable
            if (!string.IsNullOrWhiteSpace(username))
            {
                options = options.WithCredentials(username, password);
            }

            // setup handlers to add after connecting
            MQTTClient.UseConnectedHandler( async e => {
                Log.Instance.Debug("Connected to MQTT server");

                if (!string.IsNullOrWhiteSpace(UserIdentifier))
                {
                    Log.Instance.Info($"Subscribing to events for user identifier {UserIdentifier}");
                    await MQTTClient.SubscribeAsync(new TopicFilterBuilder().WithTopic($"{UserIdentifier}/event").Build());
                    State = MQTTMessageState.Listening;
                }
                else
                {
                    Log.Instance.Info($"No user identifier provided; subscribing to pairing events with pairing key {PairingKey}");
                    await MQTTClient.SubscribeAsync(new TopicFilterBuilder().WithTopic($"pair/{PairingKey}").Build());
                    State = MQTTMessageState.Pairing;
                }
            } );

            // add handler
            MQTTClient.UseApplicationMessageReceivedHandler( e => { OnReceiveMessage(e.ApplicationMessage); } );

            // connect
            await MQTTClient.ConnectAsync(options.Build(), CancellationToken.None);
        }

        void OnReceiveMessage(MqttApplicationMessage e)
        {
            Log.Instance.Debug($"Received MQTT message on topic {e.Topic}");

            // todo: add some validation to ensure the message is proper

            if (e.Topic.StartsWith("pair"))
            {
                // read json to get identifier
                var data = JObject.Parse(Encoding.UTF8.GetString(e.Payload));
                UserIdentifier = data["data"].ToObject<string>();

                // save settings for future use
                Configuration.Set("user_identifier", UserIdentifier);
                Configuration.Save();

                // subscribe to messages
                Log.Instance.Info($"Subscribing to events for user identifier {UserIdentifier}");
                MQTTClient.SubscribeAsync(new TopicFilterBuilder().WithTopic($"{UserIdentifier}/event").Build());
                State = MQTTMessageState.Listening;
            }
            else if (e.Topic.StartsWith(UserIdentifier))
            {
                var data = Encoding.UTF8.GetString(e.Payload);
                RouteMessage(JsonMessage.Parse(data));
            }
            else
            {
                Log.Instance.Warning("Unexpected MQTT message");
            }
        }
    }

    //-----------------------------------------------------------------------------

}
