

using System;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block{
    public class BlockPortUxml : VisualElement{
        public Button button;

        public VisualElement Content;

        public BlockPortUxml() : this(null){ }

        public BlockPortUxml(Action action){
            Content = new VisualElement {
                name = "PortContent"
            };
            button = new Button {
                name = "AddPortButton", text = "AddPort", clickable = new Clickable(action)
            };
            contentContainer.Add(Content);
            contentContainer.Add(button);
        }

        public class BlockPortUxmlFactory : UxmlFactory<BlockPortUxml, UxmlTraits>{ }
    }
}