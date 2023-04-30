using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Builder.SearchProviders
{
    public interface IPreviewSearchWindowProvider : ISearchWindowProvider
    {
        public void OnMouseEnter(SearchTreeEntry enter, Rect windowPosition, MouseEnterEvent mouseEnterEvent);
        public void OnMouseLeave(SearchTreeEntry enter, Rect windowPosition, MouseLeaveEvent mouseEnterEvent);
    }
}