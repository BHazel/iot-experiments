# Reaction Duel

Reaction Duel is a basic two-player game to test reaction times.  It consists of a .NET console application connected to a USB device which has two buttons, one for each player.

**Only the BBC Microbit is currently supported.**

## Setting Up

* Build the .NET application in the _ReactionDuel_ project directory.
* Optionally publish the .NET application as a single-file executable, e.g. for macOS:

```sh
dotnet build
dotnet publish -r osx-x64 -p:PublishSingleFile=true --self-contained true
```

* Copy the code in _bbc-microbit.ts_ into the MakeCode editor.
* Download the compiled code from the MakeCode editor and load into onto the BBC Microbit.

## Starting the Game

* When loaded onto the BBC Microbit a `RXN DUEL!` title appears.
* When `Awaiting` starts looping run the .NET desktop application from the command-line either using the .NET CLI or from a single-file executable.
    * Using command-line arguments provide the names for players 1 (`--player-one` or `-1`) and 2 (`--player-two` or `-2`) and the USB device ID (`--device` or `-d`).
    * If command-line arguments are omitted the application will prompt for any missing values instead.  Values in square brackets `[]` are default values.
    * Please see the BBC Microbit instructions on how to find the USB device ID.  On Unix systems it is a value similar to `/dev/tty.usbmodem12345`.

```sh
# .NET CLI
dotnet run -- \
    --player-one PlayerOneName \
    --player-two PlayerTwoName \
    --device /dev/deviceId

# Single-File Executable
./rxnduel \
    --player-one PlayerOneName \
    --player-two PlayerTwoName \
    --device /dev/deviceId
```

* The application will attempt to connect to the USB device and perform a bespoke handshake to ensure it works.  A successful set up looks like the following:

```sh
Connecting to USB device /dev/tty.usbmodem12345...
Connection established.
Initiating handshake with USB device /dev/tty.usbmodem12345...
Handshake successful.
*** REACTION DUEL! ***
Ready to play? (Y/N) [Y]:
```

## Playing the Game

* On the BBC Microbit player 1 should use **Button A** and player 2 **Button B**.
* When ready to play press **Y** followed by **Enter** on the computer.
* The BBC Microbit will display `Get Ready!` followed by a small dot.
* After a random number of seconds (between 1 and 10) an `X` will appear.
* Each player should press their button once as soon as the `X` appears.
* The winner will be displayed on the computer screen with how fast their button press was.
* The computer will request whether a new game should start and the BBC Microbit to looping `Awaiting`.