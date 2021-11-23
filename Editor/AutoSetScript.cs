//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  AutoSetScript.cs
//
//        Created by 半世癫(Roc) at 2021-01-02 09:12:36
//
//======================================================================

using System;
using System.IO;

namespace GalForUnity.Editor{
    public class AutoSetScript : UnityEditor.AssetModificationProcessor
    {
        //导入资源创建资源时候调用
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            if (path.EndsWith(".cs"))
            {
                string allText = File.ReadAllText(path);
                allText = allText.Replace("#AuthorName#", "半世癫(Roc)")
                                 .Replace("#CreateTime#", TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                                     DateTime.UtcNow, "China Standard Time").ToString("yyyy-MM-dd HH:mm:ss"));
                
                File.WriteAllText(path, allText);
            }
        }
    }
}
