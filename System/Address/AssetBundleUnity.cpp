#include "UnityEngine"

Object* LoadNonNamedObjectFromAssetBundle (AssetBundle& bundle, const std::string& name, ScriptingObjectPtr type)
{
        LocalSerializedObjectIdentifier localID = bundle.GetLocalID(name);
        vector<Object*> result;
        ProcessAssetBundleEntries(bundle,localID,type,result,true);
        if (!result.empty())
                return result[0];
        return NULL;
}