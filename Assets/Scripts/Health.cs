using UnityEngine;

public class Health : MonoBehaviour
{
    public float vieMax = 100f;
    public float vie;
    public HealthBar barre;

    public bool estMort => vie <= 0f;

    void Start()
    {
        vie = vieMax;
        if (barre) barre.Init(vieMax);
    }

    public void Degats(float d)
    {
        if (estMort) return;
        vie = Mathf.Max(0f, vie - d);
        if (barre) barre.Set(vie);
        if (vie <= 0f) Mort();
    }

    void Mort()
    {
        if (CompareTag("Player")) Debug.Log("Player mort");
        else Destroy(gameObject, 0.2f);
    }
}