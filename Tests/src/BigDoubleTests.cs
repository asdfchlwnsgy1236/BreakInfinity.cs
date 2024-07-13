// Copyright 2024 Joonhyo Choi (asdfchlwnsgy1236); Apache License Version 2.0.

namespace BreakInfinityTests {
	using System.Globalization;

	using BreakInfinity;

	[TestFixture]
	internal class BigDoubleTests {
		private const long ToleranceLow = 1L << 2;
		private const long ToleranceMedium = 1L << 16;

		private static readonly object[][] CasesUnaryBigDoubleStrictSpecialNoNaN = [
			[BigDouble.Zero, 0],
			[BigDouble.NegativeInfinity, double.NegativeInfinity],
			[BigDouble.PositiveInfinity, double.PositiveInfinity]
		];
		private static readonly object[][] CasesUnaryBigDoubleStrictSpecial = [
			.. CasesUnaryBigDoubleStrictSpecialNoNaN,
			[BigDouble.NaN, double.NaN]
		];
		private static readonly object[][] CasesUnaryBigDoubleSpecialNoNaN = [
			.. CasesUnaryBigDoubleStrictSpecialNoNaN,
			[-BigDouble.Epsilon, -double.Epsilon],
			[BigDouble.Epsilon, double.Epsilon],
			[BigDouble.MinValue, double.MinValue],
			[BigDouble.MaxValue, double.MaxValue]
		];
		private static readonly object[][] CasesUnaryBigDoubleSpecial = [
			.. CasesUnaryBigDoubleSpecialNoNaN,
			[BigDouble.NaN, double.NaN]
		];
		private static readonly object[][] CasesUnaryBigDoubleGeneralSimple = [
			[-BigDouble.Tenth, -0.1],
			[BigDouble.Tenth, 0.1],
			[-BigDouble.Half, -0.5],
			[BigDouble.Half, 0.5],
			[-BigDouble.One, -1],
			[BigDouble.One, 1],
			[-BigDouble.Two, -2],
			[BigDouble.Two, 2],
			[-BigDouble.Ten, -10],
			[BigDouble.Ten, 10],
		];
		private static readonly object[][] CasesUnaryBigDoubleGeneral = [
			.. CasesUnaryBigDoubleGeneralSimple,
			[new BigDouble(-1.234, -6, false), -1.234e-6],
			[new BigDouble(1.234, -6, false), 1.234e-6],
			[new BigDouble(-1.234, 6, false), -1.234e6],
			[new BigDouble(1.234, 6, false), 1.234e6]
		];
		private static readonly object[][] CasesUnaryBigDoubleStrictAllSimple = [
			.. CasesUnaryBigDoubleStrictSpecial,
			.. CasesUnaryBigDoubleGeneralSimple
		];
		private static readonly object[][] CasesUnaryBigDoubleStrictAll = [
			.. CasesUnaryBigDoubleStrictSpecial,
			.. CasesUnaryBigDoubleGeneral
		];
		private static readonly object[][] CasesUnaryBigDoubleAllNoNaN = [
			.. CasesUnaryBigDoubleSpecialNoNaN,
			.. CasesUnaryBigDoubleGeneral
		];
		private static readonly object[][] CasesUnaryBigDoubleAll = [
			.. CasesUnaryBigDoubleSpecial,
			.. CasesUnaryBigDoubleGeneral
		];
		private static readonly object[][] CasesBinaryBigDoubleAllSimple;
		private static readonly object[][] CasesBinaryBigDoubleAll;
		private static readonly object[][] CasesConstructorDoubleDouble = [
			[-0.0, -0.0, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[0, 123, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[0, 1.23, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[1.23, 1.23, 2.0888396925279455561, 1, ToleranceLow, ToleranceLow],
			[-123.4, 123.4, -3.0996678564828218570, 125, ToleranceMedium, ToleranceLow],
			[123.4, 123.4, 3.0996678564828218570, 125, ToleranceMedium, ToleranceLow],
			[-12.3, 123456.7, -6.1646029736154491055, 123457, ToleranceMedium, ToleranceLow],
			[12.3, 123456.7, 6.1646029736154491055, 123457, ToleranceMedium, ToleranceLow],
			[-1.1e30, 1.1e30, -1, 1.1e30, ToleranceLow, ToleranceLow],
			[1.1e30, 1.1e30, 1, 1.1e30, ToleranceLow, ToleranceLow],
			[0, double.NegativeInfinity, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[0, double.PositiveInfinity, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[double.NegativeInfinity, double.NegativeInfinity, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[double.NegativeInfinity, double.PositiveInfinity, BigDouble.NegativeInfinity.Mantissa, BigDouble.NegativeInfinity.Exponent, ToleranceLow, ToleranceLow],
			[double.PositiveInfinity, double.NegativeInfinity, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[double.PositiveInfinity, double.PositiveInfinity, BigDouble.PositiveInfinity.Mantissa, BigDouble.PositiveInfinity.Exponent, ToleranceLow, ToleranceLow],
			[0, double.NaN, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow]
		];
		private static readonly object[][] CasesConstructorDouble = [
			[-0.0, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[0, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[-double.Epsilon, -4.9406564584124654417, -324, ToleranceLow, ToleranceLow],
			[double.Epsilon, 4.9406564584124654417, -324, ToleranceLow, ToleranceLow],
			[-0.5, -BigDouble.Half.Mantissa, BigDouble.Half.Exponent, ToleranceLow, ToleranceLow],
			[0.5, BigDouble.Half.Mantissa, BigDouble.Half.Exponent, ToleranceLow, ToleranceLow],
			[-1, -BigDouble.One.Mantissa, BigDouble.One.Exponent, ToleranceLow, ToleranceLow],
			[1, BigDouble.One.Mantissa, BigDouble.One.Exponent, ToleranceLow, ToleranceLow],
			[-10, -BigDouble.Ten.Mantissa, BigDouble.Ten.Exponent, ToleranceLow, ToleranceLow],
			[10, BigDouble.Ten.Mantissa, BigDouble.Ten.Exponent, ToleranceLow, ToleranceLow],
			[double.MinValue, -1.7976931348623157081, 308, ToleranceLow, ToleranceLow],
			[double.MaxValue, 1.7976931348623157081, 308, ToleranceLow, ToleranceLow],
			[double.NegativeInfinity, BigDouble.NegativeInfinity.Mantissa, BigDouble.NegativeInfinity.Exponent, ToleranceLow, ToleranceLow],
			[double.PositiveInfinity, BigDouble.PositiveInfinity.Mantissa, BigDouble.PositiveInfinity.Exponent, ToleranceLow, ToleranceLow],
			[double.NaN, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow]
		];
		private static readonly object[][] CasesGetPowerOf10Int = [
			[0, 1, ToleranceLow],
			[-10, 1e-10, ToleranceLow],
			[10, 1e10, ToleranceLow],
			[-323, 1e-323, ToleranceLow],
			[308, 1e308, ToleranceLow]
		];
		private static readonly object[][] CasesGetStandardNameInt = [
			[0, ""],
			[3, "K"],
			[6, "M"],
			[65, "Vig"],
			[66, ""]
		];
		private static readonly object[][] CasesIsDoubleBigDouble = [
			[BigDouble.Zero, true],
			[BigDouble.Epsilon, false],
			[new BigDouble(double.Epsilon), true],
			[BigDouble.Half, true],
			[BigDouble.One, true],
			[BigDouble.Two, true],
			[new BigDouble(double.MinValue), true],
			[new BigDouble(double.MaxValue), true],
			[BigDouble.MinValue, false],
			[BigDouble.MaxValue, false],
			[BigDouble.NegativeInfinity, true],
			[BigDouble.PositiveInfinity, true],
			[BigDouble.NaN, true]
		];
		private static readonly object[][] CasesTryParseStringBigDouble = [
			["0", BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			["-e-1.23e30", -1, -1.23e30, ToleranceLow, ToleranceLow],
			["1.23e-4.56e9", 1.23, -4.56e9, ToleranceLow, ToleranceLow],
			["0.1", BigDouble.Tenth.Mantissa, BigDouble.Tenth.Exponent, ToleranceLow, ToleranceLow],
			["-1", -BigDouble.One.Mantissa, BigDouble.One.Exponent, ToleranceLow, ToleranceLow],
			["10", BigDouble.Ten.Mantissa, BigDouble.Ten.Exponent, ToleranceLow, ToleranceLow],
			["-1k", -1, 3, ToleranceLow, ToleranceLow],
			["256vig", 2.56, 65, ToleranceLow, ToleranceLow],
			["-1234e12.3", -2.4621536966715974280, 15, ToleranceMedium, ToleranceLow],
			["e123e123", 1, 1.23e125, ToleranceLow, ToleranceLow],
			["-Infinity", BigDouble.NegativeInfinity.Mantissa, BigDouble.NegativeInfinity.Exponent, ToleranceLow, ToleranceLow],
			["Infinity", BigDouble.PositiveInfinity.Mantissa, BigDouble.PositiveInfinity.Exponent, ToleranceLow, ToleranceLow],
			["NaN", BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow]
		];
		private static readonly object[][] CasesBinaryPlusOperatorBigDoubleBigDoubleBeyond = [
			[new BigDouble(1, -900, false), new BigDouble(1, -1000, false), 1, -900],
			[new BigDouble(1, -1000, false), new BigDouble(1, -900, false), 1, -900],
			[new BigDouble(1, -1000, false), new BigDouble(1, -1000, false), 2, -1000],
			[new BigDouble(1, -999, false), new BigDouble(1, -1000, false), 1.1, -999],
			[new BigDouble(1, -1000, false), new BigDouble(1, -999, false), 1.1, -999],
			[new BigDouble(1, 1000, false), new BigDouble(1, 900, false), 1, 1000],
			[new BigDouble(1, 900, false), new BigDouble(1, 1000, false), 1, 1000],
			[new BigDouble(1, 1000, false), new BigDouble(1, 1000, false), 2, 1000],
			[new BigDouble(1, 1000, false), new BigDouble(1, 999, false), 1.1, 1000],
			[new BigDouble(1, 999, false), new BigDouble(1, 1000, false), 1.1, 1000]
		];
		private static readonly object[][] CasesBinaryMinusOperatorBigDoubleBigDoubleBeyond = [
			[new BigDouble(1, -900, false), new BigDouble(1, -1000, false), 1, -900],
			[new BigDouble(1, -1000, false), new BigDouble(1, -900, false), -1, -900],
			[new BigDouble(1, -1000, false), new BigDouble(1, -1000, false), 0, 0],
			[new BigDouble(1, -999, false), new BigDouble(1, -1000, false), 9, -1000],
			[new BigDouble(1, -1000, false), new BigDouble(1, -999, false), -9, -1000],
			[new BigDouble(1, 1000, false), new BigDouble(1, 900, false), 1, 1000],
			[new BigDouble(1, 900, false), new BigDouble(1, 1000, false), -1, 1000],
			[new BigDouble(1, 1000, false), new BigDouble(1, 1000, false), 0, 0],
			[new BigDouble(1, 1000, false), new BigDouble(1, 999, false), 9, 999],
			[new BigDouble(1, 999, false), new BigDouble(1, 1000, false), -9, 999]
		];
		private static readonly object[][] CasesMultiplicationBigDoubleBigDoubleBeyond = [
			[new BigDouble(4, -1000, false), new BigDouble(5, -900, false), 2, -1899],
			[new BigDouble(4, 1000, false), new BigDouble(5, 900, false), 2, 1901]
		];
		private static readonly object[][] CasesDivisionBigDoubleBigDoubleBeyond = [
			[new BigDouble(4, -1000, false), new BigDouble(5, -900, false), 8, -101],
			[new BigDouble(4, 1000, false), new BigDouble(5, 900, false), 8, 99]
		];
		private static readonly object[][] CasesReciprocalBigDoubleBeyond = [
			[new BigDouble(2, -1000, false), 5, 999],
			[new BigDouble(2, 1000, false), 5, -1001]
		];
		private static readonly object[][] CasesSqrtBigDoubleBeyond = [
			[new BigDouble(4, -1000, false), 2, -500],
			[new BigDouble(4, 1000, false), 2, 500]
		];
		private static readonly object[][] CasesCbrtBigDoubleBeyond = [
			[new BigDouble(8, -1200, false), 2, -400],
			[new BigDouble(8, 1200, false), 2, 400]
		];
		private static readonly object[][] CasesSquareBigDoubleBeyond = [
			[new BigDouble(5, -1000, false), 2.5, -1999],
			[new BigDouble(5, 1000, false), 2.5, 2001]
		];
		private static readonly object[][] CasesCubeBigDoubleBeyond = [
			[new BigDouble(5, -1000, false), 1.25, -2998],
			[new BigDouble(5, 1000, false), 1.25, 3002]
		];
		private static readonly object[][] CasesLog10BigDoubleBeyond = [
			[new BigDouble(2, -1000, false), -999.69897000433601880],
			[new BigDouble(2, 1000, false), 1000.3010299956639811]
		];
		private static readonly object[][] CasesLnBigDoubleBeyond = [
			[new BigDouble(2, -1000, false), -2.3018919458134857387, 3],
			[new BigDouble(2, 1000, false), 2.3032782401746056293, 3],
		];
		private static readonly object[][] CasesLog2BigDoubleBeyond = [
			[new BigDouble(2, -1000, false), -3.3209280948873623478, 3],
			[new BigDouble(2, 1000, false), 3.3229280948873623478, 3]
		];
		private static readonly object[][] CasesLogBigDoubleDoubleBeyond = [
			[new BigDouble(2, -1000, false), 4, -1.6604640474436811739, 3],
			[new BigDouble(2, 1000, false), 4, 1.6614640474436811739, 3]
		];
		private static readonly object[][] CasesLogBigDoubleBigDoubleBeyond = [
			[new BigDouble(2, -1000, false), new BigDouble(4, -500, false), 2.0018083574533292256, 0],
			[new BigDouble(2, -1000, false), new BigDouble(4, 500, false), -1.9969933204462922505, 0],
			[new BigDouble(2, 1000, false), new BigDouble(4, -500, false), -2.0030139290888820427, 0],
			[new BigDouble(2, 1000, false), new BigDouble(4, 500, false), 1.9981959922677753503, 0]
		];
		private static readonly object[][] CasesExp10DoubleBeyond = [
			[-1000, 1, -1000],
			[-999.9, 1.2589254117941672104, -1000],
			[999.9, 7.9432823472428150206, 999],
			[1000, 1, 1000]
		];
		private static readonly object[][] CasesPowBigDoubleDoubleBeyond = [
			[new BigDouble(2, -1000, false), 4, 1.6, -3999],
			[new BigDouble(2, 1000, false), 4, 1.6, 4001]
		];
		private static readonly object[][] CasesExpDoubleBeyond = [
			[-2000, 2.5765358729611496521, -869],
			[2000, 3.8811801942843685764, 868]
		];
		private static readonly object[][] CasesRoundingBigDouble;
		private static readonly object[][] CasesClampBigDoubleBigDoubleBigDouble = [
			[-BigDouble.Two, -2, -BigDouble.One, -1, BigDouble.One, 1],
			[BigDouble.Zero, 0, -BigDouble.One, -1, BigDouble.One, 1],
			[BigDouble.Two, 2, -BigDouble.One, -1, BigDouble.One, 1],
			[BigDouble.NegativeInfinity, double.NegativeInfinity, -BigDouble.One, -1, BigDouble.One, 1],
			[BigDouble.PositiveInfinity, double.PositiveInfinity, -BigDouble.One, -1, BigDouble.One, 1],
			[BigDouble.NaN, double.NaN, -BigDouble.One, -1, BigDouble.One, 1]
		];
		private static readonly object[][] CasesToCustomStringBigDouble = [
			[new BigDouble(-1, -1.23456789012345678e20, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-e-1.23e20"],
			[new BigDouble(1, -1.23456789012345678e20, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "e-1.234e20"],
			[new BigDouble(-1.23456, -123456789, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-1e-1.23e8"],
			[new BigDouble(1.23456, -123456789, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "1e-1.234e8"],
			[new BigDouble(-1.23456, -12, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-1.234e-12"],
			[new BigDouble(1.23456, -12, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "1.2345e-12"],
			[new BigDouble(-1.23456, -3, false), 6, 6, 6, Notation.Scientific, CultureInfo.InvariantCulture, "-0.0012"],
			[new BigDouble(1.23456, -3, false), 6, 6, 6, Notation.Scientific, CultureInfo.InvariantCulture, "0.00123"],
			[new BigDouble(1.23456, -3, false), 6, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "0.0012"],
			[new BigDouble(-1.23456789012345678, false), 15, 15, 15, Notation.Scientific, CultureInfo.InvariantCulture, "-1.2345678901234"],
			[new BigDouble(1.23456789012345678, false), 15, 15, 15, Notation.Scientific, CultureInfo.InvariantCulture, "1.23456789012345"],
			[new BigDouble(-1.23456789012345678, false), 6, 6, 6, Notation.Scientific, CultureInfo.InvariantCulture, "-1.2345"],
			[new BigDouble(1.23456789012345678, false), 6, 6, 6, Notation.Scientific, CultureInfo.InvariantCulture, "1.23456"],
			[new BigDouble(1.23456789012345678, false), 6, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "1.2345"],
			[new BigDouble(-1.23456789012345678, 5, false), 6, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-1.23e5"],
			[new BigDouble(1.23456789012345678, 5, false), 6, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "123,456"],
			[new BigDouble(1.23456789012345678, 6, false), 6, 6, 0, Notation.Scientific, CultureInfo.InvariantCulture, "1.234e6"],
			[new BigDouble(1.23456789012345678, 6, false), 6, 2, 0, Notation.Scientific, CultureInfo.InvariantCulture, "1.23e6"],
			[new BigDouble(-1.23456789012345678, 12, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-1.2e12"],
			[new BigDouble(1.23456789012345678, 12, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "1.23e12"],
			[new BigDouble(1.23456789012345678, 123, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "1.2e123"],
			[new BigDouble(1.23456789012345678, 1234, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "1e1,234"],
			[new BigDouble(-1.23456789012345678, 12345, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-1e1e4"],
			[new BigDouble(1.23456789012345678, 12345, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "1e1.2e4"],
			[new BigDouble(-1.23456789012345678, 12345678901, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-1e1e10"],
			[new BigDouble(1.23456789012345678, 12345678901, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "1e1e10"],
			[new BigDouble(-1, 1.23456789012345678e20, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-e1e20"],
			[new BigDouble(1, 1.23456789012345678e20, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "e1.2e20"],
			[new BigDouble(-1, 1.23456789012345678e100, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-e1e100"],
			[new BigDouble(1, 1.23456789012345678e100, false), 6, 4, 4, Notation.Scientific, CultureInfo.InvariantCulture, "e1e100"],
			[new BigDouble(-9.99999999999, 12, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "-9.9999e12"],
			[new BigDouble(9.99999999999, 12, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "9.99999e12"],
			[new BigDouble(9.99999999999, 12, false), 9, 3, 4, Notation.Scientific, CultureInfo.InvariantCulture, "9.999e12"],
			[new BigDouble(9.99999999999, 12, false), 9, 0, 4, Notation.Scientific, CultureInfo.InvariantCulture, "9e12"],
			[new BigDouble(9.99999999999, 123, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "9.9999e123"],
			[new BigDouble(9.99999999999, 1234, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "9.999e1,234"],
			[new BigDouble(9.99999999999, 12345, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "9.99e12,345"],
			[new BigDouble(9.99999999999, 123456, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "9.9e123,456"],
			[new BigDouble(9.99999999999, 1234567, false), 9, 6, 4, Notation.Scientific, CultureInfo.InvariantCulture, "9e1,234,567"],
			[new BigDouble(1.23456789012345678, 6, false), 6, 6, 4, Notation.Standard, CultureInfo.InvariantCulture, "1.23M"],
			[new BigDouble(1.23456789012345678, 8, false), 6, 6, 4, Notation.Standard, CultureInfo.InvariantCulture, "123M"],
			[new BigDouble(1.23456789012345678, 12345678901, false), 6, 6, 4, Notation.Standard, CultureInfo.InvariantCulture, "1e1e10"],
			[new BigDouble(1.23456789012345678, 6, false), 6, 6, 4, Notation.Engineering, CultureInfo.InvariantCulture, "1.23e6"],
			[new BigDouble(1.23456789012345678, 8, false), 6, 6, 4, Notation.Engineering, CultureInfo.InvariantCulture, "123e6"],
			[new BigDouble(1.23456789012345678, 12345678901, false), 6, 6, 4, Notation.Engineering, CultureInfo.InvariantCulture, "1e1e10"]
		];
		private static readonly object[][] CasesAdd1OrUlpBigDouble = [
			[BigDouble.Zero, 1, 0],
			[-BigDouble.One, 0, 0],
			[BigDouble.One, 2, 0],
			[-BigDouble.Ten, -9, 0],
			[BigDouble.Ten, 1.1, 1],
			[new BigDouble(-1, 100, false), Math.BitIncrement(-10), 99],
			[new BigDouble(1, 100, false), Math.BitIncrement(1), 100],
			[new BigDouble(-1, 1000, false), Math.BitIncrement(-10), 999],
			[new BigDouble(1, 1000, false), Math.BitIncrement(1), 1000],
			[new BigDouble(-1, 1e30, false), -1, Math.BitDecrement(1e30)],
			[new BigDouble(1, 1e30, false), 1, Math.BitIncrement(1e30)]
		];
		private static readonly object[][] CasesSubtract1OrUlpBigDouble = [
			[BigDouble.Zero, -1, 0],
			[-BigDouble.One, -2, 0],
			[BigDouble.One, 0, 0],
			[-BigDouble.Ten, -1.1, 1],
			[BigDouble.Ten, 9, 0],
			[new BigDouble(-1, 100, false), Math.BitDecrement(-1), 100],
			[new BigDouble(1, 100, false), Math.BitDecrement(10), 99],
			[new BigDouble(-1, 1000, false), Math.BitDecrement(-1), 1000],
			[new BigDouble(1, 1000, false), Math.BitDecrement(10), 999],
			[new BigDouble(-1, 1e30, false), -1, Math.BitIncrement(1e30)],
			[new BigDouble(1, 1e30, false), 1, Math.BitDecrement(1e30)]
		];

		static BigDoubleTests() {
			int count = CasesUnaryBigDoubleStrictAllSimple.Length;
			CasesBinaryBigDoubleAllSimple = new object[count * count][];
			for(int a = 0, index = 0; a < count; ++a) {
				for(int b = 0; b < count; ++b, ++index) {
					CasesBinaryBigDoubleAllSimple[index] = [.. CasesUnaryBigDoubleStrictAllSimple[a], .. CasesUnaryBigDoubleStrictAllSimple[b]];
				}
			}
			count = CasesUnaryBigDoubleStrictAll.Length;
			CasesBinaryBigDoubleAll = new object[count * count][];
			for(int a = 0, index = 0; a < count; ++a) {
				for(int b = 0; b < count; ++b, ++index) {
					CasesBinaryBigDoubleAll[index] = [.. CasesUnaryBigDoubleStrictAll[a], .. CasesUnaryBigDoubleStrictAll[b]];
				}
			}
			BigDouble[] roundingBigDoubles = [new(-2.2222222222222222222, false), new(2.2222222222222222222, false), new(-5.5555555555555555555, false),
				new(5.5555555555555555555, false), new(-8.8888888888888888888, false), new(8.8888888888888888888, false)];
			double[] roundingDoubles = [-2.2222222222222222222, 2.2222222222222222222, -5.5555555555555555555, 5.5555555555555555555, -8.8888888888888888888, 8.8888888888888888888];
			int[] roundingDigits = Enumerable.Range(-3, 24).ToArray();
			CasesRoundingBigDouble = new object[roundingBigDoubles.Length * roundingDigits.Length][];
			for(int a = 0, index = 0; a < roundingBigDoubles.Length; ++a) {
				for(int b = 0; b < roundingDigits.Length; ++b, ++index) {
					CasesRoundingBigDouble[index] = [roundingBigDoubles[a], roundingDoubles[a], roundingDigits[b]];
				}
			}
		}

		private static void AssertEqualSimple<T>(T actual, T expected) => Assert.That(actual, Is.EqualTo(expected));

		private static void AssertEqualDouble(double actual, double expected, long tolerance = ToleranceLow) => Assert.That(actual, Is.EqualTo(expected).Within(tolerance).Ulps);

		private static void AssertEqualBigDoubleComponents(BigDouble actual, double expectedMantissa, double expectedExponent, long toleranceMantissa = ToleranceLow, long toleranceExponent = ToleranceLow)
			=> Assert.Multiple(() => {
				AssertEqualDouble(actual.Mantissa, expectedMantissa, toleranceMantissa);
				AssertEqualDouble(actual.Exponent, expectedExponent, toleranceExponent);
			});

		private static void AssertEqualBigDoubleComponentsExact(BigDouble actual, double expectedMantissa, double expectedExponent) => Assert.Multiple(() => {
			Assert.That(actual.Mantissa, Is.EqualTo(expectedMantissa));
			Assert.That(actual.Exponent, Is.EqualTo(expectedExponent));
		});

		private static void AssertEqualBigDoubleDouble(BigDouble actual, double expected, long tolerance = ToleranceLow) {
			if(actual.IsDouble()) {
				AssertEqualDouble((double)actual, expected, tolerance);
			}
			else {
				Assert.Fail($"The actual value ({actual.ToDebugString()}) is beyond double; remember this is only for double equivalence checks.");
			}
		}

		private static void AssertEqualString(string actual, string expected) => Assert.That(actual, Is.EqualTo(expected).IgnoreCase);

		[TestCaseSource(nameof(CasesConstructorDoubleDouble))]
		public void ConstructorDoubleDouble(double mantissa, double exponent, double expectedMantissa, double expectedExponent, long toleranceMantissa, long toleranceExponent)
			=> AssertEqualBigDoubleComponents(new(mantissa, exponent), expectedMantissa, expectedExponent, toleranceMantissa, toleranceExponent);

		[TestCaseSource(nameof(CasesConstructorDouble))]
		public void ConstructorDouble(double n, double expectedMantissa, double expectedExponent, long toleranceMantissa, long toleranceExponent)
			=> AssertEqualBigDoubleComponents(new(n), expectedMantissa, expectedExponent, toleranceMantissa, toleranceExponent);

		[TestCaseSource(nameof(CasesGetPowerOf10Int))]
		public void GetPowerOf10Int(int power, double expected, long tolerance) => AssertEqualDouble(BigDouble.GetPowerOf10(power), expected, tolerance);

		[TestCaseSource(nameof(CasesGetStandardNameInt))]
		public void GetStandardNameInt(int power, string expected) => AssertEqualString(BigDouble.GetStandardName(power), expected);

		[TestCaseSource(nameof(CasesUnaryBigDoubleAll))]
		public void IsFiniteBigDouble(BigDouble n, double d) => AssertEqualSimple(BigDouble.IsFinite(n), double.IsFinite(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleAll))]
		public void IsNaNBigDouble(BigDouble n, double d) => AssertEqualSimple(BigDouble.IsNaN(n), double.IsNaN(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleAll))]
		public void IsInfinityBigDouble(BigDouble n, double d) => AssertEqualSimple(BigDouble.IsInfinity(n), double.IsInfinity(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleAll))]
		public void IsNegativeInfinityBigDouble(BigDouble n, double d) => AssertEqualSimple(BigDouble.IsNegativeInfinity(n), double.IsNegativeInfinity(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleAll))]
		public void IsPositiveInfinityBigDouble(BigDouble n, double d) => AssertEqualSimple(BigDouble.IsPositiveInfinity(n), double.IsPositiveInfinity(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleAll))]
		public void IsNegativeBigDouble(BigDouble n, double d) => AssertEqualSimple(BigDouble.IsNegative(n), double.IsNegative(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleAll))]
		public void IsZeroBigDouble(BigDouble n, double d) => AssertEqualSimple(BigDouble.IsZero(n), d == 0);

		[TestCaseSource(nameof(CasesIsDoubleBigDouble))]
		public void IsDoubleBigDouble(BigDouble n, bool expected) => AssertEqualSimple(BigDouble.IsDouble(n), expected);

		[TestCaseSource(nameof(CasesUnaryBigDoubleAllNoNaN))]
		public void SignBigDouble(BigDouble n, double d) => AssertEqualSimple(BigDouble.Sign(n), Math.Sign(d));

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void EqualityOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualSimple(nl == nr, dl == dr);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void InequalityOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualSimple(nl != nr, dl != dr);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void LessThanOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualSimple(nl < nr, dl < dr);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void GreaterThanOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualSimple(nl > nr, dl > dr);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void LessThanOrEqualOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualSimple(nl <= nr, dl <= dr);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void GreaterThanOrEqualOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualSimple(nl >= nr, dl >= dr);

		[TestCaseSource(nameof(CasesTryParseStringBigDouble))]
		public void TryParseStringToBigDouble(string s, double expectedMantissa, double expectedExponent, long toleranceMantissa, long toleranceExponent) {
			if(!BigDouble.TryParse(s, out BigDouble n)) {
				Assert.Fail("Parsing failed when it should have succeeded.");
				return;
			}
			AssertEqualBigDoubleComponents(n, expectedMantissa, expectedExponent, toleranceMantissa, toleranceExponent);
		}

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void CastBigDoubleToDouble(BigDouble n, double d) => AssertEqualDouble((double)n, d);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void UnaryPlusOperatorBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(+n, +d);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void UnaryMinusOperatorBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(-n, -d);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void IncrementOperatorBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(++n, ++d);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void BinaryPlusOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDoubleDouble(nl + nr, dl + dr);

		[TestCaseSource(nameof(CasesBinaryPlusOperatorBigDoubleBigDoubleBeyond))]
		public void BinaryPlusOperatorBigDoubleBigDoubleBeyond(BigDouble l, BigDouble r, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(l + r, expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void DecrementOperatorBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(--n, --d);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void BinaryMinusOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDoubleDouble(nl - nr, dl - dr);

		[TestCaseSource(nameof(CasesBinaryMinusOperatorBigDoubleBigDoubleBeyond))]
		public void BinaryMinusOperatorBigDoubleBigDoubleBeyond(BigDouble l, BigDouble r, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(l - r, expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void MultiplicationBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDoubleDouble(nl * nr, dl * dr);

		[TestCaseSource(nameof(CasesMultiplicationBigDoubleBigDoubleBeyond))]
		public void MultiplicationBigDoubleBigDoubleBeyond(BigDouble l, BigDouble r, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(l * r, expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void DivisionBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDoubleDouble(nl / nr, dl / dr);

		[TestCaseSource(nameof(CasesDivisionBigDoubleBigDoubleBeyond))]
		public void DivisionBigDoubleBigDoubleBeyond(BigDouble l, BigDouble r, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(l / r, expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void ReciprocalBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Reciprocal(n), 1 / d);

		[TestCaseSource(nameof(CasesReciprocalBigDoubleBeyond))]
		public void ReciprocalBigDoubleBeyond(BigDouble n, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(BigDouble.Reciprocal(n), expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void AbsBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Abs(n), Math.Abs(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void SqrtBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Sqrt(n), Math.Sqrt(d));

		[TestCaseSource(nameof(CasesSqrtBigDoubleBeyond))]
		public void SqrtBigDoubleBeyond(BigDouble n, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(BigDouble.Sqrt(n), expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void CbrtBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Cbrt(n), Math.Cbrt(d));

		[TestCaseSource(nameof(CasesCbrtBigDoubleBeyond))]
		public void CbrtBigDoubleBeyond(BigDouble n, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(BigDouble.Cbrt(n), expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void SquareBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Square(n), d * d);

		[TestCaseSource(nameof(CasesSquareBigDoubleBeyond))]
		public void SquareBigDoubleBeyond(BigDouble n, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(BigDouble.Square(n), expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void CubeBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Cube(n), d * d * d);

		[TestCaseSource(nameof(CasesCubeBigDoubleBeyond))]
		public void CubeBigDoubleBeyond(BigDouble n, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(BigDouble.Cube(n), expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void Log10BigDouble(BigDouble n, double d) => AssertEqualDouble(BigDouble.Log10(n), Math.Log10(d));

		[TestCaseSource(nameof(CasesLog10BigDoubleBeyond))]
		public void Log10BigDoubleBeyond(BigDouble n, double expected) => AssertEqualDouble(BigDouble.Log10(n), expected);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void LnBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Ln(n), Math.Log(d));

		[TestCaseSource(nameof(CasesLnBigDoubleBeyond))]
		public void LnBigDoubleBeyond(BigDouble n, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(BigDouble.Ln(n), expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void Log2BigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Log2(n), Math.Log2(d));

		[TestCaseSource(nameof(CasesLog2BigDoubleBeyond))]
		public void Log2BigDoubleBeyond(BigDouble n, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(BigDouble.Log2(n), expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void LogBigDoubleDouble(BigDouble nl, double dl, BigDouble _, double dr) => AssertEqualBigDoubleDouble(BigDouble.Log(nl, dr), Math.Log(dl, dr), ToleranceMedium);

		[TestCaseSource(nameof(CasesLogBigDoubleDoubleBeyond))]
		public void LogBigDoubleDoubleBeyond(BigDouble n, double d, double expectedMantissa, double expectedExponent)
			=> AssertEqualBigDoubleComponents(BigDouble.Log(n, d), expectedMantissa, expectedExponent, ToleranceMedium);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void LogBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDoubleDouble(BigDouble.Log(nl, nr), Math.Log(dl, dr), ToleranceMedium);

		[TestCaseSource(nameof(CasesLogBigDoubleBigDoubleBeyond))]
		public void LogBigDoubleBigDoubleBeyond(BigDouble l, BigDouble r, double expectedMantissa, double expectedExponent)
			=> AssertEqualBigDoubleComponents(BigDouble.Log(l, r), expectedMantissa, expectedExponent, ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void Exp10Double(BigDouble _, double d) => AssertEqualBigDoubleDouble(BigDouble.Exp10(d), Math.Pow(10, d), ToleranceMedium);

		[TestCaseSource(nameof(CasesExp10DoubleBeyond))]
		public void Exp10DoubleBeyond(double d, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(BigDouble.Exp10(d), expectedMantissa, expectedExponent, ToleranceMedium);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAllSimple))]
		public void PowBigDoubleDouble(BigDouble nl, double dl, BigDouble _, double dr) => AssertEqualBigDoubleDouble(BigDouble.Pow(nl, dr), Math.Pow(dl, dr), ToleranceMedium);

		[TestCaseSource(nameof(CasesPowBigDoubleDoubleBeyond))]
		public void PowBigDoubleDoubleBeyond(BigDouble n, double d, double expectedMantissa, double expectedExponent)
			=> AssertEqualBigDoubleComponents(BigDouble.Pow(n, d), expectedMantissa, expectedExponent, ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void ExpDouble(BigDouble _, double d) => AssertEqualBigDoubleDouble(BigDouble.Exp(d), Math.Exp(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesExpDoubleBeyond))]
		public void ExpDoubleBeyond(double d, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponents(BigDouble.Exp(d), expectedMantissa, expectedExponent, ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void SinhDouble(BigDouble _, double d) => AssertEqualBigDoubleDouble(BigDouble.Sinh(d), Math.Sinh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void CoshDouble(BigDouble _, double d) => AssertEqualBigDoubleDouble(BigDouble.Cosh(d), Math.Cosh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void TanhDouble(BigDouble _, double d) => AssertEqualBigDoubleDouble(BigDouble.Tanh(d), Math.Tanh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void AsinhBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Asinh(n), Math.Asinh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void AcoshBigDouble(BigDouble n, double d) => AssertEqualBigDoubleDouble(BigDouble.Acosh(n), Math.Acosh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void AtanhBigDouble(BigDouble _, double d) => AssertEqualBigDoubleDouble(BigDouble.Atanh(d), Math.Atanh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesRoundingBigDouble))]
		public void TruncateBigDoubleInt(BigDouble n, double d, int digits) {
			double offset = Math.Pow(10, digits);
			AssertEqualBigDoubleDouble(BigDouble.Truncate(n, digits), Math.Truncate(d * offset) / offset);
		}

		[TestCaseSource(nameof(CasesRoundingBigDouble))]
		public void FloorBigDoubleInt(BigDouble n, double d, int digits) {
			double offset = Math.Pow(10, digits);
			AssertEqualBigDoubleDouble(BigDouble.Floor(n, digits), Math.Floor(d * offset) / offset);
		}

		[TestCaseSource(nameof(CasesRoundingBigDouble))]
		public void CeilingBigDoubleInt(BigDouble n, double d, int digits) {
			double offset = Math.Pow(10, digits);
			AssertEqualBigDoubleDouble(BigDouble.Ceiling(n, digits), Math.Ceiling(d * offset) / offset);
		}

		[TestCaseSource(nameof(CasesRoundingBigDouble))]
		public void RoundBigDoubleInt(BigDouble n, double d, int digits) {
			double offset = Math.Pow(10, digits);
			AssertEqualBigDoubleDouble(BigDouble.Round(n, digits), Math.Round(d * offset) / offset);
		}

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void MinBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDoubleDouble(BigDouble.Min(nl, nr), Math.Min(dl, dr));

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void MaxBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDoubleDouble(BigDouble.Max(nl, nr), Math.Max(dl, dr));

		[TestCaseSource(nameof(CasesClampBigDoubleBigDoubleBigDouble))]
		public void ClampBigDoubleBigDoubleBigDouble(BigDouble n, double d, BigDouble nmin, double dmin, BigDouble nmax, double dmax) => AssertEqualBigDoubleDouble(BigDouble.Clamp(n, nmin, nmax), Math.Clamp(d, dmin, dmax));

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void CompareToBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualSimple(nl.CompareTo(nr), dl.CompareTo(dr));

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void EqualsBigDoubleGetHashCode(BigDouble nl, double dl, BigDouble nr, double dr) => Assert.Multiple(() => {
			bool isEqual = nl.Equals(nr);
			AssertEqualSimple(isEqual, dl.Equals(dr));
			if(isEqual) {
				AssertEqualSimple(nl.GetHashCode(), nr.GetHashCode());
			}
		});

		[TestCaseSource(nameof(CasesToCustomStringBigDouble))]
		public void ToCustomStringBigDouble(BigDouble n, int length, int decimals, int smallDec, Notation notation, IFormatProvider? formatProvider, string expected)
			=> AssertEqualString(n.ToCustomString(length, decimals, smallDec, notation, formatProvider), expected);

		[TestCaseSource(nameof(CasesAdd1OrUlpBigDouble))]
		public void Add1OrUlpBigDouble(BigDouble n, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponentsExact(n.Add1OrUlp(), expectedMantissa, expectedExponent);

		[TestCaseSource(nameof(CasesSubtract1OrUlpBigDouble))]
		public void Subtract1OrUlpBigDouble(BigDouble n, double expectedMantissa, double expectedExponent) => AssertEqualBigDoubleComponentsExact(n.Subtract1OrUlp(), expectedMantissa, expectedExponent);
	}
}