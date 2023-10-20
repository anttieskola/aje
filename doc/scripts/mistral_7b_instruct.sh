#!/bin/bash
/usr/local/bin/llama.cpp/main --color -t 12 -ngl 35 --temp 1 -c 4096 --repeat-penalty 1 -n -1 -m /home/models/Mistral-7B-Instruct-v0.1-GGUF/mistral-7b-instruct-v0.1.Q5_K_M.gguf -ins
