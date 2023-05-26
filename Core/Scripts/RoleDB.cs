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
    public class RoleDB : ScriptableObject, IEnumerable<GalObject>{
        private static volatile RoleDB m_Instance;

        [SerializeField] private List<GalObject> resources = new List<GalObject>();

        private Dictionary<string, GalObject> m_DB = new Dictionary<string, GalObject>();

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

        public GalObject this[string roleName]{
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

        public IEnumerator<GalObject> GetEnumerator(){ return new RoleDBEnumerator(resources); }

        IEnumerator IEnumerable.GetEnumerator(){ return GetEnumerator(); }

        private void InitDBifNeed(){
            if (m_DB == null) m_DB = new Dictionary<string, GalObject>();
            if (m_DB.Count != resources.Count){
                m_DB.Clear();
                foreach (var roleAssets in resources) m_DB[roleAssets.objectName] = roleAssets;
            }
        }

        private static void Save(){
#if UNITY_EDITOR

            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssets();
#endif
        }

        public static void Add(GalObject galObject){
            Instance.m_DB[galObject.objectName] = galObject;
            Instance.resources.Add(galObject);
            Save();
        }

        public static List<string> Keys(){ return Instance.m_DB.Keys.ToList(); }

        public static List<GalObject> Values(){ return Instance.m_DB.Values.ToList(); }

        public static bool Contains(GalObject galObject){ return Instance.m_DB.ContainsKey(galObject.objectName); }

        public void Remove(GalObject galObject){
            Instance.m_DB.Remove(galObject.objectName);
            Instance.resources.Remove(galObject);
            Save();
        }

        public static void ForEach(Action<GalObject> items){
            foreach (var roleAssets in Instance) items.Invoke(roleAssets);
        }

        private class RoleDBEnumerator : IEnumerator<GalObject>{
            private readonly List<GalObject> m_RoleAssetsList;
            private int m_Index = -1;

            public RoleDBEnumerator(List<GalObject> roleAssetsList){ m_RoleAssetsList = roleAssetsList; }

            public bool MoveNext(){ return ++m_Index < m_RoleAssetsList.Count; }

            public void Reset(){ m_Index = -1; }

            public GalObject Current => m_RoleAssetsList[m_Index];

            object IEnumerator.Current => Current;

            public void Dispose(){ m_RoleAssetsList.Clear(); }
        }
    }
}