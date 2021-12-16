using GalForUnity.Attributes;
using GalForUnity.Model;
using GalForUnity.Model.Plot;
using GalForUnity.System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Editor
{
    [CustomPropertyDrawer(typeof(RoleDataItem))] 
    public class ReorderableListDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position,label,property))
            {
                //设置属性名宽度
                EditorGUIUtility.labelWidth = 70;
                position.height = EditorGUIUtility.singleLineHeight;

                var nameRect = new Rect(position) 
                {
                    width = position.width/2f,
                    y = position.y + 1 
                };

                var attackSliderRect = new Rect(nameRect)
                {
                    width = position.width/2.5f,
                    x = position.width/1.6f
                };
                var iconProperty = property.FindPropertyRelative("icon");
                var prefabProperty = property.FindPropertyRelative("prefab"); 
                var nameProperty = property.FindPropertyRelative("name");
                var attackProperty = property.FindPropertyRelative("value");

                //iconProperty.objectReferenceValue = EditorGUI.ObjectField(iconRect, iconProperty.objectReferenceValue, typeof(Texture), false);
                nameProperty.stringValue = EditorGUI.TextField(nameRect, GfuLanguage.Parse("valueName"),nameProperty.stringValue);
                //prefabProperty.objectReferenceValue = EditorGUI.ObjectField(prefabRect, prefabProperty.objectReferenceValue,typeof(GameObject),false);
                EditorGUIUtility.labelWidth = 30;
                attackProperty.intValue = EditorGUI.IntField(attackSliderRect,GfuLanguage.Parse("value"), attackProperty.intValue);
            }
        }
        
    }
    // [CustomPropertyDrawer(typeof(RoleData.RoleDataItem))] 
    // public class RoleDataItemListDrawer : PropertyDrawer
    // {
    //
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         using (new EditorGUI.PropertyScope(position,label,property))
    //         {
    //             //设置属性名宽度
    //             EditorGUIUtility.labelWidth = 50;
    //             position.height = EditorGUIUtility.singleLineHeight;
    //
    //             var nameRect = new Rect(position) 
    //             {
    //                 width = position.width /2f,
    //                 y = position.y + 1 
    //             };
    //
    //             var attackSliderRect = new Rect(nameRect)
    //             {
    //                 width = position.width /3f,
    //                 x = position.width     /1.5f
    //             };
    //
    //             var iconProperty = property.FindPropertyRelative("icon");
    //             var prefabProperty = property.FindPropertyRelative("prefab"); 
    //             var nameProperty = property.FindPropertyRelative("name");
    //             var attackProperty = property.FindPropertyRelative("value");
    //
    //             //iconProperty.objectReferenceValue = EditorGUI.ObjectField(iconRect, iconProperty.objectReferenceValue, typeof(Texture), false);
    //             nameProperty.stringValue = EditorGUI.TextField(nameRect, new GUIContent("数值名称"),nameProperty.stringValue);
    //             //prefabProperty.objectReferenceValue = EditorGUI.ObjectField(prefabRect, prefabProperty.objectReferenceValue,typeof(GameObject),false);
    //             EditorGUIUtility.labelWidth = 30;
    //             attackProperty.intValue = EditorGUI.IntField(attackSliderRect,new GUIContent("数值"), attackProperty.intValue);
    //         }
    //     }
    //     
    // }

    // [CustomPropertyDrawer(typeof(SceneModel))]
    // public class SceneModelDrawer : PropertyDrawer{
    //     public override VisualElement CreatePropertyGUI(SerializedProperty property)
    //     {
    //         return base.CreatePropertyGUI(property);
    //     }
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
    //         // base.OnGUI(position,property,label);
    //         using (new EditorGUI.PropertyScope(position,label,property)){
    //             // Debug.Log(1);
    //             EditorGUIUtility.labelWidth = 50;
    //             // position.height = EditorGUIUtility.singleLineHeight;
    //             // position.width = EditorGUIUtility.currentViewWidth;
    //             position.y += 2;
    //             // var nameRect = new Rect(position){y=position.y +1,width = position.width / 3f};
    //             // //var valueProperty = property.FindPropertyRelative("sceneModels");
    //             // var valueRect = new Rect(nameRect){x=position.width / 2f -20, width = position.width / 2f};
    //             position.width = EditorGUIUtility.labelWidth;
    //             EditorGUI.LabelField(position,label);
    //             position.width = EditorGUIUtility.w;
    //             position.x = EditorGUIUtility.currentViewWidth - EditorGUIUtility.fieldWidth - EditorGUIUtility.labelWidth;
    //             property.objectReferenceValue=EditorGUI.ObjectField(position,property.objectReferenceValue,typeof(SceneModel),true);
    //         }
    //     }
    // }

    [CustomPropertyDrawer(typeof(PlotItem))]
    public class PlotFlowItemDrawer : PropertyDrawer{
        // ReorderableList  _plotAnimationReorderable;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return base.CreatePropertyGUI(property);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            // RenameAttribute renameAttribute = (RenameAttribute) attribute;
            // base.OnGUI(position,property,label);
            using (new EditorGUI.PropertyScope(position, label, property)){
                // Debug.Log(1);
                EditorGUIUtility.labelWidth = 40;
                position.height = EditorGUIUtility.singleLineHeight;
                // position.width = EditorGUIUtility.currentViewWidth - 20;
                var backgroundRect = new Rect(position){
                    y = position.y + 2, width = 64, height = 64
                };
                var nameRect = new Rect(backgroundRect){
                    x = backgroundRect.x + 80, width = position.width - 80, height = EditorGUIUtility.singleLineHeight
                };
                var speakRect = new Rect(nameRect){
                    y = nameRect.y + 2 + EditorGUIUtility.singleLineHeight, height = backgroundRect.height - nameRect.height - 1
                };
                var animationsRect = new Rect(speakRect){
                    x = 0 + EditorGUIUtility.labelWidth, width = position.width / 2f, y = speakRect.y + speakRect.height + 2, height = EditorGUIUtility.singleLineHeight
                };
                var audioClipRect = new Rect(animationsRect){
                    x = animationsRect.x + animationsRect.width + 5, width = position.width / 2f - 5
                };
                var animation = new Rect(animationsRect){
                    y=audioClipRect.y+EditorGUIUtility.singleLineHeight+2,width = position.width/4f
                };
                var nameProperty = property.FindPropertyRelative("name");
                var speakProperty = property.FindPropertyRelative("speak");
                var animationsProperty = property.FindPropertyRelative("animationSet");
                var audioClipProperty = property.FindPropertyRelative("audioClip");
                var backgroundProperty = property.FindPropertyRelative("background");
                
                var o = backgroundProperty.GetFirstAttribute<RenameAttribute>();
                label = new GUIContent(){
                     tooltip = o?.ToolTip ?? ""
                };
                backgroundProperty.objectReferenceValue = EditorGUI.ObjectField(backgroundRect,label, backgroundProperty.objectReferenceValue, typeof(Sprite), false);
                // Debug.Log(backgroundProperty.tooltip);
                o = nameProperty.GetFirstAttribute<RenameAttribute>();
                label = new GUIContent(){
                    text = o?.Name ?? "", tooltip = o?.ToolTip ?? ""
                };
                nameProperty.stringValue = EditorGUI.TextField(nameRect, label, nameProperty.stringValue);
                o = speakProperty.GetFirstAttribute<RenameAttribute>();
                // speakProperty.stringValue = EditorGUI.TextArea(speakRect, speakProperty.stringValue);

                GUILayout.BeginArea(speakRect);
                speakProperty.stringValue = GUILayout.TextArea(string.IsNullOrEmpty(speakProperty.stringValue) ? o.ToolTip : speakProperty.stringValue);
                GUILayout.EndArea();
                
                // EditorGUILayout.EndFadeGroup();
                speakProperty.stringValue = EditorGUI.TextArea(speakRect, speakProperty.stringValue);
                
                o = animationsProperty.GetFirstAttribute<RenameAttribute>();
                label = new GUIContent(){
                    text = o?.Name ?? "", tooltip = o?.ToolTip ?? ""
                };
                // EditorGUILayout.BeginFadeGroup(1);
                // Debug.Log(animationsProperty.objectReferenceValue);
                animationsProperty.objectReferenceValue=EditorGUI.ObjectField(animationsRect,label, animationsProperty.objectReferenceValue, typeof(PlotAnimationSet),false);
                
                o = audioClipProperty.GetFirstAttribute<RenameAttribute>();
                label = new GUIContent(){
                    text = o?.Name ?? "", tooltip = o?.ToolTip ?? ""
                };
                EditorGUIUtility.labelWidth = 32;
                audioClipProperty.objectReferenceValue=EditorGUI.ObjectField(audioClipRect, label, audioClipProperty.objectReferenceValue, typeof(AudioClip), true);
                
                // List<AnimationClip> animationClips=new List<AnimationClip>();
                for (int i = 0; i < 4; i++){
                    label = new GUIContent(){
                        text = "动作"+i, tooltip = "快捷播放的动画,如果需要定义复杂动画，请使用动画集PlotAnimationSet"
                    };
                    animation.x = animationsRect.x+position.width /4f*i;
                    var animationClip = property.FindPropertyRelative("_animationClip"+(i+1));
                    animationClip.objectReferenceValue=EditorGUI.ObjectField(animation, label,obj: animationClip.objectReferenceValue, typeof(AnimationClip), true);
                }
                
                
            }
        }
    }
}