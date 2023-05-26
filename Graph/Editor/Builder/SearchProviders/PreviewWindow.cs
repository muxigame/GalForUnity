using System;
using System.Collections.Generic;
using GalForUnity.Core;
using GalForUnity.Core.Block;
using GalForUnity.Core.External;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Pose = GalForUnity.Core.Pose;

namespace GalForUnity.Graph.Editor.Builder.SearchProviders
{
    public class PreviewData
    {
        public Anchor Anchor;
        public Pose pose;
        public AnchorSprite AnchorSprite;
        public PoseLocation poseLocation;
    }

    public class PreviewWindow : EditorWindow
    {
        private static PreviewWindow _instance;
        private readonly Vector2 _previewSize = new(300, 300);

        private readonly List<PreviewData> _previewData = new();
        private VisualElement _image;
        private bool _isOpened;
        private float _scale = 2;
        private Vector2 _showSpriteSize;
        private Sprite _sprite;
        private float _maxScale = 300;
        private float _minScale = 30;
        private Rect _previewRect;
        private static float _realRatio;

        private void CreateGUI()
        {
            _isOpened = true;
        }

        private void OnDisable()
        {
            _isOpened = false;
        }

        [MenuItem("PreviewWindow/test")]
        public static void Test()
        {
            Sprite alice = (Sprite)Resources.Load("Alice");
            Show(Resources.Load<Sprite>("Textures/poseDefault"), new Vector2(500, 500));
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }

        public static bool IsOpen()
        {
            return HasInstance() && _instance._isOpened;
        }

        private void Init(Sprite sprite, Vector2 showSpriteSize)
        {
            _sprite = sprite;
            _showSpriteSize = showSpriteSize;
            rootVisualElement.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 1));
            _image = new VisualElement
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

