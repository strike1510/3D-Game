using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
    public Transform[] points;          // 3 = quadratique, 4 = cubique
    [Range(2, 100)] public int resolution = 50;

    LineRenderer lr;

    void Awake() { lr = GetComponent<LineRenderer>(); }

    void Update()
    {
        if (points == null || points.Length < 3) { lr.positionCount = 0; return; }

        lr.positionCount = resolution + 1;
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            lr.SetPosition(i, Evaluer(t));
        }
    }

    // Point de la courbe pour t entre 0 et 1
    public Vector3 Evaluer(float t)
    {
        if (points.Length == 3)
            return Quadratique(points[0].position, points[1].position, points[2].position, t);
        else
            return Cubique(points[0].position, points[1].position, points[2].position, points[3].position, t);
    }

    Vector3 Quadratique(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        return u * u * p0
             + 2f * u * t * p1
             + t * t * p2;
    }

    Vector3 Cubique(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1f - t;
        return u * u * u * p0
             + 3f * u * u * t * p1
             + 3f * u * t * t * p2
             + t * t * t * p3;
    }

    // Affiche les points et les lignes de contrôle dans la scène
    void OnDrawGizmos()
    {
        if (points == null) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null) continue;
            Gizmos.DrawSphere(points[i].position, 0.15f);
            if (i > 0 && points[i - 1] != null)
                Gizmos.DrawLine(points[i - 1].position, points[i].position);
        }
    }
}