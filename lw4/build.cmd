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

SET _toBuildPathName= "SuperProject-%1"
SET _toBuildPathName=%_toBuildPathName:"=%
SET _toBuildPathName=%_toBuildPathName: =%

mkdir "%_toBuildPathName%"
cd "%_toBuildPathName%"
mkdir "config"
cd ../

cd src/Backend
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Backend" /property:PublishWithAspNetCoreTargetManifest=false

cd ../Frontend
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Frontend" /property:PublishWithAspNetCoreTargetManifest=false

cd ../TextRankCalc
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/TextRankCalc" /property:PublishWithAspNetCoreTargetManifest=false

cd ../TextListener
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/TextListener" /property:PublishWithAspNetCoreTargetManifest=false

cd ../RabbitMq
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/RabbitMq" /property:PublishWithAspNetCoreTargetManifest=false

cd ../Redis
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Redis" /property:PublishWithAspNetCoreTargetManifest=false

cd ../
copy run.cmd "../%_toBuildPathName%"
copy stop.cmd "../%_toBuildPathName%"

cd config
copy config.json "../../%_toBuildPathName%/config"
