﻿[← back to readme](README.md)

# Release notes
## Upcoming release
* Added 'Freeze time before 2am' option (thanks to MickeyMik!).
* Manual unfreezes now work in a more intuitive way (thanks to MickeyMik!).  
  _When you manually unfreeze time, that now only applies for the current freeze reason. For example, if you unfreeze time after a freeze-at-time and then move to a location where time should be frozen, time will correctly re-freeze since you overrode freeze-at-time but not freeze-at-location._
* Improved the 'freeze time at' config UI.  
  _It now increments in 10-minute intervals, doesn't show invalid times, and formats the displayed time._
* Improved translations. Thanks to SummerFleur2997 (updated Chinese)!

## 2.7.10
Released 11 January 2025 for SMAPI 4.1.10 or later. Updated by Pathoschild.

* Fixed edge cases like very slow time speeds causing time to stop instead.
* Fixed extra space in 'tick length set to' logs.
* Added verbose-only logs to monitor how the mod is changing time.
* Improved translations. Thanks to TwelveYO21 (added Japanese)!

## 2.7.9
Released 04 November 2024 for SMAPI 4.1.2 or later. Updated by Pathoschild.

* Fixed the previous update being broken on Linux/macOS.

## 2.7.8
Released 04 November 2024 for SMAPI 4.1.0 or later. Updated by Pathoschild.

* Updated for Stardew Valley 1.6.9.
* Improved translations. Thanks to StephHoel (updated Portuguese)!

## 2.7.7
Released 19 March 2024 for SMAPI 4.0.0 or later. Updated by Pathoschild.

* Updated for Stardew Valley 1.6.
* Improved translations. Thanks to Orkamaial (added Italian)!

## 2.7.6
Released 25 June 2023 for SMAPI 3.13.0 or later. Updated by Pathoschild.

* Embedded `.pdb` data into the DLL, which fixes error line numbers in Linux/macOS logs.

## 2.7.5
Released 09 January 2023 for SMAPI 3.13.0 or later. Updated by Pathoschild.

* Updated for the upcoming Generic Mod Config Menu 2.0.0.
* Improved translations. Thanks to Timur13240 (updated Russian)!

## 2.7.4
Released 10 October 2022 for SMAPI 3.13.0 or later. Updated by Pathoschild.

* Improved translations. Thanks to Timur13240 (added Russian)!

## 2.7.3
Released 20 August 2022 for SMAPI 3.13.0 or later. Updated by Pathoschild.

* Re-enabled keybinds when viewing a cutscene, except when a textbox is accepting input.
* Improved translations. Thanks to Puffeeydii (updated Chinese) and tqdv (added French)!

## 2.7.2
Released 25 May 2022 for SMAPI 3.13.0 or later. Updated by Pathoschild.

* Reduced the max seconds-per-minute in the config UI from 60 to 15.
* The keybinds are now ignored while viewing a cutscene.
* Improved translations. Thanks to Dracoeris (updated Spanish)!

## 2.7.1
Released 27 February 2022 for SMAPI 3.13.0 or later. Updated by Pathoschild.

* Improved translations. Thanks to neorelicon (added German) and wally232 (added Korean)!

## 2.7.0
Released 10 January 2022 for SMAPI 3.13.0 or later. Updated by Pathoschild.

* Added option to exclude specific location names from time freeze.

## 2.6.0
Released 29 December 2021 for SMAPI 3.13.0 or later. Updated by Pathoschild.

* Updated for Stardew Valley 1.5.5.
* Added in-game config UI via [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098).
* Overhauled configuration in `config.json`:
  * Time speed is now measured in seconds/minute (instead of seconds/10-minutes) for simplicity.
  * Split `TickLengthByLocation` into `SecondsPerMinute` and `FreezeTime` fields for clarity.
  * Removed `DefaultTickLength` (no longer needed with above changes).
* Improved translations. Thanks to Pedrowser (added Portuguese), xingmot (added Chinese), and Yllelder (added Spanish)!

**Update note:**  
If you previously customized the `TickLengthByLocation` or `DefaultTickLength`, you'll need to
reconfigure the mod using the new options.

## 2.5.1
Released 27 November 2021 for SMAPI 3.12.5 or later. Updated by Pathoschild.

* Fixed the default time speed in the Skull Caverns being faster than in the base game.
* Fixed `FreezeTimeAt` option not applied/unapplied when you change the time of day manually.
* Fixed `FreezeTimeAt` option not applied if you set a time which isn't a multiple of 10 (like `2205`).
* Fixed `FreezeTimeAt` option not resuming time when the next day starts.

## 2.5.0
Released 26 March 2020 for SMAPI 3.9.5 or later. Updated by Pathoschild.

