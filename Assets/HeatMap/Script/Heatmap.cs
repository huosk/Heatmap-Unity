using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Heatmap : MonoBehaviour
{
    public Texture2D heatTex;
    public Shader heatShader;

    Renderer meshRenderer;
    Material material;
    int pointCount;
    ComputeBuffer pointBuffer;
    Renderer MeshRenderer
    {
        get
        {
            if (meshRenderer == null)
                meshRenderer = GetComponent<Renderer>();

            return meshRenderer;
        }
    }

    void Start()
    {
        GenerateMeterial();
    }

    private void OnDestroy()
    {
        if (pointBuffer != null)
        {
            pointBuffer.Release();
            pointBuffer = null;
        }
    }

    /// <summary>
    /// 重新生成材质
    /// </summary>
    private void GenerateMeterial()
    {
        if (this.material != null)
            Destroy(this.material);

        this.material = new Material(heatShader);
        this.material.SetTexture("_HeatTex", heatTex);

        this.MeshRenderer.sharedMaterial = material;
    }

    /// <summary>
    /// 将参数信息提交到 Shader Program
    /// </summary>
    private void CommitToShaderProgram()
    {
        material.SetInt("_Points_Length", pointCount);
        material.SetBuffer("_PointBuffer", pointBuffer);
    }

    /// <summary>
    /// 根据指定的热点，设置热力图
    /// </summary>
    /// <param name="points"></param>
    public void SetHeatPoints(List<HeatPoint> points)
    {
        if (pointBuffer != null)
            pointBuffer.Release();

        this.pointCount = points.Count;
        this.pointBuffer = new ComputeBuffer(points.Count(), 20);
        this.pointBuffer.SetData(points);

        CommitToShaderProgram();
    }
}

[System.Serializable]
public struct HeatPoint
{
    /// <summary>
    /// 世界坐标系位置
    /// </summary>
    public Vector3 point;

    /// <summary>
    /// 半径
    /// </summary>
    public float radius;

    /// <summary>
    /// 强度
    /// </summary>
    public float intensity;
}