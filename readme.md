#Co-op Anger Management#
﻿Co-op Anger Management is a mobile enabled web page http://dh2015.azurewebsites.net that you can visit when you feel angry. It changes colors when you shake your phone, and helps you release the tension. 
﻿
﻿The mobile webpage is accompanied by a dashboard http://dh2015admin.azurewebsites.net where all angry users can compete against each other, to see who releases the most anger. 
﻿
#Known limitations#
﻿Currently the mobile enabled webpage is only working on Android and Iphone > 4.0 (according to my testing). 
﻿
#Resources:#
The following resources are used in the app. 
The mobile web page uses Alex Gibson's Shake.js library with a few modifications to detect Shakes. 
https://raw.githubusercontent.com/alexgibson/shake.js/master/shake.js

The tree map background of the admin dashboard is made with D3 from inspired by this template
http://codepen.io/WhatIsHeDoing/pen/tnKil

The .Net admin dashboard backend uses the m2mqtt library to communicate with the IOT service in IBM Bluemix
https://m2mqtt.codeplex.com/

The web pages uses this google font
https://www.google.com/fonts#UsePlace:use/Collection:Oswald

The mobile web page uses a modified version of the this sample from IBM
https://github.com/ibm-messaging/iot-python/tree/master/samples/bluemixZoneDemo

#How to build#
