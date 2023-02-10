namespace GalForUnity.Graph.Nodes{
     public interface INode{
     }
     public interface INode<T>:INode where T : RuntimeNode{
          T RuntimeNode{ get; set; }
     }
}