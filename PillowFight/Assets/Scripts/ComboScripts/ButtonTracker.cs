using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTracker : MonoBehaviour {
    GameManager gm;
    List<ComboManager> comboManagers = new List<ComboManager>();
    List<Player> players = new List<Player>();
    bool[] comboChecker = new bool[3];
    int timerLimit = 2;
    int buttonLimit = 3;

    void Start () {
        //get the gamemanager that contains the different players
        gm = gameObject.GetComponent<GameManager>();
        //get the combomanagers that contain the different combos
        foreach (Player p in gm.players) {
            ComboManager c = p.gameObject.GetComponent<ComboManager>();
            comboManagers.Add(c);
            players.Add(p);
        }
	}

	void Update () {
        foreach (Player p in gm.players) {
            if (Input.GetButtonDown(p.inputJump) && p.inputQueue.Count <= buttonLimit) {
                p.inputQueue.Add("Jump");
                p.inputTimer = 0;
                if (p.inputQueue.Count == buttonLimit) {
                    CheckForCombo();
                }
            }
            else if (Input.GetButtonDown(p.inputTRI) && p.inputQueue.Count <= buttonLimit) {
                p.inputQueue.Add("Slap");
                p.inputTimer = 0;
                if (p.inputQueue.Count == buttonLimit) {
                    CheckForCombo();
                }
            }
            else if (Input.GetButtonDown(p.inputR1) && p.inputQueue.Count <= buttonLimit) {
                p.inputQueue.Add("Throw");
                p.inputTimer = 0;
                if (p.inputQueue.Count == buttonLimit) {
                    CheckForCombo();
                }
            }
            else if (p.inputQueue.Count > 0) {
                p.inputTimer += Time.deltaTime;
            }
            else if (p.inputQueue.Count > buttonLimit) {
                p.inputQueue.RemoveAt(0);
            }
            if (p.inputTimer >= timerLimit) {
                p.inputQueue.Clear();
                p.inputTimer = 0;
            }
        }
    }

    void CheckForCombo() {
        //for every player
        for (int i = 0; i < players.Count; i++) {
            //for every combo
            for (int j = 0; j < comboManagers[i].comboData.Count; j++) {
                //for every button
                for (int k = 0; k < comboManagers[i].comboData[j].buttons.Length; k++) {
                    //make sure the queue isnt empty
                    if (players[i].inputQueue.Count > 0) {
                        //check if all buttons are equal
                        if (comboManagers[i].comboData[j].buttons[k].ToString() == players[i].inputQueue[k]) {
                            comboChecker[k] = true;
                        }
                        else {
                            comboChecker[k] = false;
                        }
                    }
                }
                //check if all the moves line up
                int correctMoves = 0;
                for (int k = 0; k < comboChecker.Length; k++) {
                    if (comboChecker[k]) {
                        correctMoves++;
                    }
                }
                if (correctMoves == comboChecker.Length) {
                    Debug.Log("Combo achieved!");
                    players[i].inputQueue.Clear();
                }
            }

        }
    }
}
