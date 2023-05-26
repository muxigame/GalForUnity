using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.Graph.Editor.Block;
using GalForUnity.Graph.Editor.Builder.SearchProviders;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor
{
    public class PreviewSearchWindow : EditorWindow
    {
        private static PreviewSearchWindow _instance;
        private TreeWrapper _currentEntry;
        private int _currentLevel;
        private SearchTreeEntry _currentSearchTreeEntry;
        private List<TreeWrapper> _list;
        private ListView _listView;
        private VisualTreeAsset _previewSearchWindow;
        private ToolbarSearchField _searchField;
        private Button _backButton;
        private SearchWindowContext _searchWindowContext;

        private IPreviewSearchWindowProvider _searchWindowProvider;
        private VisualTreeAsset _templateContainer;
        private TreeWrapper _top;

        private void Init(SearchWindowContext context, IPreviewSearchWindowProvider provider)
        {
            var vector2 = new Vector2(Math.Max(context.requestedWidth, 300), Math.Max(420, context.requestedHeight));
            var buttonRect = new Rect(context.screenMousePosition.x - vector2.x / 2f, context.screenMousePosition.y - 16f, vector2.x, 1f);
            _searchWindowProvider = provider;
            var providers = _searchWindowProvider.CreateSearchTree(_searchWindowContext);
            var parents = new Stack<TreeWrapper>();
            _list = new List<TreeWrapper>();
            _top = new TreeWrapper();
            parents.Push(_top);
            TreeWrapper current = null;
            var currentLevel = -1;
            foreach (var searchTreeEntry in providers)
            {
                if (searchTreeEntry.level <= 0) continue;
                var treeWrapper = new TreeWrapper();

                //build tree
                if (currentLevel < searchTreeEntry.level)
                {
                    current = parents.Peek();
                    current.Child ??= new List<TreeWrapper>();
                    current.Child.Add(treeWrapper);
                    treeWrapper.Parent = current;
                    parents.Push(treeWrapper);
                }
                else if (currentLevel > searchTreeEntry.level)
                {
                    if (parents.Count != 1) parents.Pop();
                    current = parents.Peek();
                    current.Child.Add(treeWrapper);
                    treeWrapper.Parent = current;
                }
                else
                {
                    Debug.Assert(current != null);
                    current.Child.Add(treeWrapper);
                    treeWrapper.Parent = current;
                }

                currentLevel = searchTreeEntry.level;

                //Init data
                treeWrapper.Entry = searchTreeEntry;
                _list.Add(treeWrapper);
            }
            while (parents.Count != 0) _currentEntry = parents.Pop();
            
            _templateContainer = UxmlHandler.instance.searchProviderItem;
            _previewSearchWindow = UxmlHandler.instance.previewSearchWindow;
            var previewWindowElement = _previewSearchWindow.Instantiate();
            rootVisualElement.Add(previewWindowElement);
            previewWindowElement.style.flexGrow = 1;
            previewWindowElement.styleSheets.Add(UxmlHandler.instance.previewSearchWindowUss);
            _listView = previewWindowElement.Q<ListView>("Content");
            _searchField = previewWindowElement.Q<ToolbarSearchField>("SearchField");
            _backButton = previewWindowElement.Q<Button>("BackButton");
            _backButton.clickable=new Clickable(() =>
            {
                _currentEntry = _currentEntry.Parent;
                BuildUI();
            });
            _searchField.RegisterValueChangedCallback(_ => { BuildUI(); });
            BuildUI();
            
            ShowAsDropDown(buttonRect, vector2);
            Focus();
            wantsMouseMove = true;
        }

        private void BuildUI()
        {
            _listView.itemsSource?.Clear();
            _listView.fixedItemHeight = 22;
            IEnumerable<TreeWrapper> treeWrappers = null;
            if (!string.IsNullOrEmpty(_searchField.value))
                treeWrappers = _list.Where(x => x.Entry.content.text.IndexOf(_searchField.value, StringComparison.OrdinalIgnoreCase) != -1);
            else
                treeWrappers = _currentEntry.Child;
            if (_currentEntry != _top)
            {
                _backButton.visible = true;
            }
            else
            {
                _backButton.visible = false;
            }
            _listView.itemsSource = treeWrappers.ToList();
            _listView.makeItem = () =>
            {
                var visualElement = _templateContainer.Instantiate();
                var previewSearchItem = visualElement.Q<VisualElement>("PreviewSearchItem");
      
                previewSearchItem.AddManipulator(new Clickable(() =>
                {
                    var treeWrapper = (TreeWrapper)visualElement.userData;
                    if (treeWrapper.Child == null)
                    {
                        _searchWindowProvider.OnSelectEntry(treeWrapper.Entry, _searchWindowContext);
                        Close();
                        return;
                    }

                    _currentEntry = treeWrapper;
                    BuildUI();
                }));
                previewSearchItem.RegisterCallback<MouseEnterEvent>(evt =>
                {
                    var treeWrapper = (TreeWrapper)visualElement.userData;
                    _searchWindowProvider.OnMouseEnter(treeWrapper.Entry, position, evt);
                });
                previewSearchItem.RegisterCallback<MouseLeaveEvent>(evt =>
                {
                    var treeWrapper = (TreeWrapper)visualElement.userData;
                    _searchWindowProvider.OnMouseLeave(treeWrapper.Entry, position, evt);
                });
                return visualElement;
            };
            _listView.bindItem = (visualElement, index) =>
            {
                var treeWrapper = (TreeWrapper)_listView.itemsSource[index];
                visualElement.Q<Label>("ItemLabel").text = treeWrapper.Entry.content.text;
                visualElement.Q<Label>("ItemArrow").visible = treeWrapper.Child != null;
                visualElement.userData = treeWrapper;
            };
            _listView.RefreshItems();
        }

        public static bool Open<T>(SearchWindowContext context, T provider) where T : ScriptableObject, IPreviewSearchWindowProvider
        {
            var objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(SearchWindow));
            if (objectsOfTypeAll.Length != 0)
                try
                {
                    ((EditorWindow)objectsOfTypeAll[0]).Close();
                    return false;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    _instance = null;
                }

            if (_instance == null)
            {
                _instance = CreateInstance<PreviewSearchWindow>();
                _instance.hideFlags = HideFlags.HideAndDontSave;
            }

            if (_instance != null) _instance.Init(context, provider);
            return true;
        }

        private class TreeWrapper
        {
            public List<TreeWrapper> Child;
            public SearchTreeEntry Entry;
            public TreeWrapper Parent;
        }
    }
}