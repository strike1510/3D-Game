using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform cible;
    public float distance = 6f;
    public float distanceMin = 2f;
    public float distanceMax = 12f;
    public float sensibilite = 3f;
    public float vitesseZoom = 4f;
    public float pitchMin = -10f;  // limite bas (pas sous le sol)
    public float pitchMax = 70f;   // limite haut (pas au-dessus de la tête)
    public Vector3 offsetCible = new Vector3(0f, 1.5f, 0f);

    float yaw = 0f;
    float pitch = 20f;

    void Update()
    {
        bool clicGauche = Input.GetMouseButton(0);
        bool clicDroit  = Input.GetMouseButton(1);

        // La caméra ne tourne que si on maintient un bouton
        if (clicGauche || clicDroit)
        {
            yaw += Input.GetAxis("Mouse X") * sensibilite;
            pitch -= Input.GetAxis("Mouse Y") * sensibilite;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }

        // Clic droit : le Player s'oriente comme la caméra (dos à la caméra)
        if (clicDroit && cible != null)
        {
            Vector3 e = cible.eulerAngles;
            cible.rotation = Quaternion.Euler(e.x, yaw, e.z);
        }

        // Zoom molette
        distance -= Input.GetAxis("Mouse ScrollWheel") * vitesseZoom;
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);
    }

    void LateUpdate()
    {
        if (cible == null) return;
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 point = cible.position + offsetCible;
        transform.position = point - rot * Vector3.forward * distance;
        transform.rotation = rot;
    }
}