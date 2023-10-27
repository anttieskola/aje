#!/bin/bash
pushd llama.cpp
make -j LLAMA_CUBLAS=1
cp main /usr/local/bin/Llama/
cp server /usr/local/bin/Llama/
popd
cp llama.sh /usr/local/bin/Llama/