            rootVisualElement.Add(_image);
            var label = new Label
            {
                style =
                {
                    color = new StyleColor(new Color(0, 0, 0, 1)),
                    fontSize = 15
                },
                text = $"current texture filter mode:{sprite.texture.filterMode}"
            };
            rootVisualElement.Add(label);
            _image.RegisterCallback<WheelEvent>(x =>
            {
                if (x.delta.y > 0) _scale = Mathf.MoveTowards(_scale, _maxScale, 10f);
                else if (x.delta.y < 0) _scale = Mathf.MoveTowards(_scale, _minScale, 10f);
                _previewRect.size = new Vector2(_scale, _scale);
                _previewRect.center = x.localMousePosition * _realRatio;
                _previewRect = EdgeProcess(_previewRect);
                UpdateScalePreview();
            });
            _image.RegisterCallback<MouseMoveEvent>(x =>
            {
                _previewRect.center = x.localMousePosition * _realRatio;
                _previewRect = EdgeProcess(_previewRect);
                UpdateScalePreview();
            });
            _image.style.backgroundImage = new StyleBackground(sprite);
            _maxScale = Mathf.Min(Mathf.Min(_previewSize.x, sprite.rect.width), Mathf.Min(_previewSize.y, sprite.rect.height));
            _scale = _maxScale / 2;
            _previewRect = new Rect(0, 0, _scale, _scale);
        }

        private Rect EdgeProcess(Rect rect)
        {
            var min = Mathf.Min(_sprite.rect.width, _sprite.rect.height);
            if (rect.width > min || rect.height > min)
            {
                rect.width = min;
                rect.height = min;
            }

            if (rect.x < 0) rect.x = 0;

            if (rect.y < 0) rect.y = 0;

            if (rect.x + rect.width > _sprite.rect.width)
            {
                var rectWidth = _sprite.rect.width - rect.width;
                rect.x = rectWidth;
            }

            if (rect.y + rect.height > _sprite.rect.height)
            {
                var rectHeight = _sprite.rect.height - rect.height;
                rect.y = rectHeight;
            }

            return rect;
        }

        private void UpdateScalePreview()
        {
            if (ElementScalePreviewWindow.IsOpen()) ElementScalePreviewWindow.SetPreview(ConvertToTextureSpace(_previewRect));
            else ElementScalePreviewWindow.Show(CreateRealSprite(),position.position + new Vector2(position.width, 0), _previewSize,ConvertToTextureSpace(_previewRect));
        }

        private Rect ConvertToTextureSpace(Rect rect)
        {
            rect.y = _sprite.rect.height - rect.height - rect.y;
            return rect;
        }

        public static bool Show(Sprite sprite, Vector2 position)
        {
            if (!sprite) return false;
            var objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(PreviewWindow));
            if (objectsOfTypeAll.Length != 0)
                try
                {
                    ((EditorWindow)objectsOfTypeAll[0])?.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    _instance = null;
                }

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
                _realRatio = 1;
                if (spriteWidth > maxWidth)
                {
                    _realRatio = spriteWidth / maxWidth;
                    spriteHeight /= _realRatio;
                    spriteWidth = maxWidth;
                }


                var spriteShowSize = new Vector2(spriteWidth, spriteHeight);
                var buttonRect = new Rect(position.x, position.y, 0, 0);
                _instance.Init(sprite, spriteShowSize);
                _instance.ShowAsDropDown(buttonRect, spriteShowSize);
                return true;
            }

            return false;
        }

        public static void SetPreview(Sprite sprite)
        {
            if (_instance && _instance._image != null)
            {
                _instance._image.style.backgroundImage = new StyleBackground(sprite);
                _instance._previewData.Clear();
                _instance._image.Clear();
            }
        }

        private Sprite CreateRealSprite()
        {
            var realImage = CopyTexture2D(_sprite.texture);
            foreach (var previewData in _instance._previewData)
            {
                var bindingPoint = previewData.Anchor;
                var faceSprite = CopySprite(previewData.AnchorSprite.sprite);
                var poseSprite = ((SpritePose)previewData.pose).sprite;
                var startX = (int)(bindingPoint.pivot.x * poseSprite.rect.width - faceSprite.pivot.x);
                var startY = (int)(bindingPoint.pivot.y * poseSprite.rect.height - faceSprite.pivot.y);
                for (var x = (int)faceSprite.rect.x; x < faceSprite.rect.width; x++)
                {
                    for (var y = (int)faceSprite.rect.y; y < faceSprite.rect.height; y++)
                    {
                        realImage.SetPixel(startX + x, startY + y, faceSprite.texture.GetPixel(x, y));
                    }
                }
            }

            realImage.Apply();
            var copySprite = CopySprite(_sprite, realImage);
            return copySprite;
        }

        private Texture2D CopyTexture2D(Texture2D texture2D)
        {
            RenderTexture tmp = RenderTexture.GetTemporary( 
                texture2D.width,
                texture2D.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);
            
            Graphics.Blit(texture2D, tmp);

            var tempTexture = new Texture2D(texture2D.width, texture2D.height)
            {
                filterMode = FilterMode.Point
            };
            
            RenderTexture previous = RenderTexture.active;
            try
            {
                RenderTexture.active = tmp;
                tempTexture.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0);
                tempTexture.Apply();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                RenderTexture.active = previous;
            }
            
            return tempTexture;
        }
        private Sprite CopySprite(Sprite sprite, Texture2D requestTexture = null)
        {
            
            if (requestTexture)
            {
                return Sprite.Create(requestTexture, sprite.rect, sprite.GetPointPivot(), sprite.pixelsPerUnit);
            }
            return Sprite.Create(CopyTexture2D(sprite.texture), sprite.rect, sprite.GetPointPivot(), sprite.pixelsPerUnit);
        }

        public static void AddBindPointImage(PreviewData previewData)
        {
            if (_instance && _instance._image != null)
            {
                var sprite = previewData.AnchorSprite.sprite;
                var bindingPoint = previewData.Anchor;
                var poseSprite = ((SpritePose)previewData.pose).sprite;
                var startX = (int)(bindingPoint.pivot.x * poseSprite.rect.width);
                var startY = (int)((1 - bindingPoint.pivot.y) * poseSprite.rect.height);
                _instance._previewData.Add(previewData);
                _instance._image.Add(new VisualElement
                {
                    style =
                    {
                        left = (startX - sprite.pivot.x) / _realRatio,
                        top = (startY - sprite.pivot.y) / _realRatio,
                        position = Position.Absolute,
                        width = sprite.rect.width / _realRatio,
                        height = sprite.rect.height / _realRatio,
                        backgroundImage = new StyleBackground(sprite),
                    }
                });
            }
        }
    }

    internal class SpriteScalePreviewWindow : EditorWindow
    {
        private static SpriteScalePreviewWindow _instance;
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
            if (!sprite) return false;
            var objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(SpriteScalePreviewWindow));
            if (objectsOfTypeAll.Length != 0)
                try
                {
                    var editorWindow = ((EditorWindow)objectsOfTypeAll[0]);
                    if(editorWindow) editorWindow.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    _instance = null;
                }

            if (_instance == null)
            {
                _instance = CreateInstance<SpriteScalePreviewWindow>();
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

    internal class ElementScalePreviewWindow : EditorWindow
    {
        private static ElementScalePreviewWindow _instance;
        private static VisualElement _image;
        private bool _isOpened;
        private Sprite _sprite;

        private void CreateGUI()
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

        public static bool Show(Sprite sprite, Vector2 position, Vector2 size, Rect rect)
        {
            if (HasOpenInstances<ElementScalePreviewWindow>())
            {
                if (_instance) _instance.Close();
                else
                {
                    var objectsOfTypeAll = Resources.FindObjectsOfTypeAll<ElementScalePreviewWindow>();
                    if (objectsOfTypeAll.Length != 0)
                        try
                        {
                            var editorWindow = ((ElementScalePreviewWindow)objectsOfTypeAll[0]);
                            if (editorWindow && editorWindow._isOpened) editorWindow.Close();
                        }
                        catch (Exception e)
                        {
                            _instance = null;
                            Debug.LogError(e);
                        }
                }
            }


            if (_instance == null)
            {
                _instance = CreateInstance<ElementScalePreviewWindow>();
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
                _instance._sprite = sprite;
                _instance.rootVisualElement.Add(_image);
                _image.style.borderBottomColor
                    = _image.style.borderTopColor
                        = _image.style.borderLeftColor
                            = _image.style.borderRightColor = new StyleColor(new Color(0, 0, 0, 1f));
                _image.style.borderTopWidth
                    = _image.style.borderLeftWidth
                        = _image.style.borderRightWidth
                            = _image.style.borderBottomWidth = 2;
                _image.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 1));
                SetPreview(rect);
                _instance.ShowAsDropDown(buttonRect, size);
                return true;
            }

            return false;
        }


        public static void SetPreview(Rect rect)
        {
            if (_instance && _image != null)
            {
                var sprite = Sprite.Create(_instance._sprite.texture, rect,  _instance._sprite.GetPointPivot(), _instance._sprite.pixelsPerUnit);
                _image.style.backgroundImage = new StyleBackground(sprite);
            }
        }
    }
}