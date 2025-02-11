using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BGN {
    public List<short> list = new List<short>();

    enum Structures {
        NONE,
        K,
        M,
        B,
        T,
        Qa,
        Qi,
        Sx,
        Sp,
        Oc,
        No,
        Dec
    }


    public BGN() {
    }

    public BGN(int num) {
        list.Add(0);
        addInt(num);
	}

    public void addInt(int num) {
        bool adding = true;
        int index = 0;
        while (adding) {
            if (index > list.Count - 1) {
                list.Add(0);
            }
            if (num > 999) {
                int temp = (num + list[index]) % 1000;
                list[index] = (short)temp;
                num = num - temp;

                num /= 1000;
                index++;
            } else {
                list[index] += (short)num;
                adding = false;
            }
        }
    }

    public static BGN operator +(BGN lhs, BGN rhs) {
		int maxLength = Math.Max(lhs.list.Count, rhs.list.Count);
		List<short> result = new List<short>(new short[maxLength + 1]); // +1 for possible carry overflow

		int carry = 0;

		for (int i = 0; i < maxLength || carry > 0; i++) {
			int value1 = i < lhs.list.Count ? lhs.list[i] : 0;
			int value2 = i < rhs.list.Count ? rhs.list[i] : 0;

			int sum = value1 + value2 + carry;
			result[i] = (short)(sum % 1000);
			carry = sum / 1000;

			// Ensure we have space for carry overflow
			if (i == result.Count - 1 && carry > 0) {
				result.Add((short)carry);
			}
		}

		// Remove leading zero if it was an unnecessary carry
		while (result.Count > 1 && result[^1] == 0) {
			result.RemoveAt(result.Count - 1);
		}

        BGN Value = new BGN();
        Value.list = result;

		return Value;

	}

	//subtract Values
	public static BGN operator -(BGN lhs, BGN rhs) {
		int maxLength = Math.Max(lhs.list.Count, rhs.list.Count);
		List<short> result = new List<short>(new short[maxLength]);

		int borrow = 0;

		for (int i = 0; i < maxLength; i++) {
			int value1 = i < lhs.list.Count ? lhs.list[i] : 0;
			int value2 = i < rhs.list.Count ? rhs.list[i] : 0;

			int diff = value1 - value2 - borrow;

			if (diff < 0) {
				diff += 1000; // Borrow from the next higher place value
				borrow = 1;
			} else {
				borrow = 0;
			}

			result[i] = (short)diff;
		}

		// Remove leading zeros
		while (result.Count > 1 && result[^1] == 0) {
			result.RemoveAt(result.Count - 1);
		}

		BGN Value = new BGN();
		Value.list = result;

		return Value;
	}

	//Multiply by one value
	//multiply by two values
	//Greater Than
	public static bool operator >(BGN lhs, BGN rhs) {
		// Compare length first
		if (lhs.list.Count > rhs.list.Count) return true;
		if (lhs.list.Count < rhs.list.Count) return false;

		// Compare digits from most significant to least significant
		for (int i = lhs.list.Count - 1; i >= 0; i--) {
			if (lhs.list[i] > rhs.list[i]) return true;
			if (lhs.list[i] < rhs.list[i]) return false;
		}

		// They are equal, so lhs is NOT greater
		return false;
	}

	//less than

	public static bool operator <(BGN lhs, BGN rhs) {
		// Compare length first
		if (lhs.list.Count < rhs.list.Count) return true;
		if (lhs.list.Count > rhs.list.Count) return false;

		// Compare digits from most significant to least significant
		for (int i = lhs.list.Count - 1; i >= 0; i--) {
			if (lhs.list[i] < rhs.list[i]) return true;
			if (lhs.list[i] > rhs.list[i]) return false;
		}

		// They are equal, so lhs is NOT less
		return false;
	}

	public static BGN operator *(BGN lhs, BGN rhs) {
		int lhsLen = lhs.list.Count;
		int rhsLen = rhs.list.Count;

		// Result list with enough space (max possible length)
		List<short> result = new List<short>(new short[lhsLen + rhsLen]);

		// Perform multiplication using base-1000 logic
		for (int i = 0; i < lhsLen; i++) {
			int carry = 0;
			for (int j = 0; j < rhsLen || carry > 0; j++) {
				int currentPos = i + j;
				int value1 = lhs.list[i];
				int value2 = j < rhsLen ? rhs.list[j] : 0;

				long product = (long)value1 * value2 + result[currentPos] + carry;
				result[currentPos] = (short)(product % 1000);
				carry = (int)(product / 1000);
			}
		}

		// Remove leading zeros
		while (result.Count > 1 && result[^1] == 0) {
			result.RemoveAt(result.Count - 1);
		}

		return new BGN { list = result };
	}


	//update value
	public override string ToString() {
		if (list.Count == 0) return "0"; // Edge case for empty list

		int temp = list.Count - 1;
		double mainValue = list[temp]; // Most significant part
		double fraction = (temp > 0) ? list[temp - 1] / 1000.0 : 0; // Next part as fraction

		double finalValue = mainValue + fraction; // Merge both parts
		string formatted = finalValue.ToString("0.0#"); // Keeps up to 2 decimal places but max 3 digits total

		return temp > 0 ? $"{formatted}{(Structures)temp}" : formatted; // Append suffix if needed
	}

}
