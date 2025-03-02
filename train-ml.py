import subprocess

command = [
    "mlagents-learn",
    "config.yaml",
    "--results-dir=Assets/results",
    #"--force",
    "--resume",
    "--no-graphics",
    #"--run-id=AIPlayer-NavigatingTerrain-DodgingBullets-LookingAtPlayer",
    "--run-id=AIPlayer-LookingAtPlayer",
    #"--initialize-from=AIPlayerDodging",
    "--torch-device", "cuda"
]

subprocess.run(command)
