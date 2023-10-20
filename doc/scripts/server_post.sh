#!/bin/bash
curl --request POST \
--url http://localhost:8080/completion \
--header "Content-Type: application/json" \
--data @$1