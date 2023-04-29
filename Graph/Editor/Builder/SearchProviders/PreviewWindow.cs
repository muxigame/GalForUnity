using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Builder.SearchProviders
{
    public class PreviewWindow : EditorWindow
    {
        private const float MaxScale = 10;
        private const float MinScale = 1;
        private static PreviewWindow _instance;
        private readonly Vector2 _previewSize = new(300, 300);
        private float _scale = 2;
        private Vector2 _showSpriteSize;
        private Sprite _sprite;

        [MenuItem("PreviewWindow/test")]
        public static void Test()
        {
            Show(Resources.Load<Sprite>("Textures/poseDefault"), new Vector2(500, 500));
        }

        private void Init(Sprite sprite, Vector2 showSpriteSize)
        {
            _sprite = sprite;
            _showSpriteSize = showSpriteSize;
            rootVisualElement.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 1));
            var child = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    paddingBottom = 0,
                    paddingLeft = 0,
                    paddingTop = 0,
                    marginRight = 0,
                    marginTop = 0,
                    marginLeft = 0,
                    marginBottom = 0
                }
            };
            rootVisualElement.Add(new Label
            {
                style =
                {
                  
                    color = new StyleColor(new Color(0, 0, 0, 1)),
                    fontSize = 15
                },
                text = $"current texture filter mode:{sprite.texture.filterMode}"
            });
            rootVisualElement.Add(child);
            child.RegisterCallback<WheelEvent>(x =>
            {
                if (x.delta.y > 0) _scale = Mathf.MoveTowards(_scale, MinScale, 0.5f);
                else if (x.delta.y < 0) _scale = Mathf.MoveTowards(_scale, MaxScale, 0.5f);
                UpdateScalePreview(x.localMousePosition);
            });
            child.RegisterCallback<MouseMoveEvent>(x => { UpdateScalePreview(x.localMousePosition); });
            child.style.backgroundImage = new StyleBackground(sprite);
        }

        private void UpdateScalePreview(Vector2 mousePosition)
        {
            var scaleSize = _previewSize / _scale;
            var rect = new Rect(
                Math.Min(mousePosition.x / _showSpriteSize.x * _sprite.rect.width - scaleSize.x / 2f, _sprite.rect.width - scaleSize.x),
                Math.Min((1 - mousePosition.y / _showSpriteSize.y) * _sprite.rect.height - scaleSize.y / 2f, _sprite.rect.height - scaleSize.y),
                scaleSize.x,
                scaleSize.y);
            var scaleSprite = Sprite.Create(_sprite.texture, rect, new Vector2(0, 0), _sprite.pixelsPerUnit);
            if (ScalePreviewWindow.IsOpen()) ScalePreviewWindow.SetPreview(scaleSprite);
            else ScalePreviewWindow.Show(scaleSprite, position.position + new Vector2(position.width, 0), _previewSize);
        }

        public static bool Show(Sprite sprite, Vector2 position)
        {
            var objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(PreviewWindow));
            if (objectsOfTypeAll.Length != 0)
                try
                {
                    ((EditorWindow)objectsOfTypeAll[0]).Close();
                    return false;
                }
                catch (Exception ex)
                {
                    _instance = null;
                }

            Debug.Log(_instance);
            if (_instance == null)
            {
                _instance = CreateInstance<PreviewWindow>();
                _instance.hideFlags = HideFlags.HideAndDontSave;
            }


            if (_instance != null)
            {
                var spriteWidth = sprite.rect.width;
                var spriteHeight = sprite.rect.height;
                var maxWidth = 300;
                float scale = 1;
                if (spriteWidth > maxWidth)
                {
                    scale = spriteHeight / maxWidth;
                    spriteHeight *= scale;
                    spriteWidth = maxWidth;
                }


                var spriteShowSize = new Vector2(spriteWidth, spriteHeight);
                var buttonRect = new Rect(position.x - spriteShowSize.x / 2f, position.y, 0, 0);
                _instance.Init(sprite, spriteShowSize);
                _instance.ShowAsDropDown(buttonRect, spriteShowSize);
                return true;
            }

            return false;
        }
    }

    internal class ScalePreviewWindow : EditorWindow
    {
        private static ScalePreviewWindow _instance;
        private static VisualElement _image;
        private bool _isOpened;

        private void OnEnable()
        {
            _isOpened = true;
        }

        private void OnDisable()
        {
            _isOpened = false;
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }

        public static bool IsOpen()
        {
            return HasInstance() && _instance._isOpened;
        }

        public static bool Show(Sprite sprite, Vector2 position, Vector2 size)
        {
            var objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(ScalePreviewWindow));
            if (objectsOfTypeAll.Length != 0)
                try
                {
                    ((EditorWindow)objectsOfTypeAll[0]).Close();
                    return false;
                }
                catch (Exception ex)
                {
                    _instance = null;
                }

            if (_instance == null)
            {
                _instance = CreateInstance<ScalePreviewWindow>();
                _instance.hideFlags = HideFlags.HideAndDontSave;
            }


            if (_instance != null)
            {
                var buttonRect = new Rect(position.x, position.y, 0, 0);
                _image = new VisualElement
                {
                    style =
                    {
                        flexGrow = 1,
                        flexShrink = 1
                    }
                };
                _instance.rootVisualElement.Add(_image);
                _image.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 1));
                _image.style.backgroundImage = new StyleBackground(sprite);
                _image.style.borderBottomColor
                    = _image.style.borderTopColor
                        = _image.style.borderLeftColor
                            = _image.style.borderRightColor = new StyleColor(new Color(0, 0, 0, 1f));
                _image.style.borderTopWidth
                    = _image.style.borderLeftWidth
                        = _image.style.borderRightWidth
                            = _image.style.borderBottomWidth = 2;
                _instance.ShowAsDropDown(buttonRect, size);
                return true;
            }

            return false;
        }

        public static void SetPreview(Sprite sprite)
        {
            if (_instance && _image != null) _image.style.backgroundImage = new StyleBackground(sprite);
        }
    }
}