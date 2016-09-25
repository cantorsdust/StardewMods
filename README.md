# TimeSpeed
Slows game clock by a configurable amount in Stardew Valley.

Adjusts the game clock speed by a configurable amount.  Speed up, slow down, or completely freeze time.  Now includes all FreezeInside functionality--it is recommended not to run the two mods together.
By cantorsdust and Syndlig with technical help from Zoryn.

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
INSTALLATION--SMAPI
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

The relevant to your platform release 7z contains three files, TimeSpeed.dll, config.json and manifest.json. These files should be placed in these directories for your relevant platform. Note that these may change based on your setup, future Stardew Valley updates, or future Steam updates.

Windows:
%appdata%\StardewValley\Mods or your main Stardew Valley\Mods folder.

Thus, the total path for both of the two files required for this mod to function are:
%appdata%\StardewValley\Mods\TimeSpeed.dll
AND
%appdata%\StardewValley\Mods\config.json
AND
%appdata%\StardewValley\Mods\manifest.json

Linux
~/.steam/steam/SteamApps/common/Stardew Valley/Mods

OSX
~/Library/Application Support/Steam/SteamApps/common/Stardew Valley/Contents/MacOS
Please run "chflags nohidden ~/Library" to show the Library folder if on a recent version of OSX.

Newest release REQUIRES SMAPI 0.40+ to be installed! Please follow the installation here for Windows:
https://github.com/ClxS/SMAPI/releases
Or here for OSX:
https://github.com/MacLeek/SMAPI/releases
Or here for Linux:
https://github.com/tomkrizan/SMAPI/releases

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
INSTALLATION--STORM
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

STORM support has been dropped.

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
USAGE
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

The speed at which the game clock runs will be changed according to your settings in the config.json file. A default config.json file was provided with the release binary with 
settings that double time. This is not required. You may run the mod once and it will generate a config.json for changing later.

All of the default time change settings below can be configured in the config.json. Please note that it is not intuitive. The config.json values must match their related .NET 
Framework key values located here: https://msdn.microsoft.com/en-us/library/system.windows.forms.keys.aspx
FOR EXAMPLE: A comma "," is not merely the comma value but is instead "OemComma" and this is what must be placed in config.json. See the release config.json for an example.

You can override your freeze time settings and immediately freeze time by pressing N.  Pressing N again restores your previous freeze time settings.

You can halve each of your tick lengths by pressing ,

You can double each of your tick lengths by pressing .

You can restore your original config settings that are stored in your config file by pressing B.

Please note that this game comes with a config.json file with these options:

"OutdoorTickLength": 14,
"IndoorTickLength": 14,
"MineTickLength": 14,
"ChangeTimeSpeedOnFestivalDays": false,
"FreezeTimeOutdoors": false,
"FreezeTimeIndoors": false,
"FreezeTimeInMines": false,
"LetMachinesRunWhileTimeFrozen": false,
"FreezeTimeAt1230AM": false
"TimeFreezeOverrideKey": "N",
"HalfTimeTickLengthKey": "OemComma",
"DoubleTimeTickLengthKey": "OemPeriod",
"RestoreTimeTickLengthKey": "B"

OutdoorTickLength, which defaults to 14. 
Controls the length of the 10 minute tick outdoors. Set to your desired ten minute tick length in seconds. 
The base day is 7 seconds. Setting TenMinuteTickLength to 0 seconds or lower will not make time run backwards ;)

IndoorTickLength, which defaults to 14. 
Controls the length of the 10 minute tick indoors. Set to your desired ten minute tick length in seconds. 
The base day is 7 seconds. Setting TenMinuteTickLength to 0 seconds or lower will not make time run backwards ;)

IndoorTickLength, which defaults to 14. 
Controls the length of the 10 minute tick while in the mine. Set to your desired ten minute tick length in seconds. 
The base day is 7 seconds. Setting TenMinuteTickLength to 0 seconds or lower will not make time run backwards ;)

ChangeTimeSpeedOnFestivalDays, which defaults to false. 
Set to true to enable time changing on festival days. This option is here because some users have reported problems with festival days if time is changed.

FreezeTimeOutdoors, which defaults to false. 
Set to true to freeze time outdoors.

FreezeTimeIndoors, which defaults to false. 
Set to true to freeze time indoors, except for the mines.

FreezeTimeInMines, which defaults to false. 
Set to true to freeze time in the mines.

LetMachinesRunWhileTimeFrozen, which defaults to false. 
If set to true, machines continue to run while you are inside and time is frozen. 
Some consider this "cheaty". Setting it to false will prevent machines from running while you are inside. As a general rule, I recommend this be set to false unless you desire the gameplay change. This option is more likely to break things than any other--although most obvious bugs have been removed.

FreezeTimeAt1230AM, which defaults to false. 
Set to true to freeze time when the day reaches 12:30 AM (24:30 military time). This occurs no matter where you are.

TimeFreezeOverrideKey, which defaults to N. Please note the above note on changing the value based on .NET key bindings if desired to change default.

HalfTimeTickLengthKey, which defaults to a comma ",". Please note the above note on changing the value based on .NET key bindings if desired to change default.

DoubleTimeTickLengthKey, which defaults to a period ".". Please note the above note on changing the value based on .NET key bindings if desired to change default.

RestoreTimeTickLengthKey, which defaults to B. Please note the above note on changing the value based on .NET key bindings if desired to change default.

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
LICENSE
-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

TimeSpeed is licensed under GPL v3.  You will find a copy of its source at the same place you downloaded it, https://github.com/cantorsdust/TimeSpeed

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
