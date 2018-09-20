using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Display the graphs/datasets.
/// </summary>
public class PlotterScript : MonoBehaviour
{
    [SerializeField]
    AxisManager manager;

    public enum ViewingModes { Lissajous, Model4D, Graph3d, Graph2d }
    public ViewingModes viewingMode;

    public enum DisplayModes { LineRenderer, Shader }
    public DisplayModes displayMode;

    private Mesh mesh;
    private MeshRenderer renderer;
    private Material material;
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
        renderer = GetComponent<MeshRenderer>();
        material = GetComponent<Renderer>().material;

        
        MakePlane(200);

        ViewingMode = ViewingModes.Lissajous;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(manager.XScale, manager.YScale, manager.ZScale);
        material.SetVector("_CamPos", Camera.main.transform.position);

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

    public Vector3 Lissajous2(int i, int j, int numberPoints1, int numberPoints2)
    {
        float t = (float)i / numberPoints1 * 2 * Mathf.PI;
        float r = (float)j / numberPoints2 * 2 * Mathf.PI;
        Vector3 point = new Vector3();

        point.x = (3 * Mathf.Cos(5.2f * t) + 1.7f * Mathf.Cos(26 * t))  * Mathf.Sin(r);
        point.y = (3 * Mathf.Sin(5.2f * t) + 1.7f * Mathf.Sin(26 * t)) * Mathf.Sin(r);
        point.z = (3 * Mathf.Sin(5.2f * t) + 1.7f * Mathf.Cos(26 * t)) * Mathf.Cos(r);
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

    public Vector3[] Graph3DFigure(Func<int, int, int, int, Vector3> formula, int numberPoints1, int numberPoints2)
    {
        Vector3[] pointArray = new Vector3[numberPoints1 * numberPoints2];

        for (int i = 0; i < numberPoints1; i++)
        {
            for (int j = 0; j < numberPoints2; j++)
            {
                pointArray[i * numberPoints1 + j] = formula(i, j, numberPoints1, numberPoints2);
            }
        }

        return pointArray;
    }


    public void MakePlane(int edgeCount)
    {
        Vector3[] vertices = new Vector3[edgeCount * edgeCount];
        Vector2[] uv = new Vector2[edgeCount * edgeCount];
        Vector2[] uv2 = new Vector2[edgeCount * edgeCount];
        Vector3[] normals = new Vector3[edgeCount * edgeCount];
        int[] tris;

        if (edgeCount < 3)
        {
            edgeCount = 3;
        }
        List<int> triList = new List<int>();

        for (int i = 0; i < edgeCount; i++)
        {
            for (int j = 0; j < edgeCount; j++)
            {
                Vector3 point = 10 * new Vector3((float)i / edgeCount, 0, (float)j / edgeCount);
                vertices[i * edgeCount + j] = point;
                uv[i * edgeCount + j] = new Vector2(point.x, point.z);
                uv2[i * edgeCount + j] = new Vector2(i, j);
                normals[i * edgeCount + j] = Vector3.up;

            }
        }

        for (int i = 0; i < edgeCount; i++)
        {
            for (int j = 0; j < edgeCount; j++)
            {
                if (j + 1 < edgeCount && i + 1 < edgeCount)
                {
                    triList.Add(i * edgeCount + j);
                    triList.Add(i * edgeCount + j + 1);
                    triList.Add((i + 1) * edgeCount + j);
                }
                if (j - 1 >= 0 && i - 1 >= 0)
                {
                    triList.Add(i * edgeCount + j);
                    triList.Add(i * edgeCount + j - 1);
                    triList.Add((i - 1) * edgeCount + j);
                }
            }
        }

        tris = triList.ToArray();

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv2;
        mesh.normals = normals;
        mesh.triangles = tris;

        material.SetFloat("_RowCount", edgeCount);
        material.SetFloat("_ColumnCount", edgeCount);
        material.SetFloat("_GridStep", 1f / edgeCount); 
    }


    public ViewingModes ViewingMode
    {
        get
        {
            return viewingMode;
        }
        set
        {
            viewingMode = value;

            switch (viewingMode)
            {
                case ViewingModes.Lissajous:
                    if (displayMode == DisplayModes.LineRenderer)
                    {
                        renderer.enabled = false;
                        lineRenderer.enabled = true;
                        Vector3[] pointArray = Graph3DFigure(Lissajous2, 100, 100);
                        lineRenderer.positionCount = 10000;
                        lineRenderer.SetPositions(pointArray);
                    }
                    else
                    {
                        lineRenderer.enabled = false;
                        renderer.enabled = true;
                    }
                    break;

            }
        }
    }

    public DisplayModes DisplayMode
    {
        get
        {
            return displayMode;
        }
        set
        {
            displayMode = value;
            ViewingMode = viewingMode;
        }
    }

}
