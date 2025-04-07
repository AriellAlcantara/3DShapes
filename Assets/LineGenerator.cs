using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public Material material;
    public Vector3 cubeCenter;
    public Vector3 cubeOtherCenter;
    public Vector3 cubeRotation;
    public float cubeSideLength;
    public float focalLength;

    private void OnPostRender()
    {
        DrawLine();
    }

    public void OnDrawGizmos()
    {
        DrawLine();
    }

    public void DrawLine()
    {
        if (material == null)
        {
            Debug.LogError("You need to add a material");
            return;
        }

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        DrawSphere(cubeCenter);
        DrawSphere(cubeOtherCenter);

        GL.End();
        GL.PopMatrix();
    }

    public void DrawSphere(Vector3 center)
    {
        int segments = 10;
        float radius = cubeSideLength * 0.5f;

        for (int i = 0; i < segments; i++)
        {
            float theta1 = (i / (float)segments) * Mathf.PI * 2;
            float theta2 = ((i + 1) / (float)segments) * Mathf.PI * 2;

            Vector3 p1 = ApplyRotation(new Vector3(center.x + Mathf.Cos(theta1) * radius, center.y, center.z + Mathf.Sin(theta1) * radius));
            Vector3 p2 = ApplyRotation(new Vector3(center.x + Mathf.Cos(theta2) * radius, center.y, center.z + Mathf.Sin(theta2) * radius));

            GL.Vertex3(p1.x, p1.y, 0);
            GL.Vertex3(p2.x, p2.y, 0);
        }
    }

    public Vector3 ApplyRotation(Vector3 point)
    {
        var centered = point - cubeCenter;
        var rotatedX = RotateBy(cubeRotation.x, centered.y, centered.z);
        var rotatedY = RotateBy(cubeRotation.y, centered.x, centered.z);
        var rotatedZ = RotateBy(cubeRotation.z, centered.x, centered.y);

        return new Vector3(rotatedY.x, rotatedZ.y, rotatedX.x) + cubeCenter;
    }

    public Vector2 RotateBy(float angle, float axis1, float axis2)
    {
        float cosA = Mathf.Cos(angle);
        float sinA = Mathf.Sin(angle);
        return new Vector2(axis1 * cosA - axis2 * sinA, axis2 * cosA + axis1 * sinA);
    }
}
