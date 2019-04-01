using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;


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
            if (cfg.onePerFile == false)
            {
                if (cfg.filePrefix[0] == '/' || cfg.filePrefix[1] == ':')
                    filenames = cfg.filePrefix + cfg.fileSuffix;
                else
                    filenames = cfg.ROOT_PATH + "/" + cfg.filePrefix + cfg.fileSuffix;

                // If a single line, then write it using BinaryFormatter,
                // otherwise use the AppendAllText 
                if (cfg.singleLine)
                {
                    if (File.Exists(filenames))
                        file = File.OpenWrite(filenames);
                    else
                        file = File.Create(filenames);
                    
                    line += cfg.eol;
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(file, line);
                    file.Close();
                }
                else
                {
                    File.AppendAllText(filenames, line + cfg.eol);
                }
                line = "";
            }
        }


        FileStream file;
        string fname;
        public void WriteFile(string fileName, string value)
        {
            if (cfg.onePerFile == false)
            {
                line += value + cfg.separator;
            }
            else
            {
                 fname = "";
                if (cfg.filePrefix[0] == '/' || cfg.filePrefix[1] == ':')
                    fname = cfg.filePrefix + fileName + cfg.fileSuffix;
                else
                    fname = cfg.ROOT_PATH + cfg.filePrefix + "." + fileName + cfg.fileSuffix;

                if (File.Exists(fname))
                    file = File.OpenWrite(fname);
                else
                    file = File.Create(fname);
                
                value += cfg.eol;
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, value);
                file.Close();

            }
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

        string decimalPlaces;
        public void Start()
        {
            cfg.LoadConfiguration();
            decimalPlaces = "F" + cfg.decimalPlaces.ToString();
            StartCoroutine(DoBackgroundJob());
        }


#if false
        public void FixedUpdate()
        {
            if (Time.realtimeSinceStartup - lastUpdate < cfg.refreshRate)
                return;

            lastUpdate = Time.realtimeSinceStartup;
            UpdateOBSData();
        }
