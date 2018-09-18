using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlotterScript : MonoBehaviour
{
    [SerializeField]
    AxisManager manager;

    public enum ViewingMode { Lissajous, Model4D, Graph3d, Graph2d }
    public ViewingMode viewingMode;

    // Use this for initialization
    void Start()
    {
        Vector3[] pointArray = Graph2DFigure(Lissajous1, 1000);
        GetComponent<LineRenderer>().positionCount = 1000;
        GetComponent<LineRenderer>().SetPositions(pointArray);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(manager.XScale, manager.YScale, manager.ZScale);
    }

    public Vector3 Lissajous1(int i, int numberPoints)
    {
        float t = (float)i / numberPoints * 10;
        Vector3 point = new Vector3();

        point.x = 3 * Mathf.Cos(5.2f * t) + 1.7f * Mathf.Cos(26 * t);
        point.y = 3 * Mathf.Sin(5.2f * t) + 1.7f * Mathf.Sin(26 * t);
        point.z = 3 * Mathf.Sin(5.2f * t) + 1.7f * Mathf.Cos(26 * t);
        return point;
    }

    public Vector3[] Graph2DFigure(Func<int, int, Vector3> formula, int numberPoints)
    {
        Vector3[] pointArray = new Vector3[numberPoints];

        for (int i = 0; i < numberPoints; i++)
        {
            pointArray[i] = formula(i, numberPoints);
        }

        return pointArray;
    }

    public Vector3[] Graph3DFigure(Func<int, int, Vector3> formula, int numberPoints)
    {
        Vector3[] pointArray = new Vector3[numberPoints];

        for (int i = 0; i < numberPoints; i++)
        {
            pointArray[i] = formula(i, numberPoints);
        }

        return pointArray;
    }
}
