using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;


namespace KSPLogger
{
    class BackgroundThread
    {
        internal static Semaphore semaphore;
        internal static bool initted = false;
        internal static bool doExit = false;

        // All data values are copied into these static variables by the main thread in a CoRoutine.

        internal static double ship_geeForce, ship_latitude, ship_longitude, ship_srfSpeed, ship_verticalSpeed, verticalAcceleration;
        internal static double ship_obtSpeed;
        internal static double altitude, terrainAltitude, atmDensity, atmosphericTemperature, distanceToSun, geeForce, geeForce_immediate, horizontalSrfSpeed,
            horizontalAcceleration, indicatedAirSpeed, mach, missionTime;
        internal static double obt_speed, ApA, PeA;

        internal static float heightFromSurface, heightFromTerrain;

        internal static int currentStage, KDead, KMissing;

        internal static string landedAt, vesselName;
        static string decimalPlaces;
        
        static string line = "";
        internal static string filenames = "";
        static Config cfg;


        internal static void ThreadFunc()
        {
            cfg = KSPLogger.instance.cfg;
            decimalPlaces = "F" + cfg.decimalPlaces.ToString();

            semaphore = new Semaphore(1, 1);

            initted = true;
            while (true)
            {
                Thread.Sleep((int)(cfg.refreshRate * 1000));

                while (!BackgroundThread.semaphore.Wait(0, "ThreadFunc"))
                {
                    // This will sleep for 1/10 of the refreshRate
                     Thread.Sleep((int)(cfg.refreshRate * 100));
                }

                WriteOBSData();

                BackgroundThread.semaphore.TryRelease("ThreadFunc");
            }
        }

#region WriteDataItems
        static void WriteOBSData()
        {

            filenames = "";
            // From FlightGlobals
            if (cfg.ship_geeForce)
                WriteFile("ship_geeForce", ship_geeForce.ToString(decimalPlaces));

            if (cfg.ship_latitude)
                WriteFile("ship_latitude", ship_latitude.ToString(decimalPlaces));

            if (cfg.ship_longitude)
                WriteFile("ship_longitude", ship_longitude.ToString(decimalPlaces));

            if (cfg.ship_obtSpeed)
                WriteFile("ship_obtSpeed", FormatSpeed(ship_obtSpeed, cfg.ship_obtSpeedUnits));

            if (cfg.ship_srfSpeed)
                WriteFile("ship_srfSpeed", FormatSpeed(ship_srfSpeed, cfg.ship_srfSpeedUnits));

            if (cfg.ship_verticalSpeed)
                WriteFile("ship_verticalSpeed", FormatSpeed(ship_verticalSpeed, cfg.ship_verticalSpeedUnits));

            if (cfg.verticalAcceleration)
            {
                WriteFile("verticalAcceleration", verticalAcceleration.ToString(decimalPlaces));
            }
            
            if (cfg.altitude)
                WriteFile("altitude", FormatAltitude(altitude, cfg.altitudeUnits));

            if (cfg.terrainAltitude)
                WriteFile("terrainAltitude", FormatAltitude(terrainAltitude, cfg.terrainAltitudeUnits));
            if (cfg.atmDensity)
                WriteFile("atmDensity", atmDensity.ToString(decimalPlaces));
            if (cfg.atmosphericTemperature)
                WriteFile("atmosphericTemperature", atmosphericTemperature.ToString(decimalPlaces));

            if (cfg.currentStage)
                WriteFile("currentStage", currentStage.ToString(decimalPlaces));
            if (cfg.distanceToSun)
                WriteFile("distanceToSun", distanceToSun.ToString(decimalPlaces));
            if (cfg.geeForce)
                WriteFile("geeForce", geeForce.ToString(decimalPlaces));
            if (cfg.geeForce_immediate)
                WriteFile("geeForce_immediate", geeForce_immediate.ToString(decimalPlaces));
            if (cfg.heightFromSurface)
                WriteFile("heightFromSurface", FormatAltitude(heightFromSurface, cfg.heightFromSurfaceUnits));

            if (cfg.heightFromTerrain)
                WriteFile("heightFromTerrain", FormatAltitude(heightFromTerrain, cfg.heightFromTerrainUnits));
            if (cfg.horizontalSrfSpeed)
                WriteFile("horizontalSrfSpeed", FormatSpeed(horizontalSrfSpeed, cfg.horizontalSrfSpeedUnits));
            if (cfg.horizontalAcceleration)
            {
                WriteFile("horizontalAcceleration", horizontalAcceleration.ToString(decimalPlaces));
            }

            if (cfg.indicatedAirSpeed)
                WriteFile("indicatedAirSpeed", indicatedAirSpeed.ToString(decimalPlaces));
            if (cfg.landedAt)
                WriteFile("landedAt", landedAt);
            if (cfg.mach)
                WriteFile("mach", mach.ToString(decimalPlaces));
            if (cfg.missionTime)
                WriteFile("missionTime", missionTime.ToString(decimalPlaces));
            if (cfg.obt_speed)
                WriteFile("obt_speed", FormatSpeed(obt_speed, cfg.ship_obtSpeedUnits));

            if (cfg.ApA)
                WriteFile("ApA", FormatAltitude(ApA, cfg.ApA_Units));
            if (cfg.PeA)
                WriteFile("PeA", FormatAltitude(PeA, cfg.PeA_Units));


            if (cfg.vesselName)
                WriteFile("vesselName", Localizer.Format(vesselName));



            if (cfg.KIA)
                WriteFile("KIA", KDead.ToString());
            if (cfg.MIA)
                WriteFile("MIA", KDead.ToString());

            if (cfg.onePerFile == false)
                 WriteLine();
        }
        #endregion

        #region WriteToFiles

        // Using binaryWriter to avoid any serialization issues with Streamwriter
        // more details on binary writing here: https://www.csharpstar.com/create-read-write-binary-file-csharp/

        static string fname;
        static BinaryWriter bw;

        static void WriteFile(string fileName, string value)
        {
            if (cfg.onePerFile == false)
            {
                line += value + cfg.separator;
            }
            else
            {
                if (cfg.filePrefix[0] == '/' || cfg.filePrefix[1] == ':')
                    fname = cfg.filePrefix + fileName + cfg.fileSuffix;
                else
                    fname = cfg.ROOT_PATH + cfg.filePrefix + "." + fileName + cfg.fileSuffix;
                filenames += ":" + fname;
                value += cfg.eol;
                byte[] bytes = Encoding.ASCII.GetBytes(line);

                //create the file

                try
                {
                    bw = new BinaryWriter(new FileStream(fname, FileMode.Create));
                }
                catch (IOException e)
                {
                    Log.Error(e.Message + "\n Cannot create file.");
                    return;
                }
                //writing into the file
                try
                {
                    for (int i = 0; i < value.Length; i++)
                        bw.Write(value[i]);
                }
                catch (IOException e)
                {
                    Log.Error(e.Message + "\n Cannot write to file.");
                    return;
                }
                bw.Close();
            }
        }

        static void WriteLine()
        {
            if (cfg.onePerFile == false)
            {
                filenames = cfg.ROOT_PATH + cfg.filePrefix + cfg.fileSuffix;
                File.AppendAllText(filenames, line + cfg.eol);
            }

            line = "";
        }
#endregion

#region Formatters
        static string FormatSpeed(double speed, bool units)
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

        static string FormatAltitude(double alt, bool units)
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
#endregion

    }
}