* Added [multi-key binding](https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings#Multi-key_bindings) support.
* Added translation support.
* Added options to configure time speed for the `SkullCavern`, `VolcanoDungeon`, and [`DeepWoods`](https://www.nexusmods.com/stardewvalley/mods/2571) areas.
* You can now change time during events.
* Updated for split-screen mode in Stardew Valley 1.5.
* The mod is now enabled on festival days by default. (This doesn't affect players who already have a `config.json` file.)
* Fixed compatibility with [unofficial 64-bit mode](https://stardewvalleywiki.com/Modding:Migrate_to_64-bit_on_Windows).

**Breaking changes:**
* The `Mine` option in `TickLengthsByLocation` no longer applies to the Skull Cavern (see `SkullCavern` for that).

## 2.4.3
Released 26 November 2019 for SMAPI 3.0 or later. Updated by Pathoschild.

* Updated for Stardew Valley 1.4.

## 2.4.2
Released 27 December 2018 for SMAPI 2.9.3 or later. Updated by Pathoschild.

* Updated for SMAPI 3.0.

## 2.4.1
Released 01 September 2018 for SMAPI 2.8 or later. Updated by Pathoschild.

* Changed the default time speed to match the game, except indoors where it runs at half-speed. (Previously time ran at half-speed everywhere by default.)

## 2.4.0
Released 09 August 2018 for SMAPI 2.6 or later. Updated by Pathoschild.

* Updated for Stardew Valley 1.3.

## 2.3.0
Released 25 January 2018 for SMAPI 2.4 or later. Updated by Pathoschild.

* Updated to SMAPI 2.4.
* Added automatic update checks via SMAPI.
* Added controller support.
* Added `Mine` as an option in per-location settings.

## 2.2.1
Released 02 June 2017 for SMAPI 1.14 or later. Updated by Pathoschild.

* Updated to SMAPI 1.14.

## 2.2.0
Released 24 April 2017 for SMAPI 1.10 or later. Updated by Pathoschild.

* Updated for Stardew Valley 1.2.

## 2.1.0
Released 05 April 2017 for SMAPI 1.9 or later. Updated by Pathoschild.

* Updated to SMAPI 1.9.
* Internal refactoring.

## 2.0.3
Released 29 January 2017 for SMAPI 1.7 or later.

* Fixed hotkeys being processed when a menu is open.

## 2.0.2
Released 26 December 2016 for SMAPI 1.4 or later.

* Fixed hotkeys being processed before game is loaded.
* Fixed an issue when time is frozen between ticks.

## 2.0.1
Released 25 October 2016 for SMAPI 0.40 or later.

* Added Linux/Mac support.
* Fixed config resetting & parsing.

## 2.0.0
Released 23 October 2016 for SMAPI 0.40 or later.

* Internal refactoring.

## 1.9.5
Released 21 October 2016 for SMAPI 0.40 or later.

* Fixed time scaling in game areas that have a different clock rate.
* Fixed an issue with festival time scaling.

## 1.9.4
Released 21 October 2016 for SMAPI 0.40 or later.

* Internal refactoring.
* Added hotkey to reload settings from `config.json`.

## 1.9.3
Released 20 October 2016 for SMAPI 0.40 or later.

* Added smoother time flow changes.
* Added validation to prevent invalid time flow changes.

## 1.9.2
Released 19 October 2016 for SMAPI 0.40 or later.

* Added option to disable time changes on festival days.
* Added support for holding `Alt` to change time flow by 0.1-second intervals.

## 1.9.1
Released 18 October 2016 for SMAPI 0.40 or later.

* Added notifications when time is frozen or unfrozen.

## 1.9.0
_(Lost to the sands of time.)_

## 1.8.0
Released 23 March 2016 for SMAPI 0.39 or later.

* Updated for SMAPI 0.39.

## 1.7.0
Released 22 March 2016 for SMAPI 0.38 or later.

* Updated for SMAPI 0.38.
* Migrated ini file to `config.json`.
* Added hotkeys to freeze/unfreeze/speed up/slow down time, and reset settings.
* Added support for holding `Shift` to change time flow by 10-second intervals.

## 1.6.0
Released 17 March 2016 for SMAPI 0.37.3 or later.

* Added options to freeze time automatically based on location.
* Added smoother time slowing.
* Fixed overflow bug for long tick lengths.

## 1.5.0
_(Storm API release.)_

## 1.4.2
Released 06 March 2016 for SMAPI 0.37.1 or later.

* Fixed fast time bug if user updated from 1.2 without updating config.

## 1.4.1
Released 05 March 2016 for SMAPI 0.37 or later.

* Fixed config being read from wrong path in some cases.

## 1.4.0
_(Test release.)_

## 1.3.0
Released 05 March 2016 for SMAPI 0.37 or later.

* Updated for SMAPI 0.37.
* Fixed config being autocreated with the wrong field name.

## 1.2.0
Released 03 March 2016 for SMAPI 0.35 or later.

* Renamed config setting for clarity.

## 1.1.0
Released 03 March 2016 for SMAPI 0.35 or later.

* Added support for speeding up and slowing down time.

## 1.0.0
Released 03 March 2016 for SMAPI 0.35 or later.

* Initial release.
