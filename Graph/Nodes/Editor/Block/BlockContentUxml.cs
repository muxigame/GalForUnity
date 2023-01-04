//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  BlockContentUxml.cs Created at 2022-09-28 21:04:15
//
//======================================================================

using System;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
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