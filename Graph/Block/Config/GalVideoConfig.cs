//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalVideoConfig.cs 2022-10-11 21:13:42
//
//======================================================================

using GalForUnity.Attributes;
using UnityEngine.Video;

namespace GalForUnity.Graph.Block.Config{
    public class GalVideoConfig : IGalConfig<VideoPlayer>{
        public VideoClip videoClip;
        [Rename("")]
        public bool? playOnAwake;
        [Rename("")]
        public bool? waitForFirstFrame;
        [Rename("")]
        public bool? isLooping;
        [Rename("")]
        public bool? skipOnDrop;
        [Rename("")]
        public float? playbackSpeed;
        [Rename("")]
        public VideoAspectRatio? aspectRatio;
        [Rename("")]
        public VideoAudioOutputMode? audioOutputMode;
        [Rename("")]
        public float? volume;

        public void Process(VideoPlayer t){
            
            if (videoClip != null) t.clip = videoClip;
            if (playOnAwake != null) t.playOnAwake = (bool) playOnAwake;
            if (waitForFirstFrame != null) t.waitForFirstFrame = (bool) waitForFirstFrame;
            if (isLooping != null) t.isLooping = (bool) isLooping;
            if (skipOnDrop != null) t.skipOnDrop = (bool) skipOnDrop;
            if (playbackSpeed != null) t.playbackSpeed = (int) playbackSpeed;
            if (aspectRatio != null) t.aspectRatio = (VideoAspectRatio) aspectRatio;
            if (audioOutputMode != null) t.audioOutputMode = (VideoAudioOutputMode) audioOutputMode;
        }
    }

}
    
    