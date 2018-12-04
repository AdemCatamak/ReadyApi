string[] Projects = new string[]{
    "ReadyApi.Common.Model",
    "ReadyApi.Common.Exceptions",
    "ReadyApi.AspNetCore.Controllers",
    "ReadyApi.AspNetCore.BasicAuth",
    "ReadyApi.AspNetCore.Middleware",
    "ReadyApi.Common.ExceptionFilters",
};

// TOOLS

// ARGUMENTS

string BranchName = Argument("branchName", "");
string BuildConfig = Argument("buildType", "Release");
string NugetApiKey = Argument("nugetKey", "");
string NugetSourceUrl = Argument("nugetServer", "https://api.nuget.org/v3/index.json");

// VARIABLES

FilePathCollection slnFiles;

string NugetPackagePath = "./packages/";

string[] RemovableDirectories = new string[]{
"./**/bin/**",
"./**/obj/**",
"./**/build/**",
"./**/octoPackages/**"
};

string[] TestProjects = new string[]{
    "./**/*Test.csproj",
    "./**/*Tests.csproj",
};

var ReleaseBranch = new string[]
{
    "master"
};

// STAGE NAMES

string ResultStage = "RESULT";
string PushStage = "Push Package";
string PackageStage = "Create Package";
string TestStage = "Test";
string BuildStage = "Build";
string NugetRestoreStage = "Nuget Restore";
string CleanStage = "Clean";
string FindSlnStage = "Find .sln files";
string CheckEnvVarStage = "Check Environment Name";

// RUN OPERATION

var target = Argument("target", ResultStage);

Task(ResultStage)
.IsDependentOn(PushStage);

Task(PushStage)
.IsDependentOn(PackageStage)
.Does(()=>
{
    foreach (var project in Projects)
    {
        var npkgFiles = GetFiles($"./**/Release/*{project}*.nupkg");
        Console.WriteLine("NUPKG Files");
        foreach(var nupkgFile in npkgFiles)
        {
            Console.WriteLine(nupkgFile);
            var nugetPushSettings = new NuGetPushSettings
            {
                ApiKey = NugetApiKey,
                Source = NugetSourceUrl 
            };
        
            NuGetPush(nupkgFile.FullPath, nugetPushSettings);
        }
    }
   
});

Task(PackageStage)
.IsDependentOn(TestStage)
.Does(()=>
{
    // packages created build stage
});

Task(TestStage)
.IsDependentOn(BuildStage)
.Does(()=>
{
    foreach (var testProject in TestProjects)
    {
        var projectFiles = GetFiles(testProject);
        foreach(var file in projectFiles)
        {
            DotNetCoreTest(file.FullPath);
        }
    }
});

Task(BuildStage)
.IsDependentOn(NugetRestoreStage)
.Does(()=>
{
    DotNetCoreBuild(".", new DotNetCoreBuildSettings()
                        {
                            Configuration = BuildConfig,
                            ArgumentCustomization = args => args.Append("--no-restore"),
                        });
});

Task(NugetRestoreStage)
.IsDependentOn(CleanStage)
.Does(()=>
{
    DotNetCoreRestore();
});


Task(CleanStage)
.IsDependentOn(CheckEnvVarStage)
.Does(()=>
{
    foreach (var directoryPath in RemovableDirectories)
    {
        var directories = GetDirectories(directoryPath);

        foreach (var directory in directories)
        {
            if(!DirectoryExists(directory))
                continue;

            Console.WriteLine("Directory is cleaning : " + directory.ToString());            
            DeleteDirectory(directory, new DeleteDirectorySettings
            {
                Force = true,
                Recursive  = true
            });
        }

    }   
});

Task(CheckEnvVarStage)
.Does(()=>
{
    if(string.IsNullOrEmpty(BranchName))
    {
        throw new Exception("Branch Name should be provided");
    }
    Console.WriteLine("Branch Name = " + BranchName);

    if(string.IsNullOrEmpty(NugetApiKey))
    {
        throw new Exception("Nuget Api Key should be provided");
    }
});

RunTarget(target);