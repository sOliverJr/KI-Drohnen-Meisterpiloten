# KI-Projekt

### Requirements
Unity 2021.3.25
Python 3.9 / 3.10

### Installation
```bash
python3.9 -m venv venv 
source venv/bin/activate # opens virtual environment
pip install torch
pip install mlagents
pip install protobuf==3.20
mlagents-learn --help # to verify installation
```
<<<<<<< HEAD

### Run Training
```bash
mlagents-learn --run-id=fly_training --train --force # starts training with id
mlagents-learn --run-id=fly_training --train --resume # resumes training with given id
```
Enter play mode in Unity
=======
>>>>>>> 6113d8c (ml test)

### Links
**Unity Drone Simulator**:
https://github.com/UAVs-at-Berkeley/UnityDroneSim

**UNITY MACHINE-LEARNING-AGENTS**:
https://unity.com/de/products/machine-learning-agents

**Unity ML might not be suited for 3D**:
https://answers.unity.com/questions/1509600/how-to-implement-the-flying-ai-path-finding.html
