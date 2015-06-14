(function (window) {
    var ax = 0, ay = 0, az = 0, oa = 0, ob = 0, og = 0;

    var client;
    var orgId;
    var clientId;
    var password;

    var topic = "iot-2/evt/sensorData/fmt/json";
    var isConnected = false;

    var createRingBuffer = function (length) {

        var pointer = 0, buffer = [];

        return {
            get: function (key) { return buffer[key]; },
            push: function (item) {
                buffer[pointer] = item;
                pointer = (length + pointer + 1) % length;
            },
            length: function() {
                return length;
            },
            min  : function(){return Math.min.apply(Math, buffer);},            
            avg: function() {return buffer.reduce(function (a, b) { return a + b; }, 0) / length;}
        };
    };

    var buffer = createRingBuffer(100);

    window.ondevicemotion = function (event) {
        ax = parseFloat((event.acceleration.x || 0));
        ay = parseFloat((event.acceleration.y || 0));
        az = parseFloat((event.acceleration.z || 0));

        //document.getElementById("accx").innerHTML = ax.toFixed(2);
        //document.getElementById("accy").innerHTML = ay.toFixed(2);
        //document.getElementById("accz").innerHTML = az.toFixed(2);
    }

    window.ondeviceorientation = function (event) {

        oa = (event.alpha || 0);
        ob = (event.beta || 0);
        og = (event.gamma || 0);

        if (event.webkitCompassHeading) {
            oa = -event.webkitCompassHeading;
        }

   

        document.getElementById("doDirection").innerHTML = oa.toFixed(2);
        document.getElementById("doTiltFB").innerHTML = ob.toFixed(2);
        document.getElementById("doTiltLR").innerHTML = og.toFixed(2);
    }

    function publish() {
        // We only attempt to publish if we're actually connected, saving CPU and battery
        if (isConnected) {
            var payload = {
                
                    "ax": ax.toFixed(2),
                    "ay": ay.toFixed(2),
                    "az": az.toFixed(2),
                    "oa": oa.toFixed(2),
                    "ob": ob.toFixed(2),
                    "og": og.toFixed(2)                
            };
            var message = new Paho.MQTT.Message(JSON.stringify(payload));
            message.destinationName = topic;
            try {
                client.send(message);
                console.log("[%s] Published", new Date().getTime());
            }
            catch (err) {
                alert('err');
                // This occurs if we've actually lost our connection. We should therefore attempt to reconnect. 
                // We wait for a second before doing this, so that chronic issues with the network do not lead
                // to rapid connect attempts and hence drain the battery/use unnecessary network bandwidth.
                // Alternatively, we could use Paho's onConnectionLost mechanism.
                isConnected = false;
                console.error(err);
               
                document.getElementById("connection").innerHTML = "Disconnected";
                setTimeout(connectDevice(client), 1000);
            }
        }
    }

    function onConnectSuccess() {
        // The device connected successfully
        console.log("Connected Successfully!");
        isConnected = true;

        document.getElementById("connection").innerHTML = "Connected";
        $('#login').hide();
        $('#main').show();
    }

    function onConnectFailure(err) {
        // The device failed to connect. Let's try again in one second.
        document.getElementById("error").innerHTML = "Unable to connect, device might not be supported! Retrying in 1 second." + JSON.stringify(err);
        document.getElementById("connection").innerHTML = "Retrying";
        setTimeout(connectDevice(client), 1000);
    }

    function connectDevice(client) {

        document.getElementById("connection").innerHTML = "Connecting...";
        //console.log("Connecting device to IoT Foundation...");
        // Disable the connect button and gray it out as we have started the MQTT connect cycle. If a connection fails,
        // or if it's later dropped, the device will automatically try reconnecting again - no manual intervention
        // is required from this point on.
        
        client.connect({
            onSuccess: onConnectSuccess,
            onFailure: onConnectFailure,
            userName: 'a-9cg8pi-1r7oiahrki',//"use-token-auth",
            password: password,
        });
    }

    function getDeviceCredentials() {
        //var data = { "email": window.deviceId, "pin": document.getElementById('pin').value };
        //$.ajax({
        //    url: "/auth",
        //    type: "POST",
        //    data: JSON.stringify(data),
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    success: function (response) {
        orgId = '9cg8pi';
        //d:org_id:device_type:device_id
        //clientId = 'd:' + orgId + ':phone:simped'; //the real way to do it
        clientId = 'a:' + orgId + ':'+ (new Date()).getTime(); //pesudo random number
        password = '0w*iZgqsn4*S56k0AK';

        //var password = response.authtoken;
        var username = $('#username').val();
        if (username != "")
        {
            topic = "iot-2/type/phone/id/simped/evt/" + username + "/fmt/json";
        }

                client = new Paho.MQTT.Client(orgId + ".messaging.internetofthings.ibmcloud.com", 1883, clientId);

                console.log("Attempting connect");

                connectDevice(client);

                /*
				 * Now start the publish cycle to publish 10 times a second. This offers smooth animations 
				 * in the demo, but in most cases a publish rate of 10 msg/sec will be far in excess of any 
				 * real world needs.
				 */
                //setInterval(publish, 100);
        //    },
        //    error: function (xhr, status, error) {
        //        if (xhr.status == 403) {
        //            // Authentication check succeeded and told us we're invalid
        //            alert("Incorrect code!");
        //        } else {
        //            // Something else went wrong
        //            alert("Failed to authenticate! " + error);
        //        }
        //    }
        //});
    }

    $(document).ready(function () {
        $('#start').click(getDeviceCredentials);
    });

    var deviceId = window.deviceId;
    window.publish = publish;

}(window));