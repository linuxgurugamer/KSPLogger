using System;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;
using KSP.UI.Screens;
using KSPAssets;

namespace KSPLogger
{

    public class Config
    {
        public string ROOT_PATH { get { return KSPUtil.ApplicationRootPath; } }
        string DIR = "GameData/KSPLogger";
        static readonly string DEFAULT_CFG_FILE = "PluginData/KSPLogger_Default.cfg";
        static readonly string CFG_FILE = "PluginData/KSPLogger.cfg";

        static readonly string NODENAME = "KSPLogger";
        private static ConfigNode configFile = null;
        private static ConfigNode configFileNode = null;

        // Data items follow
        public string filePrefix = "dataLog";
        public string fileSuffix = ".txt";
        public bool unixFormat = false;
        public string eol = "\r\n";
        public bool onePerFile = true;
        public string separator = ",";
        public bool singleLine = true;
        public int decimalPlaces = 0;
        public float refreshRate = 0.1f;
        public bool deleteOnExit = false;

        // From FlightGlobals

        public bool ship_geeForce = false;
        public bool ship_latitude = false;
        public bool ship_longitude = false;
        public bool ship_obtSpeed = false;
        public bool ship_srfSpeed = false;
        public bool ship_verticalSpeed = false;
        public bool verticalAcceleration = false;

        //fromFlightGlobals.ActiveVessel
        public bool altitude = false;
        public bool terrainAltitude = false;
        public bool atmDensity = false;
        public bool atmosphericTemperature = false;
        public bool currentStage = false;
        public bool distanceToSun = false;
        public bool geeForce = false;
        public bool geeForce_immediate = false;
        public bool heightFromSurface = false;
        public bool heightFromTerrain = false;
        public bool horizontalSrfSpeed = false;
        public bool horizontalAcceleration = false;
        public bool indicatedAirSpeed = false;
        public bool landedAt = false;
        public bool mach = false;
        public bool missionTime = false;
        public bool obt_speed = false;
        public bool ApA = false;
        public bool PeA = false;

        public bool speed = false;
        public bool vesselName = false;

        public bool KIA = false;
        public bool MIA = false;

        public bool inclination = false;
        public bool biome = false;

        public bool scaleMeters;
        public bool ship_obtSpeedUnits;
        public bool ship_srfSpeedUnits;
        public bool ship_verticalSpeedUnits;
        public bool altitudeUnits;
        public bool terrainAltitudeUnits;
        public bool heightFromTerrainUnits, heightFromSurfaceUnits;
        public bool horizontalSrfSpeedUnits;
        public bool ApA_Units;
        public bool PeA_Units;

        public bool fixed_ship_obtSpeed = false;
        public double fixed_ship_obtSpeed_divisor = 1000;
        public string fixed_ship_obtSpeedUnits = "km/sec";

        public bool fixed_ship_srfSpeed = false;
        public double fixed_ship_srfSpeed_divisor = 1000;
        public string fixed_ship_srfSpeedUnits = "km/sec";

        public bool fixed_ship_verticalSpeed = false;
        public double fixed_ship_verticalSpeed_divisor = 1000;
        public string fixed_ship_verticalSpeedUnits = "km/sec";

        public bool fixed_altitude = false;
        public double fixed_altitude_divisor = 1000;
        public string fixed_altitudeUnits = "km";

        public bool fixed_terrainAltitude = false;
        public double fixed_terrainAltitude_divisor = 1000;
        public string fixed_terrainAltitudeUnits = "km";