#endif
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
                    KDead++;
                }
                if (kerbal.rosterStatus.ToString() == "Missing")
                {
                    //if (KCareerStrings.Contains(kerbal.experienceTrait.Title))
                    KMissing++;
                }
            }
        }

        IEnumerator DoBackgroundJob()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(cfg.refreshRate);
                UpdateOBSData();
            }
        }

        string FormatSpeed(double speed, bool units)
        {
            if (!units || !cfg.scaleMeters)
                return speed.ToString(decimalPlaces);
            if (speed > 1000)
            {
                speed /= 1000;
                return speed.ToString(decimalPlaces) + " Km/s";
            }
            return speed.ToString(decimalPlaces) + " m/s";
        }

        string FormatAltitude(double alt, bool units)
        {
            if (!units || !cfg.scaleMeters)
                return alt.ToString(decimalPlaces);
            if (alt > 1000)
            {
                alt /= 1000;
                return alt.ToString(decimalPlaces) + " Km";
            }
            return alt.ToString(decimalPlaces) + " m";
        }

        public IEnumerator UpdateOBSData()
        {


            filenames = "";
            // From FlightGlobals
            if (cfg.ship_geeForce)
            { WriteFile("ship_geeForce", FlightGlobals.ship_geeForce.ToString(decimalPlaces)); yield return null; }

            if (cfg.ship_latitude)
            { WriteFile("ship_latitude", FlightGlobals.ship_latitude.ToString(decimalPlaces)); yield return null; }

            if (cfg.ship_longitude)
            { WriteFile("ship_longitude", FlightGlobals.ship_longitude.ToString(decimalPlaces)); yield return null; }

            if (cfg.ship_obtSpeed)
            { WriteFile("ship_obtSpeed", FormatSpeed( FlightGlobals.ship_obtSpeed, cfg.ship_obtSpeedUnits)); yield return null; }

            if (cfg.ship_srfSpeed)
            { WriteFile("ship_srfSpeed", FormatSpeed(FlightGlobals.ship_srfSpeed, cfg.ship_srfSpeedUnits)); yield return null; }

            if (cfg.ship_verticalSpeed)
            { WriteFile("ship_verticalSpeed", FormatSpeed(FlightGlobals.ship_verticalSpeed, cfg.ship_verticalSpeedUnits)); yield return null; }

            if (cfg.verticalAcceleration)
            {
                this.verticalAcceleration = (FlightGlobals.ship_verticalSpeed - this.verticalSpeed) / TimeWarp.fixedDeltaTime;
                this.verticalSpeed = FlightGlobals.ship_verticalSpeed;
                { WriteFile("verticalAcceleration", this.verticalAcceleration.ToString(decimalPlaces)); yield return null; }
            }

            //fromFlightGlobals.ActiveVessel
            if (cfg.altitude)
            { WriteFile("altitude", FormatAltitude(FlightGlobals.ActiveVessel.altitude, cfg.altitudeUnits)); yield return null; }

            if (cfg.terrainAltitude)
            { WriteFile("terrainAltitude", FormatAltitude(FlightGlobals.ActiveVessel.terrainAltitude, cfg.terrainAltitudeUnits)); yield return null; }
            if (cfg.atmDensity)
            { WriteFile("atmDensity", FlightGlobals.ActiveVessel.atmDensity.ToString(decimalPlaces)); yield return null; }
            if (cfg.atmosphericTemperature)
            { WriteFile("atmosphericTemperature", FlightGlobals.ActiveVessel.atmosphericTemperature.ToString(decimalPlaces)); yield return null; }

            if (cfg.currentStage)
            { WriteFile("currentStage", FlightGlobals.ActiveVessel.currentStage.ToString(decimalPlaces)); yield return null; }
            if (cfg.distanceToSun)
            { WriteFile("distanceToSun", FlightGlobals.ActiveVessel.distanceToSun.ToString(decimalPlaces)); yield return null; }
            if (cfg.geeForce)
            { WriteFile("geeForce", FlightGlobals.ActiveVessel.geeForce.ToString(decimalPlaces)); yield return null; }
            if (cfg.geeForce_immediate)
            { WriteFile("geeForce_immediate", FlightGlobals.ActiveVessel.geeForce_immediate.ToString(decimalPlaces)); yield return null; }
            if (cfg.heightFromSurface)
            { WriteFile("heightFromSurface", FormatAltitude(FlightGlobals.ActiveVessel.heightFromSurface, cfg.heightFromSurfaceUnits)); yield return null; }

            if (cfg.heightFromTerrain)
            { WriteFile("heightFromTerrain", FormatAltitude(FlightGlobals.ActiveVessel.heightFromTerrain, cfg.heightFromTerrainUnits)); yield return null; }
            if (cfg.horizontalSrfSpeed)
            { WriteFile("horizontalSrfSpeed", FormatSpeed(FlightGlobals.ActiveVessel.horizontalSrfSpeed, cfg.horizontalSrfSpeedUnits)); yield return null; }
            if (cfg.horizontalAcceleration)
            {
                this.horizontalAcceleration = (this.horizontalAcceleration - this.horizontalSpeed) / TimeWarp.fixedDeltaTime;
                this.horizontalSpeed = FlightGlobals.ActiveVessel.horizontalSrfSpeed;
                { WriteFile("horizontalAcceleration", this.horizontalAcceleration.ToString(decimalPlaces)); yield return null; }
            }

            if (cfg.indicatedAirSpeed)
            { WriteFile("indicatedAirSpeed", FlightGlobals.ActiveVessel.indicatedAirSpeed.ToString(decimalPlaces)); yield return null; }
            if (cfg.landedAt)
            { WriteFile("landedAt", FlightGlobals.ActiveVessel.landedAt); yield return null; }
            if (cfg.mach)
            { WriteFile("mach", FlightGlobals.ActiveVessel.mach.ToString(decimalPlaces)); yield return null; }
            if (cfg.missionTime)
            { WriteFile("missionTime", FlightGlobals.ActiveVessel.missionTime.ToString(decimalPlaces)); yield return null; }
            if (cfg.obt_speed)
            { WriteFile("obt_speed", FormatSpeed(FlightGlobals.ActiveVessel.obt_speed, cfg.ship_obtSpeedUnits)); yield return null; }

            if (cfg.ApA)
            { WriteFile("ApA", FormatAltitude(FlightGlobals.ActiveVessel.orbit.ApA, cfg.ApA_Units)); yield return null; }
            if (cfg.PeA)
            { WriteFile("PeA", FormatAltitude(FlightGlobals.ActiveVessel.orbit.PeA, cfg.PeA_Units)); yield return null; }


            if (cfg.vesselName)
            { WriteFile("vesselName", Localizer.Format(FlightGlobals.ActiveVessel.vesselName)); yield return null; }


            // Only do the dCheck function once every 10 seconds
            if ((cfg.KIA || cfg.MIA) && Time.realtimeSinceStartup - lastDcheckUpdate >= 10)
            {
                dCheck();
                lastDcheckUpdate = Time.realtimeSinceStartup;

                if (cfg.KIA)
                { WriteFile("KIA", KDead.ToString()); yield return null; }
                if (cfg.MIA)
                { WriteFile("MIA", KMissing.ToString()); yield return null; }
            }
            if (cfg.onePerFile == false)
                WriteLine();
        }
    }
}
