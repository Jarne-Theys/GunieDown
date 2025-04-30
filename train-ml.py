import subprocess

# command = [
#     "mlagents-learn",
#     "config.yaml",
#     "--results-dir=Assets/results",
#     #"--resume",
#     "--force",
#     "--no-graphics",
#     "--run-id=AIPlayer-Editor",
#     #"--initialize-from=AIPlayerDodging",
#     "--torch-device", "cuda"
# ]

command = [
    "mlagents-learn",
    "config.yaml",
    #"--env=C:\\UnityProjects\\Powor\\Build\\Powor.exe",
    "--results-dir=AI",
    #"--resume", # Or "--force" if you want to start fresh each time
    "--force",
    "--no-graphics",
    "--run-id=outerwalls-shooting",
    #"--initialize-from=AIPlayer-fasterbullet",
    "--torch-device", "cuda"
]

subprocess.run(command)
