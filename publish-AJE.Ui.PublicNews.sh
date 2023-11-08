#!/bin/bash
pushd src/Ui/PublicNews
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Ui.PublicNews/
popd
