using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.Core;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Nodes.Editor.Block.Config.ConfigAdditions
{
    [NodeEditor(typeof(AdditionPose))]
    public class AdditionPoseUxml : ConfigAdditionUxml<AdditionPose>
    {
        // private DropdownField _pose;
        // private DropdownField _anchor;
        // private DropdownField _face;
        public AdditionPoseUxml(RoleAssets roleAssets, AdditionPose additionPose,GalPlotConfig galPlotConfig):base(roleAssets,additionPose,galPlotConfig)
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
          
            var button = new Button(){text = "选择"};
            button.RegisterCallback<ClickEvent>(evt =>
            {
                var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + (Vector2)evt.position);
                var poseAdditionSearchTypeProvider = PoseAdditionSearchTypeProvider.Create(roleAssets);
                poseAdditionSearchTypeProvider.OnSelectEntryHandler += (x, y) =>
                {
                    value.text = x.userData.ToString();
                    return true;
                };
                SearchWindow.Open(searchWindowContext, poseAdditionSearchTypeProvider);
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