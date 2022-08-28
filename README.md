# percentage-plus
This is a fork of https://github.com/kas/percentage .

See your battery percentage in the Windows 10 or Windows 11 system tray.

## Features

Compared to the original project, percentage-plus contains the additional features:  

  - Updated the project to .NET 6 in order to make it future-proof and correctly work with Windows 11  
  - Display the remaining time in tray tooltip<br />
  ![](https://raw.githubusercontent.com/jschw/percentage-plus/master/images/tooltip.jpg)  
  - Support for multiple batteries installed (e.g. some Lenovo ThinkPads has this option)  
  - Added left-click and context menus for tablet users without a pointing device which can hover the icon to display the tooltip<br />
  ![](https://raw.githubusercontent.com/jschw/percentage-plus/master/images/left_click.jpg)
  Context menu:<br />
  ![](https://raw.githubusercontent.com/jschw/percentage-plus/master/images/context.jpg)    
  - Customizable display options which are stored persistently<br />
  ![](https://raw.githubusercontent.com/jschw/percentage-plus/master/images/settings.jpg)  
  - Hotkey support to display the left-click menu at the mouse pointer position
  - Display detailed information about installed batteries<br />
  ![](https://raw.githubusercontent.com/jschw/percentage-plus/master/images/battery_infos.jpg)

## Installation

1. [Download the latest release](https://github.com/jschw/percentage-plus/releases)
   - Choose the version Win64_independent if you <b>do not have</b> .NET Runtime 6 installed
   - Choose the version Win64_DotNet6 if <b>have</b> .NET Runtime 6 installed
   - you can download the .NET 6 Runtime at: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
2. Put it in a folder of your choice (I recommend `C:\ProgramFiles\percentage-plus`)
3. To autostart the application, you have three options:
   - Put percentage.exe in your startup folder. To get to your startup folder, press Windows+R, type `shell:startup`, then press enter.
   - Create a new registry string in `HKEY_CurrentUser/Software/Microsoft/Windows/CurrentVersion/Run`. The value of this string entry has to be the absolute path to percentage-plus.exe .
   - Use the Windows Task Scheduler and create a new task which runs the app after user login (see: https://docs.microsoft.com/de-de/windows/win32/taskschd/using-the-task-scheduler)

<b>Note:</b> The application uses a settings file in `C:\Users\username\AppData\Roaming\percentage-plus` which will be newly created at first start. If problems with the settings occur after an update, you can try deleting this file. The user who starts the app also needs to have write permissions to this folder.

## Compiling

This project was compiled with Visual Studio Community 2022.

Select ".NET desktop development" when setting up Visual Studio.

To build the project
1. Open the percentage-plus/percentage-plus.sln file with Visual Studio
2. Click "Build > Build Solution"
3. percentage-plus.exe can be found at `percentage-plus\src\bin\Debug\net6.0-windows\percentage-plus.exe`

## Known Bugs
:beetle: No hot-plug support for multi-battery systems. The application has to be restarted if a battery was removed or installed.  
:beetle: On multi-battery systems: The percentage in windows settings can differ about 1%  