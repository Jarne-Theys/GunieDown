import subprocess

command = [
    "mlagents-learn",
    "config.yaml",
    "--results-dir=Assets/results",
    "--resume",
    #"--no-graphics",
    "--run-id=AIPlayer-NavigatingTerrain-DodgingBullets-LookingAtPlayer",
    #"--initialize-from=AIPlayerDodging",
    "--torch-device", "cuda"
]

subprocess.run(command)
