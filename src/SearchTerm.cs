using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TModel
{
    // Specifies filter options for files
    public class SearchTerm
    {
        // ************ Search Terms ***********

        // Strings that the file name should NOT start with.
        // EXAMPLE: use 'WID' for FileNameStart
        //          and 'WID_Harvest_Pickaxe' for FileFalseNameStart,
        //          CheckName() returns true if the path is a weapon but not a pickaxe.
        public string[] FileFalseNameStart { set; get; } = Array.Empty<string>();

        // The string the the name must start with
        // Examples are 'CID' or 'WID'
        public string[] FileNameStart { set; get; } = Array.Empty<string>();

        // Paths must end with '/'
        // CheckName() will return true if the given path is in this
        // immediate directory NOT in sub folders of it.
        public string[] SpecificPaths { set; get; } = Array.Empty<string>();

        // Empty construct for SearchTerm
        public SearchTerm() { }

        // The path must be the full path and must include the extension
        public bool CheckName(string path)
        {
            foreach (var item in SpecificPaths)
                if (path.StartsWith(item))
                {
                    string Name = path.Substring(item.Length);
                    if (Name.IndexOf('/') != -1)
                        break; // Has subfolder - NOT valid
                    return true;
                }

            string FileName = Path.GetFileName(path);
            foreach (var NameStart in FileNameStart)
                if (FileName.StartsWith(NameStart))
                    foreach (var FalseStart in FileFalseNameStart)
                        if (!FileName.StartsWith(FalseStart))
                            return true;

            return false;
        }
    }
}
