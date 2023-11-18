#!/bin/bash

# cleanup
rm -rf .sonarqube
dotnet clean

# prepare
dotnet-sonarscanner begin /key:AJE /name:AJE /d:sonar.host.url=http://localhost:9999 /d:sonar.login=admin /d:sonar.password=sonar /d:sonar.exclusions=**/tailwind.config.js,**/tailwind.css,**/app.css /d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml

# build
dotnet build --no-incremental

# test with coverage
dotnet-coverage collect "dotnet test" -f xml -o "coverage.xml"

# analyze
dotnet-sonarscanner end /d:sonar.login=admin /d:sonar.password=sonar
