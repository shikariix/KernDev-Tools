using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour {
    //list of all combos used by a single character
    public List<string> Combos = new List<string>();
    public ComboDataBase db;

    public List<Combo> comboData = new List<Combo>();

    void Awake() { 
        foreach (string s in Combos) {
            for (int i = 0; i < db.combos.Count; i++) {
                if (s == db.combos[i].comboName) {
                    comboData.Add(db.combos[i]);
                }
            }
        }
    }
}