//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalVideoConfig.cs 2022-10-11 21:13:42
//
//======================================================================

using System;
using GalForUnity.Attributes;
using UnityEngine.Video;

namespace GalForUnity.Graph.Block.Config{
    [Serializable]
    [NodeEditor(typeof(PlotVideoBlockEditorUxml))]
    public class GalVideoConfig : GalConfig<VideoPlayer>{
        public VideoClip videoClip;
        [Rename("")] public bool playOnAwake;
        [Rename("")] public bool waitForFirstFrame;
        [Rename("")] public bool isLooping;
        [Rename("")] public bool skipOnDrop;
        [Rename("")] public float playbackSpeed;
        [Rename("")] public VideoAspectRatio aspectRatio;
        [Rename("")] public VideoAudioOutputMode audioOutputMode;
        [Rename("")] public float volume;

        public override void Process(VideoPlayer t){
            if (field.Contains(nameof(videoClip))) t.clip = videoClip;
            if (field.Contains(nameof(playOnAwake))) t.playOnAwake = playOnAwake;
            if (field.Contains(nameof(waitForFirstFrame))) t.waitForFirstFrame = waitForFirstFrame;
            if (field.Contains(nameof(isLooping))) t.isLooping = isLooping;
            if (field.Contains(nameof(skipOnDrop))) t.skipOnDrop = skipOnDrop;
            if (field.Contains(nameof(playbackSpeed))) t.playbackSpeed = (int) playbackSpeed;
            if (field.Contains(nameof(aspectRatio))) t.aspectRatio = aspectRatio;
            if (field.Contains(nameof(audioOutputMode))) t.audioOutputMode = audioOutputMode;
        }
    }
}