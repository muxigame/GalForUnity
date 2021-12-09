//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  IAddressable.cs
//
//        Created by 半世癫(Roc) at 2021-12-04 12:33:55
//
//======================================================================

namespace GalForUnity.System.Address{
    public interface IAddressable{
        bool Get(string expression,out object value);
        void Set(string expression,object value);
        string Parse(object value);
    }
}
