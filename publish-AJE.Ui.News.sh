#!/bin/bash
pushd src/Ui/News
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Ui.News/
popd
