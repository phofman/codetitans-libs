@echo off

if not {%CODETITANS_SETUP%} == {} goto ALREADY_SETUP
call "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86
set CodeTitans_Setup=VS10_x86
echo Working directory: %CD%

:ALREADY_SETUP

echo Building libraries...

rem Remove old compilation temporary files...
set TempFolder=%CD%\temp_bin
rd /S /Q %TempFolder%

set builder=msbuild /nologo /noconlog /maxcpucount /p:Configuration=Release;DebugSymbols=false;DebugType=None /t:Rebuild
set nuget=nuget

echo JSon@CodeTitans for all platforms
set OutputPath=%TempFolder%\JSON@CodeTitans\lib
%builder% ../JSON@CodeTitans/CodeTitans.Silverlight.JSon.VS2010.csproj /p:TargetFrameworkVersion=v3.0 /property:OutDir=%OutputPath%\sl30
%builder% ../JSON@CodeTitans/CodeTitans.Silverlight.JSon.VS2010.csproj /p:TargetFrameworkVersion=v4.0 /property:OutDir=%OutputPath%\sl40
%builder% ../JSON@CodeTitans/CodeTitans.Silverlight.JSon.VS2010.csproj /p:TargetFrameworkVersion=v5.0 /property:OutDir=%OutputPath%\sl50

%builder% ../JSON@CodeTitans/CodeTitans.JSon.VS2010.csproj /p:TargetFrameworkVersion=v2.0 /property:OutDir=%OutputPath%\net20
%builder% ../JSON@CodeTitans/CodeTitans.JSon.VS2010.csproj /p:TargetFrameworkVersion=v3.0 /property:OutDir=%OutputPath%\net30
%builder% ../JSON@CodeTitans/CodeTitans.JSon.VS2010.csproj /p:TargetFrameworkVersion=v3.5 /property:OutDir=%OutputPath%\net35
%builder% ../JSON@CodeTitans/CodeTitans.JSon.VS2010.csproj /p:TargetFrameworkVersion=v4.0 /property:OutDir=%OutputPath%\net40
%builder% ../JSON@CodeTitans/CodeTitans.JSon.VS2010.csproj /p:TargetFrameworkVersion=v4.5 /property:OutDir=%OutputPath%\net45

%builder% ../JSON@CodeTitans/CodeTitans.Phone.JSon.csproj /property:OutDir=%OutputPath%\wp7
%builder% ../JSON@CodeTitans/CodeTitans.Phone.JSon.csproj /property:OutDir=%OutputPath%\wp71

%builder% ../JSON@CodeTitans/CodeTitans.WinStore.JSon.VS2012.csproj /property:OutputPath=%OutputPath%\windows8\

copy codetitans-json.nuspec %OutputPath%\.. > NUL:
echo Building NuGet package

%nuget% pack %OutputPath%\..\codetitans-json.nuspec
echo Release prepared, find packages at '%CD%'

echo Removing temporary files
rd /S /Q %TempFolder%
