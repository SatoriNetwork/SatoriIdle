using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class BGN {
    public List<short> list = new List<short>();

	public enum Structures {
		NONE, K, M, B, T, Qa, Qi, Sx, Sp, Oc, No,
		Dec, Und, Ddu, Tdu, Qdu, Qidu, Sdu, Spdu, Odu, Ndu,
		Vig, UnVig, DuVig, TrVig, QaVig, QiVig, SxVig, SpVig, OcVig, NoVig,
		Tri, UnTri, DuTri, TrTri, QaTri, QiTri, SxTri, SpTri, OcTri, NoTri,
		Qua, UnQua, DuQua, TrQua, QaQua, QiQua, SxQua, SpQua, OcQua, NoQua,
		Qui, UnQui, DuQui, TrQui, QaQui, QiQui, SxQui, SpQui, OcQui, NoQui,
		Sex, UnSex, DuSex, TrSex, QaSex, QiSex, SxSex, SpSex, OcSex, NoSex,
		Sep, UnSep, DuSep, TrSep, QaSep, QiSep, SxSep, SpSep, OcSep, NoSep,
		Oct, UnOct, DuOct, TrOct, QaOct, QiOct, SxOct, SpOct, OcOct, NoOct,
		Nov, UnNov, DuNov, TrNov, QaNov, QiNov, SxNov, SpNov, OcNov, NoNov,
		Cent
	}



	public BGN() {
    }

	public BGN(float amount, Structures structure) {
		list = new List<short>();

		// Determine the power of 1000 based on the structure
		int power = (int)structure; // Enum value corresponds to 1000^power

		// Convert amount to an integer representation in base 1000
		double scaledAmount = amount * Math.Pow(1000, power);

		// Extract digits into the list (similar to addInt but from a float)
		while (scaledAmount >= 1) {
			list.Add((short)(scaledAmount % 1000));
			scaledAmount /= 1000;
		}

		// Ensure at least one entry (even if it's zero)
		if (list.Count == 0) {
			list.Add(0);
		}
	}


	public BGN(int num) {
        list.Add(0);
        addInt(num);
	}

	public BGN(string saveName) {
		Load(saveName);
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

	public static BGN Pow(float baseValue, int exponent) {
		if (exponent < 0) {
			throw new ArgumentException("Exponent cannot be negative for BGN calculations.");
		}

		double result = Math.Pow(baseValue, exponent); // Compute power using double precision

		BGN output = new BGN();
		double temp = result;

		// Convert the result into base-1000 representation
		while (temp >= 1) {
			output.list.Add((short)(temp % 1000));
			temp /= 1000;
		}

		// Ensure at least one entry (even if result is zero)
		if (output.list.Count == 0) {
			output.list.Add(0);
		}

		return output;
	}


	public void Save(string SaveName) {
		int index = 0;
		PlayerPrefs.SetInt(SaveName + "Count", list.Count);
		foreach (var item in list) {
			PlayerPrefs.SetInt(SaveName + index.ToString(), item);
			index++;
		}
		PlayerPrefs.Save();
	}

	public void Load(string SaveName) {
		int count = PlayerPrefs.GetInt(SaveName + "Count", 0);
		list.Clear();
		for (int i = 0; i < count; i++) {
			list.Add((short)PlayerPrefs.GetInt(SaveName + i.ToString(), 0));
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

	public static bool operator >=(BGN lhs, BGN rhs) {
		return lhs > rhs || lhs == rhs;
	}

	public static bool operator <=(BGN lhs, BGN rhs) {
		return lhs < rhs || lhs == rhs;
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


	public static bool operator ==(BGN lhs, BGN rhs) {
		// Check for null references
		if (ReferenceEquals(lhs, rhs)) return true;
		if (lhs is null || rhs is null) return false;

		// Compare list counts first
		if (lhs.list.Count != rhs.list.Count) return false;

		// Compare all digits
		for (int i = 0; i < lhs.list.Count; i++) {
			if (lhs.list[i] != rhs.list[i]) return false;
		}

		return true;
	}

	public static bool operator !=(BGN lhs, BGN rhs) {
		return !(lhs == rhs);
	}

	// Override Equals and GetHashCode for proper object comparisons
	public override bool Equals(object obj) {
		if (obj is BGN other) {
			return this == other;
		}
		return false;
	}

	public override int GetHashCode() {
		int hash = 17;
		foreach (short num in list) {
			hash = hash * 31 + num.GetHashCode();
		}
		return hash;
	}


	//update value
	public override string ToString() {
		if (list.Count == 0) return "0"; // Edge case for empty list

		int temp = list.Count - 1;
		double mainValue = list[temp]; // Most significant part
		double fraction = (temp > 0) ? list[temp - 1] / 1000.0 : 0; // Next part as fraction
		double finalValue = mainValue + fraction; // Merge both parts

		string formatted = finalValue.ToString("0.0#"); // Keeps up to 2 decimal places, max 3 digits total
		int enumlen = Enum.GetNames(typeof(Structures)).Length;
		if (temp < enumlen) {
			return temp > 0 ? $"{formatted}{(Structures)temp}" : formatted; // Append suffix if in range
		} else {
			// Engineering notation: "E" followed by the exponent
			int exponent = temp * 3; // Each index represents an increase of 1000 (10^3)
			return $"{formatted}E{exponent}";
		}
	}


}
