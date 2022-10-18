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
            var projectPath = args[0];
            var listOfDependencies = new List<string>();

            var listOfProjectsChecked = GetProjectDependencies(projectPath);
            listOfProjectsChecked.Add(projectPath);
            var finalList = listOfProjectsChecked.Distinct().ToList();
            Console.WriteLine($"'{projectPath}' contains {finalList.Count} distinct dependencies from {listOfProjectsChecked.Count} found in total");
            finalList.ForEach(i => {
                var newname = i.Replace("..\\","");
                var lastindex = newname.LastIndexOf("\\");
                newname = newname.Substring(0, lastindex );
                Console.WriteLine($"     - {newname}");
            });
        }

        static List<string> GetProjectDependencies(string projectPath)
        {
            // if(!File.Exists(projectPath)) {
            //     Console.WriteLine($"'{projectPath}' does not exist!!");
            //     return new List<string>();
            // }
            // Set up a set of projects using https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-add-reference
            var listOfProjectsChecked = new List<string>();
            
            //Console.WriteLine($"Getting project dependencies for {projectPath}");

            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/6df34918-8c1e-4659-b6af-5872924d36b7/how-to-open-csproj-in-xml-file-through-c-code-and-add-a-file-as-a-link?forum=csharpgeneral
         
            XmlDocument doc = new XmlDocument();

            doc.Load(projectPath);

            XmlNodeList elemList = doc.GetElementsByTagName("ProjectReference");

            //Console.WriteLine($"There are '{elemList.Count}' dependencies for '{projectPath}'");
            for (int i = 0; i < elemList.Count; i++)
            {
                string attrVal = elemList[i].Attributes["Include"].Value;
                listOfProjectsChecked.Add(attrVal);
                var newPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), attrVal.Replace("\\", "/")));
                listOfProjectsChecked.AddRange(GetProjectDependencies(newPath.Canonicalize()));
            }
            
            return listOfProjectsChecked;
        }


    }

    public static class Extensions
    {
        
        // source: https://stackoverflow.com/a/60480478
        /// <summary>
        ///     Fixes "../.." etc
        /// </summary>
        public static string Canonicalize(this string path)
        {
            if (path.IsAbsolutePath())
                return Path.GetFullPath(path);
            var fakeRoot = Environment.CurrentDirectory; // Gives us a cross platform full path
            var combined = Path.Combine(fakeRoot, path);
            combined = Path.GetFullPath(combined);
            return combined.RelativeTo(fakeRoot);
        }
        private static bool IsAbsolutePath(this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return
                Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                && !Path.GetPathRoot(path).Equals(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }
        private static string RelativeTo(this string filespec, string folder)
        {
            var pathUri = new Uri(filespec);
            // Folders must end in a slash
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString())) folder += Path.DirectorySeparatorChar;
            var folderUri = new Uri(folder);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString()
                .Replace('/', Path.DirectorySeparatorChar));
        }
    }
}