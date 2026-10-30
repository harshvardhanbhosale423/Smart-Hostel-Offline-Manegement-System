; Inno Setup script for Smart Hostel Full GUI App

#define MyAppName "Smart Hostel Full GUI"
#define MyAppVersion "1.0"
#define MyAppExeName "SmartHostel.exe"

[Setup]
AppId={{D8C7E4A2-5F62-4C2D-9BE3-1B0CC7A7F441}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={localappdata}\SmartHostelFullGUI
DefaultGroupName=Smart Hostel Full GUI
OutputDir=installer_output
OutputBaseFilename=SmartHostelFullGUISetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest

[Files]
Source: "SmartHostel.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "students.txt"; DestDir: "{app}"; Flags: onlyifdoesntexist
Source: "entrylog.txt"; DestDir: "{app}"; Flags: onlyifdoesntexist
Source: "exitlog.txt"; DestDir: "{app}"; Flags: onlyifdoesntexist
Source: "visitors.txt"; DestDir: "{app}"; Flags: onlyifdoesntexist
Source: "README.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "installer_uninstall.bat"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Smart Hostel Full GUI"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\Smart Hostel Full GUI"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch Smart Hostel Full GUI"; Flags: nowait postinstall skipifsilent
