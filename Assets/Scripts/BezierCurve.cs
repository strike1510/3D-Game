using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
    public Transform[] points;          // n points de contrôle (n >= 2)
    [Range(2, 100)] public int resolution = 50;

    LineRenderer lr;

    void Awake() { lr = GetComponent<LineRenderer>(); }

    void Update()
    {
        if (points == null || points.Length < 2) { lr.positionCount = 0; return; }

        lr.positionCount = resolution + 1;
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            lr.SetPosition(i, Evaluer(t));
        }
    }

    // Point de la courbe pour t entre 0 et 1, quel que soit le nombre de points
    public Vector3 Evaluer(float t)
    {
        Vector3[] p = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            p[i] = points[i].position;
        return DeCasteljau(p, t);
    }

    // Algorithme récursif de De Casteljau :
    // réduit n points en n-1 par interpolation, jusqu'à 1 seul point
    Vector3 DeCasteljau(Vector3[] p, float t)
    {
        if (p.Length == 1)
            return p[0];

        Vector3[] suivant = new Vector3[p.Length - 1];
        for (int i = 0; i < suivant.Length; i++)
            suivant[i] = Vector3.Lerp(p[i], p[i + 1], t);

        return DeCasteljau(suivant, t);
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