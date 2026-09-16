using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image remplissage;
    Transform cam;
    float max;

    void Start() { RecupererCamera(); }

    void RecupererCamera()
    {
        if (Camera.main != null) cam = Camera.main.transform;
    }

    public void Init(float m) { max = m; Set(m); }
    public void Set(float v) { if (remplissage) remplissage.fillAmount = v / max; }

    void LateUpdate()          // toujours face caméra
    {
        if (cam == null) { RecupererCamera(); return; }
        transform.forward = cam.forward;
    }
}