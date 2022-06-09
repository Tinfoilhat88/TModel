using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TModel.Export.Materials;

namespace TModel.Export
{
    public class BlenderExportInfo
    {
        public string Name { get; set; }

        public List<ModelRef> Models { get; } = new List<ModelRef>();

        // If the Ik constraints and colored bones should be created
        public bool IsCharacter = false;

        // Returns list of all materials of all Models combined excluding duplicates.
        // (Some models may use the same materials so it doesnt need to load the same one
        // more than once)
        public List<CMaterial> GetMaterials()
        {
            List<CMaterial> Result = new List<CMaterial>();
            foreach (var model in Models)
                if (model != null)
                    foreach (var material in model.Materials)
                    {
                        bool AlreadyAdded = false;
                        foreach (var item in Result)
                            if (item.Name == material.Name)
                            {
                                AlreadyAdded = true;
                                break;
                            }
                        if (!AlreadyAdded)
                            Result.Add(material);
                    }
            return Result;
        }

        public void Save()
        {
            string BlenderDataPath = Path.Combine(Preferences.StorageFolder, "BlenderData.export");
            CBinaryWriter Writer = null;
            try
            {
                File.Delete(BlenderDataPath);
                Writer = new CBinaryWriter(File.Open(BlenderDataPath, FileMode.OpenOrCreate, FileAccess.Write));
            }
            catch (Exception e)
            {
                Log.Error("Cant write to export file (try restarting blender): " + e.Message);
            }
            if (Writer != null)
            {
                Writer.Write(IsCharacter);
                Writer.Write((byte)Models.Count);
                foreach (var model in Models)
                    if (model != null)
                        model.SaveMesh(Writer);

                List<CMaterial> Materials = GetMaterials();
                Writer.Write((byte)Materials.Count);
                foreach (var material in Materials)
                {
                    material.SaveAndWriteBinary(Writer);
                }

                Writer.Close();
                Log.Information("\n======== Export Complete ========");
            }
            else
            {
                Log.Error("\n======== Export Failed ========");
            }
        }
    }
}
