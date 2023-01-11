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
using GalForUnity.Core;
using UnityEngine.Video;

namespace GalForUnity.Graph.Block.Config{
    [Serializable]
    [NodeEditor(typeof(PlotVideoBlockEditorUxml))]
    public class GalVideoConfig : GalConfig{
        public VideoClip videoClip;
        public VideoPlayer videoPlayer;
        [Rename("")] public bool playOnAwake;
        [Rename("")] public bool waitForFirstFrame;
        [Rename("")] public bool isLooping;
        [Rename("")] public bool skipOnDrop;
        [Rename("")] public float playbackSpeed;
        [Rename("")] public VideoAspectRatio aspectRatio;
        [Rename("")] public VideoAudioOutputMode audioOutputMode;
        [Rename("")] public float volume;

        public override void Process(GalCore galCore){
            if (field.Contains(nameof(videoClip))) videoPlayer.clip = videoClip;
            if (field.Contains(nameof(playOnAwake))) videoPlayer.playOnAwake = playOnAwake;
            if (field.Contains(nameof(waitForFirstFrame))) videoPlayer.waitForFirstFrame = waitForFirstFrame;
            if (field.Contains(nameof(isLooping))) videoPlayer.isLooping = isLooping;
            if (field.Contains(nameof(skipOnDrop))) videoPlayer.skipOnDrop = skipOnDrop;
            if (field.Contains(nameof(playbackSpeed))) videoPlayer.playbackSpeed = (int) playbackSpeed;
            if (field.Contains(nameof(aspectRatio))) videoPlayer.aspectRatio = aspectRatio;
            if (field.Contains(nameof(audioOutputMode))) videoPlayer.audioOutputMode = audioOutputMode;
        }
    }
}