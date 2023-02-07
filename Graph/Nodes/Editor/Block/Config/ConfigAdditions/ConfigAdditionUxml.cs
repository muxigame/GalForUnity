using GalForUnity.Core;
using GalForUnity.Graph.Block;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Nodes.Editor.Block.Config.ConfigAdditions
{
    public class ConfigAdditionUxml<T>:VisualElement where T:ConfigAddition
    {
        public ConfigAdditionUxml(RoleAssets roleAssets, T additionPose,GalPlotConfig galPlotConfig)
        {
            var templateContainer = UxmlHandler.instance.configAddition.Instantiate();
            // templateContainer.style.flexGrow = 1;
            // templateContainer.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            Content = templateContainer.Q<VisualElement>("Content");
            Add(templateContainer);
            var button = this.Q<Button>("DeleteButton");
            button.clickable = new Clickable(() =>
            {
                parent.Remove(this);
                galPlotConfig.configAdditions.Remove(additionPose);
            });
        }
        public VisualElement Content;
    }
}