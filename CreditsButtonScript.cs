using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsButtonScript : MonoBehaviour
{
    public GameObject panel;
    public void Deactivate()
    {
        panel.SetActive(false);
    }

    public void Activate()
    {
        panel.SetActive(true);
    }
}