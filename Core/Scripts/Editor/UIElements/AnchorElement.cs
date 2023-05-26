using System.Collections.Generic;
using GalForUnity.Graph.Editor.Block;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor
{
    public class AnchorElement : Button, INotifyValueChanged<Vector2>
    {
        private static readonly float ParentBorderOffset = 2f;
        private bool m_Init;
        private Vector2 m_Origin;
        private Vector2? m_StartPoint;
        private Vector2 m_Value = new(0.5f, 0.5f);

        public AnchorElement() : base(() => { })
        {
        }

        public AnchorElement(Vector2 point) : base(() => { })
        {
            m_Value = point;
            style.backgroundImage = new StyleBackground(ResourceHandler.instance.poseBindingAnchor);

            style.width = 16;
            style.height = 16;
            style.marginTop = style.marginLeft = style.marginRight = style.marginBottom = 0;
            style.overflow = Overflow.Visible;
            style.paddingTop = style.paddingLeft = style.paddingRight = style.paddingBottom = 0;
            style.borderBottomWidth = style.borderRightWidth = style.borderLeftWidth = style.borderTopWidth = 0;
            style.translate = new StyleTranslate(new Translate(new Length(-8), new Length(8), 0));
            style.position = Position.Absolute;
            RegisterCallback<MouseMoveEvent>(Callback, TrickleDown.TrickleDown);
            RegisterCallback<MouseUpEvent>(Up, TrickleDown.TrickleDown);
            RegisterCallback<MouseLeaveEvent>(Leave, TrickleDown.TrickleDown);
            RegisterCallback<MouseDownEvent>(Down, TrickleDown.TrickleDown);
        }

        private float OffsetedWidth => parent.worldBound.width - ParentBorderOffset;
        private float OffsetedHeight => parent.worldBound.height - ParentBorderOffset;
        public VisualElement Preview { get; private set; }

        public void SetValueWithoutNotify(Vector2 newValue)
        {
            m_Value = newValue;
            style.left = OffsetedWidth * m_Value.x;
            style.bottom = OffsetedHeight * m_Value.y;
        }

        public Vector2 value
        {
            get => m_Value;
            set
            {
                if (EqualityComparer<Vector2>.Default.Equals(m_Value, value))
                    return;
                if (panel != null)
                    using (var pooled = ChangeEvent<Vector2>.GetPooled(m_Value, value))
                    {
                        pooled.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(pooled);
                    }
                else
                    SetValueWithoutNotify(value);

                m_Init = true;
            }
        }

        public void HideAnchor()
        {
            style.unityBackgroundImageTintColor = new StyleColor(Color.clear);
            style.backgroundColor = new StyleColor(Color.clear);
        }

        public void ShowAnchor()
        {
            style.unityBackgroundImageTintColor = new StyleColor(Color.white);
            style.backgroundColor = Color.clear;
        }

        public void HidePreview()
        {
            ShowAnchor();
            contentContainer.Clear();
        }

        public void ShowPreview(Sprite sprite, Vector2? size = null)
        {
            HideAnchor();
            contentContainer.Add(Preview = new VisualElement
            {
                style =
                {
                    position = Position.Absolute,
                    left = -(size?.x ?? sprite.texture.width) / 2f +
                           (worldBound.width - style.borderLeftWidth.value) / 2f,
                    bottom = -(size?.y ?? sprite.texture.height) / 2f +
                             (worldBound.height - style.borderBottomWidth.value) / 2f,
                    width = size?.x ?? sprite.texture.width,
                    height = size?.y ?? sprite.texture.height,
                    backgroundImage = new StyleBackground(sprite)
                }
            });
        }

        private void Leave(MouseLeaveEvent evt)
        {
            // startPoint = null;
        }

        private void Down(MouseDownEvent evt)
        {
            if (!m_Init)
            {
                m_Init = true;
                style.left = OffsetedWidth / 2f;
                style.bottom = OffsetedHeight / 2f;
            }

            m_StartPoint = new Vector2(evt.mousePosition.x, evt.mousePosition.y);
            m_Origin = new Vector2(value.x * OffsetedWidth, value.y * OffsetedHeight);
        }

        private void Up(MouseUpEvent evt)
        {
            value = new Vector2(style.left.value.value / OffsetedWidth,
                style.bottom.value.value / OffsetedHeight);
            m_StartPoint = null;
        }

        private void Callback(MouseMoveEvent evt)
        {
            if (m_StartPoint != null)
            {
                var offset = (Vector2)m_StartPoint;
                var left = m_Origin.x + (evt.mousePosition.x - offset.x);
                var bottom = m_Origin.y - (evt.mousePosition.y - offset.y);
                if (left < 0)
                {
                    style.left = 0;
                }
                else if (bottom < 0)
                {
                    style.bottom = 0;
                }
                else if (left > OffsetedWidth)
                {
                    style.left = OffsetedWidth;
                }
                else if (bottom > OffsetedHeight)
                {
                    style.bottom = OffsetedHeight;
                }
                else
                {
                    style.left = left;
                    style.bottom = bottom;
                }

                Debug.Log(style.bottom);
            }
        }

        ~AnchorElement()
        {
            UnregisterCallback<MouseMoveEvent>(Callback);
            UnregisterCallback<MouseUpEvent>(Up);
            UnregisterCallback<MouseLeaveEvent>(Leave);
            UnregisterCallback<MouseDownEvent>(Down);
        }

        public class PoseBindingAnchorUxmlFactory : UxmlFactory<AnchorElement, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualElement = base.Create(bag, cc);
                return visualElement;
            }
        }
    }
}