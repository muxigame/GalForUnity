//======================================================================
//
//       CopyRight 2019-#YEAR_NOW# © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  #FILE_NAME#
//
//        Created by #AUTHOR# at #TIME_NOW#
//
//======================================================================

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace MUX.Support{
    public class AutoCopyRight:MonoBehaviour{
        
#if UNITY_EDITOR
        
        [FormerlySerializedAs("AuthorName")] 
        public string authorName="半世癫(Roc)";
        public static string AuthorNameStatic="半世癫(Roc)";

        private void OnValidate(){
            AuthorNameStatic = authorName;
        }

        public class  WriteData: UnityEditor.AssetModificationProcessor{
            private const string DateFormat = "yyyy/MM/dd HH:mm:ss";
    
            private static void OnWillCreateAsset(string path)
            {
                path = path.Replace(".meta", "");
                if (path.EndsWith(".cs"))
                {
                    string allText = 
@"//======================================================================
//
//       CopyRight 2019-#YEAR_NOW# © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  #FILE_NAME#
//
//        Created by #AUTHOR# at #TIME_NOW#
//
//======================================================================"+"\r\n"+File.ReadAllText(path);
                    allText = allText.Replace("#AUTHOR#", AuthorNameStatic);
                    allText = allText.Replace("#TIME_NOW#", DateTime.Now.ToString(DateFormat).Replace("/","-"));
                    allText = allText.Replace("#YEAR_NOW#", DateTime.Now.Year+"");
                    allText = allText.Replace("#FILE_NAME#", path.Substring(path.LastIndexOf("/", StringComparison.Ordinal)+1));            
                    File.WriteAllText(path, allText);
                    UnityEditor.AssetDatabase.Refresh();
                }
            }
        }
#endif
    }
}
