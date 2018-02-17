@echo off
setlocal

set /p string=<src/config/config.json

rem Remove quotes
set string=%string:"=%
rem Remove braces
set "string=%string:~2,-2%"
rem Change colon+space by equal-sign
set "string=%string:: ==%"
rem Separate parts at comma into individual assignments
set "%string:, =" & set "%"
                     
SET _toBuildPathName= "%project_name%-%1"
SET _toBuildPathName=%_toBuildPathName:"=%
SET _toBuildPathName=%_toBuildPathName: =%

mkdir "%_toBuildPathName%"
cd "%_toBuildPathName%"
mkdir "config"
cd ../

cd src/Backend
dotnet build --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Backend"

cd ../Frontend
dotnet build --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Frontend"

cd ../
copy run.cmd "../%_toBuildPathName%"
copy stop.cmd "../%_toBuildPathName%"

cd config
copy config.json "../../%_toBuildPathName%/config"
