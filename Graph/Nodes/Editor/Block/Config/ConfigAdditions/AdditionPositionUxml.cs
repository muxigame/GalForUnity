using GalForUnity.Attributes;
using GalForUnity.Core;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Nodes.Editor.Block.Config.ConfigAdditions
{
    [NodeEditor(typeof(AdditionPosition))]
    public class AdditionPositionUxml : ConfigAdditionUxml<AdditionPosition>
    {
        public AdditionPositionUxml(RoleAssets roleAssets, AdditionPosition additionPose, GalPlotConfig galPlotConfig) : base(roleAssets, additionPose, galPlotConfig)
        {
            var type = typeof(AdditionPosition);
            var vector2Field = new Vector2Field()
            {
                style =
                {
                    minWidth = 300,
                    flexGrow = 1,
                }
            };
            var xInput = vector2Field.Q<FloatField>("unity-x-input");
            var yInput = vector2Field.Q<FloatField>("unity-y-input");
            var xUnit = new EnumField(Unit.Percentage);
            var yUnit = new EnumField(Unit.Percentage);
            // xInput.parent.Insert(xInput.parent.IndexOf(xInput) + 1, xUnit);
            // yInput.parent.Insert(yInput.parent.IndexOf(yInput) + 1, yUnit);
            xInput.Add(xUnit);
            yInput.Add(yUnit);
            xUnit.CreateBinder(type.GetField("xUnit"), additionPose);
            yUnit.CreateBinder(type.GetField("yUnit"), additionPose);
            vector2Field.CreateBinder(type.GetField("position"), additionPose);

            var label = new Label("位置");
            label.AddToClassList("gal-label");
            var fieldContainer = new FieldContainer()
            {
                style =
                {
                    marginLeft = 3,
                    marginRight = 3
                }
            };
            fieldContainer.Add(label);
            fieldContainer.Add(vector2Field);
            Content.Add(fieldContainer);
        }
    }
}