        void parseConfigNode(ref ConfigNode root)
        {
            try { filePrefix = root.GetValue("filePrefix"); } catch { }
            try { fileSuffix = root.GetValue("fileSuffix"); } catch { }

            try { unixFormat = Boolean.Parse(root.GetValue("unixFormat")); } catch { }
            try { onePerFile = Boolean.Parse(root.GetValue("onePerFile")); } catch { }

            try { separator = root.GetValue("separator"); } catch { }

            try { singleLine = Boolean.Parse(root.GetValue("singleLine")); } catch { }

            try { refreshRate = (float)Convert.ToDouble(root.GetValue("refreshRate")); } catch (Exception) { }

            try { decimalPlaces = Convert.ToUInt16(root.GetValue("decimalPlaces")); } catch (Exception) { }

            try { deleteOnExit = Boolean.Parse(root.GetValue("deleteOnExit")); } catch { }

            try { ship_geeForce = Boolean.Parse(root.GetValue("ship_geeForce")); } catch { }
            try { ship_latitude = Boolean.Parse(root.GetValue("ship_latitude")); } catch { }
            try { ship_longitude = Boolean.Parse(root.GetValue("ship_longitude")); } catch { }
            try { ship_obtSpeed = Boolean.Parse(root.GetValue("ship_obtSpeed")); } catch { }
            try { ship_srfSpeed = Boolean.Parse(root.GetValue("ship_srfSpeed")); } catch { }
            try { ship_verticalSpeed = Boolean.Parse(root.GetValue("ship_verticalSpeed")); } catch { }
            try { verticalAcceleration = Boolean.Parse(root.GetValue("verticalAcceleration")); } catch { }

            try { ApA = Boolean.Parse(root.GetValue("ApA")); } catch { }
            try { PeA = Boolean.Parse(root.GetValue("PeA")); } catch { }

            try { altitude = Boolean.Parse(root.GetValue("altitude")); } catch { }
            try { terrainAltitude = Boolean.Parse(root.GetValue("terrainAltitude")); } catch { }
            try { atmDensity = Boolean.Parse(root.GetValue("atmDensity")); } catch { }
            try { atmosphericTemperature = Boolean.Parse(root.GetValue("atmosphericTemperature")); } catch { }
            try { currentStage = Boolean.Parse(root.GetValue("currentStage")); } catch { }
            try { distanceToSun = Boolean.Parse(root.GetValue("distanceToSun")); } catch { }
            try { geeForce = Boolean.Parse(root.GetValue("geeForce")); } catch { }
            try { geeForce_immediate = Boolean.Parse(root.GetValue("geeForce_immediate")); } catch { }
            try { heightFromTerrain = Boolean.Parse(root.GetValue("heightFromTerrain")); } catch { }
            try { horizontalSrfSpeed = Boolean.Parse(root.GetValue("horizontalSrfSpeed")); } catch { }
            try { horizontalAcceleration = Boolean.Parse(root.GetValue("horizontalAcceleration")); } catch { }
            try { indicatedAirSpeed = Boolean.Parse(root.GetValue("indicatedAirSpeed")); } catch { }
            try { landedAt = Boolean.Parse(root.GetValue("landedAt")); } catch { }
            try { mach = Boolean.Parse(root.GetValue("mach")); } catch { }
            try { missionTime = Boolean.Parse(root.GetValue("missionTime")); } catch { }
            try { obt_speed = Boolean.Parse(root.GetValue("obt_speed")); } catch { }
            try { heightFromSurface = Boolean.Parse(root.GetValue("heightFromSurface")); } catch { }
            try { vesselName = Boolean.Parse(root.GetValue("vesselName")); } catch { }

            try { KIA = Boolean.Parse(root.GetValue("KIA")); } catch { }
            try { MIA = Boolean.Parse(root.GetValue("MIA")); } catch { }


            try { scaleMeters = Boolean.Parse(root.GetValue("scaleMeters")); } catch { }

            try { ship_obtSpeedUnits = Boolean.Parse(root.GetValue("ship_obtSpeedUnits")); } catch { }
            try { ship_srfSpeedUnits = Boolean.Parse(root.GetValue("ship_srfSpeedUnits")); } catch { }
            try { ship_verticalSpeedUnits = Boolean.Parse(root.GetValue("ship_verticalSpeedUnits")); } catch { }
            try { altitudeUnits = Boolean.Parse(root.GetValue("altitudeUnits")); } catch { }
            try { terrainAltitudeUnits = Boolean.Parse(root.GetValue("terrainAltitudeUnits")); } catch { }
            try { heightFromSurfaceUnits = Boolean.Parse(root.GetValue("heightFromSurfaceUnits")); } catch { }
            try { heightFromTerrainUnits = Boolean.Parse(root.GetValue("heightFromTerrainUnits")); } catch { }
            try { horizontalSrfSpeedUnits = Boolean.Parse(root.GetValue("horizontalSrfSpeedUnits")); } catch { }

            try { ApA_Units = Boolean.Parse(root.GetValue("ApA_Units")); } catch { }
            try { PeA_Units = Boolean.Parse(root.GetValue("PeA_Units")); } catch { }

            try { inclination = Boolean.Parse(root.GetValue("inclination")); } catch { }
            try { biome = Boolean.Parse(root.GetValue("biome")); } catch { }

            try { fixed_ship_obtSpeed = Boolean.Parse(root.GetValue("fixed_ship_obtSpeed")); } catch { }
            try { fixed_ship_obtSpeed_divisor = Double.Parse(root.GetValue("fixed_ship_obtSpeed_divisor")); } catch { }
            try { fixed_ship_obtSpeedUnits = root.GetValue("fixed_ship_obtSpeedUnits"); } catch { }

            try { fixed_ship_srfSpeed = Boolean.Parse(root.GetValue("fixed_ship_srfSpeed")); } catch { }
            try { fixed_ship_srfSpeed_divisor = Double.Parse(root.GetValue("fixed_ship_srfSpeed_divisor")); } catch { }
            try { fixed_ship_srfSpeedUnits = root.GetValue("fixed_ship_srfSpeedUnits"); } catch { }

            try { fixed_ship_verticalSpeed = Boolean.Parse(root.GetValue("fixed_ship_verticalSpeed")); } catch { }
            try { fixed_ship_verticalSpeed_divisor = Double.Parse(root.GetValue("fixed_ship_verticalSpeed_divisor")); } catch { }
            try { fixed_ship_verticalSpeedUnits = root.GetValue("fixed_ship_verticalSpeedUnits"); } catch { }

            try { fixed_altitude = Boolean.Parse(root.GetValue("fixed_altitude")); } catch { }
            try { fixed_altitude_divisor = Double.Parse(root.GetValue("fixed_altitude_divisor")); } catch { }
            try { fixed_altitudeUnits = root.GetValue("fixed_altitudeUnits"); } catch { }

            try { fixed_terrainAltitude = Boolean.Parse(root.GetValue("fixed_terrainAltitude")); } catch { }
            try { fixed_terrainAltitude_divisor = Double.Parse(root.GetValue("fixed_terrainAltitude_divisor")); } catch { }
            try { fixed_terrainAltitudeUnits = root.GetValue("fixed_terrainAltitudeUnits"); } catch { }
        }

        DateTime lastWriteTime;

        string fname {  get { return ROOT_PATH + DIR + "/" + CFG_FILE; } }

        public void ReloadIfChanged()
        {
            Log.Info("ReloadIfChanged");
            if (lastWriteTime != File.GetLastWriteTime(fname))
                LoadConfiguration();
        }

        public void LoadConfiguration()
        {
            Log.Info("LoadConfiguration");
            lastWriteTime = File.GetLastWriteTime(fname);
           
            Log.Info("LoadConfiguration, fname: " + fname);
            if (!File.Exists(fname))
            {
                File.Copy(ROOT_PATH + DIR + "/" + DEFAULT_CFG_FILE, fname);
            }
            configFile = ConfigNode.Load(fname);

            if (configFile != null)
            {
                Log.Info("Loading config: " + fname);
                configFileNode = configFile.GetNode(NODENAME);
                if (configFileNode != null)
                {
                    parseConfigNode(ref configFileNode);
                    if (unixFormat)
                        eol = "\n";
                }
            }
            else
                Log.Error("No config found");
        }


    }
}
