using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform cible;
    public float distance = 6f;
    public float distanceMin = 2f;
    public float distanceMax = 12f;
    public float sensibilite = 3f;
    public float vitesseZoom = 4f;
    public float pitchMin = -10f;
    public float pitchMax = 70f;
    public Vector3 offsetCible = new Vector3(0f, 1.5f, 0f);

    [Header("Collision caméra")]
    public LayerMask masqueObstacles = ~0; // ce qui bloque la vue
    public float rayonCollision = 0.3f;    // épaisseur du test (SphereCast)
    public float margeObstacle = 0.2f;     // recul devant l'obstacle
    public float vitesseCollision = 12f;   // fluidité du rapprochement

    float yaw = 0f;
    float pitch = 20f;
    float distanceActuelle = 6f;

    void Start() { distanceActuelle = distance; }

    void Update()
    {
        bool clicGauche = Input.GetMouseButton(0);
        bool clicDroit  = Input.GetMouseButton(1);

        if (clicGauche || clicDroit)
        {
            yaw += Input.GetAxis("Mouse X") * sensibilite;
            pitch -= Input.GetAxis("Mouse Y") * sensibilite;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }

        if (clicDroit && cible != null)
        {
            Vector3 e = cible.eulerAngles;
            cible.rotation = Quaternion.Euler(e.x, yaw, e.z);
        }

        distance -= Input.GetAxis("Mouse ScrollWheel") * vitesseZoom;
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);
    }

    void LateUpdate()
    {
        if (cible == null) return;

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 point = cible.position + offsetCible;   // pivot (autour de la tête)
        Vector3 dir = rot * Vector3.forward;            // direction pivot -> caméra

        // Distance voulue (celle du zoom)
        float distanceVoulue = distance;

        // --- Détection d'obstacle entre le Player et la caméra ---
        // Le rayon part du pivot, vers l'arrière (où se place la caméra)
        if (Physics.SphereCast(point, rayonCollision, -dir, out RaycastHit hit,
                               distance, masqueObstacles, QueryTriggerInteraction.Ignore))
        {
            // On rapproche la caméra juste devant l'obstacle
            distanceVoulue = Mathf.Clamp(hit.distance - margeObstacle, distanceMin, distanceMax);
        }

        // --- Transition fluide ---
        // Rapprochement instantané souhaité mais lissé pour éviter les à-coups
        distanceActuelle = Mathf.Lerp(distanceActuelle, distanceVoulue,
                                      Time.deltaTime * vitesseCollision);

        transform.position = point - dir * distanceActuelle;
        transform.rotation = rot;
    }
}