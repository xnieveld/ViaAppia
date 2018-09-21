﻿using System.Collections;
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

    public enum ViewingModes { Lissajous, Simplex}
    public ViewingModes viewingMode;
    public int number = 0;

    public enum DisplayModes { LineRenderer, Shader }
    public DisplayModes displayMode;

    private Mesh mesh;
    private MeshRenderer renderer;
    private Material material;
    private LineRenderer lineRenderer;

    Vector3[] megapoit = new Vector3[1];

    //private MeshGenerator meshGen;

    [SerializeField]
    private Material gridMaterial;

    [SerializeField]
    private Material multiDimensionalMaterial;

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

        if (viewingMode == ViewingModes.Lissajous)
        {
            material.SetVector("_CamPos", Camera.main.transform.position);

            // boundsTarget is the center of the camera's frustum, in world coordinates:
            Vector3 camPosition = Camera.main.transform.position;
            Vector3 normCamForward = Vector3.Normalize(Camera.main.transform.forward);
            float boundsDistance = (Camera.main.farClipPlane - Camera.main.nearClipPlane) / 2 + Camera.main.nearClipPlane;
            Vector3 boundsTarget = camPosition + (normCamForward * boundsDistance);

            // The game object's transform will be applied to the mesh's bounds for frustum culling checking.
            // We need to "undo" this transform by making the boundsTarget relative to the game object's transform:
            Vector3 realtiveBoundsTarget = this.transform.InverseTransformPoint(boundsTarget);

            // Set the bounds of the mesh to be a 1x1x1 cube (actually doesn't matter what the size is)
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            mesh.bounds = new Bounds(realtiveBoundsTarget, Vector3.one);
        }
    }
    /*
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
    */

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

    private int OverFlow(int value, int min, int max)
    {
        if (value > max)
        {
            value -= (max - min + 1);
        }
        if (value < min)
        {
            value += (max - min + 1);
        }
        return value;
    }

    public void PointsToMesh(Vector3[] pointList)
    {
        mesh.Clear();
        mesh.vertices = pointList;
        megapoit = pointList;

        int[] tris;
        
        List<int> triList = new List<int>();

        for (int i = 0; i < pointList.Length; i++)
        {
            triList.Add(i);
            triList.Add(OverFlow(i + 1, 0, pointList.Length - 1));
            triList.Add(OverFlow(i - 1, 0, pointList.Length - 1));

            triList.Add(i);
            triList.Add(OverFlow(i - 1, 0, pointList.Length - 1));
            triList.Add(OverFlow(i + 1, 0, pointList.Length - 1));


            triList.Add(OverFlow(i + 1, 0, pointList.Length - 1));
            triList.Add(i);
            triList.Add(OverFlow(i - 1, 0, pointList.Length - 1));

            triList.Add(OverFlow(i + 1, 0, pointList.Length - 1));
            triList.Add(OverFlow(i - 1, 0, pointList.Length - 1));
            triList.Add(i);

            triList.Add(OverFlow(i - 1, 0, pointList.Length - 1));
            triList.Add(i);
            triList.Add(OverFlow(i + 1, 0, pointList.Length - 1));

            triList.Add(OverFlow(i - 1, 0, pointList.Length - 1));
            triList.Add(OverFlow(i + 1, 0, pointList.Length - 1));
            triList.Add(i);
        }
        tris = triList.ToArray();
        foreach (int a in tris)
        {
            print(a);
        }
        
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }

    public void SetViewingMode(int mode)
    {
        ViewingMode = (ViewingModes)mode;
    }

    public ViewingModes ViewingMode
    {
        get
        {
            return viewingMode;
        }
        set
        {
            if (viewingMode != value)
            {
                viewingMode = value;
                switch (viewingMode)
                {
                    case ViewingModes.Lissajous:
                        renderer.material = gridMaterial;
                        material = GetComponent<Renderer>().material;
                        MakePlane(200);
                        material.SetFloat("_FigureNumber", number);

                        if (displayMode == DisplayModes.LineRenderer)
                        {/*
                        renderer.enabled = false;
                        lineRenderer.enabled = true;
                        Vector3[] pointArray = Graph3DFigure(Lissajous2, 100, 100);
                        lineRenderer.positionCount = 10000;
                        lineRenderer.SetPositions(pointArray);*/
                        }
                        else
                        {
                            //lineRenderer.enabled = false;
                            renderer.enabled = true;
                        }
                        break;
                    case ViewingModes.Simplex:
                        renderer.material = multiDimensionalMaterial;
                        material = GetComponent<Renderer>().material;
                        Vector3[] points = new Vector3[4];
                        points[0] = new Vector3(-1, -1, 1);
                        points[1] = new Vector3(-1, 1, -1);
                            points[2] = new Vector3(1, -1, -1);
                            points[3] = new Vector3(1, 1, 1);

                        //meshGen.dimensions = number;
                        //points = meshGen.CreateSimplex();
                        PointsToMesh(points);
                        break;

                }
            }
        }
    }

    public int Number
    {
        get
        {
            return number;
        }
        set
        {
            number = value;
            if (viewingMode == ViewingModes.Lissajous)
            {
                material.SetFloat("_FigureNumber", number);
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

    private void OnDrawGizmos()
    {
        for (int i = 0; i < megapoit.Length; i++)
        {
            Gizmos.DrawSphere(megapoit[i],0.1f);
        }
    }

}
