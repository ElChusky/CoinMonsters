﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP_Bar_Control : MonoBehaviour
{
    private Color green = new Color(31f / 255, 255f / 255, 30f / 255);
    private Color yellow = new Color(255f / 255, 255f / 255, 30f / 255);
    private Color red = new Color(255f / 255, 30f / 255, 30f / 255);

    private void Update()
    { 
        if (transform.localScale.x <= 0.25f)
        { 
            GetComponent<Image>().color = red;
        } 
        else if (transform.localScale.x <= 0.5f)
        {
            GetComponent<Image>().color = yellow;
        } else
        {
            GetComponent<Image>().color = green;
        }
    }

    public void SetHp(float hpNormalized)
    {
        transform.localScale = new Vector3(hpNormalized, 1f);
    }
}
