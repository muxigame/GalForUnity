using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Direction = GalForUnity.Graph.SceneGraph.Direction;

namespace GalForUnity.Graph.Build{
    // public class PortList{
    //     private List<(Type,GfuPort)> _ports=new List<(Type, GfuPort)>();
    //     public object this[int index]{
    //         get{
    //             
    //         }
    //         set{
    //             if ((uint) index >= (uint) _ports.Count)
    //                 throw new ArgumentOutOfRangeException();
    //             var valueAttributes = value.;
    //             _ports[index] = (value,GfuPort.Create<Edge>());
    //         }
    //     }
    // }
    public class PortList<T> : BindableElement, INotifyValueChanged<List<T>> where T : Port{
        private List<T> _value;

        public PortList(IEnumerable<T> list, Direction direction){
            if (direction == Direction.Output)
                style.flexDirection = FlexDirection.ColumnReverse;
            else
                style.flexDirection = FlexDirection.Column;
            style.minWidth = 0;

            value = list.ToList();
            // List<>
        }

        public List<T> ContentToList => contentContainer.Children().Cast<T>().ToList();
        public void SetValueWithoutNotify(List<T> newValue){ value = newValue; }

        public List<T> value{
            get{
                Check();
                return _value;
            }
            set{
                _value = value;
                if (_value == null || _value.Count == 0){
                    contentContainer.Clear();
                    return;
                }

                for (var i = 0; i < _value.Count; i++){
                    if (contentContainer.Contains(_value[i])){
                        var indexOf = contentContainer.IndexOf(_value[i]);
                        var visualElement = contentContainer[indexOf];
                        contentContainer.RemoveAt(indexOf);
                        contentContainer.Insert(i, visualElement);
                    } else{
                        contentContainer.Insert(i, _value[i]);
                    }

                    for (var j = _value.Count; j < contentContainer.childCount; j++) contentContainer.RemoveAt(j);
                }
            }
        }

        public void Add(T port){
            using var pooled = ChangeEvent<List<T>>.GetPooled(ContentToList, _value);
            _value.Add(port);
            contentContainer.Add(port);
            SendEvent(pooled);
        }

        public void Remove(T port){
            using var pooled = ChangeEvent<List<T>>.GetPooled(ContentToList, _value);
            _value.Remove(port);
            contentContainer.Remove(port);
            SendEvent(pooled);
        }

        public new void RemoveAt(int index){
            using var pooled = ChangeEvent<List<T>>.GetPooled(ContentToList, _value);
            _value.RemoveAt(index);
            contentContainer.RemoveAt(index);
            SendEvent(pooled);
        }


        private async void Check(){
            await Task.Yield();
            using (var pooled = ChangeEvent<List<T>>.GetPooled(ContentToList, _value)){
                if (contentContainer.childCount != _value.Count)
                    SendEvent(pooled);
                else
                    for (var i = 0; i < contentContainer.childCount; i++)
                        if (!contentContainer[i].Equals(_value[i]) && !ReferenceEquals(contentContainer[i], _value[i])){
                            SendEvent(pooled);
                            return;
                        }
            }
        }
    }
}