using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;

#endif

namespace GalForUnity.Core{
    [Serializable]
    public class RoleDB : ScriptableObject, IEnumerable<RoleAssets>{
        private static volatile RoleDB m_Instance;

        [SerializeField] private List<RoleAssets> resources = new List<RoleAssets>();

        private Dictionary<string, RoleAssets> m_DB = new Dictionary<string, RoleAssets>();

        public static RoleDB Instance{
            get{
                if (!m_Instance) m_Instance = Resources.Load<RoleDB>("RoleDB");
                if (!m_Instance) m_Instance = Resources.FindObjectsOfTypeAll<RoleDB>().FirstOrDefault();
#if UNITY_EDITOR
                if (!m_Instance){
                    m_Instance = CreateInstance<RoleDB>();
                    var resourcesPath = Path.Combine(Application.dataPath, "Resources/");
                    if (!Directory.Exists(resourcesPath)) Directory.CreateDirectory(resourcesPath);
                    AssetDatabase.CreateAsset(m_Instance, "Assets/Resources/RoleDB.asset");
                    AssetDatabase.Refresh();
                }
#endif
                Debug.Assert(m_Instance != null, "m_Instance!=null");
                m_Instance.InitDBifNeed();
                return m_Instance;
            }
        }

        public RoleAssets this[string roleName]{
            get{
                InitDBifNeed();
                return m_DB[roleName];
            }
            set{
                InitDBifNeed();
                if (m_DB.ContainsKey(roleName)){
                    resources.Remove(m_DB[roleName]);
                    m_DB.Remove(roleName);
                }

                m_DB[roleName] = value;
                resources.Add(value);
            }
        }

        public IEnumerator<RoleAssets> GetEnumerator(){ return new RoleDBEnumerator(resources); }

        IEnumerator IEnumerable.GetEnumerator(){ return GetEnumerator(); }

        private void InitDBifNeed(){
            if (m_DB == null) m_DB = new Dictionary<string, RoleAssets>();
            if (m_DB.Count != resources.Count){
                m_DB.Clear();
                foreach (var roleAssets in resources) m_DB[roleAssets.roleName] = roleAssets;
            }
        }

        private static void Save(){
#if UNITY_EDITOR

            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssets();
#endif
        }

        public static void Add(RoleAssets roleAssets){
            Instance.m_DB[roleAssets.roleName] = roleAssets;
            Instance.resources.Add(roleAssets);
            Save();
        }

        public static List<string> Keys(){ return Instance.m_DB.Keys.ToList(); }

        public static List<RoleAssets> Values(){ return Instance.m_DB.Values.ToList(); }

        public static bool Contains(RoleAssets roleAssets){ return Instance.m_DB.ContainsKey(roleAssets.roleName); }

        public void Remove(RoleAssets roleAssets){
            Instance.m_DB.Remove(roleAssets.roleName);
            Instance.resources.Remove(roleAssets);
            Save();
        }

        public static void ForEach(Action<RoleAssets> items){
            foreach (var roleAssets in Instance) items.Invoke(roleAssets);
        }

        private class RoleDBEnumerator : IEnumerator<RoleAssets>{
            private readonly List<RoleAssets> m_RoleAssetsList;
            private int m_Index = -1;

            public RoleDBEnumerator(List<RoleAssets> roleAssetsList){ m_RoleAssetsList = roleAssetsList; }

            public bool MoveNext(){ return ++m_Index < m_RoleAssetsList.Count; }

            public void Reset(){ m_Index = -1; }

            public RoleAssets Current => m_RoleAssetsList[m_Index];

            object IEnumerator.Current => Current;

            public void Dispose(){ m_RoleAssetsList.Clear(); }
        }
    }
}