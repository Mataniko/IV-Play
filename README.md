# Overview

IV/Play (pronounced ‘Four Play’) is a desktop oriented GUI front-end for [MAME](http://www.mamedev.org)™. It was designed and commissioned by John IV as an alternative to [MAMEUI](http://www.mameui.info). It has a narrow and particular feature-set, is keyboard driven, and utilises many of the navigation and keyboard short cuts of MAMEUI. It is available as a combo x64/x86 app and is targeted towards Windows 8.1 with leveraged features like Jump List support. IV/Play is decoupled from setting MAME options directly in an effort to future proof and guard against continual core command line changes.

**Naming conventions**: The application name is ‘IV/Play’ but due to WinOS file-naming limitations, ‘IV-Play’ is used where necessary.

<!-- TOC depth:6 withLinks:1 updateOnSave:1 -->
- [Overview](#overview)
- [Requirements](#requirements)
- [Installation](#installation)
- [Resources](#resources)
- [Major Application Functions](#major-application-functions)
	- [Import XML Data from MAME.exe](#import-xml-data-from-mameexe)
	- [Configuration Screen](#configuration-screen)
	- [Display XML Data in UI](#display-xml-data-in-ui)
		- [Title Bar](#title-bar)
		- [Game List](#game-list)
        - [Auditing Game List](#auditing-the-game-list)
		- [Favorites](#favorites)
		- [Icons](#icons)
		- [Art Area](#art-area)
			- [Normal View](#normal-view)
			- [Vertical Stretch View](#vertical-stretch-view)
			- [Super Large View](#super-large-view)
		- [Text Filter Dialogue](#text-filter-dialogue)
		- [Background Selection and Randomization](#background-selection-and-randomization)
		- [Properties View](#properties-view)
	- [Launch Game](#launch-game)
	- [Windows 7 Jump List / Start Menu](#windows-7-jump-list-start-menu)
	- [Accelerator / Navigation Keys](#accelerator-navigation-keys)
- [External Files](#external-files)
	- [IV-Play.cfg](#iv-playcfg)
	- [IV-Play.dat](#iv-playdat)
- [IV/Play vs. MAMEUI](#ivplay-vs-mameui)
- [IV/Play 1.5 Features](#ivplay-15-features)
	- [Unlimited Art Types](#unlimited-art-types)
	- [History.dat / MAMEInfo.dat Support](#historydat-mameinfodat-support)
	- [Favorites Toggle](#favorites-toggle)
	- [Non-Working game art/history Support](#non-working-game-arthistory-support)
	- [Command Line Override](#command-line-override)
- [IV/Play 1.5.3 Feature](#ivplay-153-feature)
	- [Mechanical Games Filter](#mechanical-games-filter)
- [IV/Play 1.5.5 Feature](#ivplay-155-feature)
	- [Filter on input](#filter-on-input)
- [Notes/Hints](#noteshints)
- [Credits](#credits)
<!-- /TOC -->

# Requirements

Windows 8.1 is the preferred platform; Vista and XP are supported without Jump List support. Faster modern processors and solid-state drives help with image/resource loading speed. (a 3.6 GHz C2D loads the app in 1 second). On Windows versions prior to 8, IV/Play may require the [Microsoft .NET Framework 4](http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992&displaylang=en).

# Installation

The easiest installation option is to place the IV/Play files into the top level of an existing MAMEUI directory structure alongside the .exe. IV/Play will utilise the structure’s icons, art types, and bkground folders. Baseline MAME binaries can be used instead of MAMEUI if desired; though IV/Play can link to either one. IV/Play does not rely on registry settings or an install so it is easily portable on a external media like thumb drive with MAME and attendant files.

# Resources

The current build, icons, and snapshots for IV/Play are found at its [homepage](http://www.mameui.info).

# Major Application Functions

IV/Play is, at its core, a game launcher. It depends on the presence of a MAME.exe to import its XML (populating its list view and properties), and to pass back its game launch command. IV/Play is designed to be as low maintenance as possible and will not directly set MAME options.

## Import XML Data from MAME.exe

IV/Play resides in the same directory as the MAME executable or in its own directory. Upon initial launch it searches the local directory for a MAME version. Failing to find one a dialogue box appears asking to locate the MAME.exe to ‘link’. It then grabs the output of the (currently) 174MB MAME XML file (MAME.exe –listxml). **F5** refreshes the XML using the same named MAME.exe, (new version released for example) while **F4** re-opens the dialogue box to select a different MAMExx.exe. **Note**: IV/Play can utilise any version of MAME that produces the XML output needed, including MAMEUI32 or MAMEUI64, the various MAME .exes are used interchangeably in this document.

## Configuration Screen

**F1** brings up the configuration screen. The UI settings are stored in the IV-Play.cfg file.

## Display XML Data in UI

### Title Bar

The title bar displays the version of MAME linked along with the number of games supported in that version and favorites in use. The number of favorites displayed will change if the favorites and filter are used.

### Game List

The game list displays all the games from the MAME XML output. Clones are automatically offset from their parents. The font is selectable as is the color of the parents, clones, and favorites. Navigation through the list is via regular direction keys like **up**, **down**, **page** **up** **page** **down**. **Left** or **right**, will move one letter’s worth of games at a time and loop back through.

Display of the year and manufacturer is configurable through **F1**.

Typing ‘donkey kong’ in the game list goes to the first instance depending where focus is when typing; if there are no instances downward it will go to the first instance of ‘donkey kong’ which may be all the way at the top in the favorites.

### Auditing the Game List

By default, IV/Play will display all roms, whether they exist in the roms folder or not.

To limit the display to only those roms which are available and playable, place a MAME_g.ini file in the same directory as IV-Play.exe. The file MAME_g.ini is produced by MameUI64 after running an audit of all games.

To produce this file, follow these steps:

1. Run MameUI64.
2. Select File->Audit all games.
3. Copy MAME_g.ini into the same folder as IV-Play.exe.

IV-Play will then limit the game list to only games that are available for play.

### Favorites

IV/Play supports the use of a .\\favorites.ini file. This is a text file with one game per line, for example:

10yard
1941
dkong

Favorites are toggled by **Alt-D** and can be set in the **F1** configuration screen. The favorites are displayed prior to the MAME generated game list in alphabetical order. The icons are not offset for favorites to avoid confusion if a clone is added as a favorite and its parent is not there. The favorites can be displayed in different colors than the regular game list (set via **F1** in the configuration dialogue). The **Alt-D** toggle will cycle through favorites off, favorites + game list, favorites by themselves.

Favorites are added by **Ctrl-D** from the game list and removed by **Ctrl-D** from the favorites area.

**Up**/**Down**/**Left**/**Right**/**Page Up**/**Page Down** behave the same way they do in the game list. A single press of **down** **arrow** on the last favorite goes to the first regular game list item. However, the **Home** and **End** buttons on the keyboard make two stops: If focus is in regular game list, hitting home goes to the top of the regular game list. Hitting **Home** again goes to the top of the favorites. Likewise if focus is in favorites, hitting **End** goes to the end of the favorites, hitting **End** a second time will go to the end of the regular game list.

### Icons

Each game supported by MAME can have an associated, freestanding \*.ico in the .\\icons directory. The game list icons display in 16x16.

If Icon Zoom is turned on in **F1** configuration screen, when a game is highlighted, the icon will increase in size to its native 32x32.

If a game is displayed for which there is no parent icon, then the default blank white icon will be used. A clone will take its own icon or if not present, use its parent. If the parent is not present, the clone icon will be light yellow. Non-working games can take their own icon, then a freestanding icon called .\\icons\\nonworking.ico, if that is not present, it will display the built in maroon icon.

If ‘Draw non-working overlay’ is toggled on in the configuration menu , a small red circle slash will be overlaid on any non-working game.

### Art Area

The upper right of the game list is the art area. The art area displays the current art type for the selected game. These are freestanding \*.png files that reside in their respective directories and take the name of each game, e.g. dkong.png.

**Alt-1 through Alt-0** toggles the various art types (which are added through the **F1** configuration dialogue).

Opacity for the art types is set in **F1** configuration dialogue.

The art views toggle with **Alt-P**. Each press of **Alt-P** cycles through: Normal, Vertical Stretch, and Super Large view.

The border width and color for the art can be set with **F1**.

Left clicking on the art area with the cursor toggles the art type. Right clicking on the art area toggles through the art views.

A .\\snap\\nonworking.png displays if the game is flagged as \<emulation status !=”good”\> in the XML.

If a game is displayed for which there is no parent snapshot (new version of MAME released prior to a snapshot pack), then nothing displays in the art area, the bkground.png will show through.

IV/Play supports history.dat and MAMEInfo.dat. They are searched for in the MAME.exe directory on initial IV/Play launch and can be added at any time through the **F1** configuration menu. Navigation through the entries are via **Ctrl-Page Up**, **Ctrl-Page Down**, **Ctrl-Up**, **Ctrl-Down**, **Ctrl-Home**, **Ctrl-End**. A separate font with size and color is selectable for the text area on the **F1** configuration menu.

#### Normal View

Normal view is the default view. Snapshots are placed in the upper right of the window offset from the edge. Flyers (since they are typically very tall) are automatically scaled to fit the vertical height of the window with the same offset. If art is wider than 66% (control panels) of the horizontal width of the window, it will be scaled to 66% ala Super Large View. Border width and color are set in the **F1** screen.

#### Vertical Stretch View

This setting vertically scales art items with aspect ratio intact to full height of the current window with an offset. Flyers scale the same way they do in Normal View. Border width and color are set in the **F1** screen.

#### Super Large View

The Super Large view scales the snapshot or flyer to 66% the horizontal width of the window while retaining aspect ratio, thus cropping out the bottom portion.

### Text Filter Dialogue

The Text Filter dialogue box is accessible with either **Ctrl-F** or **Ctrl-E**. When typing in this field and hitting enter it will search and display resulting hits, like a filter. For example, typing ‘donk’ would bring up Donkey Kong and all other items with ‘donk’ in its description including something like ‘Crazy Donkey’.

The filter can also take manufacturer, year, and driver source for the game, i.e. ‘dkong.c’. Filtered game list is maintained until the filter is cleared. The filter includes favorites if toggled on. The dialogue can be dismissed with **ESC**. Filtered result counts are displayed on the title bar.

### Background Selection and Randomization

The bottom art layer of the main list area takes a user defined background \*.png image from the .\\bkground\\ directory. The default background is .\\bkground\\bkground.png. If this file is not present, the background is the built-in green gradient. The image will tile (from upper left) if it does not exactly fit the current dimensions of the window. The **F1** configuration dialogue toggles the background image rotation, so IV/Play will use a random \*.png from the .\\bkground directory on each launch. **Note**: If bkground image is ‘larger’ than current game window it will not scale, it will be drawn into the upper left corner and allowed to crop. If bkground image is smaller than current game window, it will tile from upper left. To prevent tiling on a maximized IV/Play window, use a background image the same size the monitor resolution, i.e. 1920x1080.

### Properties View

**Alt-ENTER** displays the properties dialogue for the selected game. It can be dismissed with the **ESC** key.

## Launch Game

Launching a game via Enter or double-clicking sends the simple argument ‘MAME.exe dkong’ to the linked MAME.exe. The configuration for MAME is done using its own \*.ini files.

## Windows 7 Jump List / Start Menu

IV/Play can be pinned to the taskbar in Windows 7. The icon will display a list of recently played games when right-clicked. The recent games can then be pinned. IV/Play can also be pinned to the Start Menu and the recent games will fly out to the right when selected. It is necessary to start and quit some games before they begin to show up on the Jump List.

## Accelerator / Navigation Keys

The following are the keyboard shortcuts in IV/Play:

| **Alt-1 through Alt-0**           | Go directly to art type. (left click on art area)                                                                                                                 |
|-----------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Alt-D**                         | Toggle the display of favorites.ini. (off, favorites + games, favorites only).                                                                                    |
| **Alt-Enter**                     | Display properties for the game selected.                                                                                                                         |
| **Alt-P**                         | Toggle art views: Normal / Vertical Stretch / Super Large. (right click on art area)                                                                              |
| **Arrow Left / Right**            | Navigate one letters worth of games.                                                                                                                              |
| **Arrow Up / Down**               | Navigate one game at a time.                                                                                                                                      |
| **Ctrl-Backspace**                | Clears progressive game list text filter if toggled on with F1.                                                                                                   |
| **Ctrl-D**                        | Add or remove a favorite from favorites.ini.                                                                                                                      |
| **Ctrl-F / Ctrl-E**               | Open text filter dialogue.                                                                                                                                        |
| **Ctrl-R**                        | Select a random game.                                                                                                                                             |
| **Ctrl-Up / Ctrl-Down**           | Move one line through History.dat or MAMEInfo.dat                                                                                                                 |
| **Ctrl-Page UP / Ctrl-Page Down** | Move one page worth of text through History.dat or MAMEInfo.dat                                                                                                   |
| **Ctrl-Home / Ctrl-End**          | Move to the beginning and end of History.dat or MAMEInfo.dat                                                                                                      |
| **F1**                            | Display the configuration dialogue.                                                                                                                               |
| **F4**                            | Select the MAME.exe.                                                                                                                                              |
| **F5**                            | Refresh the XML import from MAME.exe                                                                                                                              |
| **Home / End**                    | Move to the beginning / end of the game list, or as a stop at the beginning and end of the favorites (press home/end a second time to navigate out of favorites). |
| **Page Up / Page Down**           | Navigate one window height worth of games.                                                                                                                        |

# External Files

IV/Play.exe can reside in its own folder or along with MAME.exe

C:\\games\\IV-Play\\IV-Play.exe
C:\\games\\IV-Play\\IV-Play.cfg
C:\\games\\IV-Play\\IV-Play.dat
C:\\games\\IV-Play\\IV-Play User Guide.pdf
C:\\games\\IV-Play\\Favorites.ini

Once linked to a MAME.exe, IV/Play will use its (MAMEUI’s) structure for its art.

C:\\games\\MAME\\MAMEUI64.exe
C:\\games\\MAME\\History.dat
C:\\games\\MAME\\MAMEInfo.dat
C:\\games\\MAME\\INI\\MAME.ini
C:\\games\\MAME\\icons\\*gamename*.ico
C:\\games\\MAME\\snap\\*gamename*.png
C:\\games\\MAME\\flyers\\*gamename*.png
C:\\games\\MAME\\cabinets\\*gamename*.png
C:\\games\\MAME\\PCBs\\*gamename*.png
C:\\games\\MAME\\marquees\\*gamename*.png
C:\\games\\MAME\\titles\\*gamename*.png
C:\\games\\MAME\\cabinets\\*gamename*.png
C:\\games\\MAME\\bkground\\*bkground*.png

## IV-Play.cfg

This file is created on first launch of IV/Play if it is not present. It contains the configuration settings for the application. Delete this file to start fresh.

## IV-Play.dat

The DAT file is created from the contents of the XML output produced by the MAME engine. It is created on first run if not present after asking for the .exe. If IV/Play is run without the \*.dat in its directory it will ask for a location of the MAME.exe and use that path in the IV-Play.cfg file. **F5** refreshes the XML and updates the \*.dat. **F4** re-links to a new MAMExx.exe. Delete this file to start fresh.

# IV/Play vs. MAMEUI

[MAMEUI](http://www.mameui.info/) is an integrated UI to [MAME](http://www.mamedev.org) and is compiled using the base source. It leverages a lot of the underlying MAME code and depends on it to produce its UI and functionality. This is its strength and weakness. As MAMEUI has matured (b. 1997), the original development team has waned and it often takes a long time to catch up with the sweeping core changes that periodically take place in the MAME engine. While the number of supported arcade games and gambling ‘Fruit machines’ has neared 30,000+ the UI has slowed down when all the icons and screenshots are processed by the 18 year old controls; this has also introduced memory leaks over time. Recent changes to the core have also resulted in huge memory footprints and 5x longer load times.

IV/Play was born out of the desire to continue a similar interface to MAMEUI without the attendant issues of being coupled to the core.

# IV/Play 1.5 Features

## Unlimited Art Types

IV/Play allows for the creation of ‘unlimited’ art types. In the **F1** configuration dialogue, any directory can be added that contains art types following the *gamename*.png format. IV/Play will automatically add snap, flyers, cabinets, PCB, marquees, cpanel, and titles on initial launch if it finds them. The view order can be set my moving the folders up or down the list. The art types will be assigned **Alt-X** shortcut keys depending on position, **Alt-1** through **Alt-0**.

## History.dat / MAMEInfo.dat Support

IV/Play supports history.dat and MAMEInfo.dat. They are searched for in the MAME.exe directory on initial IV/Play launch and can be added at any time through the **F1** configuration menu. Navigation through the entries are via **Ctrl-Page Up**, **Ctrl-Page Down**, **Ctrl-Up**, **Ctrl-Down**, **Ctrl-Home**, **Ctrl-End**. A separate font with size and color is selectable for the text area on the **F1** configuration menu.

## Favorites Toggle

Favorites can be cycled with **Alt-D** through off, on with game-list, and favorites only.

## Non-Working game art/history Support

IV/Play will display icons and any art type for non-working games if they are present. If they are not, nonworking.png and nonworking.ico are used. If those are not present then the built-in icons are used. This also allows viewing of history.dat and MAMEInfo.dat entries for non-working games.

## Command Line Override

The command line override in the **F1** configuration dialogue allows the use of various switches to be added to launched games, e.g. –window to play games windowed without having to drop to the mame.ini for editing.

# IV/Play 1.5.3 Features

## Mechanical Games Filter

The F1 configuration dialogue contains a check box for hiding non-working mechanical games. These are games that have been added to MAME that contain mechanical or physical elements that cannot currently be emulated; including pinball games and gambling fruit/slot machines. By toggling this, 8000+ non-working mechanical games are removed from the game list.

# IV/Play 1.5.5 Features

## Filter on input

The F1 configuration contains an option to filter on input. This will display progressive results directly in the game list by typing the game name, year, manufacturer, or source file. Backspace will delete one character at a time and CTRL-backspace will clear the typed filter. The progressive filtered text will appear on the titlebar. (Note this produces the same functional results as doing a CTRL-F filter search).

# IV/Play 1.6.0 Features

Update IV/Play to work with MAME 0.162. IV/Play is now also open source under the MIT License. For more information refer to the LICENSE file.

# Notes/Hints

-   The initial IV/Play dimensions are 1015x432, a 2.35 aspect ratio in honor of [TohoScope](http://en.wikipedia.org/wiki/TohoScope), used for Godzilla movies.
-   If games are not populating the Jump List, verify on the start menu properties dialogue for Windows that ‘Store and display recently opened items in the Start menu and the taskbar’ is checked.
-   Left and Right arrows navigate a letter’s worth of games.
-   Typing out the full name of a game navigates there.

# Credits

John L. Hardy IV / Design & Test / [Forum](http://www.mameworld.info/ubbthreads/showflat.php?Cat=&Number=269101&page=0&view=collapsed&sb=5&o=&fpart=1&vc=1&new=1321816604#Post269153)

Matan Bareket / Development
