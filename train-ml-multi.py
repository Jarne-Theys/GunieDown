import subprocess
import time  # Import the time module

def launch_training_instance(run_id, port):
    command = [
        "mlagents-learn",
        "config.yaml",  # Use trainer_config.yaml now
        "--env=C:\\UnityProjects\\Powor\\Build\\Powor.exe",  # **REPLACE WITH YOUR UNITY BUILD PATH**
        "--results-dir=Assets/results",
        #"--resume", # Or "--force" if you want to start fresh each time
        "--no-graphics",
        f"--run-id={run_id}", # Unique run-id
        f"--base-port={port}",     # Unique port
        "--torch-device", "cuda"
    ]
    print(f"Launching instance: {' '.join(command)}") # Print the command for debugging
    process = subprocess.Popen(command) # Use Popen for non-blocking execution
    return process

if __name__ == "__main__":
    num_instances = 5  # Set the number of parallel instances you want to run
    base_port = 5005     # Starting port number
    processes = []

    for i in range(num_instances):
        run_id = f"AIPlayer-LookingAtPlayer-DodgingBullets-AvoidWalls-Run{i+1}" # Unique run-id for each instance
        port = base_port + i                             # Unique port for each instance
        process = launch_training_instance(run_id, port)
        processes.append(process)
        time.sleep(2) # Optional: Wait a couple of seconds between launching instances to avoid port conflicts

    print(f"Launched {num_instances} training instances. Run IDs: {[p.args[-3].split('=')[1] for p in processes]}") # Print run IDs
    print("To monitor progress, run TensorBoard: tensorboard --logdir=Assets/results")
    print("Keep this script running to keep the training instances alive.")

    # You can add code here to monitor the processes if needed,
    # but for basic parallel training, just keeping the script running is sufficient.

    # To keep the script running indefinitely (until you manually stop it):
    try:
        while True:
            time.sleep(60) # Check every minute (adjust as needed)
    except KeyboardInterrupt:
        print("Terminating training instances...")
        for process in processes:
            process.terminate() # Or process.kill() for a stronger signal
        print("Training instances terminated.")