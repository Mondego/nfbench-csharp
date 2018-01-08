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
var benchmarkTestDir = Directory("./NFBench.Tests/bin/") + Directory(configuration);
var benchmarkAllBugsBuildDir = Directory("./NFBench.Benchmark/NFBench.Benchmark.Uncategorized/bin") + Directory(configuration);
var benchmarkRunnerBuildDir = Directory("./NFBench.Runner/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(benchmarkAllBugsBuildDir);
    CleanDirectory(benchmarkRunnerBuildDir);
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
