#!/bin/bash

# zeus
#/usr/local/bin/Llama/server -t 12 -ngl 35 -c 16384 -m /home/models/Mistral-7B-OpenOrca-GGUF/mistral-7b-openorca.Q5_K_M.gguf
# awesome looking this neuralhermes - passes all integration tests (calculates words correctly)
/usr/local/bin/Llama/server -t 12 -ngl 35 -c 20480 -m /home/models/NeuralHermes-2.5-Mistral-7B-GGUF/neuralhermes-2.5-mistral-7b.Q5_K_M.gguf

# god
# /usr/local/bin/Llama/server --host 127.0.0.1 --port 5999  -t 12 -ngl 35 -c 12384 -m /home/models/Mistral-7B-OpenOrca-GGUF/mistral-7b-openorca.Q5_K_M.gguf


