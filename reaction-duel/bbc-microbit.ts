/**
 * Value indicating whether a game is running.
 * @description Indicates that a game has been startred and is in progress.
 */
let isGameActive = 0;

/**
 * Value indicating whether a duel is running.
 * @description Indicates that a duel has started and is in progress.
 */
let isDuelRunning = 0;

/**
 * Value indicating whether a duel is live.
 * @description Indicates that a duel is awaiting a player to win.
 */
let isDuelLive = 0;

/**
 * Value to store received USB data.
 */
let usbInput = "";

/**
 * Value to store the winner player ID.
 */
let winnerId = "";

/**
 * Runs a duel.
 * @description Sends a duel start notification via USB and runs the duel.
 */
function RunDuel() {
    serial.writeLine("rxn-duel:start");
    isDuelRunning = 1;
    while (isDuelLive == 0) {
        basic.showLeds(`
            . . . . .
            . . . . .
            . . # . .
            . . . . .
            . . . . .
            `
        );
    }

    while (isDuelLive == 1) {
        basic.showLeds(`
            # . . . #
            . # . # .
            . . # . .
            . # . # .
            # . . . #
            `
        );
    }

    isDuelRunning = 0;
}

/**
 * Wins the game for the specified player.
 * @param winner The player to win the game.
 * @description Wins the game for the specified player if a duel is running and is live and sends the ID via USB.
 *              If not, an early declaration is sent for the player over USB.
 */
function WinGame(winner: string) {
    if (isDuelRunning == 1) {
        if (isDuelLive == 1) {
            winnerId = winner;
            serial.writeLine("rxn-duel:winner-" + winner);
            isDuelLive = 0;
        } else {
            serial.writeLine("rxn-duel:early-" + winner);
        }
    }
}

/**
 * Prints the game title on start-up.
 */
basic.showString("RXN DUEL!");

/**
 * Runs the game forever.
 * @description Until a game is requested an awaiting message is printed.
 *              When a game is requested a get ready message is printed and a duel is started.
 */
basic.forever(() => {
    if (isGameActive == 0) {
        basic.showString("Awaiting");
    } else {
        basic.showString("Get Ready!");
        RunDuel();
        isGameActive = 0;
    }
});

/**
 * Handle a press of button A.
 * @description Attempts to win the game for player 1.
 */
input.onButtonPressed(Button.A, () => {
    WinGame("P1");
});

/**
 * Handle a press of button B.
 * @description Attempts to win the game for player 2.
 */
input.onButtonPressed(Button.B, () => {
    WinGame("P2");
});

/**
 * Handles new-line-delimited data received via USB.
 * @description All input data should start with "rxn-duel:" and handles handshake request, game request and
 *              duel live trigger.
 */
serial.onDataReceived(serial.delimiters(Delimiters.NewLine), () => {
    usbInput = serial.readLine();
    if (usbInput.includes("rxn-duel:handshake")) {
        serial.writeLine("rxn-duel:ack");
    } else if (usbInput.includes("rxn-duel:ready")) {
        isGameActive = 1;
    } else if (usbInput.includes("rxn-duel:go")) {
        isDuelLive = 1;
    } else {   
    }
});