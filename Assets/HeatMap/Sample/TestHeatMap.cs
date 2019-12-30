/*
 * @Author: huosk 
 * @Date: 2019-12-30 10:58:46 
 * @Last Modified by: huosk
 * @Last Modified time: 2019-12-30 11:48:38
 */

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TestHeatMap : MonoBehaviour
{

    public int count = 50;
    public List<HeatPoint> heatPoints;
    public Heatmap heatmap;
    void Start()
    {
        heatPoints.Clear();

        for (int i = 0; i < count; i++)
        {
            heatPoints.Add(new HeatPoint()
            {
                point = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)),
                radius = Random.Range(0.1f, 0.2f),
                intensity = Random.Range(0.25f, 1f),
            });
        }
    }

    void Update()
    {
        heatmap.SetHeatPoints(heatPoints.Select((v)=>new HeatPoint(){
            point = transform.TransformPoint(v.point),
            radius = v.radius,
            intensity = v.intensity
        }));
    }
}