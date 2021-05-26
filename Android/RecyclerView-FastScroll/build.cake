var TARGET = Argument ("t", Argument ("target", "ci"));

var NUGET_VERSION = "2.0.1";

var AAR_VERSION = "2.0.1";
var AAR_URL = $"https://repo1.maven.org/maven2/com/simplecityapps/recyclerview-fastscroll/{AAR_VERSION}/recyclerview-fastscroll-{AAR_VERSION}.aar";

Task ("externals")
	.Does (() =>
{
	EnsureDirectoryExists ("./externals");
	
	DownloadFile(JAR_URL, "./externals/recyclerview-fastscroll.aar");

	// Update .csproj nuget versions
	XmlPoke("./source/RecyclerView-FastScroll/RecyclerView-FastScroll.csproj", "/Project/PropertyGroup/PackageVersion", NUGET_VERSION);
});

Task("nuget")
	.IsDependentOn("externals")
	.Does(() =>
{
	MSBuild ("./source/RecyclerView-FastScroll.sln", c => {
		c.Configuration = "Release";
		c.Restore = true;
		c.MaxCpuCount = 0;
		c.Targets.Clear();
		c.Targets.Add("Pack");
		c.Properties.Add("PackageOutputPath", new [] { MakeAbsolute(new FilePath("./output")).FullPath });
		c.Properties.Add("PackageRequireLicenseAcceptance", new [] { "true" });
		c.Properties.Add("DesignTimeBuild", new [] { "false" });
	});
});

Task("samples")
	.IsDependentOn("nuget")
	.Does(() =>
{
	
});

Task("ci")
	.IsDependentOn("externals")
	.IsDependentOn("nuget")
	.IsDependentOn("samples");

Task ("clean")
	.Does (() =>
{
	if (DirectoryExists ("./externals/"))
		DeleteDirectory ("./externals", new DeleteDirectorySettings {
			Recursive = true,
			Force = true
		});
});

RunTarget (TARGET);
