using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class MathEN {

    //Add values
    public static double[] AddValue(double A, int AEN, double B, int BEN) {
        int diff = AEN - BEN;
        double Value = 0;
        int ValueEN = 0;

        if (diff >= 11) {// max difference without crashing
            double[] errorValue = updateValue(A, AEN);

            return errorValue;
        } else if (diff <= -11) {
			double[] errorValue = updateValue(B, BEN);

			return errorValue;
		}

        if (diff == 0) {
            Value = A + B;
            ValueEN = AEN;
        } else if (diff > 0) {
            double TempA = A * math.pow(10, diff);
            Value = TempA + B;
            ValueEN = BEN;
        } else if (diff < 0) {
			double TempB = B * math.pow(10, math.abs(diff));
			Value = TempB + A;
			ValueEN = AEN;
		}


        double[] finalValue = updateValue(Value, ValueEN);

        return finalValue;
    }
    //subtract Values
    //Multiply by one value
    //multiply by two values
    //Greater Than
    //less than
    //update value

    public static double[] updateValue(double A, int AEN) {
        double Value = A;
        double ValueEN = AEN;
        while (Value >= 10.00) {
            Value /= 10;
            ValueEN += 1;
        }
		double[] finalValue = { Value, ValueEN };

		return finalValue;
	}

}
