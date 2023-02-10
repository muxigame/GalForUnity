

using System;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block{
    public sealed class BlockContentUxml : VisualElement{

        public BlockContentUxml():this(null){
            
        }

        public BlockContentUxml(Action action){
            Content = new VisualElement(){name="BlockContent"};
            button = new Button() {
                name = "AddConfigButton",
                text = "AddConfig",
                clickable = new Clickable(action)
            };
            contentContainer.Add(Content);
            contentContainer.Add(button);
        }
        
        public Button button;

        public VisualElement Content;

        public class BlockContentUxmlFactory : UxmlFactory<BlockContentUxml, UxmlTraits>{ }
    }
}