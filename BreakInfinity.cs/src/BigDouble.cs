// Copyright 2024 Joonhyo Choi (asdfchlwnsgy1236), licensed under the Apache License Version 2.0.

namespace BreakInfinity {
	using System;

	[Serializable]
	public enum Notation {
		Standard,
		Scientific,
		Engineering
	}

	/// <summary>
	///   <para>
	///     This is a replacement for <see cref="double"/> for use with numbers as large as 1e1e308 == 10^(10^308) and as small as 1e-1e308 == 10^-(10^308) ==
	///     1/(10^(10^308)), and prioritizes performance over accuracy.
	///   </para>
	///   <para>
	///     The most noticeable consequence of prioritizing performance is that numbers above a certain threshold no longer have a proper mantissa due to the
	///     limited precision, but this should be irrelevant for this type's intended use case (incremental games).
	///   </para>
	///   <para>Note that the instance functions suffixed with "Mod" modify the instance they are called on instead of making a copy.</para>
	/// </summary>
	[Serializable]
	public struct BigDouble: IComparable, IComparable<BigDouble>, IEquatable<BigDouble>, IFormattable {
		private const int DoubleMinExponent = -324;
		private const int DoubleMaxExponent = 308;
		private const int DoubleZeroExponentIndex = -DoubleMinExponent - 1;
		private const int ThresholdMod1Exponent = 15;
		private const int ThresholdAdd10Exponent = 17;
		private const double ThresholdMod1Double = 4503599627370496;
		private const int DefaultLength = 9;
		private const int DefaultDecimals = 3;
		private const int DefaultSmallDec = 0;
		private const Notation DefaultNotation = Notation.Scientific;

		private static readonly double[] PowersOf10 = new double[DoubleMaxExponent - DoubleMinExponent];
		private static readonly string[] StandardNotationNames = { "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd", "Nod", "Vig" };
		private static readonly double StandardNotationThreshold;
		private static readonly char[] ParseDelimiters = { 'E', 'e' };
		private static readonly BigDouble Ln10 = new(2.3025850929940456840, 0, false);

