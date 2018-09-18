using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AxisLabel : MonoBehaviour
{

    private Text text;

    // Use this for initialization
    void Awake()
    {
        text = GetComponent<Text>();

    }

    public void SetText(float number) //TODO beter werkend maken
    {

        text.fontSize = 14;
        if ((Mathf.Round(number * 100) / 100) - number > 0.01)
        {
            text.text = number.ToString("0");
        }
        else if ((Mathf.Round(number * 10) / 10) - number > 0.1)
        {
            text.text = number.ToString("0.0");
        }
        else
        {
            text.text = number.ToString("0.00");
        }
    }

    public void SetText(string name) //TODO beter werkend maken
    {
        text.text = name;
        text.fontSize = 20;
    }

    public void Update()
    {
        transform.rotation = Quaternion.LookRotation((transform.position - Camera.main.transform.position), Vector3.up);
    }
}
