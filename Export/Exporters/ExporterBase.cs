using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets;
using System.Collections.Generic;
using TModel.Modules;

namespace TModel.Export.Exporters
{
    // Base class for exporting Fortnite cosmetics, playsets and props.
    public abstract class ExporterBase
    {
        public bool bHasGameFiles;

        public List<GameContentItemPreview> GameFiles { set; get; }

        public abstract SearchTerm SearchTerm { get; }

        public abstract ItemTileInfo GetTileInfo(IPackage package);

        public abstract ExportPreviewInfo GetExportPreviewInfo(IPackage package);

        public virtual BlenderExportInfo GetBlenderExportInfo(IPackage package, int[]? styles = null)
        {
            return null;
        }

        public void EnsureInit() 
        {
            if (GameFiles is null)
            {
                GameFiles = new List<GameContentItemPreview>();
            }
        }
    }
}
