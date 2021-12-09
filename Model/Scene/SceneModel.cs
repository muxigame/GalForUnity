//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  SceneModel.cs
//
//        Created by Roc
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.InstanceID;
using GalForUnity.System;
using UnityEngine;

namespace GalForUnity.Model.Scene {
	/// <summary>
	/// 场景模型，保存着场景背景和背景音乐
	/// </summary>
	[RequireComponent(typeof(GfuInstance))]
    public class SceneModel : MonoBehaviour {

	    /// <summary>
	    /// 场景背景，
	    /// </summary>
	    [Rename(nameof(backgroundImage))]
        public Sprite backgroundImage;
	    /// <summary>
	    /// 场景背景音乐
	    /// </summary>
	    [Rename(nameof(backgroundAudio))]
	    public AudioClip backgroundAudio;
	    
#pragma warning disable 36
	    public static readonly SceneModel NULL = Activator.CreateInstance<SceneModel>();
#pragma warning disable 36
	    /// <summary>
        /// 比较两个SceneModel是否是同一个场景，暂时只支持引用比较
        /// </summary>
        /// <param name="x">SceneModel</param>
        /// <returns>是否相同引用</returns>
        public override bool Equals(object x) {
		    if ((x is GameObject g) && ReferenceEquals(g, gameObject)) {
				return true;
			}
			return x is SceneModel && ReferenceEquals(this,x);
        }

		public override int GetHashCode() {
			return base.GetHashCode();
		}
		
		public static bool operator ==(SceneModel sceneModel, SceneModel sceneModel1) {
            if ((object)sceneModel == null) return (object)sceneModel1==null;
            return sceneModel.Equals(sceneModel1);
        }
        public static bool operator !=(SceneModel sceneModel, SceneModel sceneModel1) {
            return !(sceneModel == sceneModel1);
        }
        
    }
}