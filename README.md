# FizViz-Windows
Home to the Windows Project for the FizViz Package

This is a Windows Universal App; it's been developed and tested on Windows 10 using Visual Studio 2015 Community Edition.

You can connect to your FizViz over wifi by putting in the IP address and port.  (Those values get configured in wifiConfig.h in our related Arduino project.)

Then you can control the background color scheme, the lighting behavior when the needle moves, and the min/max display.  You can manually move the needle around to test stuff out from the "Manual Position Control" pivot item.  When you're ready to hook up to Google Analytics, you need to add some credentials to the project - more on that below - and then you can pick which metric to watch from the "Google Analytics Config" pivot item.

Project Defines and Setup
-------------------------
You'll need to change one constant to make it match up with your FizViz setup - go to Commands/DisplayMode/FizVizCommand.cs, and down at the bottom, make sure NEOPIXEL_COUNT matches what you've got in the Arduino project.

