# FizViz-Windows
Home to the Windows Project for the FizViz Package

This is a Windows Universal App; it's been developed and tested on Windows 10 using Visual Studio 2015 Community Edition.

You can connect to your FizViz over wifi by putting in the IP address and port.  (Those values get configured in wifiConfig.h in our related Arduino project.)

Then you can control the background color scheme, the lighting behavior when the needle moves, and the min/max display.  You can manually move the needle around to test stuff out from the "Manual Position Control" pivot item.  When you're ready to hook up to Google Analytics, you need to add some credentials to the project - more on that below - and then you can pick which metric to watch from the "Google Analytics Config" pivot item.

Project Defines and Setup
-------------------------
You'll need to change one constant to make it match up with your FizViz setup - go to Commands/DisplayMode/FizVizCommand.cs, and down at the bottom, make sure NEOPIXEL_COUNT matches what you've got in the Arduino project.

Use of Remote Arduino Wiring
-------------------------
We're using the Firmata protocol to talk to the FizViz, but instead of directly controlling output pins and such, we set up a number of custom commands ("Move needle to this position", "Change background colors" etc.).  In order to do that, we forked the remote-wiring repository and made one small change - adding a pass-through function that allows us to send out custom Firmata control messages.

Setting up Google Analytics
---------------------------

The FizViz project allows you to pull data directly from a Google Analtyics account. However, there are a few steps that you need to complete in order to enable API access to Google Analytics. Follow these steps carefully - hopefully we can help to prevent some headaches with the process!

1) First you will need to create an application in the Google cloud console and enable the Analytics API.

* Go to http://code.google.com/apis/console
* Select the drop down and create a project if you do not already have one
* Once project is created click on services
* From here enable the Analytics API

2) Next step, you'll need to create some credentials to allow access to your project. Google should prompt you for "Go To Credentials" right after enabling the API.

* Select "Analytics API" for the type of credentials you need
* You will be calling the API from "Other UI (e.g. Windows, CLI tool)"
* You will be accessing "User Data"

Set those up, and click the "What Credentials do I need?" button.

3) Next step is to create an OAuth 2.0 Client ID and Consent Screen for the account. We usually call ours "FizViz"

* Google will prompt you to specify a product name

Set that up and Hit "Continue"

4) **Important!** Now, you should be ready to download your credentials - you will get a JSON file downloaded from Google.

The contents will look something like this:

```
{"installed":{"client_id":"XXXXXXXXX","project_id":"XXXXX","auth_uri":"https://accounts.google.com/o/oauth2/auth","token_uri":"https://accounts.google.com/o/oauth2/token","auth_provider_x509_cert_url":"https://www.googleapis.com/oauth2/v1/certs","client_secret":"XXXXXXX","redirect_uris":["urn:ietf:wg:oauth:2.0:oob","http://localhost"]}}
```

5) Cut and paste the contents of this file into the /FizVizController/FizVizController/Assets/client-secrets.json file and save it.

Once you've built your project with those credentials, you should be set to log in to Google Analytics using the FizViz User Interface!




