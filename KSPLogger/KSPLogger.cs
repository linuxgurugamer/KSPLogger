using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
//using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;


namespace KSPLogger
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public partial class KSPLogger : MonoBehaviour
    {
        internal static KSPLogger instance;
        static internal Config cfg = new Config();

        float lastDcheckUpdate = 0.0f;

        static double lastHorizontalSpeed = 0.0;
        double lastVerticalSpeed = 0.0;

        public void OnDestroy()
        {
            Log.Info("OnDestroy");

            if (cfg != null)
            {
                if (cfg.singleLine && cfg.deleteOnExit)
                {
                    if (filenames.Length > 0)
                    {
                        char[] delimiterChars = { ':' };
                        string[] fname = filenames.Split(delimiterChars);
                        foreach (string s in fname)
                        {
                            if (s != "")
                                File.Delete(s);
                        }
                    }
                }
            }
            StopCoroutine("ThreadFunc");
        }

        static string decimalPlaces;

        public void Start()
        {
            Log.Info("Start");
            instance = this;
            cfg.LoadConfiguration();
            decimalPlaces = "F" + cfg.decimalPlaces.ToString();
            StartCoroutine("ThreadFunc");
            //StartCoroutine(DoBackgroundJob());
            StartCoroutine(CheckForUpdates());
        }

        //
        // Following copied from TheReadPanda's Hire mod with permission
        //
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


        IEnumerator CheckForUpdates()
        {
            Log.Info("CheckForUpdates entry");
            while (true)
            {
                Log.Info("CheckForUpdates");
                yield return new WaitForSeconds(5);
                cfg.ReloadIfChanged();
            }
        }


        void UpdateOBSData()
        {

            lastVerticalSpeed = FlightGlobals.ship_verticalSpeed;

            currentStage = FlightGlobals.ActiveVessel.currentStage;
            heightFromSurface = FlightGlobals.ActiveVessel.heightFromSurface;

            heightFromTerrain = FlightGlobals.ActiveVessel.heightFromTerrain;

            lastHorizontalSpeed = FlightGlobals.ActiveVessel.horizontalSrfSpeed;

            landedAt = FlightGlobals.ActiveVessel.landedAt;

            vesselName = FlightGlobals.ActiveVessel.vesselName;
            Log.Info("vesselName: " + FlightGlobals.ActiveVessel.vesselName);

            biome = CurrentBiome(FlightGlobals.ActiveVessel);
            inclination = FlightGlobals.ActiveVessel.orbit.inclination;

            // Only do the dCheck function once every 10 seconds
            if ((cfg.KIA || cfg.MIA) && Time.realtimeSinceStartup - lastDcheckUpdate >= 10)
            {
                dCheck();
                lastDcheckUpdate = Time.realtimeSinceStartup;

            }
        }



        // Following method from  Mechjeb
        public string CurrentBiome(Vessel vessel)
        {
            if (vessel.landedAt != string.Empty)
                return vessel.landedAt;
            if (vessel.mainBody.BiomeMap == null)
                return "N/A";
            string biome = vessel.mainBody.BiomeMap.GetAtt(vessel.latitude * UtilMath.Deg2Rad, vessel.longitude * UtilMath.Deg2Rad).name;
            if (biome != "")
                biome = "'s " + biome;

            switch (vessel.situation)
            {
                //ExperimentSituations.SrfLanded
                case Vessel.Situations.LANDED:
                case Vessel.Situations.PRELAUNCH:
                    return vessel.mainBody.displayName + (biome == "" ? "'s surface" : biome);
                //ExperimentSituations.SrfSplashed
                case Vessel.Situations.SPLASHED:
                    return vessel.mainBody.displayName + (biome == "" ? "'s oceans" : biome);
                case Vessel.Situations.FLYING:
                    if (vessel.altitude < vessel.mainBody.scienceValues.flyingAltitudeThreshold)
                        //ExperimentSituations.FlyingLow
                        return "Flying over " + vessel.mainBody.displayName + biome;
                    else
                        //ExperimentSituations.FlyingHigh
                        return "Upper atmosphere of " + vessel.mainBody.displayName + biome;
                default:
                    if (vessel.altitude < vessel.mainBody.scienceValues.spaceAltitudeThreshold)
                        //ExperimentSituations.InSpaceLow
                        return "Space just above " + vessel.mainBody.displayName + biome;
                    else
                        // ExperimentSituations.InSpaceHigh
                        return "Space high over " + vessel.mainBody.displayName + biome;
            }
        }
    }
}
