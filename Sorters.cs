using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Vfs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TModel.Modules;

namespace TModel.Sorters
{
    public class NameSort : IComparer<IAesVfsReader>,
        IComparer<GameFile>,
        IComparer<GameContentItemPreview>
    {
        public static NameSort Global { get; } = new NameSort();

        public int Compare(IAesVfsReader? x, IAesVfsReader? y)
        {
            return x.Name.CompareTo(y.Name);
        }

        public int Compare(GameFile? x, GameFile? y)
        {
            return x.Name.CompareTo(y.Name);
        }

        public int Compare(GameContentItemPreview? x, GameContentItemPreview? y)
        {
            return x.File.Name.CompareTo(y.File.Name);
        }
    }

    public class SizeSort : IComparer<IAesVfsReader>
    {
        public int Compare(IAesVfsReader? x, IAesVfsReader? y)
        {
            return x.FileCount.CompareTo(y.FileCount);
        }
    }
}
