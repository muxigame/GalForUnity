//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  OptionNode.cs
//
//        Created by 半世癫(Roc) at 2021-11-17 22:07:14
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Attributes;
using GalForUnity.Controller;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Attributes;
using GalForUnity.Model;
using GalForUnity.System;
using GalForUnity.System.Archive;
using GalForUnity.System.Event;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.AssetGraph.GFUNode.Logic{
    [NodeRename("Logic/" + nameof(OptionNode), "通过进度条来控制对应剧情的概率")]
    [Serializable]
    [NodeType(NodeCode.OptionNode)]
    [NodeAttributeUsage(NodeAttributeTargets.FlowGraph | NodeAttributeTargets.ItemGraph)]
    public class OptionNode : BaseFieldNode{
        [NodeRename(nameof(Exit1), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit1;

        [NodeRename(nameof(Exit2), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit2;

        [NodeRename(nameof(Exit3), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit3;

        [NodeRename(nameof(Exit4), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit4;

        [NodeRename(nameof(Exit5), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit5;

        public List<string> optionsName = new List<string>();
        public int currentIndex = 2;
        private List<FieldInfo> _outputPorts;

#if UNITY_EDITOR
        public class TextFieldWithIndex{
            private readonly List<TextField> _textFields = new List<TextField>();
            private readonly OptionNode _optionNode;
            private TextField _textField;

            public TextFieldWithIndex(OptionNode optionNode){ this._optionNode = optionNode; }

            public TextField Create(){
                _textField = new TextField() {
                    label = GfuLanguage.Parse("Option") + (index + 1),
                    value = _optionNode.optionsName.Count > index ? _optionNode.optionsName[index] : "",
                    labelElement = {
                        style = {
                            minWidth = 0
                        }
                    }
                };
                _textFields.Add(_textField);
                index = _textFields.Count;
                return _textField;
            }

            public TextField this[int index] => _textFields[index];
            public int index;

            public int GetIndex(TextField textField){ return _textFields.IndexOf(textField); }

            public TextField Remove(){
                TextField visualElement = _textFields[--index];
                _textFields.RemoveAt(index);
                return visualElement;
            }

            public int Count => _textFields.Count;
        }

        public TextFieldWithIndex textFieldWithIndex;
        public VisualElement buttongroup;
#endif

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
#if UNITY_EDITOR
            textFieldWithIndex = new TextFieldWithIndex(this);
            _outputPorts = GfuOutputPortFieldInfos();
            if (currentIndex == 0){
                for (var i = 0; i < _outputPorts.Count; i++){
                    if (_outputPorts[i].GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Output){
                        currentIndex++;
                    } else{
                        _outputPorts.RemoveAt(i);
                    }
                }
            }

            while (outputContainer.childCount > currentIndex){
                outputContainer.RemoveAt(outputContainer.childCount - 1);
            }

            AutoTextField();
            buttongroup = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row
                }
            };

            Button button = new Button() {
                text = GfuLanguage.Parse("Add"),
                style = {
                    width = 80,
                }
            };
            button.clickable.clicked += () => {
                foreach (var outputPort in _outputPorts){ }

                if (currentIndex >= _outputPorts.Count){
                    UnityEditor.EditorUtility.DisplayDialog("提示", "不可再多啦，选项框都要溢出屏幕啦（如需更多端口访问官网查看魔改教程）", "确认");
                    return;
                }

                AddTextField();
                outputContainer.Add(_outputPorts[currentIndex++].GetValue(this) as VisualElement);
            };
            Button button2 = new Button() {
                text = GfuLanguage.Parse("Add"),
                style = {
                    width = 80,
                }
            };
            button2.clickable.clicked += () => {
                if (currentIndex == 1){
                    UnityEditor.EditorUtility.DisplayDialog("提示", "至少需要有一个剧情出口呦", "确认");
                    return;
                }

                RemoveTextField();
                outputContainer.RemoveAt(--currentIndex);
            };
            buttongroup.Add(button);
            buttongroup.Add(button2);
            mainContainer.Add(buttongroup);
#endif
        }
#if UNITY_EDITOR
        private void AddTextField(){
            var textField = textFieldWithIndex.Create();
            textField.RegisterValueChangedCallback((evt) => {
                var optionIndex = int.Parse(textField.label.Substring(2));
                while (optionsName.Count - 1 < optionIndex){
                    optionsName.Add("");
                }

                optionsName[optionIndex] = evt.newValue;
            });
            mainContainer.Insert(mainContainer.IndexOf(buttongroup), textField);
        }

        private void RemoveTextField(){ mainContainer.Remove(textFieldWithIndex.Remove()); }
        private void AutoTextField(){
            while (textFieldWithIndex.Count < currentIndex){
                var textField = textFieldWithIndex.Create();
                textField.RegisterValueChangedCallback((evt) => {
                    var optionIndex = textFieldWithIndex.GetIndex(textField);
                    while (optionsName.Count - 1 < optionIndex){
                        optionsName.Add("");
                    }

                    optionsName[optionIndex] = evt.newValue;
                });
                mainContainer.Add(textField);
            }
        }
#endif

        private GfuOptions gfuOptions;
        public override RoleData Execute(RoleData roleData){
            gfuOptions = new GfuOptions();
            gfuOptions.Parse(optionsName);
            gfuOptions.OnSelect = Executed;
            EventCenter.GetInstance().archiveEvent.AddListener(ArchiveListener);
            OptionController.GetInstance().ShowOption(gfuOptions);
            return roleData;
        }
        void ArchiveListener(ArchiveSystem.ArchiveEventType arg0){
            if (arg0 == ArchiveSystem.ArchiveEventType.ArchiveLoadStart){
                gfuOptions.options.Clear();
                gfuOptions.OnSelect = null;
                EventCenter.GetInstance().archiveEvent.RemoveListener(ArchiveListener);
            }
        }
        public override void Executed(int index){
            HideOption();
            EventCenter.GetInstance().archiveEvent.RemoveListener(ArchiveListener);
            base.Executed(index);
        }
        public void HideOption(){
            GameSystem.Data.OptionController.HideOption();
        }
    }
}