using GalForUnity.Core;
using GalForUnity.Core.Block;
using GalForUnity.Graph.Editor.Block;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.ConfigAdditions
{
    public class ConfigAdditionUxml<T>:VisualElement where T:ConfigAddition
    {
        public ConfigAdditionUxml(RoleAssets roleAssets, T additionPose,PlotBlock plotBlock)
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
                plotBlock.configAdditions.Remove(additionPose);
            });
        }
        public VisualElement Content;
    }
}