#!/bin/bash
pushd src/Web.Ui
dotnet publish -c Release -r linux-x64 --sc --output /usr/local/bin/aje/
popd


