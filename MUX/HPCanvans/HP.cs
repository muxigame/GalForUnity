using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MUX.HPCanvans{
    public class HP : MonoBehaviour, IEventSystemHandler{

        [Header("HP Color")]
        [Tooltip("濒死血量颜色")]
        public Color near_death_color = new Color(1, 0, 0);
        [Tooltip("正常血量颜色")]
        public Color health_color = new Color(1, 1, 1);
        [Tooltip("不正常异常区间时血量的颜色")]
        public Color unknomn_health = new Color(0, 0, 0);
        [Header("Setting")]
        [Range(0, 1)]
        [Tooltip("血条衰减速度")]
        public float sub_health_speed = 0.7f;
        public interface IHPEvent : IEventSystemHandler {
            void SubValue(float harm);
            void AddValue(float treatment);
            void SetValue(float health);
        }
        Slider slider;
        void Start() {
            slider = transform.GetComponent<Slider>();
            if (!slider) {
                throw new MissingComponentException("Component isn't exits");
                //slider = gameObject.AddComponent<Slider>();
            }

            Invoke("Init", 0.5f);//初始化血量
        }
    
        void Update() {
            transform.forward = Camera.main.transform.forward;
        }
        private void Init() {
            SetValue(1);
        }

        void OnGUI() {
            SetHealthColor();//我们希望血量变色不会因为外部修改而失灵，比如说内存修改器
        }


        float new_health;

        public void SubValue(float harm) {
            new_health = slider.value - harm;
            StartCoroutine("Linear");

        }

        public void SetValue(float health) {
            new_health = health;
            StartCoroutine("Linear");
        }

        public void AddValue(float treatment) {
            new_health = slider.value + treatment;
            StartCoroutine("Linear");
        }
        public void SetHealthColor() {
            if (slider.value < 0.3f) {
                slider.fillRect.GetComponent<Image>().color = near_death_color;
            } else if (slider.value >= 0.3f && slider.value <= 1) {
                slider.fillRect.GetComponent<Image>().color = health_color;
            } else {
                slider.fillRect.GetComponent<Image>().color = unknomn_health;
            }
        }
        /// <summary>
        /// 原本血量的平滑过渡的代码写在Player里，但是现在我们希望平滑过渡是一项血条搭载的固定功能
        /// </summary>
        /// <param name="new_health">要过渡到的新血量</param>
        /// <returns></returns>
        private IEnumerator Linear() {
            while (new_health != slider.value) {
                slider.value = Mathf.MoveTowards(slider.value, new_health, Time.deltaTime * sub_health_speed);
                yield return null;
            }
        }
    }
}
