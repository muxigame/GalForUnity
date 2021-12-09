using NotImplementedException = System.NotImplementedException;

namespace GalForUnity.System.Address.Exception{
    public class ObjectNotFoundException : global::System.Exception{
        public ObjectNotFoundException(){ }
        public ObjectNotFoundException(string info):base(info){ }
    }
}