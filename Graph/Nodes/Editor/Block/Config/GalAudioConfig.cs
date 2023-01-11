//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalAudioConfig.cs Created at 2022-09-27 23:33:34
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.Core;
using GalForUnity.Graph.SceneGraph;
using UnityEngine;

namespace GalForUnity.Graph.Block.Config{
    [Serializable]
    [Rename("Audio Config")]
    [NodeEditor(typeof(PlotAudioBlockEditorUxml))]
    public class GalAudioConfig : GalConfig{
        public AudioClip audioClip;

        public AudioSource audioSource;

        [Rename("")] public bool bypassEffects;

        [Rename("")] public bool loop;

        [Rename("")] public bool mute;

        [Rename("")] public float pitch;

        [Rename("")] public int priority;

        [Rename("")] public float spatialBlend;

        [Rename("")] public float panStereo;

        [Rename("")] public float volume;

        public override void Process(GalCore galCore){
            if (field.Contains(nameof(audioClip))) audioSource.clip = audioClip;
            if (field.Contains(nameof(loop))) audioSource.loop = loop;
            if (field.Contains(nameof(mute))) audioSource.mute = mute;
            if (field.Contains(nameof(bypassEffects))) audioSource.bypassEffects = bypassEffects;
            if (field.Contains(nameof(volume))) audioSource.volume = volume;
            if (field.Contains(nameof(priority))) audioSource.priority = priority;
            if (field.Contains(nameof(pitch))) audioSource.pitch = pitch;
            if (field.Contains(nameof(panStereo))) audioSource.panStereo = panStereo;
            if (field.Contains(nameof(spatialBlend))) audioSource.spatialBlend = spatialBlend;
        }
    }

    [Serializable]
    public abstract class GalConfig : IGalConfig{
        [SerializeReference] [SerializeField] protected List<GfuPortAsset> ports = new List<GfuPortAsset>();

        [SerializeField] protected List<string> field = new List<string>();

        public void AddPort(string key){
            ports.Add(new GfuPortAsset{
                portName = key
            });
        }

        public void AddPort(GfuPortAsset port){ ports.Add(port); }

        public void AddField(string key){ field.Add(key); }

        public void Clear(){
            ports.Clear();
            field.Clear();
        }

        public List<GfuPortAsset> GetPort(){ return ports; }

        public List<string> GetField(){ return field; }

        public abstract void Process(GalCore galCore);
    }

    public interface IGalConfig : IGalBlock{
        void AddPort(string key);
        void AddPort(GfuPortAsset port);
        void AddField(string key);
        void Clear();
        List<GfuPortAsset> GetPort();
        List<string> GetField();
    }

    public interface IGalBlock{
        public void Process(GalCore galCore);
    }
}