using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextChanger : MonoBehaviour
{
    TextMeshPro tmpro;
    public string newTxt;
    public GameObject player;

    void Start()
    {
        tmpro = GetComponent<TextMeshPro>();
    }

    

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerMovement>().special != 0)
        {
            tmpro.text = newTxt;
        }
    }
}
