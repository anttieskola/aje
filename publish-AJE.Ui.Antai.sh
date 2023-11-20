#!/bin/bash
pushd src/Ui/Antai
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Ui.Antai/
popd
