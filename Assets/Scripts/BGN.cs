using System;
using System.Collections.Generic;
using System.Linq;
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





	public static BGN PowMultiply(BGN baseValue, float multiplier, int exponent) {
		if (exponent < 0) throw new ArgumentException("Exponent must be non-negative.");

		// Compute multiplier^exponent as a double
		double powerResult = Math.Pow(multiplier, exponent);

		// Multiply baseValue by the computed power
		BGN result = baseValue * powerResult;

		return result;
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
	public BGN(double num) {
		list.Add(0);
		addInt(num);
	}

	public BGN(string saveName) {
		Load(saveName);
	}

	public static BGN CalculateY(BGN x) {
		// Step 1: Multiply by 2
		BGN twoX = x * 2;

		// Step 2: Add 1/4 (BGN cannot store decimals, so approximate by adding 0)
		BGN value = twoX + new BGN(0);

		// Step 3: Compute the square root using Newton's method
		BGN sqrtValue = Sqrt(value);

		// Step 4: Subtract 1/2 (approximate in BGN as just subtracting 0)
		return sqrtValue - new BGN(0);
	}

	// Integer-based square root approximation using Newton's method
	public static BGN Sqrt(BGN n) {
		if (n == new BGN(0) || n == new BGN(1)) return n;

		BGN x0 = n;
		BGN x1 = (x0 + (n / x0)) / new BGN(2);

		while (x1 < x0) {
			x0 = x1;
			x1 = (x0 + (n/x0)) / new BGN(2);
		}

		return x0;
	}

	public static BGN Trim(BGN bgn) {
		while (bgn.list.LastOrDefault() == 0 && bgn.list.Count != 0) {
			bgn.list.RemoveAt(bgn.list.Count()-1);
		}
		return bgn;
	}

	public static float DivideF(BGN lhs, BGN rhs) {
		lhs = Trim(lhs);
		rhs = Trim(rhs);

		if (lhs.list.Count == rhs.list.Count) {
			return (float)lhs.list.LastOrDefault() / (float)rhs.list.LastOrDefault();
		} else if (lhs.list.Count > rhs.list.Count) {
			int diff = lhs.list.Count - rhs.list.Count;
			return (float)(lhs.list.LastOrDefault() * Mathf.Pow(1000, diff)) / (float)rhs.list.LastOrDefault();
		} else if (lhs.list.Count < rhs.list.Count) {
			int diff = rhs.list.Count - lhs.list.Count;
			return (float)(lhs.list.LastOrDefault()) / (float)(rhs.list.LastOrDefault() * Mathf.Pow(1000, diff));
		}
		return 0f;
	}

	public static BGN operator /(BGN lhs, BGN rhs) {
		if (rhs == new BGN(0)) throw new DivideByZeroException("Cannot divide by zero!");
		lhs = Trim(lhs);
		rhs = Trim(rhs);

		if (lhs.list.Count == rhs.list.Count) {
			return new BGN((int)((float)lhs.list.LastOrDefault() / (float)rhs.list.LastOrDefault()));
		} else if (lhs.list.Count > rhs.list.Count) {
			int diff = lhs.list.Count - rhs.list.Count;
			return new BGN(((double)(lhs.list.LastOrDefault() * Mathf.Pow(1000, diff)) / (double)rhs.list.LastOrDefault()));
		} else if (lhs.list.Count < rhs.list.Count) {
			int diff = rhs.list.Count - lhs.list.Count;
			return new BGN(((double)(lhs.list.LastOrDefault()) / (double)(rhs.list.LastOrDefault() * Mathf.Pow(1000, diff))));
		}
		return new BGN(0);
		//BGN quotient = new BGN();
		//BGN remainder = lhs;  // Make a copy of lhs for reduction
		//BGN one = new BGN(1);

		//// Ensure we are iterating over valid parts
		//while (remainder >= rhs) {
		//	BGN multiplier = new BGN(0);
		//	BGN temp = rhs;

		//	// Find the largest multiplier where (rhs * multiplier) <= remainder
		//	while (temp <= remainder) {
		//		temp = temp + rhs;
		//		multiplier = multiplier + one;
		//	}

		//	// Subtract the last valid multiple
		//	remainder = remainder - (rhs * (multiplier));
		//	quotient = quotient + (multiplier);
		//}

		//return quotient;
	}

	public void addInt(double num) {
		bool adding = true;
		int index = 0;
		while (adding) {
			if (index > list.Count - 1) {
				list.Add(0);
			}
			if (num > 999) {
				double temp = (num + list[index]) % 1000;
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

	public static BGN operator *(BGN lhs, double rhs) {
		if (rhs == 0) return new BGN(0); // Multiplication by zero case

		BGN result = new BGN();
		double carry = 0;

		for (int i = 0; i < lhs.list.Count; i++) {
			double product = lhs.list[i] * rhs + carry;  // Multiply and add carry
			result.list.Add((short)(product % 1000));   // Store last 3 digits
			carry = product / 1000;
			int j = i;// Carry is full value, including decimals

			for (j = i - 1; j > -1; j--) {
				product *= 1000;
				result.list[j] += (short)(product % 1000);
			}
		}

		// Handle any remaining carry (for cases where multiplication increases magnitude)
		while (carry >= 1) {
			result.list.Add((short)(carry % 1000));
			carry /= 1000;
		}

		return result;
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
