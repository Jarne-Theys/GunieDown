import subprocess

command = [
    "mlagents-learn",
    "config.yaml",
    "--results-dir=Assets/results",
    "--resume",
    "--no-graphics",
    "--run-id=AIPlayerDodgingAndShooting",
    "--initialisze-from=AIPlayerDodging",
    "--torch-device", "cuda"
]

subprocess.run(command)
