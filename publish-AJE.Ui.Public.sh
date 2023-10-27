#!/bin/bash
pushd src/Ui/Public
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Ui.Public/
popd
