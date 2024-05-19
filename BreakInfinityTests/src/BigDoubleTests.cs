// Copyright 2024 Joonhyo Choi (asdfchlwnsgy1236); Apache License Version 2.0.

namespace BreakInfinityTests {
	using BreakInfinity;

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
		private const long ToleranceHigh = 1L << 32;

		// TODO: Reduce test case redundancy. Maybe comparing to the double equivalent when within its range might help.

		private static readonly object[][] ConstructorDoubleDoubleData = [
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

		private static readonly object[][] ConstructorDoubleData = [
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

		private static readonly object[][] GetPowerOf10IntData = [
			[0, 1, ToleranceLow],
			[-10, 1e-10, ToleranceLow],
			[10, 1e10, ToleranceLow],
			[-323, 1e-323, ToleranceLow],
			[308, 1e308, ToleranceLow]
		];

		private static readonly object[][] GetStandardNameIntData = [
			[0, ""],
			[3, "K"],
			[6, "M"],
			[65, "Vig"],
			[66, ""]
		];

		private static readonly object[][] IsFiniteBigDoubleData = [
			[BigDouble.Zero, true],
			[BigDouble.Epsilon, true],
			[BigDouble.Ten, true],
			[BigDouble.MinValue, true],
			[BigDouble.MaxValue, true],
			[BigDouble.NegativeInfinity, false],
			[BigDouble.PositiveInfinity, false],
			[BigDouble.NaN, false]
		];

		private static readonly object[][] IsNaNBigDoubleData = [
			[BigDouble.Zero, false],
			[BigDouble.Epsilon, false],
			[BigDouble.Ten, false],
			[BigDouble.MinValue, false],
			[BigDouble.MaxValue, false],
			[BigDouble.NegativeInfinity, false],
			[BigDouble.PositiveInfinity, false],
			[BigDouble.NaN, true]
		];

		private static readonly object[][] IsInfinityBigDoubleData = [
			[BigDouble.Zero, false],
			[BigDouble.Epsilon, false],
			[BigDouble.Ten, false],
			[BigDouble.MinValue, false],
			[BigDouble.MaxValue, false],
			[BigDouble.NegativeInfinity, true],
			[BigDouble.PositiveInfinity, true],
			[BigDouble.NaN, false]
		];

		private static readonly object[][] IsNegativeInfinityBigDoubleData = [
			[BigDouble.Zero, false],
			[BigDouble.Epsilon, false],
			[BigDouble.Ten, false],
			[BigDouble.MinValue, false],
			[BigDouble.MaxValue, false],
			[BigDouble.NegativeInfinity, true],
			[BigDouble.PositiveInfinity, false],
			[BigDouble.NaN, false]
		];

		private static readonly object[][] IsPositiveInfinityBigDoubleData = [
			[BigDouble.Zero, false],
			[BigDouble.Epsilon, false],
			[BigDouble.Ten, false],
			[BigDouble.MinValue, false],
			[BigDouble.MaxValue, false],
			[BigDouble.NegativeInfinity, false],
			[BigDouble.PositiveInfinity, true],
			[BigDouble.NaN, false]
		];

		private static readonly object[][] IsNegativeBigDoubleData = [
			[BigDouble.Zero, false],
			[BigDouble.Epsilon, false],
			[-BigDouble.Ten, true],
			[BigDouble.MinValue, true],
			[BigDouble.MaxValue, false],
			[BigDouble.NegativeInfinity, true],
			[BigDouble.PositiveInfinity, false]
		];

		private static readonly object[][] IsZeroBigDoubleData = [
			[BigDouble.Zero, true],
			[BigDouble.Epsilon, false],
			[BigDouble.Ten, false],
			[BigDouble.MinValue, false],
			[BigDouble.MaxValue, false],
			[BigDouble.NegativeInfinity, false],
			[BigDouble.PositiveInfinity, false],
			[BigDouble.NaN, false]
		];

		private static readonly object[][] SignBigDoubleData = [
			[BigDouble.Zero, 0],
			[BigDouble.Epsilon, 1],
			[-BigDouble.Ten, -1],
			[BigDouble.MinValue, -1],
			[BigDouble.MaxValue, 1],
			[BigDouble.NegativeInfinity, -1],
			[BigDouble.PositiveInfinity, 1]
		];

		private static readonly object[][] EqualityOperatorBigDoubleBigDoubleData = [
			[BigDouble.Zero, BigDouble.Zero, true],
			[BigDouble.One, BigDouble.Ten, false],
			[BigDouble.Ten, new BigDouble(Math.BitIncrement(1), 1, false), false],
			[BigDouble.NegativeInfinity, BigDouble.NegativeInfinity, true],
			[BigDouble.PositiveInfinity, BigDouble.PositiveInfinity, true],
			[BigDouble.NaN, BigDouble.NaN, false]
		];

		private static readonly object[][] InequalityOperatorBigDoubleBigDoubleData = [
			[BigDouble.Zero, BigDouble.Zero, false],
			[BigDouble.One, BigDouble.Ten, true],
			[BigDouble.Ten, new BigDouble(Math.BitIncrement(1), 1, false), true],
			[BigDouble.NegativeInfinity, BigDouble.NegativeInfinity, false],
			[BigDouble.PositiveInfinity, BigDouble.PositiveInfinity, false],
			[BigDouble.NaN, BigDouble.NaN, true]
		];

		private static readonly object[][] LessThanOperatorBigDoubleBigDoubleData = [
			[BigDouble.Zero, BigDouble.Zero, false],
			[BigDouble.Zero, BigDouble.One, true],
			[BigDouble.Ten, new BigDouble(Math.BitDecrement(10), 0, false), false],
			[BigDouble.Ten, new BigDouble(Math.BitIncrement(1), 1, false), true],
			[BigDouble.NegativeInfinity, BigDouble.MinValue, true],
			[BigDouble.PositiveInfinity, BigDouble.MaxValue, false],
			[BigDouble.NaN, BigDouble.Zero, false],
			[BigDouble.NaN, BigDouble.NaN, false]
		];

		private static readonly object[][] GreaterThanOperatorBigDoubleBigDoubleData = [
			[BigDouble.Zero, BigDouble.Zero, false],
			[BigDouble.Zero, BigDouble.One, false],
			[BigDouble.Ten, new BigDouble(Math.BitDecrement(10), 0, false), true],
			[BigDouble.Ten, new BigDouble(Math.BitIncrement(1), 1, false), false],
			[BigDouble.NegativeInfinity, BigDouble.MinValue, false],
			[BigDouble.PositiveInfinity, BigDouble.MaxValue, true],
			[BigDouble.NaN, BigDouble.Zero, false],
			[BigDouble.NaN, BigDouble.NaN, false]
		];

		private static readonly object[][] LessThanOrEqualOperatorBigDoubleBigDoubleData = [
			[BigDouble.Zero, BigDouble.Zero, true],
			[BigDouble.Zero, BigDouble.One, true],
			[BigDouble.Ten, new BigDouble(Math.BitDecrement(10), 0, false), false],
			[BigDouble.Ten, new BigDouble(Math.BitIncrement(1), 1, false), true],
			[BigDouble.NegativeInfinity, BigDouble.MinValue, true],
			[BigDouble.PositiveInfinity, BigDouble.MaxValue, false],
			[BigDouble.NaN, BigDouble.Zero, false],
			[BigDouble.NaN, BigDouble.NaN, false]
		];

		private static readonly object[][] GreaterThanOrEqualOperatorBigDoubleBigDoubleData = [
			[BigDouble.Zero, BigDouble.Zero, true],
			[BigDouble.Zero, BigDouble.One, false],
			[BigDouble.Ten, new BigDouble(Math.BitDecrement(10), 0, false), true],
			[BigDouble.Ten, new BigDouble(Math.BitIncrement(1), 1, false), false],
			[BigDouble.NegativeInfinity, BigDouble.MinValue, false],
			[BigDouble.PositiveInfinity, BigDouble.MaxValue, true],
			[BigDouble.NaN, BigDouble.Zero, false],
			[BigDouble.NaN, BigDouble.NaN, false]
		];

		private static void AssertEqualSimple<T>(T actual, T expected) => Assert.That(actual, Is.EqualTo(expected));

		private static void AssertEqualDouble(double actual, double expected, long tolerance) => Assert.That(actual, Is.EqualTo(expected).Within(tolerance).Ulps);

		private static void AssertEqualBigDouble(BigDouble actual, double expectedMantissa, double expectedExponent, long toleranceMantissa, long toleranceExponent)
			=> Assert.Multiple(() => {
				AssertEqualDouble(actual.Mantissa, expectedMantissa, toleranceMantissa);
				AssertEqualDouble(actual.Exponent, expectedExponent, toleranceExponent);
			});

		private static void AssertEqualString(string actual, string expected) => Assert.That(actual, Is.EqualTo(expected).IgnoreCase);

		[TestCaseSource(nameof(ConstructorDoubleDoubleData))]
		public void ConstructorDoubleDouble(double mantissa, double exponent, double expectedMantissa, double expectedExponent, long toleranceMantissa, long toleranceExponent)
			=> AssertEqualBigDouble(new(mantissa, exponent), expectedMantissa, expectedExponent, toleranceMantissa, toleranceExponent);

		[TestCaseSource(nameof(ConstructorDoubleData))]
		public void ConstructorDouble(double n, double expectedMantissa, double expectedExponent, long toleranceMantissa, long toleranceExponent)
			=> AssertEqualBigDouble(new(n), expectedMantissa, expectedExponent, toleranceMantissa, toleranceExponent);

		[TestCaseSource(nameof(GetPowerOf10IntData))]
		public void GetPowerOf10Int(int power, double expected, long tolerance) => AssertEqualDouble(BigDouble.GetPowerOf10(power), expected, tolerance);

		[TestCaseSource(nameof(GetStandardNameIntData))]
		public void GetStandardNameInt(int power, string expected) => AssertEqualString(BigDouble.GetStandardName(power), expected);

		[TestCaseSource(nameof(IsFiniteBigDoubleData))]
		public void IsFiniteBigDouble(BigDouble n, bool expected) => AssertEqualSimple(BigDouble.IsFinite(n), expected);

		[TestCaseSource(nameof(IsNaNBigDoubleData))]
		public void IsNaNBigDouble(BigDouble n, bool expected) => AssertEqualSimple(BigDouble.IsNaN(n), expected);

		[TestCaseSource(nameof(IsInfinityBigDoubleData))]
		public void IsInfinityBigDouble(BigDouble n, bool expected) => AssertEqualSimple(BigDouble.IsInfinity(n), expected);

		[TestCaseSource(nameof(IsNegativeInfinityBigDoubleData))]
		public void IsNegativeInfinityBigDouble(BigDouble n, bool expected) => AssertEqualSimple(BigDouble.IsNegativeInfinity(n), expected);

		[TestCaseSource(nameof(IsPositiveInfinityBigDoubleData))]
		public void IsPositiveInfinityBigDouble(BigDouble n, bool expected) => AssertEqualSimple(BigDouble.IsPositiveInfinity(n), expected);

		[TestCaseSource(nameof(IsNegativeBigDoubleData))]
		public void IsNegativeBigDouble(BigDouble n, bool expected) => AssertEqualSimple(BigDouble.IsNegative(n), expected);

		[TestCaseSource(nameof(IsZeroBigDoubleData))]
		public void IsZeroBigDouble(BigDouble n, bool expected) => AssertEqualSimple(BigDouble.IsZero(n), expected);

		[TestCaseSource(nameof(SignBigDoubleData))]
		public void SignBigDouble(BigDouble n, int expected) => AssertEqualSimple(BigDouble.Sign(n), expected);

		[TestCaseSource(nameof(EqualityOperatorBigDoubleBigDoubleData))]
		public void EqualityOperatorBigDoubleBigDouble(BigDouble l, BigDouble r, bool expected) => AssertEqualSimple(l == r, expected);

		[TestCaseSource(nameof(InequalityOperatorBigDoubleBigDoubleData))]
		public void InequalityOperatorBigDoubleBigDouble(BigDouble l, BigDouble r, bool expected) => AssertEqualSimple(l != r, expected);

		[TestCaseSource(nameof(LessThanOperatorBigDoubleBigDoubleData))]
		public void LessThanOperatorBigDoubleBigDouble(BigDouble l, BigDouble r, bool expected) => AssertEqualSimple(l < r, expected);

		[TestCaseSource(nameof(GreaterThanOperatorBigDoubleBigDoubleData))]
		public void GreaterThanOperatorBigDoubleBigDouble(BigDouble l, BigDouble r, bool expected) => AssertEqualSimple(l > r, expected);

		[TestCaseSource(nameof(LessThanOrEqualOperatorBigDoubleBigDoubleData))]
		public void LessThanOrEqualOperatorBigDoubleBigDouble(BigDouble l, BigDouble r, bool expected) => AssertEqualSimple(l <= r, expected);

		[TestCaseSource(nameof(GreaterThanOrEqualOperatorBigDoubleBigDoubleData))]
		public void GreaterThanOrEqualOperatorBigDoubleBigDouble(BigDouble l, BigDouble r, bool expected) => AssertEqualSimple(l >= r, expected);
	}
}