# Quantum Telloport

Quantum Tellport is a basic application to control a Ryze Tello drone using quantum mechanics and demonstrates some quantum computing concepts.

**Only the Ryze Tello drone is currently supported.**

Connections to te Ryze Tello drone, including the simulator, are powered by the [TelloCommander](https://github.com/davewalker5/TelloCommander) library.

## Setting Up

**You will need to install the Microsoft Quantum Development Kit (QDK) before continuing.  Please follow the instructions [here](https://docs.microsoft.com/en-us/azure/quantum/install-command-line-qdk?tabs=tabid-vscode) on how to do so.**

* Build the .NET application in the _QuantumTelloport_ project directory.
* Optionally publish the .NET application as a single-file executable, e.g. for macOS:

```sh
dotnet build
dotnet publish -r osx-x64 -p:PublishSingleFile=true --self-contained true
```

To use the simulator provided by the TelloCommander library, please follow the instructions [here](https://github.com/davewalker5/TelloCommander/wiki/Drone-Simulator).  Quantum Telloport uses version 1.3.0.0 of the Tello API.

To run the program using a physical drone please connect to the device in the usual way from your computer.

## Starting the Program

Quantum Telloport is a command-line application configured using command-line arguments outlined in the table below.  To use the default values, no flags are required.

Flag | Type | Default Value | Description
--- | --- | --- | ---
`-a`, `--api` | String | 1.3.0.0 | The version of the Tello API to use.
`-r`, `--run-time` | Integer | 120 | The total run-time of the flight in seconds.
`-p`, `--pause-time` | Integer | 10 | The pause time between moves of the drone.
`-s`, `--simulate` | Boolean | False | Use the drone simulator.

As an example, to run the program using the drone simulator with a total run-time of 1 minute and moving the drone every 15 seconds run:

```sh
# .NET CLI
dotnet run -- \
    --run-time 60 \
    --pause-time 15 \
    --simulate

# Single-File Executable
./qtelloport \
    --run-time 60 \
    --pause-time 15 \
    --simulate
```

While running, the program will log basic status messages to the console, including the results of the quantum operations to control the drone flight.

```
*** QUANTUM TELLOPORT ***
Initialising drone commander...
Creating connection to drone simulator...
Starting flight...
Establishing connection...
Taking off...
Starting drone move 1...
AXIS: Qubit Superposition Measurement: Zero
DIRECTION: Qubit Entnaglement Measurements: [Zero,Zero]
DISTANCE: Qubit Superposition Measurements: [Zero,Zero,Zero]
Executing command: left 20...
Total elapsed flight time: 10s of 120s
Starting drone move 2...

    # Subsequent moves removed for brevity.

Landing...
Disconnecting...
```

## Quantum Controls

Each move of the drone is controlled by 3 separate quantum operations each demonstrating some concepts of quantum computing.

### Movement Axis

The first operation takes a single qubit, places it into a universal superposition in the Hadamard basis and measures it in computational basis.  The axis of movement is controlled by the result of the measurement.

Measurement Result | Movement Axis
--- | ---
`One` | Forward/Backward
`Zero` | Right/Left

### Movement Direction

The second operation takes two qubits in the `Zero` state and places them into an entangled state which are then measured in computational basis.  Due to the laws of quantum mechanics the measured value of both qubits is always equal and that value is used to control the direction along the movement axis.

Measurement Results | Movement Direction
--- | ---
`[One,One]` | Forward/Right
`[Zero,Zero]` | Backward/Left

In the event that the measurement of the entangled qubits is not equal then the drone is instructed to land.

### Movement Distance

The third operation takes three qubits and places them all into a universal superposition in the Hadamard basis.  They are all then measured in computational basis and combined into an integer in little-endian format to give a number between 0 and 7.  As a reminder, this is reading a number with the least-significant bit first.

This number is added to the minimum drone distance supported, which is 20cm at the time of writing, and the drone is moved that distance in the direction along its axis.