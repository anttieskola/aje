#!/bin/bash
pushd src/Service/News
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/AJE.Service.News/
popd
