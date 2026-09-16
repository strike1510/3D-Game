using UnityEngine;

[RequireComponent(typeof(Health))]
public class MonsterAI : MonoBehaviour
{
    public float vitesse = 2.5f;
    public float forceAttaque = 10f;
    public float porteeAttaque = 2f;
    public float cadence = 1.5f;
    public Animator anim;

    Transform cible;
    float timer = 0f;
    Health maVie;

    void Start()
    {
        maVie = GetComponent<Health>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter(Collider o) { if (o.CompareTag("Player")) cible = o.transform; }
    void OnTriggerExit(Collider o)  { if (o.CompareTag("Player")) cible = null; }

    void Update()
    {
        if (maVie.estMort) return;

        bool bouge = false;
        if (cible != null)
        {
            float d = Vector3.Distance(transform.position, cible.position);
            if (d > porteeAttaque)
            {
                Vector3 dir = cible.position - transform.position; dir.y = 0f;
                transform.position += dir.normalized * vitesse * Time.deltaTime;
                if (dir.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(dir);
                bouge = true;
            }
            else
            {
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (anim) anim.SetTrigger("Attack");
                    cible.GetComponent<Health>()?.Degats(forceAttaque);
                    timer = cadence;
                }
            }
        }
        if (anim) anim.SetBool("Walking", bouge);
    }
}