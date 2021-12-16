using System;
using System.Text.RegularExpressions;
using GalForUnity.External;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace GalForUnity.Attributes{
    public class LogicAttribute : PropertyAttribute{
        public string obj;
        public string name;
    }

    public class HideByAttribute : LogicAttribute{
        public HideByAttribute(string obj){
            this.obj = obj;
        }
        public HideByAttribute(string obj, string name){
            this.obj = obj;
            this.name = name;
        }
    }

    public class ShowByAttribute : LogicAttribute{
        public ShowByAttribute(string obj){
            this.obj = obj;
        }
        public ShowByAttribute(string obj, string name){
            this.obj = obj;
            this.name = name;
        }
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HideByAttribute))]
    [CustomPropertyDrawer(typeof(ShowByAttribute))]
    public class LogicDrawer : PropertyDrawer{
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            if (!LogicDrawer.CanShow(property)) return 0;
            float baseHeight = base.GetPropertyHeight(property, label);
            if (property.isExpanded){
                if (property.propertyType == SerializedPropertyType.Generic){
                    if (property.hasChildren){
                        if(property.depth !=0) return baseHeight + (EditorGUIUtility.singleLineHeight) * (property.CountInProperty());
                        return baseHeight + (EditorGUIUtility.singleLineHeight) * (property.CountInProperty());
                    }
                    return baseHeight;
                }
            }
            return baseHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            using (new EditorGUI.PropertyScope(position, label, property)){
                if (CanShow(property, attribute)){
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
        }

        public static bool CanShow(SerializedProperty property, Attribute attribute){
            // Debug.Log(property);
            SerializedProperty serializedProperty = property.GetParentProp();
            // Debug.Log(serializedProperty);
            LogicAttribute hideByAttribute = null;
            if (attribute is LogicAttribute){
                hideByAttribute = (LogicAttribute) attribute;
            }

            if (hideByAttribute == null){
                return true;
            }
            
            var expression = hideByAttribute.obj;
            if (expression.IndexOf("=", StringComparison.Ordinal) != -1||expression.IndexOf("&", StringComparison.Ordinal) != -1 || expression.IndexOf("|", StringComparison.Ordinal) != -1){
                while (expression.IndexOf(")", StringComparison.Ordinal) !=-1){
                    string subexpression="";
                    for (var i = 0; i < expression.Length; i++){
                        if (expression[i] == ')'){
                            for (int j = i - 1; j >= 0; j--){
                                if (expression[j] == '('){
                                    subexpression = expression.Substring(j, i-j+1);
                                    expression = expression.Replace(subexpression,"" +OrderParse(subexpression.Replace("(", "").Replace(")", "").Trim(), serializedProperty));
                                    break;
                                }
                            }
                        }
                    }
                }
                bool canshow = OrderParse(expression, serializedProperty);
                return canshow;
            } else{
                SerializedProperty findPropertyRelative = serializedProperty.FindPropertyRelative(expression);
            
                if (!CanShow(findPropertyRelative)) return false;
                if (findPropertyRelative.propertyType == SerializedPropertyType.Boolean){
                    if (attribute is HideByAttribute){
                        if (findPropertyRelative.boolValue == true){
                            return false;
                        }
                    } else if (attribute is ShowByAttribute){
                        if (findPropertyRelative.boolValue != true){
                            return false;
                        }
                    }
                }else if (findPropertyRelative.propertyType == SerializedPropertyType.Enum){
                    if (attribute is HideByAttribute){
                        if (findPropertyRelative.enumNames[findPropertyRelative.enumValueIndex] == hideByAttribute.name){
                            return false;
                        }
                    } else if (attribute is ShowByAttribute){
                        if (findPropertyRelative.enumNames[findPropertyRelative.enumValueIndex] != hideByAttribute.name){
                            return false;
                        }
                    }
                }
                return true;
            }
            
        }

        private static bool OrderParse(string subExpression,SerializedProperty property){
            var orderExpression = subExpression;
            while (orderExpression.IndexOf("&&",StringComparison.Ordinal)!=-1){
                //?(==\s*\w+\s*&&\s*\w+\s*==)
                var regex = new Regex(@"(\w+\s*[!=]=\s*\w+\s*|\s*(false|true)\s*)\s*&&\s*(\s*\w+\s*[!=]=\s*\w+|\s*(false|true)\s*)",RegexOptions.IgnoreCase);
                var also = new Regex(@"&&");
                var strings = also.Split(regex.Match(orderExpression).Value);
                orderExpression=regex.Replace(orderExpression,""+(GetBool(strings[0],property)&&GetBool(strings[1], property)));
            }
            while (orderExpression.IndexOf("||", StringComparison.Ordinal) !=-1){
                var regex = new Regex(@"(\w+\s*[!=]=\s*\w+\s*|\s*(false|true)\s*)\s*\|\|\s*(\s*\w+\s*[!=]=\s*\w+|\s*(false|true)\s*)",RegexOptions.IgnoreCase);
                var or = new Regex(@"\|\|");
                var strings = or.Split(regex.Match(orderExpression).Value);
                orderExpression=regex.Replace(orderExpression,"" +(GetBool(strings[0],property)||GetBool(strings[1],property)));
            }
            orderExpression = ""+GetBool(orderExpression,property);
            return bool.Parse(orderExpression.Trim());
        }

        private static bool GetBool(string subExpression,SerializedProperty property){
            // Debug.Log(subExpression);
            var noSpace = subExpression.Replace(" ", "").Trim();
            if (!noSpace.Contains("=")){
                if (noSpace.Contains("True")||noSpace.Contains("true")){
                    return true;
                } 
                if (noSpace.Contains("False") ||noSpace.Contains("false")){
                    return false;
                }
            }
            var strings = new Regex("[!=]=").Split(noSpace);
            var value1 = property.FindPropertyRelative(strings[0]);
            int index = 1;
            if (value1 == null){
                value1 = property.FindPropertyRelative(strings[1]);
                index = 0;
            }
            bool boolValue = false;
            if (value1.propertyType == SerializedPropertyType.Boolean){
                boolValue = value1.boolValue == bool.Parse(strings[index]);
            }else if(value1.propertyType == SerializedPropertyType.Enum){
                boolValue = value1.enumNames[value1.enumValueIndex] == strings[index];
            }else if(value1.propertyType == SerializedPropertyType.Float){
                boolValue = Math.Abs(value1.floatValue - float.Parse(strings[index])) < 0.000001f;
            }else if(value1.propertyType == SerializedPropertyType.Integer){
                boolValue = value1.intValue == int.Parse(strings[index]) ;
            }else if(value1.propertyType == SerializedPropertyType.String){
                boolValue = value1.stringValue == strings[index] ;
            }else if(value1.propertyType == SerializedPropertyType.ObjectReference){
                if (value1.objectReferenceValue == null) boolValue = strings[index] == "null" || property.FindPropertyRelative(strings[index]).objectReferenceValue == null;
                else boolValue = value1.objectReferenceValue == property.FindPropertyRelative(strings[index]).objectReferenceValue;
            } else{
                Debug.LogError("This type is not currently supported");
                return true;
            }
            // Debug.Log(boolValue);
            if (noSpace.IndexOf("!=", StringComparison.Ordinal) != -1){
                return !boolValue;
            }
            if (noSpace.IndexOf("==", StringComparison.Ordinal) != -1){
                //var strings = subExpression.Split("!=".ToCharArray());
                return boolValue;
            }
            Debug.LogError("Wrong expression");
            return true;
        }
        public static bool CanShow(SerializedProperty property){
            var attributes = property.GetAttributes<LogicAttribute>();
            foreach (LogicAttribute o in attributes){
                if (!CanShow(property, o)) return false;
            }
            return true;
        }
        
    }
#endif
}