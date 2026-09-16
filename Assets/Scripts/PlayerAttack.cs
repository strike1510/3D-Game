using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public KeyCode attaque = KeyCode.E;
    public float portee = 2.5f;
    public float force = 20f;
    public float cadence = 0.6f;
    public Animator anim;

    float timer = 0f;

    void Start() { if (anim == null) anim = GetComponentInChildren<Animator>(); }

    void Update()
    {
        timer -= Time.deltaTime;
        if (Input.GetKeyDown(attaque) && timer <= 0f)
        {
            timer = cadence;
            if (anim) anim.SetTrigger("Attack");

            Vector3 centre = transform.position + transform.forward + Vector3.up;
            foreach (var h in Physics.OverlapSphere(centre, portee))
            {
                if (h.CompareTag("Monster"))
                {
                    h.GetComponent<Health>()?.Degats(force);
                    break;
                }
            }
        }
    }
}