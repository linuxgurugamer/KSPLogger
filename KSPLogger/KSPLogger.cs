using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace KSPLogger
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KSPLogger : MonoBehaviour
    {
        Config cfg = new Config();

        float lastUpdate = 0.0f;
        float lastDcheckUpdate = 0.0f;

        string line = "";
        string filenames = "";

        double horizontalAcceleration = 0.0;
        double horizontalSpeed = 0.0;
        double verticalAcceleration = 0.0;
        double verticalSpeed = 0.0;


        public void WriteLine()
        {
            Log.Info("WriteLine, file: " + cfg.ROOT_PATH + "/" + cfg.filePrefix);
            if (cfg.onePerFile == false)
            {
                filenames = cfg.ROOT_PATH + "/" + cfg.filePrefix + cfg.fileSuffix;
                if (cfg.singleLine)
                    File.WriteAllText(filenames, line + cfg.eol);
                else
                    File.AppendAllText(filenames, line + cfg.eol);
                line = "";
            }
        }

        public void WriteFile(string fileName, string value)
        {
            if (cfg.onePerFile == false)
            { 
                line += value + cfg.separator;
            }
            else
            {
                string fname = cfg.ROOT_PATH + cfg.filePrefix + "." + fileName + cfg.fileSuffix;
                filenames += fname + ":";
                Log.Info("WriteFile, file: " + cfg.ROOT_PATH + "/" + cfg.filePrefix);

                if (cfg.singleLine)
                    File.WriteAllText(fname, value + cfg.eol);
                else
                    File.AppendAllText(fname, value + cfg.eol);
            }
            //  Log.Debug(fmt);
        }

        public void OnDestroy()
        {
            Log.Info("OnDestroy");
 //           return;
            if (cfg != null)
                if (cfg.singleLine && cfg.deleteOnExit)
                {
                    if (filenames.Length > 0)
                    {
                        char[] delimiterChars = { ':' };
                        string[] fname = filenames.Split(delimiterChars);
                        foreach (string s in fname)
                        {
                            File.Delete(s);
                        }
                    }
                }
            
        }

        public void Start()
        {
            cfg.LoadConfiguration();
        }

        //
        // Following copied from TheReadPanda's Hire mod with permission
        //
        int KDead = 0;
        int KMissing = 0;
        //private string[] KCareerStrings = { "Pilot", "Scientist", "Engineer" };

        void dCheck()
        {
            KDead = 0;
            KMissing = 0;
            var roster = HighLogic.CurrentGame.CrewRoster;

            // 10 percent for dead and 5 percent for missing, note can only have dead in some career modes.
            foreach (ProtoCrewMember kerbal in roster.Crew)
            {
               
                if (kerbal.rosterStatus.ToString() == "Dead")
                {
                    //if (KCareerStrings.Contains(kerbal.experienceTrait.Title))
                        KDead++ ;                  
                }
                if (kerbal.rosterStatus.ToString() == "Missing")
                {
                    //if (KCareerStrings.Contains(kerbal.experienceTrait.Title))
                        KMissing++;  
                }
            }
        }
        // use FixedUpdate since it is not called as often as Update or LateUpdate is, but 
        // is still fast enough

        public void FixedUpdate()
        {
            if (Time.realtimeSinceStartup - lastUpdate < cfg.refreshRate)
                return;

            lastUpdate = Time.realtimeSinceStartup;

            
            filenames = "";
            // From FlightGlobals
            if (cfg.ship_geeForce)
                WriteFile("ship_geeForce", FlightGlobals.ship_geeForce.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.ship_latitude)
                WriteFile("ship_latitude", FlightGlobals.ship_latitude.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.ship_longitude)
                WriteFile("ship_longitude", FlightGlobals.ship_longitude.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.ship_obtSpeed)
                WriteFile("ship_obtSpeed", FlightGlobals.ship_obtSpeed.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.ship_srfSpeed)
                WriteFile("ship_srfSpeed", FlightGlobals.ship_srfSpeed.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.ship_verticalSpeed)
                WriteFile("ship_verticalSpeed", FlightGlobals.ship_verticalSpeed.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.verticalAcceleration)
            {
                this.verticalAcceleration = (FlightGlobals.ship_verticalSpeed - this.verticalSpeed) / TimeWarp.fixedDeltaTime;
                this.verticalSpeed = FlightGlobals.ship_verticalSpeed;
                WriteFile("verticalAcceleration", this.verticalAcceleration.ToString("F" + cfg.decimalPlaces.ToString()));
            }

            //fromFlightGlobals.ActiveVessel
            if (cfg.altitude)
                WriteFile("altitude", FlightGlobals.ActiveVessel.altitude.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.terrainAltitude)
                WriteFile("terrainAltitude", FlightGlobals.ActiveVessel.terrainAltitude.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.atmDensity)
                WriteFile("atmDensity", FlightGlobals.ActiveVessel.atmDensity.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.atmosphericTemperature)
                WriteFile("atmosphericTemperature", FlightGlobals.ActiveVessel.atmosphericTemperature.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.currentStage)
                WriteFile("currentStage", FlightGlobals.ActiveVessel.currentStage.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.distanceToSun)
                WriteFile("distanceToSun", FlightGlobals.ActiveVessel.distanceToSun.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.geeForce)
                WriteFile("geeForce", FlightGlobals.ActiveVessel.geeForce.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.geeForce_immediate)
                WriteFile("geeForce_immediate", FlightGlobals.ActiveVessel.geeForce_immediate.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.heightFromSurface)
                WriteFile("heightFromSurface", FlightGlobals.ActiveVessel.heightFromSurface.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.heightFromTerrain)
                WriteFile("heightFromTerrain", FlightGlobals.ActiveVessel.heightFromTerrain.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.horizontalSrfSpeed)
                WriteFile("horizontalSrfSpeed", FlightGlobals.ActiveVessel.horizontalSrfSpeed.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.horizontalAcceleration)
            {
                this.horizontalAcceleration = (this.horizontalAcceleration - this.horizontalSpeed) / TimeWarp.fixedDeltaTime;
                this.horizontalSpeed = FlightGlobals.ActiveVessel.horizontalSrfSpeed;
                WriteFile("horizontalAcceleration", this.horizontalAcceleration.ToString("F" + cfg.decimalPlaces.ToString()));
            }
            if (cfg.indicatedAirSpeed)
                WriteFile("indicatedAirSpeed", FlightGlobals.ActiveVessel.indicatedAirSpeed.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.landedAt)
                WriteFile("landedAt", FlightGlobals.ActiveVessel.landedAt);
            if (cfg.mach)
                WriteFile("mach", FlightGlobals.ActiveVessel.mach.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.missionTime)
                WriteFile("missionTime", FlightGlobals.ActiveVessel.missionTime.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.obt_speed)
                WriteFile("obt_speed", FlightGlobals.ActiveVessel.obt_speed.ToString("F" + cfg.decimalPlaces.ToString()));

            if (cfg.ApA)
                WriteFile("ApA", FlightGlobals.ActiveVessel.orbit.ApA.ToString("F" + cfg.decimalPlaces.ToString()));
            if (cfg.PeA)
                WriteFile("PeA", FlightGlobals.ActiveVessel.orbit.PeA.ToString("F" + cfg.decimalPlaces.ToString()));



            if (cfg.vesselName)
                WriteFile("vesselName", FlightGlobals.ActiveVessel.vesselName);


            // Only do the dCheck function once every 10 seconds
            if ((cfg.KIA || cfg.MIA) && Time.realtimeSinceStartup - lastDcheckUpdate >= 10)
            {
                dCheck();
                lastDcheckUpdate = Time.realtimeSinceStartup;            

                if (cfg.KIA)
                    WriteFile("KIA", KDead.ToString());
                if (cfg.MIA)
                    WriteFile("MIA", KDead.ToString());
            }
        }

    }
}
