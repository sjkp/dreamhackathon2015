using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;

namespace BackendWeb.Controllers
{
    public class Scoreboard
    {
        private readonly static Lazy<Scoreboard> _instance = new Lazy<Scoreboard>(() => new Scoreboard(GlobalHost.ConnectionManager.GetHubContext<ScoreboardHub>().Clients));
        const string server = "9cg8pi.messaging.internetofthings.ibmcloud.com";

        private IEnumerable<int> Beattimes = new Loadbeat("PrayerInCBeat.txt").GetBeatTimestamps();

        Timer timer = new Timer();

        public Dictionary<string, Score> Users = new Dictionary<string, Score>();
        public Dictionary<string, List<int>> UserBeats = new Dictionary<string, List<int>>();

        Regex r = new Regex("iot-2/type/phone/id/simped/evt/(.+?)/fmt/json");
        private DateTime StartTime;
        public static Scoreboard Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        private Scoreboard(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            MqttClient client = new MqttClient(server);
            // register to message received 
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            string apikey = "a-9cg8pi-zsskmsjm9e";
            string clientId = "a:9cg8pi:" + Guid.NewGuid(); //a:org_id:app_id
            string authkey = "apKBS&VA&dCKHCbzBZ";

            client.Connect(clientId, apikey, authkey);
            //client.

            Console.WriteLine(client.IsConnected);

            // subscribe to the topic "/home/temperature" with QoS 2 
            client.Subscribe(new string[] { "iot-2/type/phone/id/simped/evt/+/fmt/json" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            client.ConnectionClosed += client_ConnectionClosed;

            timer.Interval = 1;
            timer.Elapsed += timer_Elapsed;
        }

        public void Start()
        {
            this.StartTime = DateTime.UtcNow;
            timer.Start();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
        }

        private void client_ConnectionClosed(object sender, EventArgs e)
        {
            Trace.WriteLine(TraceLevel.Information, "Connection Closed");
        }

        private void client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {

            var m = r.Match(e.Topic);
            var username = m.Groups[1].Value;
            var message = System.Text.Encoding.UTF8.GetString(e.Message);
            var data = JsonConvert.DeserializeObject<data>(message);
            // handle message received 
            Console.WriteLine(username + " " + message);
            
            Score count = new Score();
            if (Users.TryGetValue(username, out count))
            {
                Users[username].Last = data.MeanSquareAcc;
                Users[username].Amount++;
                if (data.MeanSquareAcc > count.Best)
                {
                    Users[username].Best = data.MeanSquareAcc;                    
                }
                Clients.All.addPow();
            }
            else
            {
                Users.Add(username, new Score()
                {
                    Best = data.MeanSquareAcc,
                    Last = data.MeanSquareAcc,
                    Amount = 1
                });
                Clients.All.addPow();
            }
            List<int> userBeats = new List<int>();
            if (UserBeats.TryGetValue(username, out userBeats))
            {
                UserBeats[username].Add((int)(DateTime.UtcNow - StartTime).TotalMilliseconds);

            }
            else
            {
                userBeats = new List<int>(); 
                userBeats.Add((int)(DateTime.UtcNow - StartTime).TotalMilliseconds);
                UserBeats.Add(username, userBeats);
            }
            
            UpdateScoreboard();
        }

        private void UpdateScoreboard()
        {
            //Clients.All.updateUserBeats(UserBeats); //Disable this.
            Clients.All.updateScoreboard(Users.Select(s => new {name = s.Key, mcap = Math.Round(s.Value.Best,2), last = Math.Round(s.Value.Last,2), amount = s.Value.Amount}).OrderByDescending(s => s.mcap).ToList());          
        }
    }


    class data
    {
        public double ax {get;set;}
        public double ay {get;set;}
        public double az {get;set;}
        public double oa {get;set;}
        public double ob {get;set;}
        public double og {get;set;}

        public double MeanSquareAcc {
            get
            {
                return Math.Sqrt(Math.Pow(oa, 2) + Math.Pow(ob, 2) + Math.Pow(og, 2));
            }
        }
    
    }
}