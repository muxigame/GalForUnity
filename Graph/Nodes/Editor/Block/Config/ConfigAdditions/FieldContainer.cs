using UnityEngine.UIElements;

namespace GalForUnity.Graph.Nodes.Editor.Block.Config.ConfigAdditions
{
    public class FieldContainer:VisualElement
    {
        public FieldContainer()
        {
            name = nameof(FieldContainer);
            style.flexDirection = FlexDirection.Row;
            // style.flexGrow = 1;
        }
    }
}