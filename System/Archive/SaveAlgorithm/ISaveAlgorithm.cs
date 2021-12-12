//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ISaveAlgorithm.cs
//
//        Created by 半世癫(Roc) at 2021-12-12 22:46:12
//
//======================================================================
//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ISaveAlgorithm.cs
//
//        Created by #AuthorName# at #CreateTime#
//
//======================================================================

using UnityEngine;

namespace GalForUnity.System.Archive.SaveAlgorithm{
    public interface ISaveAlgorithm{
        void Save(Transform transform);
        void Load();
    }
}
