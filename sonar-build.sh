#!/bin/bash

# cleanup
rm -rf .sonarqube
dotnet clean

# prepare
dotnet-sonarscanner begin /key:AJE /name:AJE /d:sonar.host.url=http://localhost:9999 /d:sonar.login=admin /d:sonar.password=admin

# build
dotnet build

# analyze
dotnet-sonarscanner end /d:sonar.login=admin /d:sonar.password=admin
