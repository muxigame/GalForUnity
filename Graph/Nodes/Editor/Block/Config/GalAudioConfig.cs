//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalAudioConfig.cs Created at 2022-09-27 23:33:34
//
//======================================================================

using System;
using GalForUnity.Attributes;
using UnityEngine;

namespace GalForUnity.Graph.Block.Config{
    [Serializable]
    [Rename("Audio Config")]
    public class GalAudioConfig : IGalConfig<AudioSource>{
        public AudioClip audioClip;
        [Rename("")]
        public bool? bypassEffect;
        [Rename("")]
        public bool? loop;
        [Rename("")]
        public bool? mute;
        [Rename("")]
        public float? pitch;
        [Rename("")]
        public int? priority;
        [Rename("")]
        public float? spatialBlend;
        [Rename("")]
        public float? stereoPan;
        [Rename("")]
        public float? volume;

        public void Process(AudioSource t){
            if (audioClip != null) t.clip = audioClip;
            if (loop         != null) t.loop = (bool) loop;
            if (mute         != null) t.mute = (bool) mute;
            if (bypassEffect != null) t.bypassEffects = (bool) bypassEffect;
            if (volume       != null) t.volume = (float) volume;
            if (priority     != null) t.priority = (int) priority;
            if (pitch        != null) t.pitch = (float) pitch;
            if (stereoPan    != null) t.panStereo = (float) stereoPan;
            if (spatialBlend != null) t.spatialBlend = (float) spatialBlend;
        }
    }

    public interface IGalConfig<T>:IGalConfig{
        void Process(T t);
    }

    public interface IGalConfig{
    
    }
}