		public static readonly BigDouble Zero = new(0, 0, false);
		public static readonly BigDouble Half = new(5, -1, false);
		public static readonly BigDouble One = new(1, 0, false);
		public static readonly BigDouble Two = new(2, 0, false);
		public static readonly BigDouble E = new(Math.E, 0, false);
		public static readonly BigDouble Pi = new(Math.PI, 0, false);
		public static readonly BigDouble Ten = new(1, 1, false);
		public static readonly BigDouble MinValue = new(-1, double.MaxValue, false);
		public static readonly BigDouble MaxValue = new(1, double.MaxValue, false);
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
				NormalizeMod();
			}
		}

		public BigDouble(double n) {
			Mantissa = n;
			Exponent = 0;
			NormalizeMod();
		}

		private static double Truncate(double n, int digits) {
			if(Math.Abs(n) >= ThresholdMod1Double) {
				return n;
			}
			double multi = GetPowerOf10(digits);
			return Math.Truncate(n * multi) / multi;
		}

		private static double Floor(double n, int digits) {
			if(Math.Abs(n) >= ThresholdMod1Double) {
				return n;
			}
			double multi = GetPowerOf10(digits);
			return Math.Floor(n * multi) / multi;
		}

		private static double Ceiling(double n, int digits) {
			if(Math.Abs(n) >= ThresholdMod1Double) {
				return n;
			}
			double multi = GetPowerOf10(digits);
			return Math.Ceiling(n * multi) / multi;
		}

		private static double Round(double n, int digits, MidpointRounding mode = default) {
			if(Math.Abs(n) >= ThresholdMod1Double) {
				return n;
			}
			double multi = GetPowerOf10(digits);
			return Math.Round(n * multi, mode) / multi;
		}

		public static double GetPowerOf10(int power) => PowersOf10[power + DoubleZeroExponentIndex];

		public static string GetStandardName(int power) => power < 3 || power >= StandardNotationNames.Length ? "" : StandardNotationNames[power / 3 - 1];

		public static bool IsFinite(BigDouble n) => n.IsFinite();

		public static bool IsNaN(BigDouble n) => n.IsNaN();

		public static bool IsInfinity(BigDouble n) => n.IsInfinity();

		public static bool IsPositiveInfinity(BigDouble n) => n.IsPositiveInfinity();

		public static bool IsNegativeInfinity(BigDouble n) => n.IsNegativeInfinity();

		public static bool IsNegative(BigDouble n) => n.IsNegative();

		public static bool IsZero(BigDouble n) => n.IsZero();

		public static int Sign(BigDouble n) => n.Sign();

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
			if(!n.IsFinite() || n.IsZero() || n.Exponent == 0) {
				return n.Mantissa;
			}
			if(n.Exponent > DoubleMaxExponent) {
				return n.IsNegative() ? double.NegativeInfinity : double.PositiveInfinity;
			}
			if(n.Exponent < DoubleMinExponent) {
				return 0;
			}
			int eo = (int)n.Exponent, eo1 = eo / 2, eo2 = eo1 + eo % 2;
			return n.Mantissa * GetPowerOf10(eo1) * GetPowerOf10(eo2);
		}

		public static BigDouble operator +(BigDouble n) => n;

		public static BigDouble operator -(BigDouble n) => n.Negate();

		public static BigDouble operator ++(BigDouble n) => n.Add(One);

		public static BigDouble operator +(BigDouble l, BigDouble r) => l.Add(r);

		public static BigDouble operator --(BigDouble n) => n.Subtract(One);

		public static BigDouble operator -(BigDouble l, BigDouble r) => l.Subtract(r);

		public static BigDouble operator *(BigDouble l, BigDouble r) => l.Multiply(r);

		public static BigDouble operator /(BigDouble l, BigDouble r) => l.Divide(r);

		public static BigDouble Reciprocal(BigDouble n) => n.Reciprocate();

		public static BigDouble Abs(BigDouble n) => n.Abs();

		public static BigDouble Sqrt(BigDouble n) => n.Sqrt();

		public static BigDouble Cbrt(BigDouble n) => n.Cbrt();

		public static BigDouble Square(BigDouble n) => n.Square();

		public static BigDouble Cube(BigDouble n) => n.Cube();

		public static double Log10(BigDouble n) => n.Log10();

		public static BigDouble Ln(BigDouble n) => n.Ln();

		public static BigDouble Log(BigDouble n, double b) => n.Log(b);

		public static double Log(BigDouble n, BigDouble b) => n.Log(b);

		public static BigDouble Exp10(double n) => n % 1 == 0 ? new(1, n, false) : new(1, n);

		public static BigDouble Pow(BigDouble b, double p) => b.Pow(p);

		public static BigDouble Exp(double p) => Math.Abs(p) <= 708 ? (BigDouble)Math.Exp(p) : E.Pow(p);

		public static BigDouble Sinh(double n) => (Exp(n) - Exp(-n)) * Half;

		public static BigDouble Cosh(double n) => (Exp(n) + Exp(-n)) * Half;

		public static BigDouble Tanh(double n) {
			BigDouble epx = Exp(n), enx = Exp(-n);
			return (epx - enx) / (epx + enx);
		}

		public static BigDouble Asinh(BigDouble n) => Ln(n + Sqrt(Square(n) + One));

		public static BigDouble Acosh(BigDouble n) => Ln(n + Sqrt(Square(n) - One));

		public static BigDouble Atanh(BigDouble n) => Ln((One + n) / (One - n)) * Half;

		public static BigDouble Truncate(BigDouble n, int digits = 0) => n.Truncate(digits);

		public static BigDouble Floor(BigDouble n, int digits = 0) => n.Floor(digits);

		public static BigDouble Ceiling(BigDouble n, int digits = 0) => n.Ceiling(digits);

		public static BigDouble Round(BigDouble n, int digits = 0, MidpointRounding mode = default) => n.Round(digits, mode);

		public static BigDouble Min(BigDouble l, BigDouble r) => l < r || l.IsNaN() ? l : r;

		public static BigDouble Max(BigDouble l, BigDouble r) => l > r || l.IsNaN() ? l : r;

		/// <summary>
		///   Unlike <see cref="Math.Clamp"/>, this does not throw an exception when min is greater than max, so it is up to the developer to provide the right
		///   parameters for the correct behavior.
		/// </summary>
		public static BigDouble Clamp(BigDouble n, BigDouble min, BigDouble max) => n < min ? min : n > max ? max : n;

		public readonly bool IsFinite() => double.IsFinite(Mantissa);

		public readonly bool IsNaN() => double.IsNaN(Mantissa);

		public readonly bool IsInfinity() => double.IsInfinity(Mantissa);

		public readonly bool IsPositiveInfinity() => double.IsPositiveInfinity(Mantissa);

		public readonly bool IsNegativeInfinity() => double.IsNegativeInfinity(Mantissa);

		public readonly bool IsNegative() => double.IsNegative(Mantissa);

		public readonly bool IsZero() => Mantissa == 0;

		public readonly int Sign() => Math.Sign(Mantissa);

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
			int ecmp = Exponent.CompareTo(other.Exponent);
			bool isNegative = IsNegative();
			return !IsFinite() || !other.IsFinite() || IsZero() || other.IsZero() || ecmp == 0 || isNegative != other.IsNegative() ? Mantissa.CompareTo(other.Mantissa) : isNegative ? -ecmp : ecmp;
		}

		public override readonly bool Equals(object other) => other is BigDouble n && Equals(n);

		public readonly bool Equals(BigDouble other) => Mantissa == other.Mantissa && Exponent == other.Exponent;

		public override readonly int GetHashCode() => HashCode.Combine(Mantissa, Exponent);

		public readonly string ToDebugString() => string.Concat("{", $"{Mantissa:g17}, {Exponent:g17}", "}");

		public override readonly string ToString() => ToString(null, null);

		/// <summary>
		///   This is the implementation of the <see cref="IFormattable"/> interface, but it mostly ignores the typical format string characters. It is still
		///   possible to use 'G' followed by a number to specify the length (the 'e' in "1e100" is counted as well).
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

		/// <summary>Makes a custom string representation that makes it easier to make this number stay within a certain length.</summary>
		/// <param name="length">
		///   The maximum length of the string representation in characters (includes minus signs and the 'e' in "1e100", excludes group separators and the
		///   decimal point).
		/// </param>
		/// <param name="decimals">The maximum number of digits to show after the decimal point when this number requires abbreviation.</param>
		/// <param name="smallDec">The maximum number of digits to show after the decimal point when this number is small enough to be shown as is.</param>
		/// <param name="notation">
		///   The type of notation to use when abbreviating this number (standard = letters like k and m, scientific = AeB = A * 10 ^ B, engineering =
		///   scientific but exponent fixed to multiples of 3).
		/// </param>
		/// <param name="formatProvider">The format provider to apply to each number component.</param>
		/// <remarks>
		///   When this number is too large for standard notation, it falls back to scientific notation. When this number is too large for scientific or
		///   engineering notation in the given length, it falls back to the format AeBeC = A * 10 ^ (B * 10 ^ C). When the magnitude of the exponent of this
		///   number is greater than <see cref="ThresholdMod1Double"/>, it stops displaying the mantissa (A in the previous format description) since it is
		///   always 1.
		/// </remarks>
		public readonly string ToString(int length = DefaultLength, int decimals = DefaultDecimals, int smallDec = DefaultSmallDec, Notation notation = DefaultNotation, IFormatProvider formatProvider = null) {
			const string NumberFormat = "#,0.###############";
			double eAbs = Math.Abs(Exponent);
			bool ismsig = eAbs < ThresholdMod1Double, ismn = double.IsNegative(Mantissa), isen = double.IsNegative(Exponent);
			if(ismsig) {
				--length;
			}
			if(ismn) {
				--length;
			}
			if(isen) {
				--length;
			}
			length = Math.Clamp(length, 2, 15);
			decimals = Math.Clamp(decimals, 0, 15);
			smallDec = Math.Clamp(smallDec, 0, 15);
			if(!IsFinite()) {
				return Mantissa.ToString(formatProvider);
			}
			if(eAbs <= length) {
				int ei = (int)Exponent;
				return Truncate(Mantissa * GetPowerOf10(ei), Math.Clamp(smallDec - ei, 0, Math.Min(length - ei, length))).ToString(NumberFormat, formatProvider);
			}
			int ee = (int)Math.Log10(eAbs);
			double m = Truncate(Mantissa, Math.Max(Math.Min(decimals, length - 2 - ee), 0));
			double me = Truncate(Exponent / GetPowerOf10(ee), Math.Max(Math.Min(decimals, length - 4 - (int)Math.Log10(ee)), 0));
			if(isen) {
				me = -me;
			}
			double offset = Exponent % 3;
			if(offset < 0) {
				offset += 3;
			}
			double e3 = Exponent - offset, m3 = Math.Truncate(Mantissa * 100) / GetPowerOf10((int)(2 - offset));
			if(ee < length - 1 || ee < 3) {
				switch(notation) {
					case Notation.Standard when e3 >= 0 && e3 < StandardNotationThreshold:
						return string.Concat(m3.ToString(NumberFormat, formatProvider), GetStandardName((int)e3));
					case Notation.Engineering:
						return string.Concat(m3.ToString(NumberFormat, formatProvider), "e", e3.ToString(NumberFormat, formatProvider));
					default:
						return string.Concat(m.ToString(NumberFormat, formatProvider), "e", Exponent.ToString(NumberFormat, formatProvider));
				}
			}
			if(ismsig) {
				return string.Concat(m.ToString(NumberFormat, formatProvider), "e", me.ToString(NumberFormat, formatProvider), "e", ee.ToString(NumberFormat, formatProvider));
			}
			return string.Concat(ismn ? "-e" : "e", me.ToString(NumberFormat, formatProvider), "e", ee.ToString(NumberFormat, formatProvider));
		}

		/// <summary>
		///   <para>Brings this number back into normal form which is one of the following:</para>
		///   <list type="bullet">
		///     <item>|exponent| &lt; threshold and 1 &lt;= |mantissa| &lt; 10 and the exponent has no fractional part</item>
		///     <item>|exponent| &gt;= threshold and |mantissa| == 1</item>
		///   </list>
		///   <para>where |n| is the absolute value of n, and threshold is <see cref="ThresholdMod1Double"/>.</para>
		/// </summary>
		/// <remarks>In the case of any non-finite values (NaN and positive/negative infinity), this sets the number to the corresponding preset.</remarks>
		public void NormalizeMod() {
			double mAbs = Math.Abs(Mantissa), eAbs = Math.Abs(Exponent), ef = Exponent % 1;
			bool ismsig = eAbs < ThresholdMod1Double, isNormalLow = mAbs >= 1 && mAbs < 10 && ef == 0;
			if(ismsig && isNormalLow || !ismsig && mAbs == 1) {
				return;
			}
			if(IsZero()) {
				this = double.IsPositiveInfinity(Exponent) ? NaN : Zero;
				return;
			}
			if(double.IsNaN(Mantissa) || double.IsNaN(Exponent)) {
				this = NaN;
				return;
			}
			bool ismi = double.IsInfinity(Mantissa), ismn = double.IsNegative(Mantissa);
			if(double.IsInfinity(Exponent)) {
				this = double.IsNegative(Exponent) ? ismi ? NaN : Zero : ismn ? NegativeInfinity : PositiveInfinity;
				return;
			}
			if(ismi) {
				Exponent = 0;
				return;
			}
			if(!isNormalLow) {
				int eo = (int)Math.Floor(Math.Log10(mAbs) + ef);
				Exponent = Math.Truncate(Exponent) + eo;
				ismsig = Math.Abs(Exponent) < ThresholdMod1Double;
				if(ismsig) {
					int eo1 = eo / 2, eo2 = eo1 + eo % 2;
					Mantissa = Mantissa / GetPowerOf10(eo1) / GetPowerOf10(eo2) * Math.Pow(10, ef);
				}
			}
			if(!ismsig) {
				Mantissa = ismn ? -1 : 1;
			}
		}

		public readonly BigDouble Normalize() => new(Mantissa, Exponent);

		public void NegateMod() => Mantissa = -Mantissa;

		public readonly BigDouble Negate() => new(-Mantissa, Exponent, false);

		public void AddMod(BigDouble other) {
			double diff = Math.Round(Exponent - other.Exponent);
			if(diff > ThresholdAdd10Exponent) {
				return;
			}
			if(diff < -ThresholdAdd10Exponent) {
				this = other;
				return;
			}
			if(!IsFinite() || !other.IsFinite()) {
				Mantissa += other.Mantissa;
				Exponent = 0;
				return;
			}
			if(Exponent < other.Exponent) {
				Mantissa += other.Mantissa * GetPowerOf10(-(int)diff);
			}
			else {
				Mantissa = Mantissa * GetPowerOf10((int)diff) + other.Mantissa;
				Exponent -= diff;
			}
			NormalizeMod();
		}

		public readonly BigDouble Add(BigDouble other) {
			BigDouble n = this;
			n.AddMod(other);
			return n;
		}

		public void Add1OrUlpMod() {
			BigDouble original = this;
			AddMod(One);
			if(this == original) {
				Mantissa = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Mantissa) + (IsNegative() ? -1 : 1));
				NormalizeMod();
				if(Mantissa < original.Mantissa && Exponent == original.Exponent) {
					Exponent = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Exponent) + 1);
					NormalizeMod();
				}
			}
		}

		public readonly BigDouble Add1OrUlp() {
			BigDouble n = this;
			n.Add1OrUlpMod();
			return n;
		}

		public void SubtractMod(BigDouble other) => AddMod(-other);

		public readonly BigDouble Subtract(BigDouble other) {
			BigDouble n = this;
			n.SubtractMod(other);
			return n;
		}

		public void Subtract1OrUlpMod() {
			BigDouble original = this;
			SubtractMod(One);
			if(this == original) {
				Mantissa = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Mantissa) + (IsNegative() ? 1 : -1));
				NormalizeMod();
				if(Mantissa > original.Mantissa && Exponent == original.Exponent) {
					Exponent = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Exponent) - 1);
					NormalizeMod();
				}
			}
		}

		public readonly BigDouble Subtract1OrUlp() {
			BigDouble n = this;
			n.Subtract1OrUlpMod();
			return n;
		}

		public void MultiplyMod(BigDouble other) {
			Mantissa *= other.Mantissa;
			Exponent += other.Exponent;
			NormalizeMod();
		}

		public readonly BigDouble Multiply(BigDouble other) {
			BigDouble n = this;
			n.MultiplyMod(other);
			return n;
		}

		public void DivideMod(BigDouble other) {
			Mantissa /= other.Mantissa;
			Exponent -= other.Exponent;
			NormalizeMod();
		}

		public readonly BigDouble Divide(BigDouble other) {
			BigDouble n = this;
			n.DivideMod(other);
			return n;
		}

		public void ReciprocateMod() {
			Mantissa = 1 / Mantissa;
			Exponent = -Exponent;
			NormalizeMod();
		}

		public readonly BigDouble Reciprocate() {
			BigDouble n = this;
			n.ReciprocateMod();
			return n;
		}

		public void AbsMod() => Mantissa = Math.Abs(Mantissa);

		public readonly BigDouble Abs() {
			BigDouble n = this;
			n.AbsMod();
			return n;
		}

		public void SqrtMod() {
			Mantissa = Math.Sqrt(Mantissa);
			Exponent /= 2;
			NormalizeMod();
		}

		public readonly BigDouble Sqrt() {
			BigDouble n = this;
			n.SqrtMod();
			return n;
		}

		public void CbrtMod() {
			Mantissa = Math.Cbrt(Mantissa);
			Exponent /= 3;
			NormalizeMod();
		}

		public readonly BigDouble Cbrt() {
			BigDouble n = this;
			n.CbrtMod();
			return n;
		}

		public void SquareMod() {
			Mantissa *= Mantissa;
			Exponent *= 2;
			NormalizeMod();
		}

		public readonly BigDouble Square() {
			BigDouble n = this;
			n.SquareMod();
			return n;
		}

		public void CubeMod() {
			Mantissa *= Mantissa * Mantissa;
			Exponent *= 3;
			NormalizeMod();
		}

		public readonly BigDouble Cube() {
			BigDouble n = this;
			n.CubeMod();
			return n;
		}

		/// <summary>Returns the log base 10 of this number as a <see cref="double"/> since all possible return values can be represented with it.</summary>
		public readonly double Log10() => Math.Log10(Mantissa) + Exponent;

		public readonly BigDouble Ln() => Math.Log(Mantissa) + (BigDouble)Exponent * Ln10;

		public readonly BigDouble Log(double b) => Math.Log(Mantissa, b) + (BigDouble)Exponent * Math.Log(10, b);

		/// <summary>
		///   This is meant for use with bases that are either greater than <see cref="double.MaxValue"/> or less than <see cref="double.Epsilon"/> in
		///   magnitude, and returns a <see cref="double"/> with that assumption.
		/// </summary>
		public readonly double Log(BigDouble b) => Log10() / Log10(b);

		public readonly BigDouble Pow(double power) {
			double mpow = Math.Pow(Mantissa, power);
			if(IsZero() || !IsFinite() || !double.IsFinite(power) || mpow != 0 && !double.IsInfinity(mpow)) {
				return new(mpow, Exponent * power);
			}
			bool isNegative = IsNegative();
			double pmod2 = power % 2;
			if(isNegative && !(pmod2 is 0 or 1)) {
				return NaN;
			}
			BigDouble n = Exp10(Log10(Abs()) * power);
			return isNegative && pmod2 == 0 ? n : -n;
		}

		public void TruncateMod(int digits = 0) {
			if(Exponent > ThresholdMod1Exponent || !IsFinite()) {
				return;
			}
			if(Exponent < -digits) {
				this = Zero;
				return;
			}
			Mantissa = Truncate(Mantissa, (int)Exponent + digits);
		}

		public readonly BigDouble Truncate(int digits = 0) {
			BigDouble n = this;
			n.TruncateMod(digits);
			return n;
		}

		public void FloorMod(int digits = 0) {
			if(Exponent > ThresholdMod1Exponent || !IsFinite()) {
				return;
			}
			if(Exponent < -digits) {
				this = IsNegative() ? -One : Zero;
				return;
			}
			Mantissa = Floor(Mantissa, (int)Exponent + digits);
			NormalizeMod();
		}

		public readonly BigDouble Floor(int digits = 0) {
			BigDouble n = this;
			n.FloorMod(digits);
			return n;
		}

		public void CeilingMod(int digits = 0) {
			if(Exponent > ThresholdMod1Exponent || !IsFinite()) {
				return;
			}
			if(Exponent < -digits) {
				this = IsNegative() ? Zero : One;
				return;
			}
			Mantissa = Ceiling(Mantissa, (int)Exponent + digits);
			NormalizeMod();
		}

		public readonly BigDouble Ceiling(int digits = 0) {
			BigDouble n = this;
			n.CeilingMod(digits);
			return n;
		}

		public void RoundMod(int digits = 0, MidpointRounding mode = default) {
			if(Exponent > ThresholdMod1Exponent || !IsFinite()) {
				return;
			}
			if(Exponent < -digits - 1) {
				this = Zero;
				return;
			}
			Mantissa = Round(Mantissa, (int)Exponent + digits, mode);
			NormalizeMod();
		}

		public readonly BigDouble Round(int digits = 0, MidpointRounding mode = default) {
			BigDouble n = this;
			n.RoundMod(digits, mode);
			return n;
		}
	}
}