using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Tooltip("Durée d'un cycle 24h en secondes réelles")]
    public float dureeCycle = 120f;   // 2 min = 24h
    [Range(0f, 1f)] public float heure = 0.25f; // 0=minuit, 0.25=aube, 0.5=midi
    public float intensiteMax = 1.2f;
    public Gradient couleurLumiere;

    void Update()
    {
        // Accélération temps : Shift accélère x10
        float mult = Input.GetKey(KeyCode.LeftShift) ? 10f : 1f;
        heure += (Time.deltaTime / dureeCycle) * mult;
        if (heure >= 1f) heure -= 1f;

        // Rotation soleil : 0h = -90° (sous horizon), 12h = +90° (zénith)
        float angle = heure * 360f - 90f;
        transform.rotation = Quaternion.Euler(angle, 170f, 0f);

        // Intensité : max à midi, 0 la nuit
        float t = Mathf.Clamp01(Mathf.Sin(heure * Mathf.PI * 2f - Mathf.PI / 2f) * 0.5f + 0.5f);
        var light = GetComponent<Light>();
        light.intensity = t * intensiteMax;
        if (couleurLumiere != null) light.color = couleurLumiere.Evaluate(heure);
    }
}