// Copyright 2024 Joonhyo Choi (asdfchlwnsgy1236); Apache License Version 2.0.
namespace BreakInfinityTests {
	using BreakInfinity;

	// TODO: Remember to add a few separate tests for the cases beyond double.

	[TestFixture]
	public class BigDoubleTests {
		private const double Epsilon = double.Epsilon;
		private const double MinValue = double.MinValue;
		private const double MaxValue = double.MaxValue;
		private const double NegativeInfinity = double.NegativeInfinity;
		private const double PositiveInfinity = double.PositiveInfinity;
		private const double NaN = double.NaN;
		private const long ToleranceLow = 1L << 2;
		private const long ToleranceMedium = 1L << 16;

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
			[0, NegativeInfinity, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[0, PositiveInfinity, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[NegativeInfinity, NegativeInfinity, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[NegativeInfinity, PositiveInfinity, BigDouble.NegativeInfinity.Mantissa, BigDouble.NegativeInfinity.Exponent, ToleranceLow, ToleranceLow],
			[PositiveInfinity, NegativeInfinity, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[PositiveInfinity, PositiveInfinity, BigDouble.PositiveInfinity.Mantissa, BigDouble.PositiveInfinity.Exponent, ToleranceLow, ToleranceLow],
			[0, NaN, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow]
		];
		private static readonly object[][] CasesConstructorDouble = [
			[-0.0, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[0, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[-Epsilon, -4.9406564584124654417, -324, ToleranceLow, ToleranceLow],
			[Epsilon, 4.9406564584124654417, -324, ToleranceLow, ToleranceLow],
			[-0.5, -BigDouble.Half.Mantissa, BigDouble.Half.Exponent, ToleranceLow, ToleranceLow],
			[0.5, BigDouble.Half.Mantissa, BigDouble.Half.Exponent, ToleranceLow, ToleranceLow],
			[-1, -BigDouble.One.Mantissa, BigDouble.One.Exponent, ToleranceLow, ToleranceLow],
			[1, BigDouble.One.Mantissa, BigDouble.One.Exponent, ToleranceLow, ToleranceLow],
			[-10, -BigDouble.Ten.Mantissa, BigDouble.Ten.Exponent, ToleranceLow, ToleranceLow],
			[10, BigDouble.Ten.Mantissa, BigDouble.Ten.Exponent, ToleranceLow, ToleranceLow],
			[MinValue, -1.7976931348623157081, 308, ToleranceLow, ToleranceLow],
			[MaxValue, 1.7976931348623157081, 308, ToleranceLow, ToleranceLow],
			[NegativeInfinity, BigDouble.NegativeInfinity.Mantissa, BigDouble.NegativeInfinity.Exponent, ToleranceLow, ToleranceLow],
			[PositiveInfinity, BigDouble.PositiveInfinity.Mantissa, BigDouble.PositiveInfinity.Exponent, ToleranceLow, ToleranceLow],
			[NaN, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow]
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
		}

		private static void AssertEqualSimple<T>(T actual, T expected) => Assert.That(actual, Is.EqualTo(expected));

		private static void AssertEqualDouble(double actual, double expected, long tolerance = ToleranceLow) => Assert.That(actual, Is.EqualTo(expected).Within(tolerance).Ulps);

		private static void AssertEqualBigDoubleComponents(BigDouble actual, double expectedMantissa, double expectedExponent, long toleranceMantissa = ToleranceLow, long toleranceExponent = ToleranceLow)
			=> Assert.Multiple(() => {
				AssertEqualDouble(actual.Mantissa, expectedMantissa, toleranceMantissa);
				AssertEqualDouble(actual.Exponent, expectedExponent, toleranceExponent);
			});

		private static void AssertEqualBigDouble(BigDouble actual, BigDouble expected, long tolerance = ToleranceLow) {
			if(actual.IsDouble() && expected.IsDouble()) {
				AssertEqualDouble((double)actual, (double)expected, tolerance);
			}
			else {
				Assert.Fail($"At least one of the numbers is beyond double (actual: {actual.ToDebugString()}, expected: {expected.ToDebugString()}); might want to implement this properly.");
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
		public void UnaryPlusOperatorBigDouble(BigDouble n, double d) => AssertEqualBigDouble(+n, +d);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void UnaryMinusOperatorBigDouble(BigDouble n, double d) => AssertEqualBigDouble(-n, -d);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void IncrementOperatorBigDouble(BigDouble n, double d) => AssertEqualBigDouble(++n, ++d);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void BinaryPlusOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDouble(nl + nr, dl + dr);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void DecrementOperatorBigDouble(BigDouble n, double d) => AssertEqualBigDouble(--n, --d);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void BinaryMinusOperatorBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDouble(nl - nr, dl - dr);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void MultiplicationBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDouble(nl * nr, dl * dr);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void DivisionBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDouble(nl / nr, dl / dr);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void ReciprocalBigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Reciprocal(n), 1 / d);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void AbsBigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Abs(n), Math.Abs(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void SqrtBigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Sqrt(n), Math.Sqrt(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void CbrtBigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Cbrt(n), Math.Cbrt(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void SquareBigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Square(n), d * d);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void CubeBigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Cube(n), d * d * d);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void Log10BigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Log10(n), Math.Log10(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void LnBigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Ln(n), Math.Log(d));

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAll))]
		public void Log2BigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Log2(n), Math.Log2(d));

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void LogBigDoubleDouble(BigDouble nl, double dl, BigDouble _, double dr) => AssertEqualBigDouble(BigDouble.Log(nl, dr), Math.Log(dl, dr), ToleranceMedium);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAll))]
		public void LogBigDoubleBigDouble(BigDouble nl, double dl, BigDouble nr, double dr) => AssertEqualBigDouble(BigDouble.Log(nl, nr), Math.Log(dl, dr), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void Exp10Double(BigDouble _, double d) => AssertEqualBigDouble(BigDouble.Exp10(d), Math.Pow(10, d), ToleranceMedium);

		[TestCaseSource(nameof(CasesBinaryBigDoubleAllSimple))]
		public void PowBigDoubleDouble(BigDouble nl, double dl, BigDouble _, double dr) => AssertEqualBigDouble(BigDouble.Pow(nl, dr), Math.Pow(dl, dr), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void ExpDouble(BigDouble _, double d) => AssertEqualBigDouble(BigDouble.Exp(d), Math.Exp(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void SinhDouble(BigDouble _, double d) => AssertEqualBigDouble(BigDouble.Sinh(d), Math.Sinh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void CoshDouble(BigDouble _, double d) => AssertEqualBigDouble(BigDouble.Cosh(d), Math.Cosh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void TanhDouble(BigDouble _, double d) => AssertEqualBigDouble(BigDouble.Tanh(d), Math.Tanh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void AsinhBigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Asinh(n), Math.Asinh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void AcoshBigDouble(BigDouble n, double d) => AssertEqualBigDouble(BigDouble.Acosh(n), Math.Acosh(d), ToleranceMedium);

		[TestCaseSource(nameof(CasesUnaryBigDoubleStrictAllSimple))]
		public void AtanhBigDouble(BigDouble _, double d) => AssertEqualBigDouble(BigDouble.Atanh(d), Math.Atanh(d), ToleranceMedium);
	}
}