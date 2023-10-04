#!/bin/bash
pushd src/Services/News
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/aje-news/
popd


