import subprocess

command = [
    "mlagents-learn",
    "config.yaml",
    "--results-dir=Assets/results",
    #"--resume",
    "--force",
    "--no-graphics",
    "--run-id=AIPlayer-Editor",
    #"--initialize-from=AIPlayerDodging",
    "--torch-device", "cuda"
]

subprocess.run(command)
