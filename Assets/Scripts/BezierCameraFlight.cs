using UnityEngine;

public class BezierCameraFlight : MonoBehaviour
{
    public BezierCurve courbe;
    public float duree = 15f;
    public bool boucle = false;
    public bool jouerAuDemarrage = true;
    public float vitesseRotation = 5f;

    [Header("Bascule de caméra")]
    public GameObject cameraCinematique; // la caméra du vol (celle-ci)
    public GameObject cameraJoueur;      // la caméra orbitale du joueur

    float t = 0f;
    bool enVol = false;

    void Start()
    {
        if (jouerAuDemarrage)
        {
            DemarrerCinematique();
        }
    }

    void DemarrerCinematique()
    {
        enVol = true;
        t = 0f;
        if (cameraCinematique != null) cameraCinematique.SetActive(true);
        if (cameraJoueur != null) cameraJoueur.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) DemarrerCinematique(); // relancer

        if (!enVol || courbe == null) return;

        t += Time.deltaTime / duree;
        if (t >= 1f)
        {
            if (boucle) t = 0f;
            else
            {
                t = 1f;
                enVol = false;
                FinCinematique(); // <- bascule vers la caméra joueur
            }
        }

        Vector3 pos = courbe.Evaluer(t);
        transform.position = pos;

        Vector3 avant = courbe.Evaluer(Mathf.Min(t + 0.01f, 1f));
        Vector3 dir = avant - pos;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion cible = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, cible, Time.deltaTime * vitesseRotation);
        }
    }

    void FinCinematique()
    {
        if (cameraJoueur != null) cameraJoueur.SetActive(true);
        if (cameraCinematique != null) cameraCinematique.SetActive(false);
    }
}