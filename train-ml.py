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
    "--env=C:\\UnityProjects\\Powor\\Build\\Powor.exe",
    "--num-envs=16",
    #"--env=/home/jarne/Documents/Unity_projects/GunieDown/Build/Build.x86_64",
    "--results-dir=Assets/AI",
    #"--resume", # Or "--force" if you want to start fresh each time
    #"--force",
    "--no-graphics",
    "--run-id=AINoDodging",
    "--initialize-from=BCAINoDodging",
    "--torch-device", "cuda"
]

subprocess.run(command)
