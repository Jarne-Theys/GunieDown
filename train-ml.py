import subprocess

command = [
    "mlagents-learn",
    "config.yaml",
    "--results-dir=Assets/results",
    #"--resume",
    "--no-graphics",
    "--run-id=AIPlayerGoalRaycastsV1",
    "--torch-device", "cuda"
]

subprocess.run(command)
