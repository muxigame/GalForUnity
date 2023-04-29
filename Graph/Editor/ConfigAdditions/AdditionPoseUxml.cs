using GalForUnity.Core;
using GalForUnity.Core.Block;
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Graph.Editor.Builder;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.ConfigAdditions
{
    [NodeEditor(typeof(AdditionPose))]
    public class AdditionPoseUxml : ConfigAdditionUxml<AdditionPose>
    {
        // private DropdownField _pose;
        // private DropdownField _anchor;
        // private DropdownField _face;
        public AdditionPoseUxml(RoleAssets roleAssets, AdditionPose additionPose,PlotBlock plotBlock):base(roleAssets,additionPose,plotBlock)
        {
            var type = typeof(AdditionPose);
            var label = new Label("表情");
            label.AddToClassList("gal-label");
            var value = new Label()
            {
                style=
                {
                    flexGrow = 1
                }
            };

            if (!string.IsNullOrEmpty(additionPose.anchorName) && !string.IsNullOrEmpty(additionPose.poseName) && !string.IsNullOrEmpty(additionPose.faceName)){
                value.text = string.Concat(additionPose.poseName, "/", additionPose.anchorName, "/", additionPose.faceName);
            }
            
            var button = new Button(){text = "选择"};
            button.RegisterCallback<ClickEvent>(evt =>
            {
                var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + (Vector2)evt.position);
                var poseAdditionSearchTypeProvider = PoseAdditionSearchTypeProvider.Create(roleAssets);
                poseAdditionSearchTypeProvider.OnSelectEntryHandler += (x, y) =>
                {
                    value.text = x.userData.ToString();
                    var strings = value.text.Split("/");
                    additionPose.poseName = strings[0];
                    additionPose.anchorName = strings[1];
                    additionPose.faceName = strings[2];
                    return true;
                };
                PreviewSearchWindow.Open(searchWindowContext, poseAdditionSearchTypeProvider);
            });
            
            var fieldContainer = new FieldContainer()
            {
                style=
                {
                    marginLeft = 3,
                    marginRight = 3
                }
            };
            fieldContainer.Add(label);
            fieldContainer.Add(value);
            fieldContainer.Add(button);
            Content.Add(fieldContainer);
        }
        
    }
}