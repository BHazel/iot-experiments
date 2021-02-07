using System;
using static System.Console;
using System.Net;
using System.Threading;
using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;
using BWHazel.Apps.QuantumTelloport.Control;
using TelloCommander.CommandDictionaries;
using TelloCommander.Commander;
using TelloCommander.Connections;

const long MinimumDroneDistance = 20;
const int MovementWaitTime = 10000;
const string TelloApiVersion = "1.3.0.0";
TimeSpan totalRunTime = new TimeSpan(0, 2, 0);

CommandDictionary commandDictionary = CommandDictionary.ReadStandardDictionary(TelloApiVersion);
TelloConnection telloConnection = new(IPAddress.Loopback.ToString(),
    TelloConnection.DefaultTelloPort,
    ConnectionType.Simulator
);

DroneCommander droneCommander = new(new TelloConnection(), commandDictionary);
droneCommander.Connect();
droneCommander.RunCommand("takeoff");

CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
{
    droneCommander.RunCommand("land");
};

DateTime startTime = DateTime.Now;
while (DateTime.Now - startTime <= totalRunTime)
{
    WriteLine(DateTime.Now - startTime);
    QuantumSimulator quantumSimulator = new();
    long axis = await DetermineAxis.Run(quantumSimulator);
    long direction = await DetermineDirection.Run(quantumSimulator);
    long distance = await DetermineDistance.Run(quantumSimulator);

    long droneDistance = distance + MinimumDroneDistance;
    string droneCommand = (axis, direction) switch
    {
        (1, 1) => $"forward {droneDistance}",
        (1, 0) => $"back {droneDistance}",
        (0, 1) => $"right {droneDistance}",
        (0, 0) => $"left {droneDistance}",
        (_, _) => $"land"
    };

    droneCommander.RunCommand(droneCommand);

    WriteLine(droneCommand);
    Thread.Sleep(MovementWaitTime);
}

droneCommander.RunCommand("land");
droneCommander.Disconnect();