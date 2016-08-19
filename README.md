# KSPLogger
This mod is created to expose some internal data for external use.  
The specific use case is for streamers who would like to display some values on screen in some manner;  for example, 
using OBS to display the values in a template

Installation
Copy KSPLogger directory into the GameData directory

Configuration
There is a config file called KSPLogger.cfg, which contains all the value which can be set.
There are three sections in the config file:

KSPLogger values - Contains values which control the behaviour of the mod.  Each line in the config file is documented

FlightGlobals values - Contains true/false settings for many of the available values in the internal FlightGlobals class.  
  Set a setting to true if you want KSPLogger to save that value to a file

FlightGlobals.ActiveVessel values - Contains true/false settings for many of the available values in the 
  FlightGlobals.ActiveVessel class.  Set a setting to true if you want KSPLogger to save that value to a file

KSPLogger is only active when in the Flight scene.  When leaving the flight scene, if the config value singleLine is 
set true, then the data files are deleted;  if it is set false, then the files are left alone