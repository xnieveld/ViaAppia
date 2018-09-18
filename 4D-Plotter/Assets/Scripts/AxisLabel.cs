using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Label on the axis.
/// </summary>
public class AxisLabel : MonoBehaviour
{

    private Text text;
    
    void Awake()
    {
        text = GetComponent<Text>();

    }
    
    /// <summary>
    /// Set the text to a number, it should keep track of the correct number of decimal points.
    /// </summary>
    /// <param name="number"></param>
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

    /// <summary>
    /// Set the text to actual text
    /// </summary>
    /// <param name="name"></param>
    public void SetText(string name)
    {
        text.text = name;
        text.fontSize = 20;
    }


    /// <summary>
    /// Face the camera.
    /// </summary>
    public void Update()
    {
        transform.rotation = Quaternion.LookRotation((transform.position - Camera.main.transform.position), Vector3.up);
    }
}
