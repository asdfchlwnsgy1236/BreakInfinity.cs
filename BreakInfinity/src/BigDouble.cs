// Copyright 2024 Joonhyo Choi (asdfchlwnsgy1236); Apache License Version 2.0.

#nullable enable

namespace BreakInfinity {
	using System;
	using System.Collections.Generic;
	using System.Globalization;

#if NET5_0_OR_GREATER
	// Starting from .NET 5, BinaryFormatter (and therefore the Serializable attribute) have been in the process of obsoletion and removal due to security vulnerabilities, which means the Serializable attribute will have no
	// reason to be used here once Unity upgrades to a modern version of .NET. This preprocessor symbol looks like the best candidate to check for this correctly even after Unity's eventual upgrade.
#elif UNITY_2021_2_OR_NEWER
	[Serializable]
#else
#error The current compiler is too old for this code (requires C# 9).
#error version
#endif
	public enum Notation {
		Standard,
		Scientific,
		Engineering
	}

	/// <summary>
	///   <para>
	///     This is a replacement for <see cref="double"/> for use with numbers as large as 1e1e308 == 10^(10^308) and as small as 1e-1e308 == 10^-(10^308) == 1/(10^(10^308)), and prioritizes performance over precision.
	///   </para>
	///   <para>
	///     The most noticeable consequence of prioritizing performance is that numbers above a certain threshold no longer have a proper mantissa due to the limited precision, but this should be irrelevant for this type's
	///     intended use case (incremental games).
	///   </para>
	///   <para>Note that the instance functions suffixed with "Mod" modify the instance they are called on instead of making a copy.</para>
	///   <para>Also note that the behavior for special cases are made to match <see cref="double"/> behavior as much as possible.</para>
	/// </summary>
#if NET5_0_OR_GREATER
#elif UNITY_2021_2_OR_NEWER
	[Serializable]
#endif
	public struct BigDouble: IComparable, IComparable<BigDouble>, IEquatable<BigDouble>, IFormattable {
		private const double DoubleMinMantissa = double.Epsilon * 1e162 * 1e162;
		private const int DoubleMinExponent = -324;
		private const double DoubleMaxMantissa = double.MaxValue * 1e-154 * 1e-154;
		private const int DoubleMaxExponent = 308;
		private const int DoubleZeroExponentIndex = -DoubleMinExponent - 1;
		private const int ThresholdMod1Exponent = 16;
		private const int ThresholdAdd10Exponent = 18;
		private const double ThresholdMod1Double = 1L << 52;
		private const int DefaultLength = 9;
		private const int DefaultDecimals = 6;
		private const int DefaultSmallDec = 6;
		private const Notation DefaultNotation = Notation.Scientific;

		private static readonly double[] PowersOf10 = new double[DoubleMaxExponent - DoubleMinExponent];
		private static readonly string[] StandardNotationNames
#if NET8_0_OR_GREATER
			= ["K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd", "Nod", "Vig"];
#else
			= { "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd", "Nod", "Vig" };
#endif
		private static readonly Dictionary<string, double> StandardNotationExponents;
		private static readonly double StandardNotationThreshold;
		private static readonly CultureInfo DefaultCulture = CultureInfo.InvariantCulture;

		public static readonly BigDouble Zero = new(0, false);
		public static readonly BigDouble Tenth = new(1, -1, false);
		public static readonly BigDouble Half = new(5, -1, false);
		public static readonly BigDouble One = new(1, false);
		public static readonly BigDouble Two = new(2, false);
		public static readonly BigDouble LnOf10 = new(2.3025850929940456840, false);
		public static readonly BigDouble E = new(Math.E, false);
		public static readonly BigDouble Pi = new(Math.PI, false);
		public static readonly BigDouble Log2Of10 = new(3.3219280948873623478, false);
		public static readonly BigDouble Ten = new(1, 1, false);
		public static readonly BigDouble MinValue = new(-1, double.MaxValue, false);
		public static readonly BigDouble MaxValue = new(1, double.MaxValue, false);
		public static readonly BigDouble Epsilon = new(1, double.MinValue, false);
		public static readonly BigDouble NaN = new(double.NaN, false);
		public static readonly BigDouble NegativeInfinity = new(double.NegativeInfinity, false);
		public static readonly BigDouble PositiveInfinity = new(double.PositiveInfinity, false);

		public double Mantissa;
		public double Exponent;

