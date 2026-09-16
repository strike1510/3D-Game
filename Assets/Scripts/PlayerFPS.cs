using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerFPS : MonoBehaviour
{
    [Header("Déplacement")]
    public float vitesseMarche = 4f;
    public float vitesseCourse = 8f;
    public float gravite = -20f;
    public float vitesseRotation = 12f;

    [Header("Touches")]
    public KeyCode avancer = KeyCode.Z;
    public KeyCode reculer = KeyCode.S;
    public KeyCode gauche  = KeyCode.Q;
    public KeyCode droite  = KeyCode.D;
    public KeyCode sprint  = KeyCode.LeftShift;
    public KeyCode saut    = KeyCode.Space;
    public KeyCode attaque = KeyCode.E;

    [Header("Saut")]
    public float hauteurSaut = 1.5f;
    public float toleranceSol = 0.15f;
    public float bufferSaut = 0.15f;

    [Header("Détection sol")]
    public LayerMask masqueSol = ~0;
    public float rayonSol = 0.3f;
    public float offsetSol = 0.1f;

    [Header("Caméra")]
    public Transform camT;   // <- glisse Main Camera ici dans l'Inspector

    [Header("Animation")]
    public Animator anim;

    CharacterController cc;
    float vitesseVerticale = 0f;
    float timerSol = 0f;
    float timerSaut = 0f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (camT == null) RecupererCamera();
    }

    void RecupererCamera()
    {
        if (Camera.main != null) camT = Camera.main.transform;
    }

    bool AuSol()
    {
        Vector3 pied = transform.position + cc.center
            + Vector3.down * (cc.height / 2f - cc.radius + offsetSol);
        return Physics.CheckSphere(pied, rayonSol, masqueSol, QueryTriggerInteraction.Ignore);
    }

    void Update()
    {
        // Sécurité : si pas de caméra (ex: pendant la cinématique), on attend
        if (camT == null)
        {
            RecupererCamera();
            if (camT == null) return;
        }

        // --- Direction ---
        float x = 0f, z = 0f;
        if (Input.GetKey(avancer)) z += 1f;
        if (Input.GetKey(reculer)) z -= 1f;
        if (Input.GetKey(droite))  x += 1f;
        if (Input.GetKey(gauche))  x -= 1f;

        Vector3 fwd = camT.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 right = camT.right; right.y = 0f; right.Normalize();
        Vector3 dir = fwd * z + right * x;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        bool enMouvement = dir.sqrMagnitude > 0.01f;
        bool auSol = AuSol();

        // --- Sprint ---
        bool court = Input.GetKey(sprint) && enMouvement;
        float vitesse = court ? vitesseCourse : vitesseMarche;

        // --- Orientation ---
        if (enMouvement)
        {
            Quaternion cible = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, cible, Time.deltaTime * vitesseRotation);
        }

        // --- Timers ---
        if (auSol) timerSol = toleranceSol;
        else timerSol -= Time.deltaTime;

        if (Input.GetKeyDown(saut)) timerSaut = bufferSaut;
        else timerSaut -= Time.deltaTime;

        if (auSol && vitesseVerticale < 0f) vitesseVerticale = -2f;

        // --- Saut ---
        if (timerSol > 0f && timerSaut > 0f)
        {
            vitesseVerticale = Mathf.Sqrt(hauteurSaut * -2f * gravite);
            timerSol = 0f;
            timerSaut = 0f;
            if (anim != null) anim.SetTrigger("Jump");
        }

        vitesseVerticale += gravite * Time.deltaTime;

        // --- Attaque ---
        if (Input.GetKeyDown(attaque) && anim != null)
            anim.SetTrigger("Attack");

        // --- Mouvement ---
        cc.Move((dir * vitesse + Vector3.up * vitesseVerticale) * Time.deltaTime);

        // --- Animations ---
        if (anim != null)
        {
            anim.SetBool("Walking", enMouvement);
            anim.SetBool("Runing", court);
            anim.SetBool("Grounded", auSol);
            anim.SetFloat("VitesseY", vitesseVerticale);
        }
    }
}