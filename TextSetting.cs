using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSetting : MonoBehaviour
{

    TextMeshPro txt;

    void Start()
    {
        txt = GetComponent<TextMeshPro>();
    }

    void FixedUpdate()
    {
        txt.text = (Mathf.Floor(GameController.coins/10)).ToString(); //change text to current coin score
    }
}
