﻿// Copyright 2024 Joonhyo Choi (asdfchlwnsgy1236), licensed under the Apache License Version 2.0.

namespace BreakInfinity {
	using System;

	[Serializable]
	public enum Notation {
		Standard,
		Scientific,
		Engineering
	}

	[Serializable]
	public struct BigDouble: IComparable, IComparable<BigDouble>, IEquatable<BigDouble>, IFormattable {
		private const int DoubleMinExponent = -324;
		private const int DoubleMaxExponent = 308;
		private const int DoubleZeroExponentIndex = -DoubleMinExponent - 1;
		private const double ThresholdExponent = 17;
		private const double ThresholdDouble = 1e17;
		private const int DefaultLength = 9;
		private const int DefaultDecimals = 3;
		private const int DefaultSmallDec = 0;
		private const Notation DefaultNotation = Notation.Scientific;

		private static readonly double[] PowersOf10 = new double[DoubleMaxExponent - DoubleMinExponent];
		private static readonly string[] StandardNotationNames = { "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd", "Nod", "Vig" };
		private static readonly int StandardNotationThreshold;

		public static readonly BigDouble Zero = new(0, 0, false);
		public static readonly BigDouble One = new(1, 0, false);
		public static readonly BigDouble Ten = new(1, 1, false);
		public static readonly BigDouble NaN = new(double.NaN, 0, false);
		public static readonly BigDouble PositiveInfinity = new(double.PositiveInfinity, 0, false);
		public static readonly BigDouble NegativeInfinity = new(double.NegativeInfinity, 0, false);

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
			double multi = GetPowerOf10(Math.Clamp(digits, 0, 15));
			return Math.Truncate(n * multi) / multi;
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

		public static BigDouble operator +(BigDouble n) => n;

		public static BigDouble operator -(BigDouble n) => n.Negated();

		public static BigDouble operator ++(BigDouble n) => n.Added(One);

		public static BigDouble operator +(BigDouble l, BigDouble r) => l.Added(r);

		public static BigDouble operator --(BigDouble n) => n.Subtracted(One);

		public static BigDouble operator -(BigDouble l, BigDouble r) => l.Subtracted(r);

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

		public readonly string ToString(string format, IFormatProvider formatProvider = null) {
			int length = DefaultLength, decimals = DefaultDecimals, smallDec = DefaultSmallDec;
			if(!string.IsNullOrEmpty(format)) {
				var parts = format.Split(',');
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
				}
			}
			return ToString(length, decimals, smallDec, DefaultNotation, formatProvider);
		}

		public readonly string ToString(int length = DefaultLength, int decimals = DefaultDecimals, int smallDec = DefaultSmallDec, Notation notation = DefaultNotation, IFormatProvider formatProvider = null) {
			const string NumberFormat = "#,0.###############";
			length = Math.Clamp(length, 3, 16);
			decimals = Math.Clamp(decimals, 0, 15);
			smallDec = Math.Clamp(smallDec, 0, 15);
			if(!IsFinite()) {
				return Mantissa.ToString(formatProvider);
			}
			if(Mantissa < 0 && length > 3) {
				--length;
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

		public void Normalize() {
			double mAbs = Math.Abs(Mantissa), er = Exponent % 1;
			if(mAbs >= 1 && mAbs < 10 && er == 0) {
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
			int eo = (int)Math.Floor(Math.Log10(mAbs) + er), eo1 = eo / 2, eo2 = eo1 + (eo % 2 == 0 ? 0 : 1);
			Mantissa = Mantissa / GetPowerOf10(eo1) / GetPowerOf10(eo2) * Math.Pow(10, er);
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
			int diff = (int)Math.Round(Math.Abs(Exponent - other.Exponent));
			if(diff >= ThresholdExponent) {
				return;
			}
			if(!IsFinite() || !other.IsFinite()) {
				Mantissa += other.Mantissa;
				Exponent = 0;
				return;
			}
			if(Exponent < other.Exponent) {
				Mantissa = Mantissa * GetPowerOf10(diff) + other.Mantissa;
			}
			else {
				Mantissa += other.Mantissa * GetPowerOf10(diff);
			}
			Normalize();
		}

		public readonly BigDouble Added(BigDouble other) {
			BigDouble n = this;
			n.Add(other);
			return n;
		}

		public void Add1OrUlp() {
			double m1 = Mantissa + 1;
			Mantissa = Mantissa == m1 ? BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Mantissa) + 1) : m1;
			Normalize();
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
			double m1 = Mantissa - 1;
			Mantissa = Mantissa == m1 ? BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(Mantissa) - 1) : m1;
			Normalize();
		}

		public readonly BigDouble Subtracted1OrUlp() {
			BigDouble n = this;
			n.Subtract1OrUlp();
			return n;
		}
	}
}