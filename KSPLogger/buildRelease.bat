rem garbage line
rem @echo off
set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%

rem set /p HOMEDIR= "Enter Home directory, or <CR> for default: "

if "%HOMEDIR%" == "" (
set HOMEDIR=%DEFHOMEDIR%
)
echo %HOMEDIR%

SET _test=%HOMEDIR:~1,1%
if "%_test%" == ":" (
set HOMEDRIVE=%HOMEDIR:~0,2%
)


set VERSIONFILE=KSPLogger.version
rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/
c:\local\jq-win64  ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile

c:\local\jq-win64  ".VERSION.MINOR"  %VERSIONFILE% >tmpfile
set /P minor=<tmpfile

c:\local\jq-win64  ".VERSION.PATCH"  %VERSIONFILE% >tmpfile
set /P patch=<tmpfile

c:\local\jq-win64  ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile
del tmpfile
set VERSION=%major%.%minor%.%patch%
if "%build%" NEQ "0"  set VERSION=%VERSION%.%build%

echo Version:  %VERSION%


set d=%HOMEDIR\install
if exist %d% goto one
mkdir %d%
:one
set d=%HOMEDIR%\install\Gamedata
if exist %d% goto two
mkdir %d%
:two
set d=%HOMEDIR%\install\Gamedata\KSPLogger
if exist %d% goto three
mkdir %d%
:three

copy bin\Release\KSPLogger.dll %HOMEDIR%\install\Gamedata\KSPLogger
copy KSPLogger.cfg %HOMEDIR%\install\Gamedata\KSPLogger
copy LICENSE.txt %HOMEDIR%\install\Gamedata\KSPLogger
copy KSPLogger.version %HOMEDIR%\install\Gamedata\KSPLogger
copy ..\README.md %HOMEDIR%\install\Gamedata\KSPLogger
copy ChangeLog.txt %HOMEDIR%\install\Gamedata\KSPLogger
copy MiniAVC.dll %HOMEDIR%\install\Gamedata\KSPLogger


%HOMEDRIVE%
cd %HOMEDIR%\install

set FILE="%RELEASEDIR%\KSPLogger-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% Gamedata\KSPLogger
