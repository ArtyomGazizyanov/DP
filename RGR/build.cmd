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

SET _toBuildPathName= "RGR-%1"
SET _toBuildPathName=%_toBuildPathName:"=%
SET _toBuildPathName=%_toBuildPathName: =%

mkdir "%_toBuildPathName%"
cd "%_toBuildPathName%"
mkdir "config"
cd ../

cd src/Node
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Node" /property:PublishWithAspNetCoreTargetManifest=false

cd ../NodeManager
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/NodeManager" /property:PublishWithAspNetCoreTargetManifest=false

cd ../Models
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Models"

cd ../Division
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Division" /property:PublishWithAspNetCoreTargetManifest=false

cd ../Client
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Client" /property:PublishWithAspNetCoreTargetManifest=false

cd ../Subtraction
dotnet publish --configuration Release -f netcoreapp2.0 -o "../../%_toBuildPathName%/Subtraction" /property:PublishWithAspNetCoreTargetManifest=false

cd ../
copy run.cmd "../%_toBuildPathName%"
copy stop.cmd "../%_toBuildPathName%"
copy NodeConfig.json "../%_toBuildPathName%"

cd config
copy config.json "../../%_toBuildPathName%/config"
