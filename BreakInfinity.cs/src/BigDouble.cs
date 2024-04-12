// Copyright 2024 Joonhyo Choi (asdfchlwnsgy1236), licensed under the Apache License Version 2.0.

namespace BreakInfinity {
	using System;

	[Serializable]
	public enum Notation {
		Standard,
		Scientific,
		Engineering
	}

	// public static int Sign(double value);

	// public static double Sqrt(double d);

	// public static double Cbrt(double d);

	// public static double Exp(double d);

	// public static double Exp10(double d);

	// public static double Pow(double x, double y);

	// public static double Log(double d);

	// public static double Log10(double d);

	// public static double Log(double a, double newBase);

	// public static double Sinh(double value);

	// public static double Cosh(double value);

	// public static double Tanh(double value);

	// public static double Asinh(double d);

	// public static double Acosh(double d);

	// public static double Atanh(double d);

	[Serializable]
	public struct BigDouble: IComparable, IComparable<BigDouble>, IEquatable<BigDouble>, IFormattable {
		private const int DoubleMinExponent = -324;
		private const int DoubleMaxExponent = 308;
		private const int DoubleZeroExponentIndex = -DoubleMinExponent - 1;
		private const int ThresholdExponentInt = 17;
		private const double ThresholdExponent = 17;
		private const double ThresholdDouble = 1e17;
		private const int DefaultLength = 9;
		private const int DefaultDecimals = 3;
		private const int DefaultSmallDec = 0;
		private const Notation DefaultNotation = Notation.Scientific;

		private static readonly double[] PowersOf10 = new double[DoubleMaxExponent - DoubleMinExponent];
		private static readonly string[] StandardNotationNames = { "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd", "Nod", "Vig" };
		private static readonly int StandardNotationThreshold;
		private static readonly char[] ParseDelimiters = { 'E', 'e' };

