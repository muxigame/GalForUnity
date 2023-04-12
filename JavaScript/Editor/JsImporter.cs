using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;

namespace Jint.CommonJS
{
    [ScriptedImporter(1, "js")]
    public class JsImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            TextAsset textAsset = new TextAsset(File.ReadAllText(ctx.assetPath));
            ctx.AddObjectToAsset("main obj", textAsset);
            ctx.SetMainObject(textAsset);
        }
    }
}