#tool "nuget:?package=GitVersion.CommandLine"
#addin "Cake.Docker"

using System.Linq;

var target = Argument("target", "Default");
var parallel = Argument("parallel", false);
var dockerUser = Argument("dockerUser", (string)null);
var dockerPwd = Argument("dockerPwd", (string)null);
var dockerRepo = Argument("dockerRepo", (string)null);
var dockerComposeOverride = Argument("dockerComposeOverride", (string)null);
var dockerRegistryPrefix = Argument("dockerRegistryPrefix", (string)null);

GitVersion gitVersion = null;
string dockerTag = null;

Task("Init")
    .Does(() =>
{
    Information("Cake build started");
});

Task("GetVersionInfo")
    .IsDependentOn("Init")
    .Does(() =>
{
    dockerTag = ":latest";
    try{
        gitVersion = GitVersion(new GitVersionSettings {
            
        });
        Information(gitVersion.InformationalVersion);
        dockerTag = gitVersion.BranchName == "master" ? ":latest" : ":" + gitVersion.BranchName;
    }
    catch (System.Exception ex)
    {
        Warning($"GitVersion failed: {ex.Message}");
    }
    Information($"Docker tag: {dockerTag}");
});

Task("RestorePackages")
    .IsDependentOn("Init")
    .Does(() =>
{
    DotNetCoreRestore("UkrTrackingBot.sln");
});

Task("UnitTests")
    .IsDependentOn("Init")
    .Does(() =>
    {
        var settings = new DotNetCoreTestSettings
        {
            Configuration = "Release",
			ArgumentCustomization = args => args.Append($"--logger \"trx;LogFileName=testResults.xml\"")
        };

        var projectFiles = GetFiles("tests/**/*UnitTests.csproj");
        foreach(var file in projectFiles)
        {
            DotNetCoreTest(file.FullPath, settings);
        }
    });

Task("PublishAll")
    .IsDependentOn("UnitTests")
    .IsDependentOn("GetVersionInfo")
    .Does(() => 
{
    var projectsToPublish = new List<string> 
    {
        "UkrTrackingBot.Telegram.Console",
        "UkrTrackingBot.ApiWrapper.DeliveryAuto",
        "UkrTrackingBot.ApiWrapper.NovaPoshta",
        "UkrTrackingBot.IdentityServer",
        "UkrTrackingBot.Web",
        "UkrTrackingBot.Telegram.Web"
    };

    if (parallel)
    {
        projectsToPublish.AsParallel().ForAll(projectName => {
            DotNetCorePublish($"{projectName}/{projectName}.csproj", new DotNetCorePublishSettings
            {
                Configuration = "Release",
                OutputDirectory = $"{projectName}/bin/Docker/publish/"
            });
        });
    } 
    else
    {
        projectsToPublish.ForEach(projectName => {
            DotNetCorePublish($"{projectName}/{projectName}.csproj", new DotNetCorePublishSettings
            {
                Configuration = "Release",
                OutputDirectory = $"{projectName}/bin/Docker/publish/"
            });
        });
    }
});

Task("DockerProcessTemplate")
    .IsDependentOn("GetVersionInfo")
    .Does(() =>
{
    if (dockerTag == null){
        Error("Docker tag is null");
    }

    string dockerComposeFile = dockerComposeOverride != null ? dockerComposeOverride.Substring(0, dockerComposeOverride.LastIndexOf(".yml")) : "docker-compose";
    string text = TransformTextFile($"./{dockerComposeFile}.template.yml", "{", "}")
        .WithToken("tag", dockerTag)
        .ToString();
    System.IO.File.WriteAllText($"./{dockerComposeFile}.yml", text);
});

Task("DockerComposeBuild")
    .IsDependentOn("PublishAll")
    .IsDependentOn("DockerProcessTemplate")
    .Does(() => 
{
    string dockerComposeFile = dockerComposeOverride != null ? dockerComposeOverride : "docker-compose.yml";
    Information($"Using {dockerComposeFile}");
    DockerComposeBuild(new DockerComposeBuildSettings{
        Files = new [] { dockerComposeFile }
    });
});

Task("DockerLogin")
    .Does(() =>
{
    if (dockerUser != "SKIP"){
        DockerLogin(new DockerRegistryLoginSettings() {Username = dockerUser, Password = dockerPwd}, dockerRepo);
    }
});

Task("DockerPush")
    .IsDependentOn("DockerComposeBuild")
    .IsDependentOn("DockerLogin")
    .Does(() =>
{
    string tag = dockerTag;

    var images = new List<string> {
        $"{dockerRegistryPrefix}/ukrtrackingbotapiwrappernovaposhta{tag}",
        $"{dockerRegistryPrefix}/ukrtrackingbotapiwrapperdeliveryauto{tag}",
        $"{dockerRegistryPrefix}/ukrtrackingbottelegramconsole{tag}",
        $"{dockerRegistryPrefix}/ukrtrackingbottelegramweb{tag}",
        $"{dockerRegistryPrefix}/ukrtrackingbotidentityserver{tag}",
        $"{dockerRegistryPrefix}/ukrtrackingbotweb{tag}",
		$"{dockerRegistryPrefix}/nginxproxy{tag}",
        $"{dockerRegistryPrefix}/sentinel{tag}"
    };
    if (parallel)
    {
        images.AsParallel().ForAll(imgName => {
            DockerPush(imgName);
        });
    }
    else
    {
        foreach(var imgName in images)
        {
            DockerPush(imgName);
        }
    }
});

Task("Default")
    .IsDependentOn("DockerPush");

RunTarget(target);