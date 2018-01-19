using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateComboDataBase {

    [MenuItem("Custom/Create Combo Database")]
    public static ComboDataBase Create() {
        ComboDataBase asset = ScriptableObject.CreateInstance<ComboDataBase>();

        AssetDatabase.CreateAsset(asset, "Assets/ComboDataBase.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }

    
    public static ComboDataBase Create(DataBaseXML temp, ComboDataBase db) {
        ComboDataBase asset = ScriptableObject.CreateInstance<ComboDataBase>();
        asset.combos = temp.combos;
        AssetDatabase.CreateAsset(asset, "Assets/" + db + ".asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}

