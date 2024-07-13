namespace BreakInfinityBenchmarks {
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Columns;
	using BenchmarkDotNet.Configs;
	using BenchmarkDotNet.Diagnosers;
	using BenchmarkDotNet.Jobs;
	using BenchmarkDotNet.Mathematics;
	using BenchmarkDotNet.Order;
	using BenchmarkDotNet.Running;

	using BreakInfinity;

	using Perfolizer.Horology;

	public class BigDoubleBenchmarks {
		private class FastConfig: ManualConfig {
			public FastConfig() {
				AddColumn(new RankColumn(NumeralSystem.Arabic));
				AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(syntax: DisassemblySyntax.Intel)));
				AddDiagnoser(MemoryDiagnoser.Default);
				AddJob(Job.Default.WithId("FastJob").WithIterationTime(TimeInterval.FromMilliseconds(125)));
				Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
			}
		}

		[Config(typeof(FastConfig))]
		public class DoubleBigDoubleAdd {
			private static readonly Random random = new();

			private readonly double[] ds;
			private readonly BigDouble[] bds;

			public DoubleBigDoubleAdd() {
				ds = [random.NextDouble() * 1024, random.NextDouble() * 1024];
				bds = [.. ds];
			}

			[Benchmark(Baseline = true)]
			public double DoubleAdd() {
				double result = 2;
				foreach(double n in ds) {
					result += n;
				}
				return result;
			}

			[Benchmark]
			public BigDouble BigDoubleAdd() {
				BigDouble result = BigDouble.Two;
				foreach(BigDouble n in bds) {
					result += n;
				}
				return result;
			}
		}

		[Config(typeof(FastConfig))]
		public class DoubleBigDoubleSubtract {
			private static readonly Random random = new();

			private readonly double[] ds;
			private readonly BigDouble[] bds;

			public DoubleBigDoubleSubtract() {
				ds = [random.NextDouble() * 1024, random.NextDouble() * 1024];
				bds = [.. ds];
			}

			[Benchmark(Baseline = true)]
			public double DoubleSubtract() {
				double result = 2;
				foreach(double n in ds) {
					result -= n;
				}
				return result;
			}

			[Benchmark]
			public BigDouble BigDoubleSubtract() {
				BigDouble result = BigDouble.Two;
				foreach(BigDouble n in bds) {
					result -= n;
				}
				return result;
			}
		}

		[Config(typeof(FastConfig))]
		public class DoubleBigDoubleMultiply {
			private static readonly Random random = new();

			private readonly double[] ds;
			private readonly BigDouble[] bds;

			public DoubleBigDoubleMultiply() {
				ds = [random.NextDouble() * 1024, random.NextDouble() * 1024];
				bds = [.. ds];
			}

			[Benchmark(Baseline = true)]
			public double DoubleMultiply() {
				double result = 2;
				foreach(double n in ds) {
					result *= n;
				}
				return result;
			}

			[Benchmark]
			public BigDouble BigDoubleMultiply() {
				BigDouble result = BigDouble.Two;
				foreach(BigDouble n in bds) {
					result *= n;
				}
				return result;
			}
		}

		[Config(typeof(FastConfig))]
		public class DoubleBigDoubleDivide {
			private static readonly Random random = new();

			private readonly double[] ds;
			private readonly BigDouble[] bds;

			public DoubleBigDoubleDivide() {
				ds = [random.NextDouble() * 1024, random.NextDouble() * 1024];
				bds = [.. ds];
			}

			[Benchmark(Baseline = true)]
			public double DoubleDivide() {
				double result = 2;
				foreach(double n in ds) {
					result /= n;
				}
				return result;
			}

			[Benchmark]
			public BigDouble BigDoubleDivide() {
				BigDouble result = BigDouble.Two;
				foreach(BigDouble n in bds) {
					result /= n;
				}
				return result;
			}
		}

		[Config(typeof(FastConfig))]
		public class DoubleBigDoublePow {
			private static readonly Random random = new();

			private readonly double[] ds;

			public DoubleBigDoublePow() {
				ds = [random.NextDouble() * 16, random.NextDouble() * 16];
			}

			[Benchmark(Baseline = true)]
			public double DoublePow() {
				double result = 2;
				foreach(double n in ds) {
					result = Math.Pow(result, n);
				}
				return result;
			}

			[Benchmark]
			public BigDouble BigDoublePow() {
				BigDouble result = BigDouble.Two;
				foreach(double n in ds) {
					result = BigDouble.Pow(result, n);
				}
				return result;
			}
		}

		[Config(typeof(FastConfig))]
		public class DoubleBigDoubleLog {
			private static readonly Random random = new();

			private readonly double[] ds;
			private readonly BigDouble[] bds;

			public DoubleBigDoubleLog() {
				ds = [random.NextDouble() * 1024, random.NextDouble() * 1024];
				bds = [.. ds];
			}

			[Benchmark(Baseline = true)]
			public double DoubleLog() {
				double result = 1e300;
				foreach(double n in ds) {
					result = Math.Log(result, n);
				}
				return result;
			}

			[Benchmark]
			public BigDouble BigDoubleLog() {
				BigDouble result = new(1, 300, false);
				foreach(BigDouble n in bds) {
					result = BigDouble.Log(result, n);
				}
				return result;
			}
		}

		private static void Main() {
			BenchmarkRunner.Run<DoubleBigDoubleAdd>();
			BenchmarkRunner.Run<DoubleBigDoubleSubtract>();
			BenchmarkRunner.Run<DoubleBigDoubleMultiply>();
			BenchmarkRunner.Run<DoubleBigDoubleDivide>();
			BenchmarkRunner.Run<DoubleBigDoublePow>();
			BenchmarkRunner.Run<DoubleBigDoubleLog>();
		}
	}
}