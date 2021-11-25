//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ShowPlotView.cs
//
//        Created by 半世癫(Roc) at 2021-01-02 09:32:16
//
//======================================================================

using GalForUnity.Attributes;
using GalForUnity.Controller;
using GalForUnity.Model.Scene;
using GalForUnity.System;
using MUX.Support;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking.Match;
using UnityEngine.Serialization;
using UnityEngine.UI;
using EventCenter = GalForUnity.System.Event.EventCenter;

namespace GalForUnity.View{
    /// <summary>
    /// 剧情视图类，负责将剧情的内容展示出来
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ShowPlotView : GfuInstanceManagerForMono<OptionController>{
        [FormerlySerializedAs("OptionController")]
        [Rename(nameof(optionController))]
        [Tooltip("控制游戏中选项的选项控制器")]
        public OptionController optionController;
        [FormerlySerializedAs("NameView")]
        [Rename(nameof(nameView))]
        [Tooltip("请放置Text或其子类，自动创建默认视图")]
        public Text nameView;
        [FormerlySerializedAs("SpeakView")]
        [Rename(nameof(speakView))]
        [Tooltip("请放置Text或其子类，如果为空，自动创建默认视图")]
        public Text speakView;
        [Rename(nameof(backgroundDistance))]
        [Tooltip("指示背景图片距离主相机的距离")]
        public float backgroundDistance=100;
        [FormerlySerializedAs("BackgroundView")] [Rename(nameof(backgroundView))]
        [Tooltip("请放置Image或其子类，如果为空，自动创建默认视图")]
        public SpriteRenderer backgroundView;
        [FormerlySerializedAs("AudioSource")]
        [Tooltip("您可以指定视图显示的画布")]
        [Rename(nameof(audioSource))]
        public AudioSource audioSource;
        [FormerlySerializedAs("ParentCanvas")]
        [Tooltip("您可以指定视图显示的画布")]
        [Rename(nameof(parentCanvas))]
        public Canvas parentCanvas;

        private void Start(){
            InitialView();
        }

        public virtual void ShowAll(string roleName, string speak, Sprite background, AudioClip audioClip){
            ShowName(roleName);
            ShowSpeak(speak,audioClip);
            ShowBackground(background);
        }

        // ReSharper disable all MemberCanBeProtected.Global
        public virtual void ShowName(string roleName) {
            nameView.text = roleName;
        }
        public virtual void ShowSpeak(string speak, AudioClip audioClip) {
            ShowSpeak(speak);
            PlayAudioClip(audioClip);
        }

        public virtual void ShowSpeak(string speak){
            var onSpeak = EventCenter.GetInstance().OnSpeak;
            speakView.text = onSpeak != null ? onSpeak(speak) : speak;
        }

        public virtual void ShowBackground(Sprite background){
            if (ReferenceEquals(background, null)) return;
            if (ReferenceEquals(backgroundView, null)) return;
            backgroundView.sprite=background;
        }
        public virtual void ShowSceneModel(SceneModel sceneModel){
            if(!sceneModel) return;
            if(sceneModel.backgroundImage) ShowBackground(sceneModel.backgroundImage);
            if(sceneModel.backgroundAudio) PlayAudioClip(sceneModel.backgroundAudio);
        }
        public virtual void PlayAudioClip(AudioClip audioClip){
            if (ReferenceEquals(audioClip, null)) return;
            if (audioSource) return;
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        public virtual void InitialView(){
            if (parentCanvas == null){
                parentCanvas = new GameObject().AddComponent<Canvas>();
                parentCanvas.gameObject.AddComponent<CanvasScaler>();
                parentCanvas.gameObject.AddComponent<GraphicRaycaster>();
                parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            if (nameView == null || speakView == null){
                GameObject load = Resources.Load("DemoSpeak") as GameObject;
                if (parentCanvas != null){
                    load = Instantiate(load, parentCanvas.transform, true);
                    load.name = "GalForUnityCanvas";
                } else{
                    var rectTransformComponent = parentCanvas.GetComponent<RectTransform>();
                    rectTransformComponent.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
                    rectTransformComponent.sizeDelta = new Vector2(Screen.width, Screen.height);
                    load = Instantiate(load, parentCanvas.transform, true);
                    load.name = "GalForUnityCanvas";
                }
                if (!(load is null)){
                    var rectTransform = load.GetComponent<RectTransform>();
                    rectTransform.offsetMax = new Vector2(0, 0);
                    rectTransform.offsetMin = new Vector2(0, 0);
                }
                if (load != null&&!nameView){
                    nameView = load.transform.GetChild(0).Find("Name").GetComponent<Text>();
                }
                if (load != null&&!speakView){
                    speakView = load.transform.GetChild(0).Find("Speak").GetComponent<Text>();
                }
            }
            parentCanvas.transform.SetParent(transform);
            if (!audioSource){
                if (!TryGetComponent(out audioSource)){
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
            if (!backgroundView){
                GameObject backGroundObj = new GameObject();
                backgroundView = backGroundObj.AddComponent<SpriteRenderer>();
                // Debug.Log(Background.bounds.size);
                backgroundView.transform.SetParent(transform);
                backgroundView.gameObject.AddComponent<BackgroundAutoSize>();
                backGroundObj.name = "Background";
            }

            backgroundView.transform.forward = Camera.main.transform.forward;
            backgroundView.transform.position = Camera.main.transform.position + Camera.main.transform.forward * backgroundDistance;
            
            var optionController = InitialSystemComponent<OptionController>();
            optionController.transform.SetParent(parentCanvas.transform);
            if (FindObjectOfType<EventSystem>() == null){
                gameObject.AddComponent<EventSystem>();
                gameObject.AddComponent<StandaloneInputModule>();
            }
            GameSystem.Data.OptionController = this.optionController = optionController;
        }

        /// <summary>
        /// 初始化系统组件，对场景内的全部组件遍历，将需要的组件移动当当前gameObject下(层级而非游戏中)
        /// </summary>
        /// <typeparam name="T">需要寻找的类型</typeparam>
        /// <returns>返回找到的第一个组件</returns>
        public T InitialSystemComponent<T>() where T : Component{
            T typeObj;
            if ((typeObj = GameObject.FindObjectOfType<T>()) == null){
                GameObject obj = new GameObject();
                obj.transform.parent = transform;
                obj.name = typeof(T).Name;
                return obj.AddComponent<T>();
            }
            return typeObj;
        }
    }
}
