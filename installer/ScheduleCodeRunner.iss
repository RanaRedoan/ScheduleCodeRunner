#define MyAppName "Schedule Code Runner"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Md. Redoan Hossain Bhuiyan"
#define MyAppExeName "ScheduleCodeRunner.App.exe"

[Setup]
AppId={{B6458C17-6A48-4379-9FE6-90C2C4C0C7D2}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL=https://github.com/ranaredoan
AppSupportURL=https://github.com/ranaredoan
AppUpdatesURL=https://github.com/ranaredoan
AppComments=Developer: Md. Redoan Hossain Bhuiyan | Email: redoanhossain630@gmail.com | LinkedIn: linkedin.com/in/mdredoanhossainbhuiyan
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\artifacts\installer
OutputBaseFilename=ScheduleCodeRunner-Setup
SetupIconFile=..\assets\App.ico
LicenseFile=..\installer\LICENSE_INFO.txt
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\artifacts\publish\app\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\artifacts\publish\worker\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
