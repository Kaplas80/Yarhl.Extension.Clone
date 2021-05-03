#load "nuget:?package=PleOps.Cake&version=0.4.2"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.PreviewNuGetFeed = "https://pkgs.dev.azure.com/kaplas80/Kaplas80-Preview/_packaging/Kaplas-Software-Preview/nuget/v3/index.json";

    info.AddLibraryProjects("Yarhl.Extension.Clone");
    info.AddTestProjects("Yarhl.Extension.CloneTests");
});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);
