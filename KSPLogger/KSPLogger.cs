using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        internal static KSPLogger instance;
        internal Config cfg = new Config();

        float lastDcheckUpdate = 0.0f;

        double lastHorizontalSpeed = 0.0;
        double lastVerticalSpeed = 0.0;

        public void OnDestroy()
        {
            Log.Info("OnDestroy");

            if (cfg != null)
            {
                if (cfg.singleLine && cfg.deleteOnExit)
                {
                    if (BackgroundThread.filenames.Length > 0)
                    {
                        char[] delimiterChars = { ':' };
                        string[] fname = BackgroundThread.filenames.Split(delimiterChars);
                        foreach (string s in fname)
                        {
                            if (s!= "")
                                File.Delete(s);
                        }
                    }
                }
            }
            if (thread1.IsAlive)
                thread1.Abort();
        }

        string decimalPlaces;
        static Thread thread1;
        public void Start()
        {
            instance = this;
            cfg.LoadConfiguration();
            decimalPlaces = "F" + cfg.decimalPlaces.ToString();
            thread1 = new Thread(new ThreadStart(BackgroundThread.ThreadFunc));
            thread1.Start();
            StartCoroutine(DoBackgroundJob());
 
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
                yield return new WaitForSeconds(cfg.refreshRate / 5);
                yield return UpdateOBSData();
                if (!thread1.IsAlive)
                {
                    Log.Error("ThreadFunc stopped, restarting");
                    thread1 = new Thread(new ThreadStart(BackgroundThread.ThreadFunc));
                    thread1.Start();
                }
            }
        }

        IEnumerator UpdateOBSData()
        {
            
            while (!BackgroundThread.initted || !BackgroundThread.semaphore.Wait(0, "UpdateOBSData") && thread1.IsAlive)
            {
                yield return null;
            }


            BackgroundThread.ship_geeForce = FlightGlobals.ship_geeForce;
            BackgroundThread.ship_latitude = FlightGlobals.ship_latitude;
            BackgroundThread.ship_longitude = FlightGlobals.ship_longitude;
            BackgroundThread.ship_obtSpeed = FlightGlobals.ship_obtSpeed;
            BackgroundThread.ship_srfSpeed = FlightGlobals.ship_srfSpeed;
            BackgroundThread.ship_verticalSpeed = FlightGlobals.ship_verticalSpeed;

            {
                BackgroundThread.verticalAcceleration = (FlightGlobals.ship_verticalSpeed - this.lastVerticalSpeed) / TimeWarp.fixedDeltaTime;
                this.lastVerticalSpeed = FlightGlobals.ship_verticalSpeed;
            }
            BackgroundThread.altitude = FlightGlobals.ActiveVessel.altitude;
            BackgroundThread.terrainAltitude = FlightGlobals.ActiveVessel.terrainAltitude;
            BackgroundThread.atmDensity = FlightGlobals.ActiveVessel.atmDensity;
            BackgroundThread.atmosphericTemperature = FlightGlobals.ActiveVessel.atmosphericTemperature;

            BackgroundThread.currentStage = FlightGlobals.ActiveVessel.currentStage;
            BackgroundThread.distanceToSun = FlightGlobals.ActiveVessel.distanceToSun;
            BackgroundThread.geeForce = FlightGlobals.ActiveVessel.geeForce;
            BackgroundThread.geeForce_immediate = FlightGlobals.ActiveVessel.geeForce_immediate;
            BackgroundThread.heightFromSurface = FlightGlobals.ActiveVessel.heightFromSurface;

            BackgroundThread.heightFromTerrain = FlightGlobals.ActiveVessel.heightFromTerrain;
            BackgroundThread.horizontalSrfSpeed = FlightGlobals.ActiveVessel.horizontalSrfSpeed;

            {
                BackgroundThread.horizontalAcceleration = (FlightGlobals.ActiveVessel.horizontalSrfSpeed - this.lastHorizontalSpeed) / TimeWarp.fixedDeltaTime;
                this.lastHorizontalSpeed = FlightGlobals.ActiveVessel.horizontalSrfSpeed;
                
            }

            BackgroundThread.indicatedAirSpeed = FlightGlobals.ActiveVessel.indicatedAirSpeed;
            BackgroundThread.landedAt = FlightGlobals.ActiveVessel.landedAt;
            BackgroundThread.mach = FlightGlobals.ActiveVessel.mach;
            BackgroundThread.missionTime = FlightGlobals.ActiveVessel.missionTime;
            BackgroundThread.obt_speed = FlightGlobals.ActiveVessel.obt_speed;

            BackgroundThread.ApA = FlightGlobals.ActiveVessel.orbit.ApA;
            BackgroundThread.PeA = FlightGlobals.ActiveVessel.orbit.PeA;


            BackgroundThread.vesselName = FlightGlobals.ActiveVessel.vesselName;


            // Only do the dCheck function once every 10 seconds
            if ((cfg.KIA || cfg.MIA) && Time.realtimeSinceStartup - lastDcheckUpdate >= 10)
            {
                dCheck();
                lastDcheckUpdate = Time.realtimeSinceStartup;

                BackgroundThread.KDead = KDead;
                BackgroundThread.KMissing = KMissing;
            }

            BackgroundThread.semaphore.TryRelease("UpdateOBSData");

            yield return null;
        }
    }
}
