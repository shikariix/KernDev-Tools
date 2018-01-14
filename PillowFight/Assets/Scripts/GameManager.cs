using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField]
    public List<Player> players = new List<Player>();
	
	// Update is called once per frame
	void Update () {
        foreach (Player p in players) {
            p.Move();
        }
    }
}
