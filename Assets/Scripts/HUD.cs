using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] TMP_Text drons;
    [SerializeField] TMP_Text resource1;
    [SerializeField] TMP_Text resource2;
    [SerializeField] TMP_Text resource3;
    Player player;

    void Start(){
        player = FindObjectOfType<Player>();
    }

    void Update(){
        drons.text = "DRONS " + player.drons.ToString();
        resource1.text = "R1: " + player.resource1.ToString();
        resource2.text = "R2: " + player.resource2.ToString();
        resource3.text = "R3: " + player.resource3.ToString();
    }
}
