using System.Reflection;
using GalForUnity.Graph.Nodes.Editor.Block;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Scripts.Editor
{
    public class PoseBindingAnchor : Button
    {
        private Vector2 _value = new(0.5f, 0.5f);

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
            style.backgroundColor = new StyleColor(new Color(0.34f,0.34f,0.34f,1));
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
                    top = -(size?.y ?? sprite.texture.height) / 2f +
                          (worldBound.height - style.borderTopWidth.value) / 2f,
                    width = size?.x ?? sprite.texture.width,
                    height = size?.y ?? sprite.texture.height,
                    backgroundImage = new StyleBackground(sprite)
                }
            });
        }

        public PoseBindingAnchor() : base(() => { })
        {
            style.backgroundImage = new StyleBackground(ResourceHandler.instance.poseBindingAnchor);
            style.width = 20;
            style.height = 20;
            style.marginTop = style.marginLeft = style.marginRight = style.marginBottom = 0;
            style.overflow = Overflow.Visible;
            style.paddingTop = style.paddingLeft = style.paddingRight = style.paddingBottom = 0;
            style.translate = new StyleTranslate(new Translate(new Length(-11), new Length(-11), 0));
            style.position = Position.Absolute;
            RegisterCallback<MouseMoveEvent>(Callback, TrickleDown.TrickleDown);
            RegisterCallback<MouseUpEvent>(Up, TrickleDown.TrickleDown);
            RegisterCallback<MouseLeaveEvent>(Leave, TrickleDown.TrickleDown);
            RegisterCallback<MouseDownEvent>(Down, TrickleDown.TrickleDown);
        }

        public Vector2 Value
        {
            get => _value;
            set
            {
                _init = true;
                _value = value;
                style.left = OffsetedWidth * _value.x;
                style.top = OffsetedHeight * _value.y;
            }
        }

        public PropertyInfo ValueFieldInfo => GetType().GetProperty(nameof(Value));

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
                style.top = OffsetedHeight / 2f;
            }

            _startPoint = new Vector2(evt.mousePosition.x, evt.mousePosition.y);
            _origin = new Vector2(style.left.value.value, style.top.value.value);
        }

        private void Up(MouseUpEvent evt)
        {
            Value = new Vector2(style.left.value.value / OffsetedWidth,
                style.top.value.value / OffsetedHeight);
            _startPoint = null;
        }

        private void Callback(MouseMoveEvent evt)
        {
            if (_startPoint != null)
            {
                var offset = (Vector2)_startPoint;
                var left = _origin.x + (evt.mousePosition.x - offset.x);
                var top = _origin.y + (evt.mousePosition.y - offset.y);
                if (left < 0)
                {
                    style.left = 0;
                }
                else if (top < 0)
                {
                    style.top = 0;
                }
                else if (left > OffsetedWidth)
                {
                    style.left = OffsetedWidth;
                }
                else if (top > OffsetedHeight)
                {
                    style.top = OffsetedHeight;
                }
                else
                {
                    style.left = left;
                    style.top = top;
                }
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