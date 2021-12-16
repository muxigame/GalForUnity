//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  PlotAnimationSet.cs 
//
//        Created by 半世癫(Roc)
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.External;
using GalForUnity.System;
using UnityEngine;

namespace GalForUnity.Model.Plot{
    /// <summary>
    /// PreView 动画集，顾名思义就是动画的集合，当有多个动画要播放时，使用动画集管理播放，PreView
    /// </summary>
    [Serializable]
    public class PlotAnimationSet : MonoBehaviour,ICloneable{
        // [NonSerialized]
        public List<AnimationClip> animations = new List<AnimationClip>();
        public int Count => animations.Count;

        public AnimationClip this[int index]{
            get => index<animations.Count?animations[index]:null;
            set{
                if (index == Count){
                    animations.Add(value); 
                } else if(index<Count){
                    animations[index] = value;
                } else{
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
        public static List<AnimationClip> operator +(PlotAnimationSet plotAnimationSet, PlotAnimationSet plotAnimationSet2){
            return plotAnimationSet+plotAnimationSet2.animations;
        }

        public static List<AnimationClip> operator +(PlotAnimationSet plotAnimationSet, AnimationClip animationClip){
            if (animationClip == null) return plotAnimationSet != null ? plotAnimationSet.animations : null;
            if (plotAnimationSet == null) return new[]{animationClip}.ToList();
            plotAnimationSet.animations.Add(animationClip);
            return plotAnimationSet.animations;
        }

        public static List<AnimationClip> operator +(PlotAnimationSet plotAnimationSet, AnimationClip[] animationClip){
            return plotAnimationSet+animationClip.ToList();
        }
        public static List<AnimationClip> operator +(PlotAnimationSet plotAnimationSet, List<AnimationClip> animationClip){
            if (animationClip == null) return plotAnimationSet != null ? plotAnimationSet.animations : null;
            return plotAnimationSet == null ? animationClip : plotAnimationSet.animations.AddAll(animationClip);
        }

        public object Clone(){
            return base.MemberwiseClone();
        }
    }
}
