using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace GalForUnity.Core
{
    public class RoleDB:ScriptableObject,IEnumerable<RoleAssets>
    {
        private static volatile RoleDB _instance;

        public static RoleDB Instance
        {
            get
            {
                if (!_instance) _instance = Resources.Load<RoleDB>("RoleDB");
#if UNITY_EDITOR
                if (!_instance)
                {
                    _instance = CreateInstance<RoleDB>();
                    AssetDatabase.CreateAsset(_instance, "Assets/Resources/RoleDB.asset");
                    AssetDatabase.Refresh();
                }
#endif
                _instance.InitDBifNeed();
                return _instance;
            }
        }
        public List<RoleAssets> resources = new List<RoleAssets>();

        public Dictionary<string, RoleAssets> DB = new Dictionary<string, RoleAssets>();

        private void InitDBifNeed()
        {
            if (DB == null) DB = new Dictionary<string, RoleAssets>();
            if (DB.Count != resources.Count)
            {
                DB.Clear();
                foreach (var roleAssets in resources)
                {
                    DB[roleAssets.name] = roleAssets;
                }
            }
        }
        public RoleAssets this[string roleName]
        {
            get
            {
                InitDBifNeed();
                return DB[roleName];
            }
            set
            {
                InitDBifNeed();
                if (DB.ContainsKey(roleName))
                {
                    resources.Remove(DB[roleName]);
                    DB.Remove(roleName);
                }
                DB[roleName] = value;
                resources.Add(value);
            }
        }

        public static void Add(RoleAssets roleAssets)
        {
            Instance.DB[roleAssets.name] = roleAssets;
            Instance.resources.Add(roleAssets);
        }
        public static List<string> Keys()
        {
            return Instance.DB.Keys.ToList();
        }
        public static List<RoleAssets> Values()
        {
            return Instance.DB.Values.ToList();
        }
        public static bool Contains(RoleAssets roleAssets)
        {
            return Instance.DB.ContainsKey(roleAssets.name);
        }
        public void Remove(RoleAssets roleAssets)
        {
            Instance.DB.Remove(roleAssets.name);
            Instance.resources.Remove(roleAssets);
        }

        public static void ForEach(Action<RoleAssets> items)
        {
            foreach (var roleAssets in Instance)
            {
                items.Invoke(roleAssets);
            }
        }
        public IEnumerator<RoleAssets> GetEnumerator()
        {
            return new RoleDBEnumerator(resources);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class RoleDBEnumerator:IEnumerator<RoleAssets>
        {

            private readonly List<RoleAssets> _roleAssetsList;
            private int _index = -1;
            public RoleDBEnumerator(List<RoleAssets> roleAssetsList)
            {
                _roleAssetsList = roleAssetsList;
            }
            public bool MoveNext()
            {
                return ++_index < _roleAssetsList.Count;
            }

            public void Reset()
            {
                _index = -1;
            }

            public RoleAssets Current => _roleAssetsList[_index];

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _roleAssetsList.Clear();
            }
        }

        
    }
}