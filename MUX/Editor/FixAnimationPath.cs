//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  FixAnimationPath.cs
//
//        Created by 半世癫(Roc) at 2021-08-18 22:46:31
//
//======================================================================

using UnityEditor;
using UnityEngine;

namespace MUX.Editor{
    public class FixAnimationPath : EditorWindow
    {
        /// <summary>
        /// 需要改变的物体
        /// </summary>
        private GameObject target;

        private string error;

        private AnimationClip ac;
        //改为原来的子对象（将删除前inChild个层级目录）
        private int inChild;
        [MenuItem("Custom/Animation/Fix Animation Path")]
        static void FixAnimationPathMethod()
        {
            Rect wr = new Rect(0, 0, 500, 500);
            FixAnimationPath window = (FixAnimationPath)EditorWindow.GetWindowWithRect(typeof(FixAnimationPath), wr, true, "Fix Animation");
            window.Show();
        }

        bool DoFix()
        {
            //AnimationClip ac = Selection.activeObject as AnimationClip;

            if (ac == null)
                error = "AnimationClip缺失";
            if (target == null)
                error = "Target丢失";

            if (ac != null)
            {
                Debug.Log("Enter ac != null");
                GameObject root = target;
                //获取所有绑定的EditorCurveBinding(包含path和propertyName)
                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(ac);

                for (int i = 0; i < bindings.Length; ++i)
                {
                    EditorCurveBinding binding = bindings[i];

                    GameObject bindObj = AnimationUtility.GetAnimatedObject(root, binding) as GameObject;

                    if (bindObj == null)
                    {
                        Debug.Log(binding.path);
                        if (inChild > 0)
                        {
                            string newPath = DeletePathPart(binding.path);
                            AnimationCurve curve = AnimationUtility.GetEditorCurve(ac, binding);

                            //remove Old
                            AnimationUtility.SetEditorCurve(ac, binding, null);
                            binding.path = newPath;
                            AnimationUtility.SetEditorCurve(ac, binding, curve);
                        }
                        else
                        {
                            bindObj = FindInChildren(root, binding.path);
                            if (bindObj)
                            {
                                string newPath = AnimationUtility.CalculateTransformPath(bindObj.transform, root.transform);
                                Debug.Log("change " + binding.path + " to " + newPath);

                                AnimationCurve curve = AnimationUtility.GetEditorCurve(ac, binding);

                                //remove Old
                                AnimationUtility.SetEditorCurve(ac, binding, null);

                                binding.path = newPath;

                                AnimationUtility.SetEditorCurve(ac, binding, curve);
                            }
                        }
                    }
                }
            }
            return true;
        }
        string DeletePathPart(string path)
        {
            string[] Strs = path.Split('/');
            path = "";
            for(int i = inChild; i < Strs.Length; i++)
            {
                path += Strs[i];
                if (i != Strs.Length - 1)
                    path += "/";
                Debug.Log(path);
            }
            return path;
        }
        GameObject FindInChildren(GameObject obj, string goName)
        {
            UnityEngine.Transform objTransform = obj.transform;

            GameObject finded = null;
            UnityEngine.Transform findedTransform = objTransform.Find(goName);

            if (findedTransform == null)
            {
                for (int i = 0; i < objTransform.childCount; ++i)
                {
                    finded = FindInChildren(objTransform.GetChild(i).gameObject, goName);
                    if (finded)
                    {
                        Debug.Log(finded.name);
                        return finded;
                    
                    }
                }

                return null;
            }

            return findedTransform.gameObject;
        }

        void OnGUI()
        {
            //Debug.Log("Fix AnimationClip");
            EditorGUILayout.LabelField("TargetRoot");
            target = EditorGUILayout.ObjectField(target, typeof(GameObject), true) as GameObject;

            EditorGUILayout.LabelField("AnimationClip");
            ac = EditorGUILayout.ObjectField(ac, typeof(AnimationClip), true) as AnimationClip;

            EditorGUILayout.LabelField("InChild");
            inChild= EditorGUILayout.IntField( inChild);

            if (GUILayout.Button("Fix", GUILayout.Width(200)))
            {
                if (this.DoFix())
                {
                    this.ShowNotification(new GUIContent("Change Complete"));
                }
                else
                {
                    this.ShowNotification(new GUIContent("Change Error " + error));
                }
            }
        }

    }
}
