# Simple setup using sdxl-turbo

- Create virtual environment using conda (to get correct python version)
    - Later it runs just with the python virtual environment not requiring conda
```bash
conda create --name stability python=3.10
```

- Activate environment
```bash
conda activate stability
```

- Create virtual environment using python
```bash
python3 -m venv ai
```

- Activate python virtual environment inside conda virtual environment
```bash
source ai/bin/activate
```

- Install requirements
```bash
pip3 install -r ai.txt
```

- Start VSCode
- Configure python intepreter (./stability/ai/bin/python)


# Links
- [SDXL Turbo @ hugginface](https://huggingface.co/stabilityai/sdxl-turbo)
- [Stability AI own repo](https://github.com/Stability-AI/generative-models)
