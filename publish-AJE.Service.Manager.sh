#!/bin/bash
pushd src/Service/Manager
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Service.Manager/
popd
