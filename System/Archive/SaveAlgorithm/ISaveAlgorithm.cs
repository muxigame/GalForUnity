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

using System;
using System.Collections.Generic;
using GalForUnity.System.Archive.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using NotImplementedException = System.NotImplementedException;

namespace GalForUnity.System.Archive.SaveAlgorithm{
    public interface ISaveAlgorithm{ 
        void Save(string path);
        void Load(string path);
    }

    [Serializable]
    public abstract class SavableAlgorithm:SerializeSelfable, ISaveAlgorithm{
        
        [SerializeField]
        public List<ScriptData> scriptData = new List<ScriptData>();
        [SerializeField] public int sceneIndex=-1;
        [NonSerialized] 
        public bool parsed=false;
        protected SavableAlgorithm(Transform transform, Scene scene){
            if (scene!=default){
                sceneIndex = scene.buildIndex;
            }
        }

        public static SavableAlgorithm Create(ArchiveEnvironmentConfig.ArchiveAlgorithmType  algorithmType,Transform transform=null, Scene scene=default){
            switch (algorithmType){
                case ArchiveEnvironmentConfig.ArchiveAlgorithmType.UnityJson: return new AddresserAndUnityJson(transform,scene);
                case ArchiveEnvironmentConfig.ArchiveAlgorithmType.ReflectionSerialization: return new ReflectionSerialization(transform,scene);
            }
            return null;
        }
        public sealed override void Save(string path){
            OnSaveStart();
            base.Save(path);
        }
        public sealed override void Load(string path){
            base.Load(path);
            Loaded();
        }

        public abstract void OnSaveStart();

        public abstract void Loaded();
    }
}
