

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.Framework{
    public class GfuObjectPool : MonoBehaviour{
        public static readonly Dictionary<string,GfuObjectPool> Pools=new Dictionary<string, GfuObjectPool>();

        private void Awake(){
            if (Pools.ContainsKey(gameObject.name)) Pools.Remove(gameObject.name);
            Pools.Add(gameObject.name,this);
        }
        public GameObject obj;
        // [NonSerialized]
        public Stack<GameObject> readyGameObjects=new Stack<GameObject>();
        // [NonSerialized]
        public List<GameObject> playingGameObjects=new List<GameObject>();

        public GameObject Get(UnityEngine.Transform otherTransform,bool rotation=true,bool scale=true){
            if (readyGameObjects.Count == 0){
                var instantiate = GameObject.Instantiate(obj, otherTransform, true);
                instantiate.transform.position = otherTransform.position;
                if(rotation) instantiate.transform.rotation = otherTransform.rotation;
                if(scale) instantiate.transform.localScale = otherTransform.localScale;
                playingGameObjects.Add(instantiate);
                transform.SetParent(instantiate.transform.parent);
                instantiate.SetActive(true);
                return instantiate;
            }else{
                var pop = readyGameObjects.Pop();
                pop.transform.position = otherTransform.position;
                if(rotation) pop.transform.rotation = otherTransform.rotation;
                if(scale) pop.transform.localScale = otherTransform.localScale;
                playingGameObjects.Add(pop);
                pop.SetActive(true);
                return pop;
            }
        }

        public void Put(GameObject gameObj){
            gameObj.SetActive(false);
            if (playingGameObjects.Contains(gameObj)){
                playingGameObjects.Remove(gameObj);
                readyGameObjects.Push(gameObj);
            } else{
                gameObj.transform.parent = transform;
                readyGameObjects.Push(gameObj);
            }
        }
        public void PutAll(){
            foreach (var playingGameObject in playingGameObjects){
                playingGameObject.SetActive(false);
                playingGameObject.GetComponent<Button>()?.onClick.RemoveAllListeners();
            }
            for (var i = 0; i <playingGameObjects.Count; i++){
                if (!readyGameObjects.Contains(playingGameObjects[i])){
                    readyGameObjects.Push(playingGameObjects[i]);
                }
                
            }

            playingGameObjects.Clear();
        }
        private void OnDestroy(){
            Pools.Clear();
        }
    }
}
