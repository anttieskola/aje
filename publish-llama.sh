#!/bin/bash
pushd llama.cpp
make -j LLAMA_CUBLAS=1
cp main /usr/local/bin/llama.cpp/
cp server /usr/local/bin/llama.cpp/
popd
