//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ColorSeeds.cs
//
//        Created by 半世癫(Roc) at 2021-04-13 20:09:30
//
//======================================================================

using System.Collections.Generic;
using UnityEngine;

namespace MUX.Support{
    public class ColorSeeds : MonoBehaviour{
        private static readonly List<Color> Colors = new List<Color>() {
            new Color(1, 1, 1),
            new Color(0.29f, 1f, 0.42f),
            new Color(1f, 0.99f, 0f),
            new Color(1f, 0.48f, 0f),
            new Color(1f, 0.33f, 0.31f),
            new Color(1f, 0.45f, 0.66f),
            new Color(0.73f, 0.34f, 0.87f),
            new Color(0.18f, 0.39f, 0.78f),
            new Color(0.4f, 1f, 0.97f),
            new Color(0.52f, 0.52f, 0.51f),
        };

        private static readonly Dictionary<string, int> Index = new Dictionary<string, int>();

        public static Color Get(string seeds = "", bool random = false){
            if (random) return Colors[Random.Range(0, Colors.Count - 1)];
            if (!Index.ContainsKey(seeds)) Index.Add(seeds, 0);
            if (Index[seeds] == Colors.Count) Index[seeds] = 0;
            return Colors[Index[seeds]++];
        }

        public static void Clear(){ Index.Clear();}

        private void OnEnable(){ Index.Clear(); }
    }
}
