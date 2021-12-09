//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  AddressableExpression.cs
//
//        Created by 半世癫(Roc) at 2021-12-04 12:42:27
//
//======================================================================

using System.Text.RegularExpressions;

namespace GalForUnity.System.Address{
    public static class AddressableExpression{
        public static Regex IsNull = new Regex(@"\s*\[\s*\]\s*");
        public static Regex IsAddress = new Regex(@"\[.+\]");
        public static Regex getExpression = new Regex(@"\w+:((\(.+\))?(\[[^\]]+\]){1}((\w+\.)*\w+)?:?\w*(\.\w+)*|\{.+\})");
        public static Regex getAddress = new Regex(@"\+(\w+\.)*\w+");
        public static Regex ClassName = new Regex(@"\w+(\.\w+)*");
        public static Regex AssemblyName = new Regex(@"\(.+\)");
        public static Regex ObjectName = new Regex(@"\[.+([\\/].+)*\]");
        public static Regex InstanceID = new Regex(@"\[[0-9]+\]");
    }
}
