namespace BWHazel.Apps.QuantumTelloport.Control {

    open Microsoft.Quantum.Convert;
    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Measurement;
    
    /// # Summary
    /// Determines the axis for the drone to follow based on the
    /// measurement of a qubit in superposition.
    ///
    /// # Output
    /// The result of the qubit measurement as an integer.
    operation DetermineAxis () : Int {
        use axis = Qubit();
        H(axis);

        let axisMeasured = MResetZ(axis);
        Message($"AXIS: Qubit Superposition Measurement: {axisMeasured}");

        return ResultAsInt(axisMeasured);
    }

    /// # Summary
    /// Determines the direction for the drone to follow on its
    /// axis based on the measurement of a pair of entangled
    /// qubits.
    ///
    /// # Output
    /// The result of the entangled qubits measurement as an
    /// integer.  Returns -1 if the qubits are measured to have
    /// different values.
    operation DetermineDirection () : Int {
        use (control, target) = (Qubit(), Qubit());
        H(control);
        CNOT(control, target);

        let entangledMeasured = MultiM([control, target]);
        Message($"DIRECTION: Qubit Entnaglement Measurements: {entangledMeasured}");

        if (entangledMeasured[0] != entangledMeasured[1]) {
            return -1;
        } else {
            return ResultAsInt(entangledMeasured[0]);
        }
    }

    /// # Summary
    /// Determines the distnace for the drone to move in the
    /// direction on its axis based on the measurement of an
    /// array of qubits in superposition.
    ///
    /// # Output
    /// The combined results of the qubit measurements as an
    /// integer in little-endian format.
    operation DetermineDistance () : Int {
        use components = Qubit[3];
        for component in components {
            H(component);
        }

        let componentsMeasured = MultiM(components);
        Message($"DISTANCE: Qubit Superposition Measurements: {componentsMeasured}");

        return ResultArrayAsInt(componentsMeasured);
    }

    /// # Summary
    /// Converts a result to an integer.
    ///
    /// # Input
    /// A result.
    ///
    /// # Output
    /// The result as an integer.
    operation ResultAsInt (result : Result) : Int {
        return (result == One ? 1 | 0);
    }
}
