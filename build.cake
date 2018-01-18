#tool nuget:?package=NUnit.ConsoleRunner&version=3.7.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories
var root = Directory("./");
var benchmarkRoot = root + Directory("NFBench.Benchmark");
var executionToolsRoot = root + Directory("NFBench.Execution");

// Benchmark Applications
var benchmarkSecurityBinDir = benchmarkRoot + Directory("NFBench.Benchmark.Security/bin") + Directory(configuration);
var benchmarkReliabilityBinDir = benchmarkRoot + Directory("NFBench.Benchmark.Reliability/bin/") + Directory(configuration);
var benchmarkPerformanceBinDir = benchmarkRoot + Directory("NFBench.Benchmark.Performance/bin/") + Directory(configuration);
var benchmarkReferenceImplementationBinDir = benchmarkRoot + Directory("NFBench.Benchmark.ReferenceImplementation/bin/") + Directory(configuration);

// Tooling
var internalToolsBinDir = executionToolsRoot + Directory("InternalTools/bin") + Directory(configuration);
var testClientApplicationsBinDir = executionToolsRoot + Directory("TestClientApplications/bin") + Directory(configuration);

// Tests
var benchmarkTestDir = root + Directory("./NFBench.Tests/bin/") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(benchmarkReferenceImplementationBinDir);
    CleanDirectory(benchmarkSecurityBinDir);
    CleanDirectory(benchmarkReliabilityBinDir);
    CleanDirectory(benchmarkPerformanceBinDir);

    CleanDirectory(internalToolsBinDir);
    CleanDirectory(testClientApplicationsBinDir);

    CleanDirectory(benchmarkTestDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("NFBench-CSharp.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild("NFBench-CSharp.sln", settings =>
        settings.SetVerbosity(Verbosity.Minimal));
});

Task("TestBenchmark")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3("./NFBench.Tests/bin/Debug/NFBench.Tests.dll", new NUnit3Settings {
        NoResults = true
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("TestBenchmark");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
