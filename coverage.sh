#!/bin/bash

# Install OpenCover and ReportGenerator, and save the path to their executables.
nuget install -Verbosity quiet -OutputDirectory packages -Version 4.6.519 OpenCover
nuget install -Verbosity quiet -OutputDirectory packages -Version 3.0.0 ReportGenerator

OPENCOVER=$PWD/packages/OpenCover.4.6.519/tools/OpenCover.Console.exe
REPORTGENERATOR=$PWD/packages/ReportGenerator.3.0.0/tools/ReportGenerator.exe

coverage=./coverage
mkdir $coverage

echo "Calculating coverage with OpenCover"
$OPENCOVER \
  -target:"c:\Program Files\dotnet\dotnet.exe" \
  -targetargs:"test ./YuukoBlog.Tests/YuukoBlog.Tests.csproj" \
  -output:$coverage/coverage.xml \
  -oldStyle \
  -filter:"+[YuukoBlog*]* -[YuukoBlog.*Tests*]*" \
  -register:user

echo "Generating HTML report"
$REPORTGENERATOR \
  -reports:$coverage/coverage.xml \
  -targetdir:$coverage \
  -verbosity:Error