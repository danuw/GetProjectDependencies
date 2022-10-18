using System.Xml;

namespace cprojdepenencies
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length <= 0) {
                Console.WriteLine($"Make sure to provide the project to analyse. Use:");
                Console.WriteLine("dotnet run myproject.csproj");
                return;
            }
            var csprojPath = args[0];
            var projPath = "";
            if(args.Length>1){
                projPath = args[1];
            }
            var listOfDependencies = new List<string>();

            var listOfProjectsChecked = GetProjectDependencies(csprojPath);

            listOfProjectsChecked.Add(csprojPath);
            var finalList = listOfProjectsChecked.Distinct().ToList();
            Console.WriteLine($"'{csprojPath}' contains {finalList.Count} distinct dependencies from {listOfProjectsChecked.Count} found in total");
            finalList.ForEach(i => {
                var path = Directory.GetParent(i).FullName;
                if(!string.IsNullOrWhiteSpace(projPath)){
                    path = path.Replace(projPath, "");
                }
                Console.WriteLine($"     - {path}/*");
                //Console.WriteLine($" - {i}");
            });
        }

        static List<string> GetProjectDependencies(string csprojPath)
        {
            // if(!File.Exists(projectPath)) {
            //     Console.WriteLine($"'{projectPath}' does not exist!!");
            //     return new List<string>();
            // }
            // Set up a set of projects using https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference
            var listOfProjectsChecked = new List<string>();
            
            Console.WriteLine($"Getting project dependencies for {csprojPath}");

            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/6df34918-8c1e-4659-b6af-5872924d36b7/how-to-open-csproj-in-xml-file-through-c-code-and-add-a-file-as-a-link?forum=csharpgeneral
         
            XmlDocument doc = new XmlDocument();

            doc.Load(csprojPath);

            XmlNodeList elemList = doc.GetElementsByTagName("ProjectReference");

            //Console.WriteLine($"There are '{elemList.Count}' dependencies for '{projectPath}'");
            for (int i = 0; i < elemList.Count; i++)
            {
                string attrVal = elemList[i].Attributes["Include"].Value;
                var newcsprojPath = GetFullPath(csprojPath, attrVal);
                
                //listOfProjectsChecked.Add(attrVal);//record project reference as is in the file
                listOfProjectsChecked.Add(newcsprojPath);// record full path
                
                listOfProjectsChecked.AddRange(GetProjectDependencies(newcsprojPath));
            }
            
            return listOfProjectsChecked;
        }

        private static string GetFullPath(string csprojPath, string relativeCsprojPath)
        {
            var parentDir = "";
            parentDir = Directory.GetParent(csprojPath).FullName;
            System.Console.WriteLine(csprojPath);
            System.Console.WriteLine(relativeCsprojPath);
            if(relativeCsprojPath.Contains(".."))// we want to go to parent path
            {
                System.Console.WriteLine("Going up a level to ...");
                parentDir = Directory.GetParent(parentDir).FullName;
                System.Console.WriteLine(parentDir);
                relativeCsprojPath = relativeCsprojPath.Replace("\\", "/");
                relativeCsprojPath = relativeCsprojPath.Substring(3, relativeCsprojPath.Length - 3);
            }
            System.Console.WriteLine("We are at ...");
                
            System.Console.WriteLine(parentDir);
            System.Console.WriteLine(relativeCsprojPath);
            System.Console.WriteLine("So...");
            System.Console.WriteLine(Path.Combine(parentDir, relativeCsprojPath));
            
            // System.Console.WriteLine(relativeCsprojPath);
            // var lastindex = relativeCsprojPath.LastIndexOf("\\");
            // System.Console.WriteLine(lastindex +"-"+relativeCsprojPath.Length);
            // var filename = relativeCsprojPath.Substring(lastindex, relativeCsprojPath.Length - lastindex );
            // //var filename = Path.GetFileName(relativeCsprojPath);
            // System.Console.WriteLine(filename);
            return Path.Combine(parentDir, relativeCsprojPath);
        }
    }

    // public static class Extensions
    // {
        
    //     // source: https://stackoverflow.com/a/60480478
    //     /// <summary>
    //     ///     Fixes "../.." etc
    //     /// </summary>
    //     public static string Canonicalize(this string path)
    //     {
    //         if (path.IsAbsolutePath())
    //             return Path.GetFullPath(path);
    //         var fakeRoot = Environment.CurrentDirectory; // Gives us a cross platform full path
    //         var combined = Path.Combine(fakeRoot, path);
    //         combined = Path.GetFullPath(combined);
    //         return combined.RelativeTo(fakeRoot);
    //     }
    //     private static bool IsAbsolutePath(this string path)
    //     {
    //         if (path == null) throw new ArgumentNullException(nameof(path));
    //         return
    //             Path.IsPathRooted(path)
    //             && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
    //             && !Path.GetPathRoot(path).Equals(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal);
    //     }
    //     private static string RelativeTo(this string filespec, string folder)
    //     {
    //         var pathUri = new Uri(filespec);
    //         // Folders must end in a slash
    //         if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString())) folder += Path.DirectorySeparatorChar;
    //         var folderUri = new Uri(folder);
    //         return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString()
    //             .Replace('/', Path.DirectorySeparatorChar));
    //     }
    // }
}