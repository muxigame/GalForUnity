//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalAudioConfig.cs Created at 2022-09-27 23:33:34
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Attributes;
using GalForUnity.System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
[Serializable]
[Rename("Audio Config")]
public class GalAudioConfig : IGalConfig<AudioSource>{
    [Rename("")]
    public bool? bypassEffect;
    [Rename("")]
    public bool? loop;
    [Rename("")]
    public bool? mute;
    [Rename("")]
    public float? pitch;
    [Rename("")]
    public int? priority;
    [Rename("")]
    public float? spatialBlend;
    [Rename("")]
    public float? stereoPan;
    [Rename("")]
    public float? volume;

    public void Process(AudioSource t){
        if (loop         != null) t.loop = (bool) loop;
        if (mute         != null) t.mute = (bool) mute;
        if (bypassEffect != null) t.bypassEffects = (bool) bypassEffect;
        if (volume       != null) t.volume = (float) volume;
        if (priority     != null) t.priority = (int) priority;
        if (pitch        != null) t.pitch = (float) pitch;
        if (stereoPan    != null) t.panStereo = (float) stereoPan;
        if (spatialBlend != null) t.spatialBlend = (float) spatialBlend;
    }
}
public class AudioConfigSearchTypeProvider : ScriptableObject, ISearchWindowProvider{
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
        var entries = new List<SearchTreeEntry>();
        try{
            entries.Add(new SearchTreeGroupEntry(new GUIContent(GfuLanguage.GfuLanguageInstance.CHANGETYPE.Value))); //添加了一个一级菜单
            var childTypes = GetChildTypes(typeof(GalAudioConfig));
            //从程序集中找到GfuNode的所有子类，并且遍历显示到目录当中
            foreach (var childType in childTypes){
                entries.Add(new SearchTreeEntry(new GUIContent(childType.Name)) {
                    level = 1, userData = childType
                });
            }
        } catch (Exception e){
            Debug.LogError(e);
        }
        return entries;
    }

    public delegate bool SearchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry, SearchWindowContext context);

    public SearchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler;

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context){
        if (OnSelectEntryHandler == null){
            return false;
        }
        return OnSelectEntryHandler(searchTreeEntry, context);
    }
    private FieldInfo[] GetChildTypes(Type parentType){
        return parentType.GetFields();
    }
}
public interface IGalConfig<T>:IGalConfig{
    void Process(T t);
}

public interface IGalConfig{
    
}