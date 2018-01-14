using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Xml.Serialization;
using System.IO;

public class ComboEditor : EditorWindow {
    
    //private XMLManager xml;
    public ComboDataBase db;
    private int viewIndex = 1;

    private DataBaseXML xml;


    [MenuItem("Custom/Combo Editor")]
    static void Init() {
        // Get existing open window or if none, make a new one:
        ComboEditor window = (ComboEditor)EditorWindow.GetWindow(typeof(ComboEditor));
        window.Show();
    }

    void OnEnable() {
        if (EditorPrefs.HasKey("ObjectPath")) {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            db = AssetDatabase.LoadAssetAtPath(objectPath, typeof(ComboDataBase)) as ComboDataBase;
        }

    }

    void OnGUI() {
        GUILayout.Label("Edit Combo's", EditorStyles.boldLabel);

        //buttons for saving & loading XML
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Load", GUILayout.MaxWidth(50))) {
            Debug.Log("Loading from Asset file.");
            OpenDataBase();
        }
        if (GUILayout.Button("Save", GUILayout.MaxWidth(50))) {
            Debug.Log("Saving changes to database is currently not implemented.");
        }
        if (GUILayout.Button("Export", GUILayout.MaxWidth(60))) {
            Debug.Log("Saving current changes to XML. The database will not be overwritten.");
            ExportDataBase();
        }
        if (GUILayout.Button("Import", GUILayout.MaxWidth(60))) {
            ImportDataBase();
        }
        GUILayout.EndHorizontal();

        if (db == null) {
            GUILayout.BeginHorizontal ();
            GUILayout.Space(10);
            if (GUILayout.Button("Create New Item List", GUILayout.ExpandWidth(false))) {
                CreateNewDataBase();
            }
            if (GUILayout.Button("Open Existing Item List", GUILayout.ExpandWidth(false))) {
                OpenDataBase();
            }
            GUILayout.EndHorizontal ();
        }
            
            GUILayout.Space(20);
            
        if (db != null) {
            GUILayout.BeginHorizontal ();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false))) {
                if (viewIndex > 1)
                    viewIndex --;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false))) {
                if (viewIndex < db.combos.Count) {
                    viewIndex ++;
                }
            }
            
            GUILayout.Space(60);

            //buttons for adding and removing combos
            if (GUILayout.Button("+", GUILayout.MaxWidth(20))) {
                AddCombo();
            }
            if (GUILayout.Button("-", GUILayout.MaxWidth(20))) {
                DeleteCombo(viewIndex);
            }

            GUILayout.EndHorizontal ();
            if (db.combos == null)
                Debug.Log("wtf");
            if (db.combos.Count > 0) {
                GUILayout.BeginHorizontal();
                viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Item", viewIndex, GUILayout.ExpandWidth(false)), 1, db.combos.Count);
                EditorGUILayout.LabelField("of   " + db.combos.Count.ToString() + "  items", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
                
                db.combos[viewIndex - 1].comboName = EditorGUILayout.TextField("Combo Name", db.combos[viewIndex - 1].comboName, GUILayout.MaxWidth(position.width - 6));
                db.combos[viewIndex - 1].pillowAmount = EditorGUILayout.IntField("Pillows to use", db.combos[viewIndex - 1].pillowAmount, GUILayout.MaxWidth(position.width - 6));
                GUILayout.BeginVertical();
                int offset = 142;
                for (int i = 0; i < db.combos[viewIndex - 1].buttons.Length; i++) {
                    db.combos[viewIndex - 1].buttons[i] = (Combo.BUTTON)EditorGUI.EnumPopup(
                    new Rect(4, offset, position.width - 6, 15),
                    "Button "+ i + ":",
                    db.combos[viewIndex - 1].buttons[i]);
                    offset += 18;
                }
            } 
            else 
            {
                GUILayout.Label ("This Inventory List is Empty.");
            }
        }
        if (GUI.changed) 
        {
            EditorUtility.SetDirty(db);
        }
        

    }

    void CreateNewDataBase() {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        db = CreateComboDataBase.Create();
        if (db) {
            db.combos = new List<Combo>();
            string relPath = AssetDatabase.GetAssetPath(db);
            EditorPrefs.SetString("ObjectPath", relPath);
        }
    }

    void OpenDataBase() {
        string absPath = EditorUtility.OpenFilePanel ("Select Combo Database", "", "");
        if (absPath.StartsWith(Application.dataPath)) {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            db = AssetDatabase.LoadAssetAtPath(relPath, typeof(ComboDataBase)) as ComboDataBase;
            if (db.combos == null)
                db.combos = new List<Combo>();
            if (db) {
                EditorPrefs.SetString("ObjectPath", relPath);
            }
            Debug.Log("Load succesful.");
        }
    }

    void SaveDataBase() {
        //move currently loaded combos to a database in the project
    }

    void ImportDataBase() {
        string absPath = EditorUtility.OpenFilePanel ("Select Combo Database", "", "");
        XmlSerializer serializer = new XmlSerializer(typeof(DataBaseXML));
        FileStream stream = null;
        stream = new FileStream(absPath, FileMode.Open);
        xml = serializer.Deserialize(stream) as DataBaseXML;
        db = xml.db;
        stream.Close();
    }

    void ExportDataBase() {
        XmlSerializer serializer = new XmlSerializer(typeof(DataBaseXML));
        FileStream stream = new FileStream(Application.persistentDataPath + "/ComboData.xml", FileMode.Create);
        xml = new DataBaseXML();
        xml.db = db;
        serializer.Serialize(stream, xml);
        stream.Close();
    }

    void AddCombo() {
        Combo newItem = new Combo();
        newItem.comboName = "Default";
        db.combos.Add(newItem);
    }

    void DeleteCombo(int index) {
        db.combos.RemoveAt(index);
    }
}