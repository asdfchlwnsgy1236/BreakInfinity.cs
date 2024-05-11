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
			[0, NaN, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[0, NegativeInfinity, BigDouble.Zero.Mantissa, BigDouble.Zero.Exponent, ToleranceLow, ToleranceLow],
			[0, PositiveInfinity, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[NegativeInfinity, NegativeInfinity, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[NegativeInfinity, PositiveInfinity, BigDouble.NegativeInfinity.Mantissa, BigDouble.NegativeInfinity.Exponent, ToleranceLow, ToleranceLow],
			[PositiveInfinity, NegativeInfinity, BigDouble.NaN.Mantissa, BigDouble.NaN.Exponent, ToleranceLow, ToleranceLow],
			[PositiveInfinity, PositiveInfinity, BigDouble.PositiveInfinity.Mantissa, BigDouble.PositiveInfinity.Exponent, ToleranceLow, ToleranceLow]
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

		private static void AssertBigDoubleIsEqualTo(BigDouble n, double expectedMantissa, double expectedExponent, long toleranceMantissa, long toleranceExponent) {
			Assert.Multiple(() => {
				Assert.That(n.Mantissa, Is.EqualTo(expectedMantissa).Within(toleranceMantissa).Ulps);
				Assert.That(n.Exponent, Is.EqualTo(expectedExponent).Within(toleranceExponent).Ulps);
			});
		}

		[TestCaseSource(nameof(ConstructorDoubleDoubleData))]
		public void ConstructorDoubleDouble(double mantissa, double exponent, double expectedMantissa, double expectedExponent, long toleranceMantissa, long toleranceExponent) {
			AssertBigDoubleIsEqualTo(new(mantissa, exponent), expectedMantissa, expectedExponent, toleranceMantissa, toleranceExponent);
		}

		[TestCaseSource(nameof(ConstructorDoubleData))]
		public void ConstructorDouble(double d, double expectedMantissa, double expectedExponent, long toleranceMantissa, long toleranceExponent) {
			AssertBigDoubleIsEqualTo(new(d), expectedMantissa, expectedExponent, toleranceMantissa, toleranceExponent);
		}
	}
}