		public static readonly BigDouble Zero = new(0, 0, false);
		public static readonly BigDouble One = new(1, 0, false);
		public static readonly BigDouble E = new(Math.E, 0, false);
		public static readonly BigDouble Pi = new(Math.PI, 0, false);
		public static readonly BigDouble Ten = new(1, 1, false);
		public static readonly BigDouble MinValue = new(-BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(10) - 1), double.MaxValue, false);
		public static readonly BigDouble MaxValue = new(BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(10) - 1), double.MaxValue, false);
		public static readonly BigDouble Epsilon = new(1, double.MinValue, false);
		public static readonly BigDouble NaN = new(double.NaN, 0, false);
		public static readonly BigDouble NegativeInfinity = new(double.NegativeInfinity, 0, false);
		public static readonly BigDouble PositiveInfinity = new(double.PositiveInfinity, 0, false);

		public double Mantissa;
		public double Exponent;

		static BigDouble() {
			for(int a = 0, power = DoubleMinExponent + 1; a < PowersOf10.Length; ++a, ++power) {
				PowersOf10[a] = Math.Pow(10, power);
			}
			StandardNotationThreshold = (StandardNotationNames.Length + 1) * 3;
		}

		public BigDouble(double mantissa, double exponent, bool needsNormalization = true) {
			Mantissa = mantissa;
			Exponent = exponent;
			if(needsNormalization) {
				Normalize();
			}
		}

		public BigDouble(double n) {
			Mantissa = n;
			Exponent = 0;
			Normalize();
		}

		private static double Truncate(double n, int digits) {
			if(Math.Abs(n) >= ThresholdDouble) {
				return n;
			}
			double multi = GetPowerOf10(Math.Clamp(digits, DoubleMinExponent + 1, ThresholdExponentInt - 1));
			return Math.Truncate(n * multi) / multi;
		}

		private static double Floor(double n, int digits) {
			if(Math.Abs(n) >= ThresholdDouble) {
				return n;
			}
			double multi = GetPowerOf10(Math.Clamp(digits, DoubleMinExponent + 1, ThresholdExponentInt - 1));
			return Math.Floor(n * multi) / multi;
		}

		private static double Ceiling(double n, int digits) {
			if(Math.Abs(n) >= ThresholdDouble) {
				return n;
			}
			double multi = GetPowerOf10(Math.Clamp(digits, DoubleMinExponent + 1, ThresholdExponentInt - 1));
			return Math.Ceiling(n * multi) / multi;
		}

		private static double Round(double n, int digits, MidpointRounding mode = MidpointRounding.ToEven) {
			if(Math.Abs(n) >= ThresholdDouble) {
				return n;
			}
			double multi = GetPowerOf10(Math.Clamp(digits, DoubleMinExponent + 1, ThresholdExponentInt - 1));
			return Math.Round(n * multi, mode) / multi;
		}

		public static double GetPowerOf10(int power) => PowersOf10[power + DoubleZeroExponentIndex];

		public static string GetStandardName(int power) => power < 3 || power >= StandardNotationNames.Length ? "" : StandardNotationNames[power / 3 - 1];

		public static bool IsFinite(BigDouble n) => n.IsFinite();

		public static bool IsNaN(BigDouble n) => n.IsNaN();

		public static bool IsInfinity(BigDouble n) => n.IsInfinity();

		public static bool IsPositiveInfinity(BigDouble n) => n.IsPositiveInfinity();

		public static bool IsNegativeInfinity(BigDouble n) => n.IsNegativeInfinity();

		public static bool operator ==(BigDouble l, BigDouble r) => l.Equals(r);

		public static bool operator !=(BigDouble l, BigDouble r) => !(l == r);

		public static bool operator <(BigDouble l, BigDouble r) => l.CompareTo(r) < 0;

		public static bool operator >(BigDouble l, BigDouble r) => l.CompareTo(r) > 0;

		public static bool operator <=(BigDouble l, BigDouble r) => l.CompareTo(r) <= 0;

		public static bool operator >=(BigDouble l, BigDouble r) => l.CompareTo(r) >= 0;

		public static bool TryParse(string s, out BigDouble result) {
			if(string.IsNullOrEmpty(s)) {
				result = default;
				return false;
			}
			string[] split = s.Split(ParseDelimiters, 1);
			double[] me = { 0, 0 };
			for(int a = 0; a < split.Length; ++a) {
				if(!double.TryParse(split[a], out me[a])) {
					result = default;
					return false;
				}
			}
			result = new(me[0], me[1]);
			return true;
		}

		public static implicit operator BigDouble(double n) => new(n);

		public static explicit operator double(BigDouble n) {
			if(!n.IsFinite() || n.Mantissa == 0 || n.Exponent == 0) {
				return n.Mantissa;
			}
			if(n.Exponent > DoubleMaxExponent) {
				return n.Mantissa < 0 ? double.NegativeInfinity : double.PositiveInfinity;
			}
			if(n.Exponent < DoubleMinExponent) {
				return 0;
			}
			int eo = (int)n.Exponent, eo1 = eo / 2, eo2 = eo1 + eo % 2;
			return n.Mantissa * GetPowerOf10(eo1) * GetPowerOf10(eo2);
		}

		public static BigDouble operator +(BigDouble n) => n;

		public static BigDouble operator -(BigDouble n) => n.Negated();

		public static BigDouble operator ++(BigDouble n) => n.Added(One);

		public static BigDouble operator +(BigDouble l, BigDouble r) => l.Added(r);

		public static BigDouble operator --(BigDouble n) => n.Subtracted(One);

		public static BigDouble operator -(BigDouble l, BigDouble r) => l.Subtracted(r);

		public static BigDouble operator *(BigDouble l, BigDouble r) => l.Multiplied(r);

		public static BigDouble operator /(BigDouble l, BigDouble r) => l.Divided(r);

		public static BigDouble Reciprocal(BigDouble n) => n.Reciprocated();

		public static BigDouble Abs(BigDouble n) => n.Abs();

		public static BigDouble Truncate(BigDouble n) => n.Truncated();

		public static BigDouble Floor(BigDouble n) => n.Floored();

		public static BigDouble Ceiling(BigDouble n) => n.Ceiled();

		public static BigDouble Round(BigDouble n, int digits = 0, MidpointRounding mode = MidpointRounding.ToEven) => n.Rounded(digits, mode);

		public static BigDouble Min(BigDouble l, BigDouble r) => l < r || l.IsNaN() ? l : r;

		public static BigDouble Max(BigDouble l, BigDouble r) => l > r || l.IsNaN() ? l : r;

		/// <summary>
		///   Unlike Math.Clamp, this does not throw an exception when min is greater than max, so it is up to the developer to provide the right parameters.
		/// </summary>
		public static BigDouble Clamp(BigDouble n, BigDouble min, BigDouble max) => n < min ? min : n > max ? max : n;

		public readonly bool IsFinite() => double.IsFinite(Mantissa);

		public readonly bool IsNaN() => double.IsNaN(Mantissa);

		public readonly bool IsInfinity() => double.IsInfinity(Mantissa);

		public readonly bool IsPositiveInfinity() => double.IsPositiveInfinity(Mantissa);

		public readonly bool IsNegativeInfinity() => double.IsNegativeInfinity(Mantissa);

		public readonly int CompareTo(object other) {
			if(other == null) {
				return 1;
			}
			if(other is BigDouble n) {
				return CompareTo(n);
			}
			throw new ArgumentException($"Object must be of type {nameof(BigDouble)}.");
		}

		public readonly int CompareTo(BigDouble other) {
			int mcmp = Mantissa.CompareTo(other.Mantissa);
			if(!IsFinite() || !other.IsFinite() || Mantissa == 0 || other.Mantissa == 0 || Mantissa < 0 && other.Mantissa > 0 || Mantissa > 0 && other.Mantissa < 0) {
				return mcmp;
			}
			int ecmp = Exponent.CompareTo(other.Exponent);
			return ecmp == 0 ? mcmp : Mantissa < 0 ? -ecmp : ecmp;
		}

		public override readonly bool Equals(object other) => other is BigDouble n && Equals(n);

		public readonly bool Equals(BigDouble other) => Mantissa == other.Mantissa && Exponent == other.Exponent;

		public override readonly int GetHashCode() => HashCode.Combine(Mantissa, Exponent);

		public readonly string ToDebugString() => string.Concat("{", $"{Mantissa:g17}, {Exponent:g17}", "}");

		public override readonly string ToString() => ToString(null, null);

		/// <summary>
		///   This is the implementation of the IFormattable interface, but it mostly ignores the typical format string characters. It is still possible to use
		///   'G' followed by a number to specify the length (the 'e' in "1e100" is counted as well).
		/// </summary>
		/// <param name="format">
		///   Up to three integers separated by commas that specify the length, decimals, and smallDec values (see the overload of <see cref="ToString"/> that
		///   takes up to five parameters for what those values affect).
		/// </param>
		/// <param name="formatProvider">The format provider to apply to each number component.</param>
		public readonly string ToString(string format, IFormatProvider formatProvider = null) {
			int length = DefaultLength, decimals = DefaultDecimals, smallDec = DefaultSmallDec;
			if(!string.IsNullOrEmpty(format)) {
				string[] parts = format.Split(',');
				if(parts.Length <= 3) {
					int[] parsedValues = { DefaultLength, DefaultDecimals, DefaultSmallDec };
					bool isValid = true;
					for(int a = 0; a < parts.Length; ++a) {
						if(!int.TryParse(parts[a], out parsedValues[a])) {
							isValid = false;
							break;
						}
					}
					if(isValid) {
						length = parsedValues[0];
						decimals = parsedValues[1];
						smallDec = parsedValues[2];
					}
					else if(int.TryParse(parts[0][1..], out parsedValues[0])) {
						length = parsedValues[0];
					}
				}
			}
			return ToString(length, decimals, smallDec, DefaultNotation, formatProvider);
		}

		/// <summary>Makes a custom string representation that makes it easier to make a number stay within a certain length.</summary>
		/// <param name="length">
		///   The maximum length of the string representation in characters (includes minus signs and the 'e' in "1e100", excludes group separators and the
		///   decimal point).
		/// </param>
		/// <param name="decimals">The maximum number of digits to show after the decimal point when the number requires abbreviation.</param>
		/// <param name="smallDec">The maximum number of digits to show after the decimal point when the number is small enough to be shown as is.</param>
		/// <param name="notation">
		///   The type of notation to use when abbreviating the number (standard = letters, scientific = #e#, engineering = scientific but exponent fixed to
		///   multiples of 3).
		/// </param>
		/// <param name="formatProvider">The format provider to apply to each number component.</param>
		/// <remarks>
		///   When the number is too large for standard notation, it falls back to scientific notation. When the number is too large for scientific or
		///   engineering notation in the given length, it falls back to the format AeBeC = A * 10 ^ (B * 10 ^ C).
		/// </remarks>
		public readonly string ToString(int length = DefaultLength, int decimals = DefaultDecimals, int smallDec = DefaultSmallDec, Notation notation = DefaultNotation, IFormatProvider formatProvider = null) {
			const string NumberFormat = "#,0.###############";
			length = Math.Clamp(length, 3, 16 - (Mantissa < 0 ? 1 : 0) - (Exponent < 0 ? 1 : 0));
			decimals = Math.Clamp(decimals, 0, 15);
			smallDec = Math.Clamp(smallDec, 0, 15);
			if(!IsFinite()) {
				return Mantissa.ToString(formatProvider);
			}
			if(Exponent < length) {
				return Truncate(Mantissa * GetPowerOf10((int)Exponent), Math.Min(Math.Min(smallDec, (int)(smallDec - Exponent)), length - 1)).ToString(NumberFormat, formatProvider);
			}
			double e = Exponent, m = Truncate(Mantissa, Math.Min(decimals, length - (int)Math.Log10(Exponent) - 3));
			int ee = (int)Math.Log10(e);
			double me = Truncate(e / GetPowerOf10(ee), Math.Min(decimals, length - (int)Math.Log10(ee) - 5));
			double offset = Exponent % 3, e3 = Exponent - offset, m3 = Math.Truncate(Mantissa * 100) / GetPowerOf10((int)(2 - offset));
			if(ee < length - 2 || ee < 3) {
				switch(notation) {
					case Notation.Standard when e3 < StandardNotationThreshold:
						return string.Concat(m3.ToString(NumberFormat, formatProvider), GetStandardName((int)e3));
					case Notation.Engineering:
						return string.Concat(m3.ToString(NumberFormat, formatProvider), "e", e3.ToString(NumberFormat, formatProvider));
					default:
						return string.Concat(m.ToString(NumberFormat, formatProvider), "e", e.ToString(NumberFormat, formatProvider));
				}
			}
			return string.Concat(m.ToString(NumberFormat, formatProvider), "e", me.ToString(NumberFormat, formatProvider), "e", ee.ToString(NumberFormat, formatProvider));
		}

		/// <summary>
		///   Brings the number back into normal form (the mantissa's magnitude is greater than or equal to 1 and less than 10, and the exponent is an integer
		///   value).
		/// </summary>
		/// <remarks>In the case of the non-finite values (NaN and positive/negative infinity), this sets the number to the corresponding preset.</remarks>
		public void Normalize() {
			double mAbs = Math.Abs(Mantissa), ef = Exponent % 1;
			if(mAbs >= 1 && mAbs < 10 && ef == 0) {
				return;
			}
			if(Mantissa == 0) {
				this = Zero;
				return;
			}
			if(double.IsNaN(Mantissa) || double.IsNaN(Exponent)) {
				this = NaN;
				return;
			}
			int eo = (int)Math.Floor(Math.Log10(mAbs) + ef), eo1 = eo / 2, eo2 = eo1 + eo % 2;
			Mantissa = Mantissa / GetPowerOf10(eo1) / GetPowerOf10(eo2) * Math.Pow(10, ef);
			Exponent = Math.Truncate(Exponent) + eo;
			bool ismi = double.IsInfinity(Mantissa);
			if(double.IsInfinity(Exponent)) {
				this = Exponent < 0 ? ismi ? NaN : Zero : Mantissa < 0 ? NegativeInfinity : PositiveInfinity;
				return;
			}
			if(ismi) {
				this = Mantissa < 0 ? NegativeInfinity : PositiveInfinity;
			}
		}

		public readonly BigDouble Normalized() => new(Mantissa, Exponent);

		public void Negate() => Mantissa = -Mantissa;

		public readonly BigDouble Negated() => new(-Mantissa, Exponent, false);

		public void Add(BigDouble other) {
			int diff = (int)Math.Round(Exponent - other.Exponent);
			if(diff >= ThresholdExponentInt) {
				return;
			}
			if(diff <= -ThresholdExponentInt) {
				this = other;
				return;
			}
			if(!IsFinite() || !other.IsFinite()) {
				Mantissa += other.Mantissa;
				Exponent = 0;
				return;
			}
			if(Exponent < other.Exponent) {
				Mantissa += other.Mantissa * GetPowerOf10(-diff);
			}
			else {
				Mantissa = Mantissa * GetPowerOf10(diff) + other.Mantissa;
				Exponent -= diff;
			}
			Normalize();
		}

		public readonly BigDouble Added(BigDouble other) {
			BigDouble n = this;
			n.Add(other);
			return n;
		}

		public void Add1OrUlp() {
			BigDouble original = this;
			Add(One);
			if(this == original) {
				Mantissa = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Mantissa) + (Mantissa < 0 ? -1 : 1));
				Normalize();
				if(Mantissa < original.Mantissa && Exponent == original.Exponent) {
					Exponent = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Exponent) + 1);
					Normalize();
				}
			}
		}

		public readonly BigDouble Added1OrUlp() {
			BigDouble n = this;
			n.Add1OrUlp();
			return n;
		}

		public void Subtract(BigDouble other) => Add(-other);

		public readonly BigDouble Subtracted(BigDouble other) {
			BigDouble n = this;
			n.Subtract(other);
			return n;
		}

		public void Subtract1OrUlp() {
			BigDouble original = this;
			Subtract(One);
			if(this == original) {
				Mantissa = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Mantissa) + (Mantissa < 0 ? 1 : -1));
				Normalize();
				if(Mantissa > original.Mantissa && Exponent == original.Exponent) {
					Exponent = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Exponent) - 1);
					Normalize();
				}
			}
		}

		public readonly BigDouble Subtracted1OrUlp() {
			BigDouble n = this;
			n.Subtract1OrUlp();
			return n;
		}

		public void Multiply(BigDouble other) {
			Mantissa *= other.Mantissa;
			Exponent += other.Exponent;
			Normalize();
		}

		public readonly BigDouble Multiplied(BigDouble other) {
			BigDouble n = this;
			n.Multiply(other);
			return n;
		}

		public void Divide(BigDouble other) {
			Mantissa /= other.Mantissa;
			Exponent -= other.Exponent;
			Normalize();
		}

		public readonly BigDouble Divided(BigDouble other) {
			BigDouble n = this;
			n.Divide(other);
			return n;
		}

		public void Reciprocate() {
			Mantissa = 1 / Mantissa;
			Exponent = -Exponent;
			Normalize();
		}

		public readonly BigDouble Reciprocated() {
			BigDouble n = this;
			n.Reciprocate();
			return n;
		}

		public readonly BigDouble Abs() => new(Math.Abs(Mantissa), Exponent, false);

		public void Truncate(int digits = 0) {
			if(Exponent >= ThresholdExponent || !IsFinite()) {
				return;
			}
			if(Exponent < -digits) {
				this = Zero;
			}
			Mantissa = Truncate(Mantissa, (int)Exponent + digits);
		}

		public readonly BigDouble Truncated(int digits = 0) {
			BigDouble n = this;
			n.Truncate(digits);
			return n;
		}

		public void Floor(int digits = 0) {
			if(Exponent >= ThresholdExponent || !IsFinite()) {
				return;
			}
			if(Exponent < -digits) {
				this = Mantissa < 0 ? -One : Zero;
			}
			Mantissa = Floor(Mantissa, (int)Exponent + digits);
			Normalize();
		}

		public readonly BigDouble Floored(int digits = 0) {
			BigDouble n = this;
			n.Floor(digits);
			return n;
		}

		public void Ceil(int digits = 0) {
			if(Exponent >= ThresholdExponent || !IsFinite()) {
				return;
			}
			if(Exponent < -digits) {
				this = Mantissa < 0 ? Zero : One;
			}
			Mantissa = Ceiling(Mantissa, (int)Exponent + digits);
			Normalize();
		}

		public readonly BigDouble Ceiled(int digits = 0) {
			BigDouble n = this;
			n.Ceil(digits);
			return n;
		}

		public void Round(int digits = 0, MidpointRounding mode = MidpointRounding.ToEven) {
			if(Exponent >= ThresholdExponent || !IsFinite()) {
				return;
			}
			if(Exponent < -digits - 1) {
				this = Zero;
			}
			Mantissa = Round(Mantissa, (int)Exponent + digits, mode);
			Normalize();
		}

		public readonly BigDouble Rounded(int digits = 0, MidpointRounding mode = MidpointRounding.ToEven) {
			BigDouble n = this;
			n.Round(digits, mode);
			return n;
		}
	}
}