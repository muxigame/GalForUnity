
//
// using GalForUnity.Attributes;
// using GalForUnity.Model;
// using GalForUnity.System;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.UI;
// using NotImplementedException = System.NotImplementedException;
//
// namespace GalForUnity.Controller{
//     // [RequireComponent(typeof(GfuInstance))]
//     public class OptionController : GfuSavableMonoInstanceManager<OptionController>{
//         [Rename(nameof(optionsPool))]
//         public GfuObjectPool optionsPool;
//         [Rename(nameof(autoSize))]
//         public bool autoSize = false;
//         [Rename(nameof(optionSpace))]
//         [Range(0.5f, 1f)] 
//         public float optionSpace = 0.8f;
//         [Rename(nameof(optionViewType))]
//         public OptionViewType optionViewType;
//         [RenameInEditor(nameof(customView))]
//         public UnityEvent<GfuOptions> customView;
//         public UnityEvent<string> onSelect=new UnityEvent<string>();
//         // [SerializeField]
//         // private bool showing=false;
//
//         public enum OptionViewType{
//             [Rename(nameof(Horizontal))]
//             Horizontal,
//             [Rename(nameof(Vertical))]
//             Vertical
//         }
//
//         public void InitialView(){
//             if (optionsPool == null) optionsPool = gameObject.AddComponent<GfuObjectPool>();
//             if (optionsPool.obj == null) optionsPool.obj = Resources.Load<GameObject>("Prefabs/GfuDemoOptionView");
//             if (!gameObject.TryGetComponent(out RectTransform rectTransform)){
//                 rectTransform = gameObject.AddComponent<RectTransform>();
//             }
//             rectTransform.anchorMax=new Vector2(1,1);
//             rectTransform.anchorMin=new Vector2(0,0);
//             rectTransform.offsetMin = new Vector2(0,0);
//             rectTransform.offsetMax=new Vector2(0,0);
//         }
//         
//         public void ShowOption(GfuOptions gfuOptionsData){
//             if (customView != null&&customView.GetPersistentEventCount()>0){
//                 customView.Invoke(gfuOptionsData);
//                 return;
//             }
//             if (optionViewType == OptionViewType.Horizontal){
//                 Horizontal(gfuOptionsData);
//             } else if (optionViewType == OptionViewType.Vertical){
//                 Vertical(gfuOptionsData);
//             }
//         }
//         public void HideOption(){
//             optionsPool.PutAll();
//         }
//         private void Horizontal(GfuOptions gfuOptions){
//             for (var i = 0; i < gfuOptions.options.Count; i++){
//                 var optionGameObject = optionsPool.Get(transform);
//                 Init(optionGameObject,gfuOptions,gfuOptions.options[i]);
//                 var optionTransform = optionGameObject.GetComponent<RectTransform>();
//                 var componentSizeDelta = optionTransform.sizeDelta;
//                 var interval = (Screen.width * optionSpace - componentSizeDelta.x * gfuOptions.options.Count) / (gfuOptions.options.Count + 1);
//                 float positionX = Screen.width * (1 - optionSpace) / 2f + interval * (i + 1) + i * componentSizeDelta.x + componentSizeDelta.x / 2f;
//                 optionTransform.anchoredPosition = new Vector2(positionX, optionTransform.anchoredPosition.y);
//             }
//         }
//
//         private void Vertical(GfuOptions gfuOptions){
//             for (var i = 0; i < gfuOptions.options.Count; i++){
//                 var optionGameObject = optionsPool.Get(transform);
//                 Init(optionGameObject,gfuOptions,gfuOptions.options[i]);
//                 var optionTransform = optionGameObject.GetComponent<RectTransform>();
//                 var componentSizeDelta = optionTransform.sizeDelta;
//                 var interval = (Screen.height * optionSpace - componentSizeDelta.y * gfuOptions.options.Count) / (gfuOptions.options.Count + 1);
//                 float positionY = Screen.height * (1 - optionSpace) / 2f + interval * (i + 1) + i * componentSizeDelta.y + componentSizeDelta.y / 2f;
//                 optionTransform.anchoredPosition = new Vector2(optionTransform.anchoredPosition.x, positionY);
//             }
//         }
//
//         public void Init(GameObject optionGameObject,GfuOptions gfuOptions,GfuOptionData gfuOptionData){
//             RectTransform rectTransform = (RectTransform) optionGameObject.transform;
//             rectTransform.anchorMin = new Vector2(0, 0);
//             rectTransform.anchorMax = new Vector2(0, 0);
//             rectTransform.anchoredPosition3D = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
//             var button = optionGameObject.GetComponent<Button>();
//             var gfuOptionsModel = optionGameObject.GetComponent<GfuOptionsModel>();
//             gfuOptionsModel.GfuOptionData.text.text = gfuOptionData.optionContent;
//             gfuOptionsModel.GfuOptionData.index = gfuOptionData.index;
//             void Listener()
//             {
//                 onSelect.Invoke(gfuOptionsModel.GfuOptionData.text.text);
//                 optionsPool.PutAll();
//                 gfuOptions.OnSelect(gfuOptionsModel.GfuOptionData.index);
//             }
//             button.onClick.AddListener(Listener);
//         }
//
//
//         private (float, float) CalculateSize(OptionViewType otherOptionViewType, int count){
//             float screen;
//             if (optionViewType == OptionViewType.Horizontal){
//                 screen = Screen.width * 0.8f;
//             } else{
//                 screen = Screen.height * 0.8f;
//             }
//
//             var maxDistance = screen   / count;
//             var distance = maxDistance * 0.3f;
//             var size = maxDistance - distance;
//             return (distance, size);
//         }
//
//         // public override void GetObjectData(ScriptData scriptData){
//         //     base.GetObjectData(scriptData);
//         //     scriptData.priority = -2;
//         //     //TODO 未来请为优先级单独配一个配置表方便维护
//         // }
//         //
//         // public override void Recover(){
//         //     //如果显示选项状态保存游戏，游戏结束的时候会保存到选项节点的调用链，读档时选项节点会被执行重新显示选项
//         //     var componentsInChildren = GetComponentsInChildren<Button>();
//         //     foreach (var componentsInChild in componentsInChildren){
//         //         componentsInChild.onClick.RemoveAllListeners();
//         //     }
//         //     HideOption(); //但是如果在选项激活的时候读档，目标档没有显示选项，那就将选项隐藏
//         // }
//     }
// }