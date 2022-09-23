using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{

    [Serializable]
    public struct BezierLine
    {
        public Transform startPoint;
        public Vector3 P0 => startPoint.position;

        public Transform controlPoint_1;
        public Vector3 P1 => controlPoint_1.position;

        public Transform controlPoint_2;
        public Vector3 P2 => controlPoint_2.position;

        public Transform endPoint;
        public Vector3 P3 => endPoint.position;

    }

    public BezierLine bezierLine;
    public Vector3 oldP3;
    public void Awake()
    {
        oldP3 = bezierLine.P1;
    }

    float t = 0f;
    public bool run;
    Vector3 curPoint;
    public string curLine;
    public void Update()
    {
        if (oldP3 != bezierLine.P1)
        {
            oldP3 = bezierLine.P1;
            run = true;
        }

        if (!run) return;

        if (t > 1f)
        {
            t = 0f;
            return;
        }

        // 公式计算坐标
        // TODO: 改用通用公式，实现任意多阶贝赛尔曲线
        if (bezierLine.controlPoint_2 != null)
        {
            // 3阶贝赛尔曲线
            curLine = "3阶贝赛尔曲线";
            curPoint = MathF.Pow((1 - t), 3f) * bezierLine.P0 + 3 * t * MathF.Pow((1 - t), 2f) * bezierLine.P1 + 3 * MathF.Pow(t, 2f) * (1 - t) * bezierLine.P2 + Mathf.Pow(t, 3) * bezierLine.P3;
        }
        else if (bezierLine.controlPoint_1 != null)
        {
            // 2阶贝赛尔曲线
            curLine = "2阶贝赛尔曲线";
            curPoint = MathF.Pow((1 - t), 2f) * bezierLine.P0 + 2 * t * (1 - t) * bezierLine.P1 + Mathf.Pow(t, 2) * bezierLine.P3;
        }

        t += UnityEngine.Time.deltaTime;

    }

    public void OnDrawGizmos()
    {
        if (run)
        {
            oldP3 = bezierLine.P1;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(curPoint, 0.2f);
        }
    }


}
