using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    public float cubeSideLength;
    public Vector3 cubeCenter;
    public Vector3 cubeRotation;
    public Material cubeMaterial;
    public float focalLength;
    public float spacing = 3.0f; // Spacing between shapes

    private void OnPostRender()
    {
        DrawShapes();
    }

    private void OnDrawGizmos()
    {
        DrawShapes();
    }

    private void DrawShapes()
    {
        if (cubeMaterial == null) return;

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        cubeMaterial.SetPass(0);

        Vector3 startPos = cubeCenter - new Vector3(spacing * 2, 0, 0);

        DrawRectangle(startPos);
        DrawPyramid(startPos + Vector3.right * spacing);
        DrawCylinder(startPos + Vector3.right * spacing * 2);
        DrawSphere(startPos + Vector3.right * spacing * 3);
        DrawCapsule(startPos + Vector3.right * spacing * 4);

        GL.End();
        GL.PopMatrix();
    }

    private void DrawRectangle(Vector3 position)
    {
        float halfSize = cubeSideLength * 0.5f;
        Vector3[] corners = {
            ApplyRotation(position, new Vector3(-halfSize, -halfSize, 0)),
            ApplyRotation(position, new Vector3(halfSize, -halfSize, 0)),
            ApplyRotation(position, new Vector3(halfSize, halfSize, 0)),
            ApplyRotation(position, new Vector3(-halfSize, halfSize, 0))
        };
        DrawEdges(corners);
    }

    private void DrawPyramid(Vector3 position)
    {
        float halfSize = cubeSideLength * 0.5f;
        Vector3 top = ApplyRotation(position, new Vector3(0, cubeSideLength, 0));
        Vector3[] baseCorners = {
            ApplyRotation(position, new Vector3(-halfSize, -halfSize, -halfSize)),
            ApplyRotation(position, new Vector3(halfSize, -halfSize, -halfSize)),
            ApplyRotation(position, new Vector3(halfSize, -halfSize, halfSize)),
            ApplyRotation(position, new Vector3(-halfSize, -halfSize, halfSize))
        };

        DrawEdges(baseCorners);
        foreach (Vector3 corner in baseCorners)
        {
            GL.Vertex3(corner.x, corner.y, corner.z);
            GL.Vertex3(top.x, top.y, top.z);
        }
    }

    private void DrawCylinder(Vector3 position)
    {
        DrawCircle(position, cubeSideLength * 0.5f, 10);
    }

    private void DrawSphere(Vector3 position)
    {
        DrawCircle(position, cubeSideLength * 0.5f, 10);
        DrawVerticalCircle(position, cubeSideLength * 0.5f, 10);
    }

    private void DrawCapsule(Vector3 position)
    {
        DrawCylinder(position);
        DrawCircle(position + Vector3.up * cubeSideLength * 0.5f, cubeSideLength * 0.5f, 10);
        DrawCircle(position - Vector3.up * cubeSideLength * 0.5f, cubeSideLength * 0.5f, 10);
    }

    private void DrawEdges(Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            GL.Vertex3(points[i].x, points[i].y, points[i].z);
            GL.Vertex3(points[(i + 1) % points.Length].x, points[(i + 1) % points.Length].y, points[(i + 1) % points.Length].z);
        }
    }

    private void DrawCircle(Vector3 position, float radius, int segments)
    {
        float angleStep = Mathf.PI * 2 / segments;
        Vector3 prevPoint = ApplyRotation(position, new Vector3(radius, 0, 0));
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 newPoint = ApplyRotation(position, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
            GL.Vertex3(prevPoint.x, prevPoint.y, prevPoint.z);
            GL.Vertex3(newPoint.x, newPoint.y, newPoint.z);
            prevPoint = newPoint;
        }
    }

    private void DrawVerticalCircle(Vector3 position, float radius, int segments)
    {
        float angleStep = Mathf.PI * 2 / segments;
        Vector3 prevPoint = ApplyRotation(position, new Vector3(0, radius, 0));
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 newPoint = ApplyRotation(position, new Vector3(0, Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius));
            GL.Vertex3(prevPoint.x, prevPoint.y, prevPoint.z);
            GL.Vertex3(newPoint.x, newPoint.y, newPoint.z);
            prevPoint = newPoint;
        }
    }

    private Vector3 ApplyRotation(Vector3 shapeCenter, Vector3 point)
    {
        var centered = point - shapeCenter;

        // Rotate around X-axis
        Vector2 rotatedX = RotateBy(cubeRotation.x * Mathf.Deg2Rad, centered.y, centered.z);
        centered.y = rotatedX.x;
        centered.z = rotatedX.y;

        // Rotate around Y-axis
        Vector2 rotatedY = RotateBy(cubeRotation.y * Mathf.Deg2Rad, centered.x, centered.z);
        centered.x = rotatedY.x;
        centered.z = rotatedY.y;

        // Rotate around Z-axis
        Vector2 rotatedZ = RotateBy(cubeRotation.z * Mathf.Deg2Rad, centered.x, centered.y);
        centered.x = rotatedZ.x;
        centered.y = rotatedZ.y;

        return shapeCenter + centered;
    }

    private Vector2 RotateBy(float angle, float axis1, float axis2)
    {
        float cosA = Mathf.Cos(angle);
        float sinA = Mathf.Sin(angle);
        return new Vector2(axis1 * cosA - axis2 * sinA, axis1 * sinA + axis2 * cosA);
    }
}
