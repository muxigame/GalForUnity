using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MUX.HPCanvans{
    public class EnemyHPCanvans : MonoBehaviour,HP.IHPEvent {

        public Vector2 anchors_Min = new Vector2(0, 0.25f);
        public Vector2 anchors_Max = new Vector2(1, 0.75f);
        public Vector2 size = new Vector2(0, 0);
        public Vector2 handle_size = new Vector2(0, 0);
        public Vector3 position = new Vector3(0, 0, 0);
        [Range(0, 1)]
        public float distance_parportion = 0.5f;
        public float distance = 1f;
        // Start is called before the first frame update
        Canvas canvas;
        Slider slider;
        Sprite background_photo;
        RectTransform slider_transform;
        MeshFilter me;
        Collider _collider;
        private GameObject hp;
        void Start() {
            init();//绝对丢失引用的初始化血条UI
        }



        //锁定对象
        private void LateUpdate() {
            if (me ==null) {
                slider_transform.anchoredPosition3D = transform.position + new Vector3(0, _collider.bounds.size.y / (2f - distance_parportion)+distance, 0);
            } else {
                slider_transform.anchoredPosition3D = transform.position + new Vector3(0, me.mesh.bounds.size.y / (2f - distance_parportion) + distance, 0);
            }
        
        }

        /// <summary>
        /// 适用于各版本无障碍迁移的血条UI创建方法
        /// </summary>
        private void init() {
            me = gameObject.GetComponent<MeshFilter>();
            if (me == null) {
                gameObject.AddComponent<BoxCollider>();
                _collider = gameObject.GetComponent<BoxCollider>();
            }
            background_photo = Resources.Load<Sprite>("health_value");
            if (GameObject.Find("MUXCanvans") == null) {
                canvas = new GameObject().AddComponent<Canvas>();
                canvas.gameObject.name = "MUXCanvans";
                canvas.renderMode = RenderMode.WorldSpace;
            } else {
                canvas = GameObject.Find("MUXCanvans").GetComponent<Canvas>();
            }
            GameObject slider_gameobject = new GameObject();
            slider_gameobject.name = "HP";
            slider_gameobject.transform.parent = canvas.transform;
            slider_gameobject.transform.position = new Vector3(0, 0, 0);
            slider_gameobject.AddComponent<RectTransform>();
            this.slider = slider_gameobject.AddComponent<Slider>();
            slider_transform = slider_gameobject.GetComponent<RectTransform>();
            slider_transform.sizeDelta = new Vector2(0.5f, 0.1f);
            GameObject fill = new GameObject();
            GameObject handle = new GameObject();
            GameObject background = new GameObject();
            GameObject fill_area = new GameObject();
            GameObject handle_area = new GameObject();


            background.transform.parent = slider_gameobject.transform;
            fill_area.transform.parent = slider_gameobject.transform;
            handle_area.transform.parent = slider_gameobject.transform;

            fill.transform.parent = fill_area.transform;
            handle.transform.parent = handle_area.transform;

            fill.name = "Fill";
            handle.name = "Handle";
            background.name = "Background";
            fill_area.name = "Fill_area";
            handle_area.name = "Handle_area";


            fill.AddComponent<Image>();
            handle.AddComponent<Image>();
            background.AddComponent<Image>();
            //fill.AddComponent<RectTransform>();
            //handle.AddComponent<RectTransform>();
            //background.AddComponent<RectTransform>();
            fill_area.AddComponent<RectTransform>();
            handle_area.AddComponent<RectTransform>();



            RectTransform background_rect = background.GetComponent<RectTransform>();
            RectTransform fill_rect = fill.GetComponent<RectTransform>();
            RectTransform handle_rect = handle.GetComponent<RectTransform>();
            RectTransform fill_area_rect = fill_area.GetComponent<RectTransform>();
            RectTransform handle_area_rect = handle_area.GetComponent<RectTransform>();

            slider.fillRect = fill_rect;
            slider.handleRect = handle_rect;




            fill_area_rect.anchorMax = anchors_Max;
            fill_area_rect.anchorMin = anchors_Min;
            handle_area_rect.anchorMax = anchors_Max;
            handle_area_rect.anchorMin = anchors_Min;
            background_rect.anchorMax = anchors_Max;
            background_rect.anchorMin = anchors_Min;

            fill_area_rect.offsetMin = new Vector2(0, 0);
            fill_area_rect.offsetMax = new Vector2(0, 0);
            handle_area_rect.offsetMin = new Vector2(0, 0);
            handle_area_rect.offsetMax = new Vector2(0, 0);
            background_rect.offsetMin = new Vector2(0, 0);
            background_rect.offsetMax = new Vector2(0, 0);


            slider.GetComponent<RectTransform>().anchoredPosition3D = position;
            fill_rect.anchoredPosition3D = new Vector3(0, 0, 0);
            handle_rect.anchoredPosition3D = new Vector3(0, 0, 0);
            background_rect.anchoredPosition3D = new Vector3(0, 0, 0);



            fill_rect.sizeDelta = size;
            handle_rect.sizeDelta = handle_size;
            background_rect.sizeDelta = size;


            background.GetComponent<Image>().sprite = background_photo;
            hp = slider_gameobject.AddComponent<HP>().gameObject;
        }

        public void SubValue(float harm){
            throw new System.NotImplementedException();
        }

        public void AddValue(float treatment){
            throw new System.NotImplementedException();
        }

        public void SetValue(float value){
            ExecuteEvents.Execute<HP>(hp, null, (xs, y) => { xs.SetValue(value);});//发送事件持续减血
        }
    }
}
