namespace BreakInfinityProfiling {
	using BreakInfinity;

	internal class BigDoubleProfiling {
		private const int DataCount = 1 << 24;

		private static readonly BigDouble[] Data1;
		private static readonly Random random = new();

		static BigDoubleProfiling() {
			Data1 = new BigDouble[DataCount];
			for(int a = 0; a < DataCount; ++a) {
				Data1[a] = new BigDouble(random.NextDouble() * 20 - 10, Math.Truncate(random.NextDouble() * 2e50 - 1e50));
			}
		}

		private static BigDouble TryAdd(BigDouble[] data) {
			BigDouble result = BigDouble.One;
			foreach(BigDouble n in data) {
				result += n;
			}
			return result;
		}

		private static BigDouble TrySubtract(BigDouble[] data) {
			BigDouble result = BigDouble.One;
			foreach(BigDouble n in data) {
				result -= n;
			}
			return result;
		}

		private static BigDouble TryMultiply(BigDouble[] data) {
			BigDouble result = BigDouble.One;
			foreach(BigDouble n in data) {
				result *= n;
			}
			return result;
		}

		private static BigDouble TryDivide(BigDouble[] data) {
			BigDouble result = BigDouble.One;
			foreach(BigDouble n in data) {
				result /= n;
			}
			return result;
		}

		private static BigDouble TryPow(BigDouble[] data) {
			BigDouble result = BigDouble.One;
			foreach(BigDouble n in data) {
				result += BigDouble.Pow(n, random.NextDouble() * 2e6 - 1e6);
			}
			return result;
		}

		private static BigDouble TryLog(BigDouble[] data) {
			BigDouble result = BigDouble.One;
			foreach(BigDouble n in data) {
				result += BigDouble.Log(n, random.NextDouble() * 1e6 + 1e-15);
			}
			return result;
		}

		private static void Main() {
			Console.WriteLine($"Number of elements: {Data1.Length}");
			Console.WriteLine($"{nameof(TryAdd)}: {TryAdd(Data1)}");
			Console.WriteLine($"{nameof(TrySubtract)}: {TrySubtract(Data1)}");
			Console.WriteLine($"{nameof(TryMultiply)}: {TryMultiply(Data1)}");
			Console.WriteLine($"{nameof(TryDivide)}: {TryDivide(Data1)}");
			Console.WriteLine($"{nameof(TryPow)}: {TryPow(Data1)}");
			Console.WriteLine($"{nameof(TryLog)}: {TryLog(Data1)}");
		}
	}
}