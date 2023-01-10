using UnityEngine;

namespace GalForUnity.Graph.Build{
    public class GetCloth : MonoBehaviour{
        public int clothIndex = 0;
        public SkinnedMeshRenderer[] clothRender;
        public Texture[] cloth;

        private void Awake(){
            if (PlayerPrefs.HasKey("Cloth")){
                clothIndex = PlayerPrefs.GetInt("Cloth");
            }

            clothRender = this.GetComponentsInChildren<SkinnedMeshRenderer>();

        }

        private void Start(){
            Debug.Assert(clothRender        != null, "clothRender != null");
            Debug.Assert(clothRender.Length != 0, "clothRender.Length!=0");
            Debug.Assert(cloth              != null, "cloth != null");
            Debug.Assert(cloth.Length       != 0, "cloth.Length!=0");
            for (int i = 0; i < clothRender.Length; i++){
                Debug.Assert(clothRender[i]?.material != null, "clothRender[i]?.material != null");
                Debug.Assert(cloth[clothIndex]        != null, "cloth[clothIndex] != null");
                clothRender[i].material.mainTexture = cloth[clothIndex];
            }
        }
    }
}