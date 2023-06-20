# KI-Projekt

### Requirements
Unity 2021.3.25
Python 3.9 / 3.10

### Installation (Unix)
```bash
python3.9 -m venv venv 
source venv/bin/activate # opens virtual environment
pip install torch
pip install mlagents
pip install protobuf==3.20
mlagents-learn --help # to verify installation
```

### Installation (Windows)
```bash
# Install Python 3.9
# Goto Repository root
py -m venv venv 
cd venv/scripts
activate.bat # opens virtual environment
pip install torch
pip install mlagents
pip install protobuf==3.20
mlagents-learn --help # to verify installation
```

### Config
```config/trainer_config.yaml```

### Run Training
```bash
# Unix
source venv/bin/activate
# Windows
cd venv/scripts
activate.bat

mlagents-learn ../config/trainer_config.yaml --run-id=fly_training --train --force # starts training with id
mlagents-learn ../config/trainer_config.yaml --run-id=fly_training --train --resume # resumes training with given id
```
Enter play mode in Unity

### Links
**Unity Drone Simulator**:
https://github.com/UAVs-at-Berkeley/UnityDroneSim

**UNITY MACHINE-LEARNING-AGENTS**:
https://unity.com/de/products/machine-learning-agents

**Unity ML might not be suited for 3D**:
https://answers.unity.com/questions/1509600/how-to-implement-the-flying-ai-path-finding.html
