// Alan Zucconi
// www.alanzucconi.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Renderer))]
public class Heatmap : MonoBehaviour
{
    public Texture2D heatTex;
    public Shader heatShader;

    List<HeatPoint> heatPoints = new List<HeatPoint>();
    Renderer meshRenderer;
    Material material;
    Transform trans;

    Renderer MeshRenderer
    {
        get
        {
            if (meshRenderer == null)
                meshRenderer = GetComponent<Renderer>();

            return meshRenderer;
        }
    }

    Transform Trans
    {
        get
        {
            if (trans == null)
                trans = transform;
            return trans;
        }
    }

    void Start()
    {
        GenerateMeterial();
    }

    void Update()
    {
        CommitToShaderProgram();
    }

    /// <summary>
    /// 重新生成材质
    /// </summary>
    private void GenerateMeterial()
    {
        /*
        * 开始时重新生成一下材质，Unity的 SetXXArray 有一个问题是，当第一次给数组赋值时，数组的最大尺寸就固定了
        * 以后再赋值更大的数组时，将会被裁减
        */
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
        if (heatPoints.Count == 0)
            return;

        material.SetInt("_Points_Length", heatPoints.Count);
        material.SetVectorArray("_Points", heatPoints.Select((v) => new Vector4(v.point.x, v.point.y, v.point.z)).ToList());
        material.SetVectorArray("_Properties", heatPoints.Select((v) => new Vector4(v.radius, v.intensity)).ToList());
    }

    /// <summary>
    /// 向当前热力图中添加指定的点
    /// </summary>
    /// <param name="position">世界坐标系坐标</param>
    /// <param name="radius">热点半径</param>
    /// <param name="intensity">热点强度</param>
    public void AddHeatPoint(Vector2 position, float radius, float intensity)
    {
        this.heatPoints.Add(new HeatPoint()
        {
            point = position,
            radius = radius,
            intensity = intensity
        });

        GenerateMeterial();

        CommitToShaderProgram();
    }

    /// <summary>
    /// 向当前热力图中添加指定的点集
    /// </summary>
    /// <param name="points"></param>
    public void AddHeatPoints(IEnumerable<HeatPoint> points)
    {
        if (points == null)
            return;

        this.heatPoints.AddRange(points.Select((v) => new HeatPoint()
        {
            point = v.point,
            radius = v.radius,
            intensity = v.intensity
        }));

        GenerateMeterial();
        CommitToShaderProgram();
    }

    /// <summary>
    /// 根据指定的热点，设置热力图
    /// </summary>
    /// <param name="points"></param>
    public void SetHeatPoints(IEnumerable<HeatPoint> points)
    {
        if (points.Count() > this.heatPoints.Count)
            GenerateMeterial();

        this.heatPoints.Clear();
        this.heatPoints.AddRange(points.Select((v) => new HeatPoint()
        {
            point = v.point,
            radius = v.radius,
            intensity = v.intensity
        }));

        CommitToShaderProgram();
    }

    public void Repaint()
    {
        CommitToShaderProgram();
    }
}

[System.Serializable]
public class HeatPoint
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