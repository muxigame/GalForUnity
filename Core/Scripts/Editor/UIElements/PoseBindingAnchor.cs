using System.Collections.Generic;
using GalForUnity.Graph.Editor.Block;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor.UIElements
{
    public class PoseBindingAnchor : Button,INotifyValueChanged<Vector2>
    {
        private Vector2 _value = new Vector2(0.5f, 0.5f);

        private static readonly float ParentBorderOffset = 2f;
        private float OffsetedWidth=>parent.worldBound.width-ParentBorderOffset;
        private float OffsetedHeight=>parent.worldBound.height-ParentBorderOffset;
        private bool _init;
        private Vector2 _origin;
        private Vector2? _startPoint;
        public VisualElement Preview { get; private set; }

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
            contentContainer.Add(Preview = new VisualElement()
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
        public PoseBindingAnchor(): base(() => { }){}
        public PoseBindingAnchor(Vector2 point) : base(() => { })
        {
            _value = point;
            style.backgroundImage = new StyleBackground(ResourceHandler.instance.poseBindingAnchor);

            style.width = 20;
            style.height = 20;
            style.marginTop = style.marginLeft = style.marginRight = style.marginBottom = 0;
            style.overflow = Overflow.Visible;
            style.paddingTop = style.paddingLeft = style.paddingRight = style.paddingBottom = 0;
            style.borderBottomWidth = style.borderRightWidth = style.borderLeftWidth = style.borderTopWidth = 0;
            style.translate = new StyleTranslate(new Translate(new Length(-11), new Length(11), 0));
            style.position = Position.Absolute;
            RegisterCallback<MouseMoveEvent>(Callback, TrickleDown.TrickleDown);
            RegisterCallback<MouseUpEvent>(Up, TrickleDown.TrickleDown);
            RegisterCallback<MouseLeaveEvent>(Leave, TrickleDown.TrickleDown);
            RegisterCallback<MouseDownEvent>(Down, TrickleDown.TrickleDown);
        }

        public void SetValueWithoutNotify(Vector2 newValue)
        {
            _value = newValue;
            style.left = OffsetedWidth * _value.x;
            style.bottom = OffsetedHeight * _value.y;
        }

        public Vector2 value
        {
            get => _value;
            set
            {
                if (EqualityComparer<Vector2>.Default.Equals(_value, value))
                    return;
                if (panel != null)
                    using (var pooled = ChangeEvent<Vector2>.GetPooled(_value, value))
                    {
                        pooled.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(pooled);
                    }
                else
                    SetValueWithoutNotify(value);
                _init = true;
            }
        }

        private void Leave(MouseLeaveEvent evt)
        {
            // startPoint = null;
        }

        private void Down(MouseDownEvent evt)
        {
            if (!_init)
            {
                _init = true;
                style.left = OffsetedWidth / 2f;
                style.bottom = OffsetedHeight / 2f;
            }

            _startPoint = new Vector2(evt.mousePosition.x, evt.mousePosition.y);
            _origin = new Vector2(value.x*OffsetedWidth, value.y*OffsetedHeight);
        }

        private void Up(MouseUpEvent evt)
        {
            value = new Vector2(style.left.value.value / OffsetedWidth,
                style.bottom.value.value / OffsetedHeight);
            _startPoint = null;
        }

        private void Callback(MouseMoveEvent evt)
        {
            if (_startPoint != null)
            {
                var offset = (Vector2)_startPoint;
                var left = _origin.x + (evt.mousePosition.x - offset.x);
                var bottom = _origin.y - (evt.mousePosition.y - offset.y);
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

        ~PoseBindingAnchor()
        {
            UnregisterCallback<MouseMoveEvent>(Callback);
            UnregisterCallback<MouseUpEvent>(Up);
            UnregisterCallback<MouseLeaveEvent>(Leave);
            UnregisterCallback<MouseDownEvent>(Down);
        }

        public class PoseBindingAnchorUxmlFactory : UxmlFactory<PoseBindingAnchor, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualElement = base.Create(bag, cc);
                return visualElement;
            }
        }
    }
}