		static BigDouble() {
			for(int a = 0, power = DoubleMinExponent + 1; a < PowersOf10.Length; ++a, ++power) {
				PowersOf10[a] = Math.Pow(10, power);
			}
			StandardNotationExponents = new(StandardNotationNames.Length, StringComparer.InvariantCultureIgnoreCase);
			for(int a = 0; a < StandardNotationNames.Length; ++a) {
				StandardNotationExponents[StandardNotationNames[a]] = (a + 1) * 3;
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

		public BigDouble(double n, bool needsNormalization = true) {
			Mantissa = n;
			Exponent = 0;
			if(needsNormalization) {
				NormalizeMod();
			}
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

		public static string GetStandardName(int power) => power < 3 || power >= (int)StandardNotationThreshold ? "" : StandardNotationNames[power / 3 - 1];

		public static bool IsFinite(BigDouble n) => n.IsFinite();

		public static bool IsNaN(BigDouble n) => n.IsNaN();

		public static bool IsInfinity(BigDouble n) => n.IsInfinity();

		public static bool IsNegativeInfinity(BigDouble n) => n.IsNegativeInfinity();

		public static bool IsPositiveInfinity(BigDouble n) => n.IsPositiveInfinity();

		public static bool IsNegative(BigDouble n) => n.IsNegative();

		public static bool IsZero(BigDouble n) => n.IsZero();

		public static bool IsDouble(BigDouble n) => n.IsDouble();

		public static int Sign(BigDouble n) => n.Sign();

		public static bool operator ==(BigDouble l, BigDouble r) => l.Mantissa == r.Mantissa && l.Exponent == r.Exponent;

		public static bool operator !=(BigDouble l, BigDouble r) => !(l == r);

		public static bool operator <(BigDouble l, BigDouble r) {
			bool isNegative = l.IsNegative(), isLess = l.Exponent < r.Exponent;
			return !l.IsFinite() || !r.IsFinite() || l.IsZero() || r.IsZero() || l.Exponent == r.Exponent || isNegative != r.IsNegative() ? l.Mantissa < r.Mantissa : isNegative ? !isLess : isLess;
		}

		public static bool operator >(BigDouble l, BigDouble r) {
			bool isNegative = l.IsNegative(), isGreater = l.Exponent > r.Exponent;
			return !l.IsFinite() || !r.IsFinite() || l.IsZero() || r.IsZero() || l.Exponent == r.Exponent || isNegative != r.IsNegative() ? l.Mantissa > r.Mantissa : isNegative ? !isGreater : isGreater;
		}

		public static bool operator <=(BigDouble l, BigDouble r) => l == r || l < r;

		public static bool operator >=(BigDouble l, BigDouble r) => l == r || l > r;

		public static string ToCustomString(BigDouble n, int length = DefaultLength, int decimals = DefaultDecimals, int smallDec = DefaultSmallDec, Notation notation = DefaultNotation, IFormatProvider? formatProvider = null)
			=> n.ToCustomString(length, decimals, smallDec, notation, formatProvider);

		public static bool TryParse(string s, out BigDouble result, IFormatProvider? formatProvider = null) {
			if(string.IsNullOrEmpty(s)) {
				result = default;
				return false;
			}
			const NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands;
			formatProvider ??= DefaultCulture;
			int epos = s.IndexOf('e', StringComparison.InvariantCultureIgnoreCase);
			double m, e;
			if(epos >= 0) {
				if(epos == 0) {
					m = 1;
				}
				else if(epos == 1 && s[0] == '-') {
					m = -1;
				}
				else if(!double.TryParse(s.AsSpan(0, epos), style, formatProvider, out m)) {
					result = default;
					return false;
				}
				if(!double.TryParse(s.AsSpan(epos + 1), style, formatProvider, out e)) {
					result = default;
					return false;
				}
				result = new(m, e);
				return true;
			}
			if(double.TryParse(s, style, formatProvider, out m)) {
				result = new(m);
				return true;
			}
			int upos;
			for(upos = s.Length - 1; upos >= 0; --upos) {
				if(char.IsDigit(s[upos])) {
					++upos;
					break;
				}
			}
			if(upos > 0 && upos < s.Length && double.TryParse(s.AsSpan(0, upos), style, formatProvider, out m) && StandardNotationExponents.TryGetValue(s[upos..], out e)) {
				result = new(m, e);
				return true;
			}
			result = default;
			return false;
		}

		public static implicit operator BigDouble(double n) => new(n);

		public static explicit operator double(BigDouble n) => n.ToDouble();

		public static BigDouble operator +(BigDouble n) => n;

		public static BigDouble operator -(BigDouble n) => n.Negate();

		public static BigDouble operator ++(BigDouble n) => n.Add(One);

		public static BigDouble operator +(BigDouble l, BigDouble r) => l.Add(r);

		public static BigDouble operator --(BigDouble n) => n.Subtract(One);

		public static BigDouble operator -(BigDouble l, BigDouble r) => l.Subtract(r);

		public static BigDouble operator *(BigDouble l, BigDouble r) => l.Multiply(r);

		public static BigDouble operator /(BigDouble l, BigDouble r) => l.Divide(r);

		public static BigDouble Reciprocal(BigDouble n) => n.Reciprocal();

		public static BigDouble Abs(BigDouble n) => n.Abs();

		public static BigDouble Sqrt(BigDouble n) => n.Sqrt();

		public static BigDouble Cbrt(BigDouble n) => n.Cbrt();

		public static BigDouble Square(BigDouble n) => n.Square();

		public static BigDouble Cube(BigDouble n) => n.Cube();

		public static double Log10(BigDouble n) => n.Log10();

		public static BigDouble Ln(BigDouble n) => n.Ln();

		public static BigDouble Log2(BigDouble n) => n.Log2();

		public static BigDouble Log(BigDouble n, double b) => n.Log(b);

		public static BigDouble Log(BigDouble n, BigDouble b) => n.Log(b);

		public static BigDouble Exp10(double n) => n - Math.Truncate(n) == 0 ? new(1, n, false) : new(1, n);

		public static BigDouble Pow(BigDouble b, double p) => b.Pow(p);

		public static BigDouble Exp(double p) => Math.Abs(p) <= 708 ? Math.Exp(p) : E.Pow(p);

		public static BigDouble Sinh(double n) => (Exp(n) - Exp(-n)) * Half;

		public static BigDouble Cosh(double n) => (Exp(n) + Exp(-n)) * Half;

		public static BigDouble Tanh(double n) => Math.Tanh(n);

		public static BigDouble Asinh(BigDouble n) => n.IsFinite() ? Ln(Sqrt(Square(n) + One) + n) : Math.Asinh((double)n);

		public static BigDouble Acosh(BigDouble n) => Ln(Sqrt(Square(n) - One) + n);

		public static BigDouble Atanh(double n) => Math.Atanh(n);

		public static BigDouble Truncate(BigDouble n, int digits = 0) => n.Truncate(digits);

		public static BigDouble Floor(BigDouble n, int digits = 0) => n.Floor(digits);

		public static BigDouble Ceiling(BigDouble n, int digits = 0) => n.Ceiling(digits);

		public static BigDouble Round(BigDouble n, int digits = 0, MidpointRounding mode = default) => n.Round(digits, mode);

		public static BigDouble Min(BigDouble l, BigDouble r) => l < r || l.IsNaN() ? l : r;

		public static BigDouble Max(BigDouble l, BigDouble r) => l > r || l.IsNaN() ? l : r;

		/// <summary>Unlike <see cref="Math.Clamp"/>, this does not throw an exception when min is greater than max, so it is up to the developer to provide the right parameters for the correct behavior.</summary>
		public static BigDouble Clamp(BigDouble n, BigDouble min, BigDouble max) => n < min ? min : n > max ? max : n;

		public readonly bool IsFinite() => double.IsFinite(Mantissa);

		public readonly bool IsNaN() => double.IsNaN(Mantissa);

		public readonly bool IsInfinity() => double.IsInfinity(Mantissa);

		public readonly bool IsNegativeInfinity() => double.IsNegativeInfinity(Mantissa);

		public readonly bool IsPositiveInfinity() => double.IsPositiveInfinity(Mantissa);

		public readonly bool IsNegative() => double.IsNegative(Mantissa);

		public readonly bool IsZero() => Mantissa == 0;

		public readonly bool IsDouble() {
			if(!IsFinite() || Exponent > DoubleMinExponent && Exponent < DoubleMaxExponent) {
				return true;
			}
			double mabs = Math.Abs(Mantissa);
			return Exponent == DoubleMinExponent && mabs >= DoubleMinMantissa || Exponent == DoubleMaxExponent && mabs <= DoubleMaxMantissa;
		}

		public readonly int Sign() => Math.Sign(Mantissa);

		public readonly int CompareTo(object? obj) {
			if(obj == null) {
				return 1;
			}
			if(obj is BigDouble n) {
				return CompareTo(n);
			}
			throw new ArgumentException($"Object must be of type {nameof(BigDouble)}.");
		}

		public readonly int CompareTo(BigDouble other) {
			int ecmp = Exponent.CompareTo(other.Exponent);
			bool isNegative = IsNegative();
			return !IsFinite() || !other.IsFinite() || IsZero() || other.IsZero() || ecmp == 0 || isNegative != other.IsNegative() ? Mantissa.CompareTo(other.Mantissa) : isNegative ? -ecmp : ecmp;
		}

		public override readonly bool Equals(object? obj) => obj is BigDouble n && Equals(n);

		public readonly bool Equals(BigDouble other) => Mantissa.Equals(other.Mantissa) && Exponent.Equals(other.Exponent);

		public override readonly int GetHashCode() => HashCode.Combine(Mantissa, Exponent);

		public readonly string ToDebugString(IFormatProvider? formatProvider = null) {
			formatProvider ??= DefaultCulture;
#if NET6_0_OR_GREATER
			return string.Create(formatProvider, $"{{{Mantissa:g17}, {Exponent:g17}}}");
#elif NET5_0_OR_GREATER
			return string.Format(formatProvider, "{{{0:g17}, {1:g17}}}", Mantissa, Exponent);
#else
			return string.Format(formatProvider, "{{{0:g17}, {1:g17}{2}", Mantissa, Exponent, "}");
#endif
		}

		public override readonly string ToString() => ToCustomString();

		public readonly string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

		/// <summary>This is the implementation for <see cref="IFormattable"/> for the cases where a simple string representation is sufficient.</summary>
		/// <param name="format">The format string to apply to each number component.</param>
		/// <param name="formatProvider">The format provider to apply to each number component.</param>
		public readonly string ToString(string? format, IFormatProvider? formatProvider = null) {
			formatProvider ??= DefaultCulture;
			if(IsDouble()) {
				return ((double)this).ToString(format, formatProvider);
			}
#if NET5_0_OR_GREATER
			return string.Format(formatProvider, $"{{0:{format}}}e{{1:{format}}}", Mantissa, Exponent);
#else
			return string.Format(formatProvider, $"{{0:{format}{'}'}e{{1:{format}{'}'}", Mantissa, Exponent);
#endif
		}

		/// <summary>Makes a custom string representation that makes it easier to make this number stay within a certain length.</summary>
		/// <param name="length">The maximum length of the string representation in characters (includes minus signs and the 'e' in "1e100", excludes group separators and the decimal point).</param>
		/// <param name="decimals">The maximum number of digits to show after the decimal point when this number requires abbreviation.</param>
		/// <param name="smallDec">The maximum number of digits to show after the decimal point when this number does not require abbreviation.</param>
		/// <param name="notation">
		///   The type of notation to use when abbreviating this number (standard = letters like k and m, scientific = AeB = A * 10 ^ B, engineering = scientific but exponent fixed to multiples of 3).
		/// </param>
		/// <param name="formatProvider">The format provider to apply to each number component.</param>
		/// <remarks>
		///   When this number is too large for standard notation, it falls back to scientific notation. When this number is too large for scientific or engineering notation in the given length, it falls back to the format
		///   AeBeC = A * 10 ^ (B * 10 ^ C). When the magnitude of the exponent of this number is greater than <see cref="ThresholdMod1Double"/>, it stops displaying the mantissa (A in the previous format description) since
		///   it is always 1.
		/// </remarks>
		public readonly string ToCustomString(int length = DefaultLength, int decimals = DefaultDecimals, int smallDec = DefaultSmallDec, Notation notation = DefaultNotation, IFormatProvider? formatProvider = null) {
			double eabs = Math.Abs(Exponent);
			bool ismsig = eabs < ThresholdMod1Double, ismn = double.IsNegative(Mantissa);
			length = Math.Clamp(length - (ismsig ? 1 : 0) - (ismn ? 1 : 0), 2, 15);
			decimals = Math.Clamp(decimals, 0, 15);
			smallDec = Math.Clamp(smallDec, 0, 15);
			formatProvider ??= DefaultCulture;
			if(!IsFinite()) {
				return Mantissa.ToString(formatProvider);
			}
			if(eabs <= length) {
				int ei = (int)Exponent;
				return Truncate(Mantissa * GetPowerOf10(ei), Math.Min(smallDec, Math.Min(length - ei, length))).ToString("#,0.###############", formatProvider);
			}
			length = Math.Max(double.IsNegative(Exponent) ? length - 1 : length, 2);
			int ee = (int)Math.Log10(eabs);
			double m = Truncate(Mantissa, Math.Max(Math.Min(decimals, length - 2 - ee), 0));
			double me = Truncate(Exponent / GetPowerOf10(ee), Math.Max(Math.Min(decimals, length - 4 - (int)Math.Log10(ee)), 0));
			double offset = Exponent % 3;
			if(offset < 0) {
				offset += 3;
			}
			double e3 = Exponent - offset, m3 = Math.Truncate(Mantissa * 100) / GetPowerOf10((int)(2 - offset));
			if(ee < length - 1 || ee < 3) {
				switch(notation) {
					case Notation.Standard when e3 >= 0 && e3 < StandardNotationThreshold:
#if NET6_0_OR_GREATER
						return string.Create(formatProvider, $"{m3:#,0.###############}{GetStandardName((int)e3)}");
#else
						return string.Format(formatProvider, "{0:#,0.###############}{1}", m3, GetStandardName((int)e3));
#endif
					case Notation.Engineering:
#if NET6_0_OR_GREATER
						return string.Create(formatProvider, $"{m3:#,0.###############}e{e3:#,0.###############}");
#else
						return string.Format(formatProvider, "{0:#,0.###############}e{1:#,0.###############}", m3, e3);
#endif
					default:
#if NET6_0_OR_GREATER
						return string.Create(formatProvider, $"{m:#,0.###############}e{Exponent:#,0.###############}");
#else
						return string.Format(formatProvider, "{0:#,0.###############}e{1:#,0.###############}", m, Exponent);
#endif
				}
			}
			if(ismsig) {
#if NET6_0_OR_GREATER
				return string.Create(formatProvider, $"{m:#,0.###############}e{me:#,0.###############}e{ee:#,0.###############}");
#else
				return string.Format(formatProvider, "{0:#,0.###############}e{1:#,0.###############}e{2:#,0.###############}", m, me, ee);
#endif
			}
#if NET6_0_OR_GREATER
			return string.Create(formatProvider, $"{(ismn ? "-" : "")}e{me:#,0.###############}e{ee:#,0.###############}");
#else
			return string.Format(formatProvider, "{0}e{1:#,0.###############}e{2:#,0.###############}", ismn ? "-" : "", me, ee);
#endif
		}

		public readonly double ToDouble() {
			if(!IsFinite() || Exponent == 0) {
				return Mantissa;
			}
			if(Exponent > DoubleMaxExponent) {
				return IsNegative() ? double.NegativeInfinity : double.PositiveInfinity;
			}
			if(Exponent < DoubleMinExponent) {
				return 0;
			}
			int eo = (int)Exponent, eo1 = eo / 2, eo2 = eo1 + eo % 2;
			return Mantissa * GetPowerOf10(eo1) * GetPowerOf10(eo2);
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
			double mabs = Math.Abs(Mantissa), ef = Exponent - Math.Truncate(Exponent);
			bool ismsig = Math.Abs(Exponent) < ThresholdMod1Double || !double.IsFinite(Exponent), isNormalLow = mabs >= 1 && mabs < 10 && ef == 0;
			if(ismsig && isNormalLow || !ismsig && mabs == 1) {
				return;
			}
			if(double.IsNaN(Mantissa) || double.IsNaN(Exponent)) {
				this = NaN;
				return;
			}
			if(IsZero()) {
				this = double.IsPositiveInfinity(Exponent) ? NaN : Zero;
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
				int eo = (int)Math.Floor(Math.Log10(mabs) + ef);
				Exponent = Math.Truncate(Exponent + eo);
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

		public readonly BigDouble Normalize() {
			BigDouble n = this;
			n.NormalizeMod();
			return n;
		}

		public void NegateMod() => Mantissa = -Mantissa;

		public readonly BigDouble Negate() {
			BigDouble n = this;
			n.NegateMod();
			return n;
		}

		public void AddMod(BigDouble other) {
			double diff = Math.Round(Exponent - other.Exponent);
			if(diff >= ThresholdAdd10Exponent) {
				return;
			}
			if(diff <= -ThresholdAdd10Exponent) {
				this = other;
				return;
			}
			if(!IsFinite() || !other.IsFinite()) {
				Mantissa += other.Mantissa;
				Exponent = 0;
				return;
			}
			int idiff = (int)diff;
			if(idiff <= 0) {
				Mantissa += other.Mantissa * GetPowerOf10(-idiff);
			}
			else {
				Mantissa = Mantissa * GetPowerOf10(idiff) + other.Mantissa;
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
				long offset = IsNegative() ? -1 : 1;
				Mantissa = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Mantissa) + offset);
				NormalizeMod();
				if(Mantissa <= original.Mantissa && Exponent == original.Exponent) {
					Exponent = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Exponent) + offset);
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
				long offset = IsNegative() ? 1 : -1;
				Mantissa = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Mantissa) + offset);
				NormalizeMod();
				if(Mantissa >= original.Mantissa && Exponent == original.Exponent) {
					Exponent = BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Exponent) + offset);
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

		public void ReciprocalMod() {
			Mantissa = 1 / Mantissa;
			Exponent = -Exponent;
			NormalizeMod();
		}

		public readonly BigDouble Reciprocal() {
			BigDouble n = this;
			n.ReciprocalMod();
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

		public readonly BigDouble Ln() => Math.Log(Mantissa) + Exponent * LnOf10;

		public readonly BigDouble Log2() {
#if NET5_0_OR_GREATER
			return Math.Log2(Mantissa) + Exponent * Log2Of10;
#else
			return Math.Log(Mantissa, 2) + Exponent * Log2Of10;
#endif
		}

		public readonly BigDouble Log(double b) {
			BigDouble ml = Math.Log(Mantissa, b);
			if(Exponent == 0) {
				return ml;
			}
			return ml + (BigDouble)Exponent * Math.Log(10, b);
		}

		public readonly BigDouble Log(BigDouble b) {
			if(b.IsDouble()) {
				return Log((double)b);
			}
			return Log10() / Log10(b);
		}

		public readonly BigDouble Pow(double power) {
			double mpow = Math.Pow(Mantissa, power);
			if(IsZero() || Exponent == 0 && (Math.Abs(Mantissa) == 1 || !double.IsFinite(power)) || !IsFinite() || power == 0 || double.IsNaN(mpow)) {
				return new(mpow, false);
			}
			if(mpow != 0 && !double.IsInfinity(mpow)) {
				return new(mpow, Exponent * power);
			}
			BigDouble n = Exp10(Log10(Abs()) * power);
			return IsNegative() && power % 2 == 1 ? -n : n;
		}

		public void TruncateMod(int digits = 0) {
			if(Exponent >= ThresholdMod1Exponent || !IsFinite()) {
				return;
			}
			double actualDigits = Exponent + digits;
			if(actualDigits < 0) {
				this = Zero;
				return;
			}
			Mantissa = Truncate(Mantissa, (int)actualDigits);
		}

		public readonly BigDouble Truncate(int digits = 0) {
			BigDouble n = this;
			n.TruncateMod(digits);
			return n;
		}

		public void FloorMod(int digits = 0) {
			if(Exponent >= ThresholdMod1Exponent || !IsFinite()) {
				return;
			}
			double actualDigits = Exponent + digits;
			if(actualDigits < 0) {
				this = IsNegative() ? -Exp10(-digits) : Zero;
				return;
			}
			Mantissa = Floor(Mantissa, (int)actualDigits);
			NormalizeMod();
		}

		public readonly BigDouble Floor(int digits = 0) {
			BigDouble n = this;
			n.FloorMod(digits);
			return n;
		}

		public void CeilingMod(int digits = 0) {
			if(Exponent >= ThresholdMod1Exponent || !IsFinite()) {
				return;
			}
			double actualDigits = Exponent + digits;
			if(actualDigits < 0) {
				this = IsNegative() ? Zero : Exp10(-digits);
				return;
			}
			Mantissa = Ceiling(Mantissa, (int)actualDigits);
			NormalizeMod();
		}

		public readonly BigDouble Ceiling(int digits = 0) {
			BigDouble n = this;
			n.CeilingMod(digits);
			return n;
		}

		public void RoundMod(int digits = 0, MidpointRounding mode = default) {
			if(Exponent >= ThresholdMod1Exponent || !IsFinite()) {
				return;
			}
			double actualDigits = Exponent + digits;
			if(actualDigits < -1) {
				this = Zero;
				return;
			}
			Mantissa = Round(Mantissa, (int)actualDigits, mode);
			NormalizeMod();
		}

		public readonly BigDouble Round(int digits = 0, MidpointRounding mode = default) {
			BigDouble n = this;
			n.RoundMod(digits, mode);
			return n;
		}
	}
}