using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    public float amplitude = 2.5f; // amplitude de la levitation
    public float speed = 2f; // vitesse de la levitation
    public float startY; // position de départ du GameObject

    void Start()
    {
        startY = transform.position.y; // sauvegarde la position de départ du GameObject
    }

    void Update()
    {
        // calcule la position de la levitation en fonction du temps
        float yPos = startY + amplitude * Mathf.Sin(speed * Time.time);

        // applique la position de la levitation au GameObject
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }
}

