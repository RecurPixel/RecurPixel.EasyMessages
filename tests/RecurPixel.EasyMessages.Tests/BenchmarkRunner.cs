using BenchmarkDotNet.Running;
using RecurPixel.EasyMessages.Tests.Unit;

// Run the benchmarks
BenchmarkRunner.Run<MessagePerformanceBenchmarks>();
