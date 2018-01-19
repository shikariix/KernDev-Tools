using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Xml.Serialization;
using System.IO;

public class ComboEditor : EditorWindow {
    
    public ComboDataBase db;
    private int viewIndex = 1;

    private DataBaseXML xml;
    private DataBaseXML temp;


    [MenuItem("Custom/Combo Editor")]
    static void Init() {
        // Get existing open window or if none, make a new one:
        ComboEditor window = (ComboEditor)EditorWindow.GetWindow(typeof(ComboEditor));
        window.Show();
    }

    void OnEnable() {
        if (temp == null) {
            temp = new DataBaseXML();
            if (EditorPrefs.HasKey("ObjectPath")) {
                string objectPath = EditorPrefs.GetString("ObjectPath");
                db = AssetDatabase.LoadAssetAtPath(objectPath, typeof(ComboDataBase)) as ComboDataBase;
               
                if (temp.combos == null) {
                     temp.combos = db.combos;
                }
            }
        }
        

    }

    void OnGUI() {
        GUILayout.Label("Edit Combo's", EditorStyles.boldLabel);

        //buttons for saving & loading XML
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Load", GUILayout.MaxWidth(40))) {
            Debug.Log("Loading from Asset file.");
            OpenDataBase();
        }
        if (GUILayout.Button("Save", GUILayout.MaxWidth(40))) {
            Debug.Log("Saved new asset.");
            SaveDataBase();
        }
        if (GUILayout.Button("Export", GUILayout.MaxWidth(50))) {
            Debug.Log("Saving current changes to XML. You can find the file in StreamingAssets.");
            ExportDataBase();
        }
        if (GUILayout.Button("Import", GUILayout.MaxWidth(60))) {
            ImportDataBase();
        }
        if (GUILayout.Button("Convert", GUILayout.MaxWidth(90))) {
            Debug.Log("Convert an ItemData XML file to a combo database.");
            ImportOther();
        }
        GUILayout.EndHorizontal();

