using GalForUnity.Core;
using GalForUnity.Core.Block;
using GalForUnity.Core.Editor;
using GalForUnity.Graph.Editor.Builder;
using GalForUnity.Graph.Editor.Builder.SearchProviders;
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
        public AdditionPoseUxml(GalObject galObject, AdditionPose additionPose,PlotBlock plotBlock):base(galObject,additionPose,plotBlock)
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
                var poseAdditionSearchTypeProvider = PoseAdditionSearchTypeProvider.Create(galObject);
                poseAdditionSearchTypeProvider.OnSelectEntryHandler += (x, y) =>
                {
                    var xUserData = (PreviewData)x.userData;
                    additionPose.poseName = xUserData.pose.name;
                    additionPose.anchorName = xUserData.Anchor.name;
                    additionPose.faceName = xUserData.AnchorSprite.name;
                    value.text = $"{xUserData.poseLocation.roleName}/{additionPose.poseName}/{additionPose.anchorName}/{additionPose.faceName}";
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