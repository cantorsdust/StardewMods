/*
    Copyright 2016 cantorsdust

    TimeSpeed is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TimeSpeed is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TimeSpeed.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
//using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Threading;
using System.IO;
using StardewValley;
//using System.Configuration;
//using System.Web.Script.Serialization;

namespace TimeSpeed
{
    public class TimeSpeed : Mod
    {
        public static ModConfig TimeSpeedConfig { get; private set; }

        public bool TimeFreezeOverride = false;
        public bool oldFreezeTimeOutdoors = false;
        public bool oldFreezeTimeIndoors = false;
        public bool oldFreezeTimeInMines = false;


        public double timeCounter = 0;
        public double lastGameTimeInterval = 0;
        public int lasttime = 600;

        public override void Entry(params object[] objects)
        {
            runConfig();
            Console.WriteLine("TimeSpeed Has Loaded");
            ControlEvents.KeyPressed += Events_KeyPressed;
            TimeEvents.DayOfMonthChanged += Events_NewDay;
            TimeEvents.TimeOfDayChanged += Events_TimeChanged;
            GameEvents.UpdateTick += Events_UpdateTick;


        }

        void runConfig()
        {
            TimeSpeedConfig = new ModConfig().InitializeConfig(BaseConfigPath);
        }

        public void Events_NewDay(object sender, EventArgs e)
        {
            Game1.gameTimeInterval = 0;
            timeCounter = 0;
            lastGameTimeInterval = 0;
        }

        void Events_UpdateTick(object sender, EventArgs e)
        {
            GameLocation location = Game1.currentLocation;
            int time = Game1.timeOfDay;

            if (!TimeSpeedConfig.LetMachinesRunWhileTimeFrozen &&
            (location != null) &&
            ((location.isOutdoors && TimeSpeedConfig.FreezeTimeOutdoors) ||
            (!location.isOutdoors && !location.name.Equals("UndergroundMine") && TimeSpeedConfig.FreezeTimeIndoors) ||
            (!location.isOutdoors && location.name.Equals("UndergroundMine") && TimeSpeedConfig.FreezeTimeInMines) ||
            (time >= 2430 && TimeSpeedConfig.FreezeTimeAt1230AM)))
            {
                Game1.gameTimeInterval = 0;
                //Console.WriteLine("Freeze Time without machines requirements met");
            }
            else if (Game1.currentSeason != null &&
            Game1.currentLocation != null &&
            (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) || TimeSpeedConfig.ChangeTimeSpeedOnFestivalDays))
            {
                //Console.WriteLine("Freeze Time without machines requirements NOT met");
                timeCounter += Math.Abs((Game1.gameTimeInterval - lastGameTimeInterval));
                double proportion;

                if (Game1.currentLocation.isOutdoors)
                {
                    proportion = Math.Abs(7 * timeCounter / TimeSpeedConfig.OutdoorTickLength);
                }
                else if (Game1.currentLocation.name == "UndergroundMine")
                {
                    proportion = Math.Abs(7 * timeCounter / TimeSpeedConfig.MineTickLength);
                }
                else
                {
                    proportion = Math.Abs(7 * timeCounter / TimeSpeedConfig.IndoorTickLength);
                }


                Game1.gameTimeInterval = Convert.ToInt32(proportion);
                lastGameTimeInterval = Game1.gameTimeInterval;
                /*
                Console.WriteLine(Game1.gameTimeInterval.ToString("g"));
                Console.WriteLine(lastGameTimeInterval.ToString("g"));
                Console.WriteLine(timeCounter.ToString("g"));
                Console.WriteLine(proportion.ToString("g"));
                */
            }
        }

        void Events_TimeChanged(object sender, EventArgsIntChanged e)
        {
            //Console.WriteLine("Firing Pre10MinuteClockUpdateCallback");
            var location = Game1.currentLocation;
            timeCounter = 0;
            lastGameTimeInterval = 0;
            Game1.gameTimeInterval = 0;
            int time = Game1.timeOfDay;

            if (location != null)
            {
                //Console.WriteLine("Location name is: " + location.name);
                //Console.WriteLine("Location is outdoors is: " + location.isOutdoors.ToString());
            }
            //Console.WriteLine("time is " + time.ToString("G"));
            /*  REQUIREMENTS FOR FREEZING TIME
                IF (location is not null), AND,
                IF the difference between time and lasttime is not more than 10 minutes, 
                AND one of the following is true:
                    IF location is outdoors and FreezeTimeOutdoors == true, OR
                    IF location is not outdoors, not named "UndergroundMine", and FreezeTimeIndoors == true, OR
                    IF location is not outdoors, named "UndergroundMine", and FreezeTimeInMines == true, OR
                    IF the time is 12:30 AM and FreezeTimeAt1230AM == true
            */
            if (TimeSpeedConfig.LetMachinesRunWhileTimeFrozen &&
                (location != null) &&
                (Math.Abs(time - lasttime) <= 10) &&
                ((location.isOutdoors && TimeSpeedConfig.FreezeTimeOutdoors) ||
                (!location.isOutdoors && !location.name.Equals("UndergroundMine") && TimeSpeedConfig.FreezeTimeIndoors) ||
                (!location.isOutdoors && location.name.Equals("UndergroundMine") && TimeSpeedConfig.FreezeTimeInMines) ||
                (time >= 2430 && TimeSpeedConfig.FreezeTimeAt1230AM)))
            {
                //Console.WriteLine("location requirements met, resetting time");
                Game1.timeOfDay -= (time % 100 == 0) ? 50 : 10;
                //Console.WriteLine("resetting time to: " + time.ToString("G"));
            }
            else
            {
                lasttime = time;
                //Console.WriteLine("location requirements not met, time advancing normally");
                //Thread.Sleep(5000);
            }
        }

        void Events_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            //Console.WriteLine("Key Pressed: " + e.KeyPressed.ToString());

			// Default N toggles freeze time override, freezing time everywhere.  hitting it again restores old values.
			if (e.KeyPressed.ToString().Equals(TimeSpeedConfig.TimeFreezeOverrideKey))
            {
                if (!TimeFreezeOverride)
                {
                    TimeFreezeOverride = true;
                    oldFreezeTimeIndoors = TimeSpeedConfig.FreezeTimeIndoors;
                    oldFreezeTimeInMines = TimeSpeedConfig.FreezeTimeInMines;
                    oldFreezeTimeOutdoors = TimeSpeedConfig.FreezeTimeOutdoors;
                    TimeSpeedConfig.FreezeTimeOutdoors = true;
                    TimeSpeedConfig.FreezeTimeIndoors = true;
                    TimeSpeedConfig.FreezeTimeInMines = true;
                }
                else
                {
                    TimeFreezeOverride = false;
                    TimeSpeedConfig.FreezeTimeOutdoors = oldFreezeTimeOutdoors;
                    TimeSpeedConfig.FreezeTimeIndoors = oldFreezeTimeIndoors;
                    TimeSpeedConfig.FreezeTimeInMines = oldFreezeTimeInMines;
                }
            }
            // Default , halves current tick lengths
			else if (e.KeyPressed.ToString().Equals(TimeSpeedConfig.HalfTimeTickLengthKey))
            {
                TimeSpeedConfig.OutdoorTickLength = (TimeSpeedConfig.OutdoorTickLength / 2);
                TimeSpeedConfig.IndoorTickLength = (TimeSpeedConfig.IndoorTickLength / 2);
                TimeSpeedConfig.MineTickLength = (TimeSpeedConfig.MineTickLength / 2);
            }
            // Default . doubles current tick lengths
			else if (e.KeyPressed.ToString().Equals(TimeSpeedConfig.DoubleTimeTickLengthKey))
            {
                TimeSpeedConfig.OutdoorTickLength = (TimeSpeedConfig.OutdoorTickLength * 2);
                TimeSpeedConfig.IndoorTickLength = (TimeSpeedConfig.IndoorTickLength * 2);
                TimeSpeedConfig.MineTickLength = (TimeSpeedConfig.MineTickLength * 2);
            }
            // Default B restores tick lengths to the original ini values
			else if (e.KeyPressed.ToString().Equals(TimeSpeedConfig.RestoreTimeTickLengthKey))
            {
                TimeSpeedConfig = TimeSpeedConfig.ReloadConfig();
            }
        }
    }

    public class ModConfig : Config
    {
        public float OutdoorTickLength { get; set; }
        public float IndoorTickLength { get; set; }
        public float MineTickLength { get; set; }
        public bool ChangeTimeSpeedOnFestivalDays { get; set; }
        public bool FreezeTimeOutdoors { get; set; }
        public bool FreezeTimeIndoors { get; set; }
        public bool FreezeTimeInMines { get; set; }
        public bool LetMachinesRunWhileTimeFrozen { get; set; }
        public bool FreezeTimeAt1230AM { get; set; }
		public string TimeFreezeOverrideKey { get; set; }
		public string HalfTimeTickLengthKey { get; set; }
		public string DoubleTimeTickLengthKey { get; set; }
		public string RestoreTimeTickLengthKey { get; set; }

        public override T GenerateDefaultConfig<T>()
        {
            OutdoorTickLength = 14;
            IndoorTickLength = 14;
            MineTickLength = 14;
            ChangeTimeSpeedOnFestivalDays = false;
            FreezeTimeOutdoors = false;
            FreezeTimeIndoors = false;
            FreezeTimeInMines = false;
            LetMachinesRunWhileTimeFrozen = false;
            FreezeTimeAt1230AM = false;
			TimeFreezeOverrideKey = "N";
			HalfTimeTickLengthKey = "OemComma";
			DoubleTimeTickLengthKey = "OemPeriod";
			RestoreTimeTickLengthKey = "B";

            return this as T;
        }
    }
}