        if (temp == null) {
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
            
        if (temp != null) {
            GUILayout.BeginHorizontal ();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false))) {
                if (viewIndex > 1)
                    viewIndex --;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false))) {
                if (viewIndex < temp.combos.Count) {
                    viewIndex ++;
                }
            }
            
            GUILayout.Space(60);

            //buttons for adding and removing combos
            if (GUILayout.Button("+", GUILayout.MaxWidth(20))) {
                AddCombo();
            }
            if (GUILayout.Button("-", GUILayout.MaxWidth(20))) {
                DeleteCombo(viewIndex-1);
            }

            GUILayout.EndHorizontal ();
            if (temp.combos == null)
                Debug.Log("wtf");
            if (temp.combos.Count > 0) {
                GUILayout.BeginHorizontal();
                viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Item", viewIndex, GUILayout.ExpandWidth(false)), 1, temp.combos.Count);
                EditorGUILayout.LabelField("of   " + temp.combos.Count.ToString() + "  items", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();

                temp.combos[viewIndex - 1].comboName = EditorGUILayout.TextField("Combo Name", temp.combos[viewIndex - 1].comboName, GUILayout.MaxWidth(position.width - 6));
                //pas dit aan naar range
                temp.combos[viewIndex - 1].pillowAmount = EditorGUILayout.IntField("Objects to use", temp.combos[viewIndex - 1].pillowAmount, GUILayout.MaxWidth(position.width - 6));
                GUILayout.BeginVertical();
                int offset = 122;
                for (int i = 0; i < temp.combos[viewIndex - 1].buttons.Length; i++) {
                    temp.combos[viewIndex - 1].buttons[i] = (Combo.BUTTON)EditorGUI.EnumPopup(
                    new Rect(4, offset, position.width - 6, 15),
                    "Button "+ i + ":",
                    temp.combos[viewIndex - 1].buttons[i]);
                    offset += 18;
                }
            } 
            else {
                GUILayout.Label ("This Combo Database is Empty."); //YEET
            }
        }

    }

    void CreateNewDataBase() {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        db = CreateComboDataBase.Create(temp, db);
        if (db) {
            db.combos = new List<Combo>();
            string relPath = AssetDatabase.GetAssetPath(db);
            EditorPrefs.SetString("ObjectPath", relPath);
        }
        temp.combos = db.combos;
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
            temp.combos = db.combos;
            Debug.Log("Load succesful.");
        }
    }

    void SaveDataBase() {
        //save settings to database that's being edited rn
        db = CreateComboDataBase.Create(temp, db);
    }

    void ImportDataBase() {
        string absPath = EditorUtility.OpenFilePanel ("Select Combo Database XML", "", "");
        XmlSerializer serializer = new XmlSerializer(typeof(DataBaseXML));
        FileStream stream = null;
        stream = new FileStream(absPath, FileMode.Open);
        xml = serializer.Deserialize(stream) as DataBaseXML;
        temp.combos = xml.combos;
        stream.Close();
    }

    void ExportDataBase() {
        XmlSerializer serializer = new XmlSerializer(typeof(DataBaseXML));
        FileStream stream = new FileStream(Application.streamingAssetsPath + "/ComboData.xml", FileMode.Create);
        xml = new DataBaseXML();
        xml.combos = temp.combos;
        serializer.Serialize(stream, xml);
        stream.Close();
    }

    void ImportOther() {
        string absPath = EditorUtility.OpenFilePanel ("Select Combo Database XML", "", "");
        XmlSerializer serializer = new XmlSerializer(typeof(ItemData));
        FileStream stream = null;
        stream = new FileStream(absPath, FileMode.Open);
        ItemData data = serializer.Deserialize(stream) as ItemData;
        temp.combos = new List<Combo>();
        foreach (Item i in data.Data.Item) {
            Combo c = new Combo();
            c.comboName = i.Name;
            c.pillowAmount = i.AmoutOfTags;

            //Set buttons based on strings in the items
            char startC = i.Category[0];
            if (startC == 'A' || 
                startC == 'B' || 
                startC == 'C' || 
                startC == 'D' || 
                startC == 'E' || 
                startC == 'G' || 
                startC == 'H' || 
                startC == 'I') {
                c.buttons[0] = Combo.BUTTON.Slap;
            } else if (
                startC == 'J' ||
                startC == 'F' ||
                startC == 'K' ||
                startC == 'L' ||
                startC == 'M' ||
                startC == 'N' ||
                startC == 'O' ||
                startC == 'P' ||
                startC == 'Q' ||
                startC == 'R') {
                c.buttons[0] = Combo.BUTTON.Jump;
            } else {
                c.buttons[0] = Combo.BUTTON.Throw;
            }

            if (i.PrefabName.Contains("00")) {
                c.buttons[1] = Combo.BUTTON.Slap;
            } else if (i.PrefabName.Contains("01")) {
                c.buttons[1] = Combo.BUTTON.Jump;
            } else {
                c.buttons[1] = Combo.BUTTON.Throw;
            }

            char startD = i.Description[0];
            if (startD == 'A' ||
                startD == 'B' ||
                startD == 'C' ||
                startD == 'D' ||
                startD == 'E' ||
                startD == 'F' ||
                startD == 'G' ||
                startD == 'H' ||
                startD == 'I') {
                c.buttons[0] = Combo.BUTTON.Throw;
            }
            else if (
              startD == 'J' ||
              startD == 'K' ||
              startD == 'L' ||
              startD == 'M' ||
              startD == 'N' ||
              startD == 'O' ||
              startD == 'P' ||
              startD == 'Q' ||
              startD == 'R') {
                c.buttons[0] = Combo.BUTTON.Jump;
            }
            else {
                c.buttons[0] = Combo.BUTTON.Slap;
            }

            //when all info has been set, the new combo can be added to the combo database
            temp.combos.Add(c);
        }
    }

    void AddCombo() {
        Combo newItem = new Combo();
        newItem.comboName = "Default";
        temp.combos.Add(newItem);
    }

    void DeleteCombo(int index) {
        temp.combos.RemoveAt(index);
    }
}