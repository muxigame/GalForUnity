﻿using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Graph.Editor.Block;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GalForUnity.Core.Editor.UIElements
{
    public sealed class PoseView : BindableElement, INotifyValueChanged<Sprite>
    {
        private static readonly float ParentBorderOffset = 2f;
        private readonly Label _label;
        private Sprite _value;
        public float ScaleRadio;

        public PoseView(bool canEditor)
        {
            if (canEditor)
            {
                contentContainer.Add(_label = new Label("点击更换姿势")
                {
                    style =
                    {
                        fontSize = 24,
                        color = new StyleColor(new Color(0.45f, 0.45f, 0.45f, 1))
                    }
                });
                RegisterCallback<MouseUpEvent>(x =>
                {
                    var type = Type.GetType("UnityEditor.ObjectSelector,UnityEditor");
                    var objectSelector = type.GetProperty("get", BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null, null);
                    Object selected = _value;
                    Action<Object> update = selectedObject =>
                    {
                        selected = selectedObject;
                        if (selectedObject is Sprite sprite) ShowPose(sprite);

                        if (selectedObject == null) RemovePose();
                    };
                    Action<Object> close = selectedObject =>
                    {
                        if (selectedObject is Sprite sprite) ShowPose(sprite);

                        if (selectedObject == null) RemovePose();
                    };
                    type.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                            null,
                            new Type[7]
                            {
                                typeof(Object),
                                typeof(Type[]),
                                typeof(Object),
                                typeof(bool),
                                typeof(List<int>),
                                typeof(Action<Object>),
                                typeof(Action<Object>)
                            }, null)
                        ?.Invoke(objectSelector,
                            new object[7]
                            {
                                selected, new[] { typeof(Sprite) }, selected, false, null, close , update
                            });
                });
            }

            style.borderBottomWidth =  style.borderLeftWidth =  style.borderRightWidth =  style.borderTopWidth = 2;
            style.borderBottomColor =  style.borderLeftColor =  style.borderRightColor =  style.borderTopColor = new Color(0.14f,0.14f,0.14f,1);
            style.backgroundImage = new StyleBackground(ResourceHandler.instance.defaultPose);
            style.width = 300;
            style.height = 600;
        }

        public PoseView() : this(true)
        {

        }

        private float OffsetedWidth => parent.worldBound.width - ParentBorderOffset;
        private float OffsetedHeight => parent.worldBound.height - ParentBorderOffset;

        public void SetValueWithoutNotify(Sprite newValue)
        {
            _value = newValue;
            if (_value == null)
            {
                style.backgroundImage = new StyleBackground(ResourceHandler.instance.defaultPose);
                ScaleRadio = ResourceHandler.instance.defaultPose.texture.width / 300f;
                style.height = ResourceHandler.instance.defaultPose.texture.height / ScaleRadio;
                return;
            }
            style.backgroundImage = new StyleBackground(_value);
            ScaleRadio = _value.texture.width / 300f;
            style.height = _value.texture.height / ScaleRadio;
        }

        public Sprite value
        {
            get => _value;
            set
            {
                if (EqualityComparer<Sprite>.Default.Equals(_value, value))
                    return;
                if (panel != null)
                    using (var pooled = ChangeEvent<Sprite>.GetPooled(_value, value))
                    {
                        pooled.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(pooled);
                    }
                else
                    SetValueWithoutNotify(value);
            }
        }

        public void ShowPose(Sprite sprite)
        {
            if (sprite == null)
            {
                RemovePose();
                return;
            }
            value = sprite;
            if (_label != null) _label.visible = false;
        }

        public void RemovePose()
        {
            if (_label != null) _label.visible = true;
            style.backgroundImage = new StyleBackground(ResourceHandler.instance.defaultPose);
            value = null;
            ScaleRadio = ResourceHandler.instance.defaultPose.texture.width / 300f;
            style.height = ResourceHandler.instance.defaultPose.texture.height / ScaleRadio;
        }

        public void ShowAnchor(Sprite sprite, PoseBindingAnchor poseBindingAnchor)
        {
            poseBindingAnchor.ShowPreview(sprite,
                new Vector2(sprite.texture.width / ScaleRadio, sprite.texture.height / ScaleRadio));
        }

        public void HideAnchor(PoseBindingAnchor poseBindingAnchor)
        {
            poseBindingAnchor.HidePreview();
        }

        public class PoseViewUxmlFactory : UxmlFactory<PoseView, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualElement = base.Create(bag, cc);
                return visualElement;
            }
        }
    }
}