using System;

namespace MUX.Type{
    /// <summary>
    /// 带内存保护机制的int，还未完全进行测试，谨慎使用
    /// </summary>
    public struct safeint {
        [NonSerialized]
        private int _value;//按位异运算后的值
        [NonSerialized]
        private readonly int key;
        [NonSerialized]
        private int hash;//用于篡改确认的hash值
        private int safevalue {
            set {
                hash = value.GetHashCode();
                _value = value ^ key;
            }
            get {
                int geting_value = _value ^ key;
                if (geting_value.GetHashCode() != hash) {//如果值的hash于保存的hash不一致
                    throw new MemoryTamperException($"value:{_value},hash:{hash}.mismatching");//用户篡改了内存
                }
                return geting_value;
            }
        }
    
        public safeint(int i) {
            key = new Random().Next(0, 0xffff);
            _value = 0;
            hash = 0;
            safevalue = i;
        } 
        public safeint(safeint i) {
            key = new Random().Next(0, 0xffff);
            _value = 0;
            hash = 0;
            safevalue = i;
        }
    
        public static implicit operator int(safeint value) {
            return value.ToInt();//自动转换为int
        }
        public static implicit operator safeint(int value) {
            return new safeint(value);//自动转换为safeint
        }

        //确认过不太可能出错的方法
        public int ToInt() {
            return safevalue;
        }
        public float ToFloat() {
            return safevalue;
        }

        public override int GetHashCode() {
            return safevalue.GetHashCode();
        } 
        public override string ToString() {
            return Convert.ToString(safevalue);
        }
        public override bool Equals(object obj) {
            return safevalue.Equals(obj);
        }

        public static safeint operator +(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue + safeint2.safevalue;
        }
        public static safeint operator -(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue - safeint2.safevalue;
        }
        public static safeint operator *(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue * safeint2.safevalue;
        }
        public static safeint operator /(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue / safeint2.safevalue;
        }
        public static safeint operator ^(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue ^ safeint2.safevalue;
        }
        public static safeint operator |(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue | safeint2.safevalue;
        }
        public static safeint operator &(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue & safeint2.safevalue;
        }
        public static bool operator ==(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue == safeint2.safevalue;
        }
        public static bool operator !=(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue != safeint2.safevalue;
        }
        public static bool operator >(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue > safeint2.safevalue;
        }
        public static bool operator <(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue < safeint2.safevalue;
        }
        public static bool operator >=(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue >= safeint2.safevalue;
        }
        public static bool operator <=(safeint safeint1, safeint safeint2) {
            return safeint1.safevalue <= safeint2.safevalue;
        }

        public class MemoryTamperException : Exception {
            public MemoryTamperException() : base() {

            }
            public MemoryTamperException(string e) : base(e){
            
            }
        };
    }
}
