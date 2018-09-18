﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisManager : MonoBehaviour {

    public GameObject axisLabelPrefab;
    public int labelCount = 5;

    private AxisLabel[][] labelList;
    private AxisLabel[] axisNameLabels;

    float xMax = 10;
    float yMax = 10;
    float zMax = 10;

    float xScale = 1;
    float yScale = 1;
    float zScale = 1;

    bool reLabelX = false;
    bool reLabelY = false;
    bool reLabelZ = false;

	// Use this for initialization
	void Start () {
        labelList = new AxisLabel[3][];
        labelList[0] = new AxisLabel[labelCount * 2 + 1];
        labelList[1] = new AxisLabel[labelCount * 2 + 1];
        labelList[2] = new AxisLabel[labelCount * 2 + 1];

        axisNameLabels = new AxisLabel[3];
        axisNameLabels[0] = GameObject.Instantiate(axisLabelPrefab, new Vector3(1, 0, 0), Quaternion.identity, transform).GetComponent<AxisLabel>();
        axisNameLabels[1] = GameObject.Instantiate(axisLabelPrefab, new Vector3(0, 1, 0), Quaternion.identity, transform).GetComponent<AxisLabel>();
        axisNameLabels[2] = GameObject.Instantiate(axisLabelPrefab, new Vector3(0, 0, 1), Quaternion.identity, transform).GetComponent<AxisLabel>();

        axisNameLabels[0].SetText("x");
        axisNameLabels[1].SetText("y");
        axisNameLabels[2].SetText("z");

        for (int i = -labelCount; i <= labelCount; i++)
        {
            labelList[0][i + labelCount] = GameObject.Instantiate(axisLabelPrefab, new Vector3(i, 0, 0), Quaternion.identity, transform).GetComponent<AxisLabel>();
            labelList[1][i + labelCount] = GameObject.Instantiate(axisLabelPrefab, new Vector3(0, i, 0), Quaternion.identity, transform).GetComponent<AxisLabel>();
            labelList[2][i + labelCount] = GameObject.Instantiate(axisLabelPrefab, new Vector3(0, 0, i), Quaternion.identity, transform).GetComponent<AxisLabel>();
        }
        reLabelX = reLabelY = reLabelZ = true;
        ReLabel();
		
	}
    

    public void ReLabel()
    {
        if (reLabelX)
        {
            axisNameLabels[0].transform.position = new Vector3(xMax / labelCount * (labelCount + 1), 0, 0);
        }
        if (reLabelY)
        {
            axisNameLabels[1].transform.position = new Vector3(0, yMax / labelCount * (labelCount + 1), 0);
        }
        if (reLabelZ)
        {
            axisNameLabels[2].transform.position = new Vector3(0, 0, zMax / labelCount * (labelCount + 1));
        }

        for (int i = -labelCount; i <= labelCount; i++)
        {
            if (reLabelX)
            {
                labelList[0][i + labelCount].transform.position = new Vector3(xMax / labelCount * i, 0, 0);
                labelList[0][i + labelCount].SetText(xScale / labelCount * i);
            }

            if (reLabelY)
            {
                labelList[1][i + labelCount].transform.position = new Vector3(0, yMax / labelCount * i, 0);
                labelList[1][i + labelCount].SetText(yScale / labelCount * i);
            }

            if (reLabelZ)
            {
                labelList[2][i + labelCount].transform.position = new Vector3(0, 0, zMax / labelCount * i);
                labelList[2][i + labelCount].SetText(zScale / labelCount * i);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (reLabelX || reLabelY || reLabelZ)
        {
            ReLabel();
        }
        reLabelX = reLabelY = reLabelZ = false;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (Input.GetKey(KeyCode.X))
            {
                XScale *= 1.01f;
            }
            if (Input.GetKey(KeyCode.Y))
            {
                YScale *= 1.01f;
            }
            if (Input.GetKey(KeyCode.Z))
            {
                ZScale *= 1.01f;
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.X))
            {
                XScale /= 1.01f;
            }
            if (Input.GetKey(KeyCode.Y))
            {
                YScale /= 1.01f;
            }
            if (Input.GetKey(KeyCode.Z))
            {
                ZScale /= 1.01f;
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.X))
            {
                XMax *= 1.01f;
            }
            if (Input.GetKey(KeyCode.Y))
            {
                YMax *= 1.01f;
            }
            if (Input.GetKey(KeyCode.Z))
            {
                ZMax *= 1.01f;
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.X))
            {
                XMax /= 1.01f;
            }
            if (Input.GetKey(KeyCode.Y))
            {
                YMax /= 1.01f;
            }
            if (Input.GetKey(KeyCode.Z))
            {
                ZMax /= 1.01f;
            }
        }
    }

    public float XScale
    {
        get
        {
            return xScale;
        }
        set
        {
            xScale = value;
            reLabelX = true;
        }
    }
    public float YScale
    {
        get
        {
            return yScale;
        }
        set
        {
            yScale = value;
            reLabelY = true;
        }
    }
    public float ZScale
    {
        get
        {
            return zScale;
        }
        set
        {
            zScale = value;
            reLabelZ = true;
        }
    }

    public float XMax
    {
        get
        {
            return xMax;
        }
        set
        {
            xMax = value;
            reLabelX = true;
        }
    }
    public float YMax
    {
        get
        {
            return yMax;
        }
        set
        {
            yMax = value;
            reLabelY = true;
        }
    }
    public float ZMax
    {
        get
        {
            return zMax;
        }
        set
        {
            zMax = value;
            reLabelZ = true;
        }
    }

}
