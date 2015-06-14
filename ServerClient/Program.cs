using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ServerClient
{
    class Program
    {
        const string server = "9cg8pi.messaging.internetofthings.ibmcloud.com";
        static MqttClient client = new MqttClient(server);
        static void Main(string[] args)
        {

            // create client instance 
           


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
            Console.Read();
        }

        static void client_ConnectionClosed(object sender, EventArgs e)
        {

            Console.WriteLine("Connection Closed");
        }


        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Regex r = new Regex("iot-2/type/phone/id/simped/evt/(.+?)/fmt/json");
            var m = r.Match(e.Topic);
            var username = m.Groups[1].Value;
            // handle message received 
            Console.WriteLine(username  + " " + System.Text.Encoding.UTF8.GetString(e.Message));
        } 
    }
}
