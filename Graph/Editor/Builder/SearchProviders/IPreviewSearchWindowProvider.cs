using UnityEditor.Experimental.GraphView;

namespace GalForUnity.Graph.Editor.Builder.SearchProviders
{
    public interface IPreviewSearchWindowProvider:ISearchWindowProvider
    {
        public void OnMouseEnter(SearchTreeEntry enter);
    }
}