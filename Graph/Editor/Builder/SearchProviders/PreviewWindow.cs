using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PreviewWindow : EditorWindow
{
    private static PreviewWindow _instance;

    // public static void Show(VisualElement visualElement)
    // {
    //     PreviewWindow wnd = GetWindow<PreviewWindow>();
    //    
    //     wnd.rootVisualElement.Add(visualElement);
    //     var vector2 = new Vector2(Math.Max(visualElement.worldBound.width, 300), Math.Max(420, visualElement.worldBound.height));
    //     var buttonRect = new Rect(context.screenMousePosition.x - vector2.x / 2f, context.screenMousePosition.y - 16f, vector2.x, 1f);
    //     _searchWindowProvider = provider;
    //     wnd.titleContent = new GUIContent("PreviewWindow");
    // }

    [MenuItem("PreviewWindow/test")]
    public static void Test()
    {
        Show(Resources.Load<Sprite>("vbimfile"), new Vector2(500, 500));
    }

    public static bool Show(Sprite sprite, Vector2 position)
    {
        var objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(SearchWindow));
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

            _instance.rootVisualElement.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 1));
            var child = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1
                }
            };

            _instance.rootVisualElement.Add(child);
            var magnify = new VisualElement
            {
                style =
                {
                    width = 100,
                    height = 100,
                    backgroundColor = new Color(1, 0, 1, 1)
                }
            };
            child.Add(magnify);
            child.RegisterCallback<PointerMoveEvent>(x =>
            {
                var rect = new Rect((x.localPosition.x / spriteWidth) * sprite.rect.width, (spriteHeight / x.localPosition.y) * sprite.rect.height, 100, 100);
                Debug.Log(x.localPosition);
                Debug.Log(x.position);
                magnify.style.backgroundImage = new StyleBackground(Sprite.Create(sprite.texture, rect, new Vector2(0f, 0f), sprite.pixelsPerUnit));
            });

            _instance.rootVisualElement.style.backgroundImage = new StyleBackground(sprite);
            var vector2 = new Vector2(spriteWidth, spriteHeight);
            var buttonRect = new Rect(position.x - vector2.x / 2f, position.y - 16f, vector2.x, 1f);
            _instance.ShowAsDropDown(buttonRect, vector2);
            return true;
        }

        return false;
    }

    public static void SetPreview(Sprite sprite)
    {
        if (_instance) _instance.rootVisualElement.style.backgroundImage = new StyleBackground(sprite);
    }

    public class IgnoreAllEventVisualElement : VisualElement
    {
        // protected override void ExecuteDefaultAction(EventBase evt)
        // {
        //     evt.PreventDefault();
        // }
        //
        // protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        // {
        //     evt.PreventDefault();
        // }
    }
}