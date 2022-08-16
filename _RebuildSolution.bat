@ECHO off
TITLE Rebuild Solution
ECHO Rebuilding Solution...

SET CurrPath=%~dp0
CD /D %CurrPath%

IF NOT EXIST _BatVariable.txt (
    ECHO file '_BatVariable.txt' doesn't exist.
	PAUSE
    EXIT
)

FOR /f "delims== tokens=*" %%G in (_BatVariable.txt) DO SET %%G
:: below are variable need to be inside 
::ECHO %DomainID%
::ECHO %DomainPassword%
::ECHO %MSBuildexePath%
::ECHO %ServerPublishPath%
::ECHO %Title%

SET ProjectSolutionPath=%CurrPath%NuGetPackage.sln

"%MSBuildexePath%" "%ProjectSolutionPath%" /t:Clean,Restore,Build /p:RestorePackagesConfig=true /property:Configuration=Release

ECHO.
ECHO Rebuild Solution Completed.
ECHO.
PAUSE
