# What is this?

GunDown is a fast-paced 1v1 first-person shooter built in Unity. Face off against a challenging AI opponent in compact arenas where tactical positioning, reflexes, and strategic use of power-ups determine victory.
Features include:

🔫 1v1 FPS Gameplay – Tight, focused combat designed for short, intense rounds.

🤖 AI Opponent – Powered by Unity ML-Agents, the enemy learns to dodge, aim, and improve over time.

🧠 Reward System – AI receives training signals based on hits, misses, and tactical decisions to improve through reinforcement learning.

💥 Power-Ups – After each round, both the player and AI receive a random power-up that can change the tide of battle. (in progress)


# Installation

## Clone the Repository

`git clone https://github.com/yourusername/GunDown.git`

## Open in Unity

Use Unity Hub to open the project.

Target Unity version: 6000.0.43f1

## Install ML-Agents

Set up Unity ML-Agents Toolkit using Python.

Follow the official installation guide for your OS.

## Training

Use the included training script in the repository root (train-ml.py)

Define your training scenarios using YAML config files. (for example see config.yaml)

## Run training:

`python .\train-ml.py`


# Controls
- Move:	WASD
- Look:	Mouse
- Shoot: Left Mouse Button