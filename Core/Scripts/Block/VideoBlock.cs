

using System;
using System.Linq;
using System.Threading.Tasks;
using GalForUnity.Core.Editor.Attributes;
using UnityEngine.Video;

namespace GalForUnity.Core.Block{
    [Serializable]
    public class VideoBlock : GalBlock{
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

        public override Task Process(GalCore galCore){
            if (!videoPlayer) videoPlayer = galCore.mainVideoPlayer;
            if (field.Contains(nameof(videoClip))) videoPlayer.clip = videoClip;
            if (field.Contains(nameof(playOnAwake))) videoPlayer.playOnAwake = playOnAwake;
            if (field.Contains(nameof(waitForFirstFrame))) videoPlayer.waitForFirstFrame = waitForFirstFrame;
            if (field.Contains(nameof(isLooping))) videoPlayer.isLooping = isLooping;
            if (field.Contains(nameof(skipOnDrop))) videoPlayer.skipOnDrop = skipOnDrop;
            if (field.Contains(nameof(playbackSpeed))) videoPlayer.playbackSpeed = playbackSpeed;
            if (field.Contains(nameof(aspectRatio))) videoPlayer.aspectRatio = aspectRatio;
            if (field.Contains(nameof(audioOutputMode))) videoPlayer.audioOutputMode = audioOutputMode;

            GalSynchronizationContextUtility.AsyncStart(() => GetPort().Select(x => {
                bool portOver = false;
                switch (x.portName){
                    case nameof(videoClip):
                        (videoPlayer.clip, portOver) = x.GetValueIfExist<VideoClip>();
                        break;
                    case nameof(playOnAwake):
                        (videoPlayer.playOnAwake, portOver) = x.GetValueIfExist<bool>();
                        break;
                    case nameof(waitForFirstFrame):
                        (videoPlayer.waitForFirstFrame, portOver) = x.GetValueIfExist<bool>();
                        break;
                    case nameof(isLooping):
                        (videoPlayer.isLooping, portOver) = x.GetValueIfExist<bool>();
                        break;
                    case nameof(skipOnDrop):
                        (videoPlayer.skipOnDrop, portOver) = x.GetValueIfExist<bool>();
                        break;
                    case nameof(playbackSpeed):
                        (videoPlayer.playbackSpeed, portOver) = x.GetValueIfExist<float>();
                        break;
                    case nameof(aspectRatio):
                        (videoPlayer.aspectRatio, portOver) = x.GetValueIfExist<VideoAspectRatio>();
                        break;
                    case nameof(audioOutputMode):
                        (videoPlayer.audioOutputMode, portOver) = x.GetValueIfExist<VideoAudioOutputMode>();
                        break;
                }
                return portOver;
            }).All(x => x));
            return Task.CompletedTask;
        }
    }
}