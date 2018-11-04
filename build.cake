#addin "Cake.Incubator"
#addin "Cake.Yaml"
#addin "YamlDotNet"
#addin "Cake.FileHelpers"

// CLI Arguments For Cake Script
var buildTarget = Argument("target", "hypervstep-local");
var os = Argument("os","Windows2016StdCore");
var productKey = Argument("productKey","");
var ramSize = Argument("ramSize","1024");

// These need to be environment variables if doing a vagrant cloud build
if (buildTarget.Contains("vagrant-cloud")) {
  EnvironmentVariable<string>("ATLAS_TOKEN");
  EnvironmentVariable<string>("ATLAS_USERNAME");
  EnvironmentVariable<string>("ATLAS_VERSION");
}

string hypervstepBuilderPath = "builders/hypervstep";
string hypervBuilderPath = "builders/hyperv";

// load build config yaml
var OSES = LoadYAMLConfig("./build.supported_os.yaml", os);
// Arguments passed overwrite YAML
if(false == String.IsNullOrWhiteSpace(productKey))
{
    OSES.productKey = productKey;
}
if(false == String.IsNullOrWhiteSpace(ramSize))
{
    OSES.ramSize = ramSize;
}

public class OSToBuild
{
    public string Name { get; set; }
    public string osName { get; set; }
    public string ramSize { get; set; }
    public string guestOSType { get; set; }
    public string imageName { get; set; }
    public string isoURL { get; set; }
    public string isoChecksum { get; set; }
    public string isoChecksumType { get; set; }
    public string productKey { get; set; }
}

public OSToBuild LoadYAMLConfig(string yaml_path, string os)
{
    //OSToBuild os_to_build_properties;
    try
    {
        var oses = DeserializeYamlFromFile<List<OSToBuild>> (yaml_path);

        // check if the OS the user passed exists
        bool matchingOS = oses.Any(n => n.Name == os);

        if (matchingOS == true)
        {
            // return the matching os to build
            return oses.Where(n => n.Name == os).First();
        }
        else
        {
            string exceptionMsg = $"Could not find a matching operating system in {yaml_path}. You passed in: {os}";
            throw new System.ArgumentException(exceptionMsg);
        }
    }
    catch(Exception e)
    {
        throw new System.ArgumentException($"Your YAML file at {yaml_path} is invalid!", e.Message);
    }
}


public string GetPackerSourcePath(string os_name, string source_path)
{
    string source_path_var = String.Format(source_path,
      os_name
    );

    Information("Source Path: " + source_path_var);

    return source_path_var;
}

public ProcessSettings RunPacker(OSToBuild os, string source_path, string json_file_path)
{
  string source_path_var;

  if (source_path != null)
  {
    source_path_var = String.Format(" -var \"source_path={0}\"",
      GetPackerSourcePath(os.osName, source_path)
    );
  }
  else
  {
    source_path_var = "";
  }

  var autoAttendFile = System.IO.Path.Combine(Environment.CurrentDirectory, "answer_files", os.osName, "Autounattend.xml");
  var tempFileDir = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString(), os.osName));
  var newAutoattendFile = System.IO.Path.Combine(tempFileDir.FullName, "Autounattend.xml");
  System.IO.File.Copy(autoAttendFile, newAutoattendFile);
  Information(newAutoattendFile);
  autoAttendFile = newAutoattendFile.Replace(@"\", "/");
    
  if(false == String.IsNullOrWhiteSpace(os.productKey))
  {
    Context.ReplaceRegexInFiles(newAutoattendFile, @"<!--<Key>SetKey</Key>-->", $"<Key>{os.productKey}</Key>");
  }
    
  if(false == String.IsNullOrWhiteSpace(os.imageName))
  {
    Context.ReplaceRegexInFiles(newAutoattendFile, @"(<Key>\/IMAGE\/NAME.*\r?\n.*<Value>).+?(<\/Value>)", $"$1{os.imageName}$2");
  }

    string packer_cmd = $"-var \"os_name={os.osName}\" -var \"answer_file={autoAttendFile}\"  -var \"ram_size={os.ramSize}\" -var \"iso_checksum={os.isoChecksum}\" -var \"iso_checksum_type={os.isoChecksumType}\" -var \"iso_url={os.isoURL}\" -var \"guest_os_type={os.guestOSType}\" -var \"full_os_name={os.Name}\" {source_path_var} {json_file_path}";

  Information(packer_cmd);

  var settings = new ProcessSettings
  {
    Arguments = new ProcessArgumentBuilder().Append("build").Append(packer_cmd)
  };
  return settings;
}

// Clean
Task("clean")
  .Does(() =>
{
    Information("Clean was invoked");
    var directoriesToClean = GetDirectories("./output-*/**");

    var deleteSettings = new DeleteDirectorySettings {
      Recursive = true,
      Force = true,
    };

    foreach (var directory in directoriesToClean)
    {
        if (DirectoryExists(directory))
        {
            DeleteDirectory(directory, deleteSettings);
            Information($"Deleted directory {directory}.");
        }
    }
});

// Hypervstep Tasks
Task("hypervstep-01-windows-base")
  .Does(() =>
{
    string jsonToBuild = $"{hypervstepBuilderPath}/01-windows-base.json";
    StartProcess("packer", RunPacker(OSES, "", jsonToBuild));
});

Task("hypervstep-02-win_updates-wmf5")
  .IsDependentOn("hypervstep-01-windows-base")
  .Does(() =>
{
    string jsonToBuild = $"{hypervstepBuilderPath}/02-win_updates-wmf5.json";
    StartProcess("packer", RunPacker(OSES, "./output-{0}-base/", jsonToBuild));
});

Task("hypervstep-02-1-win_base-software")
//   .IsDependentOn("hypervstep-01-windows-base")
  .Does(() =>
{
    string jsonToBuild = $"{hypervstepBuilderPath}/02-1-win_base-software.json";
    StartProcess("packer", RunPacker(OSES, "./output-{0}-base/", jsonToBuild));
});
Task("hypervstep-03-cleanup")
  .IsDependentOn("hypervstep-02-win_updates-wmf5")
  .Does(() =>
{
    string jsonToBuild = $"{hypervstepBuilderPath}/03-cleanup.json";
    StartProcess("packer", RunPacker(OSES, "./output-{0}-updates_wmf5/", jsonToBuild));
});

Task("hypervstep-local")
  .IsDependentOn("hypervstep-03-cleanup")
  .Does(() =>
{
    string jsonToBuild = $"{hypervstepBuilderPath}/04-local.json";
    StartProcess("packer", RunPacker(OSES, "./output-{0}-cleanup/", jsonToBuild));
});

Task("hypervstep-vagrant-cloud")
  .IsDependentOn("hypervstep-03-cleanup")
  .Does(() =>
{
    string jsonToBuild = $"{hypervstepBuilderPath}/04-vagrant-cloud.json";
    StartProcess("packer", RunPacker(OSES, "./output-{0}-cleanup/", jsonToBuild));
});

// Hyper-V Tasks
Task("hyperv-local")
  .Does(() =>
{
    string jsonToBuild = $"{hypervBuilderPath}/01-windows-local.json";
    StartProcess("packer", RunPacker(OSES, "", jsonToBuild));
});

Task("hyperv-vagrant-cloud")
  .Does(() =>
{
    string jsonToBuild = $"{hypervBuilderPath}/01-windows-vagrant-cloud.json";
    StartProcess("packer", RunPacker(OSES, "", jsonToBuild));
});

RunTarget(buildTarget);
