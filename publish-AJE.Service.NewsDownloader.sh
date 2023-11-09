#!/bin/bash
pushd src/Service/NewsDownloader
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Service.NewsDownloader/
popd
