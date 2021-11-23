//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  UIEvent.cs
//
//        Created by 半世癫(Roc) at 2021-01-07 17:41:14
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using GalForUnity.Model;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using NotImplementedException = System.NotImplementedException;

namespace GalForUnity.View{
    
    [Serializable]
    public class UIEventTrigger :MonoBehaviour, IEventSystemHandler{
        public bool onPointerClick;
        public bool onPointerDown;
        public bool onnPointerUp;
        public bool onPointerEnter;
        public bool onPointerExit;
        public class TriggerEvent : UnityEvent<BaseEventData>
        {}
        [Serializable]
        public class Entry
        {
            /// <summary>
            /// What type of event is the associated callback listening for.
            /// </summary>
            public EventTriggerType eventID = EventTriggerType.PointerClick;

            /// <summary>
            /// The desired TriggerEvent to be Invoked.
            /// </summary>
            public UnityEvent callback = new UnityEvent();
        }

        [SerializeField]
        private List<Entry> m_Delegates;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Please use triggers instead (UnityUpgradable) -> triggers", true)]
        public List<Entry> delegates { get { return triggers; } set { triggers = value; } }
        public List<Entry> triggers
        {
            get
            {
                if (m_Delegates == null)
                    m_Delegates = new List<Entry>();
                return m_Delegates;
            }
            set { m_Delegates = value; }
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

                
        /// <summary>
        /// 鼠标点击事件
        /// </summary>
        // /// <param name="eventData"></param>
        // public override void OnPointerClick(PointerEventData eventData){
        //     base.OnPointerClick(eventData);
        //     
        // }
        // /// <summary>
        // /// 鼠标按下事件
        // /// </summary>
        // /// <param name="eventData"></param>
        // public override void OnPointerDown(PointerEventData eventData){
        //     base.OnPointerDown(eventData);
        //     
        // }
        //
        // /// <summary>
        // /// 鼠标抬起事件
        // /// </summary>
        // /// <param name="eventData"></param>
        // public override void OnPointerUp(PointerEventData eventData){
        //     base.OnPointerUp(eventData);
        //     
        // }
        //
        // /// <summary>
        // /// 鼠标进入事件
        // /// </summary>
        // /// <param name="eventData"></param>
        // public override void OnPointerEnter(PointerEventData eventData){
        //     base.OnPointerEnter(eventData);
        //     
        // }
        //
        // /// <summary>
        // /// 鼠标离开事件
        // /// </summary>
        // /// <param name="eventData"></param>
        // public override void OnPointerExit(PointerEventData eventData){
        //     base.OnPointerExit(eventData);
        //     
        // }
        
    }
}
