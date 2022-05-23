using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRoute : MonoBehaviour
{
    [SerializeField] private float speed = 30; //velocidad a la que se va a mover el personaje
    
    //Puntos a seguir
    [SerializeField] private Transform[] points; //lista de puntos por donde va a pasar
    private int totalPoints; //cuantos puntos hay que recorrer
    private int nextPoint; //cual es el siguiente punto que vamos a recorrer? el 1, el 2, el 3, etc
    
    void Start()
    {
        transform.position = points[0].position; //posición inicial es la primera, para evitar problemas de localizar el enemigo
        totalPoints = points.Length; //esta es la longitud del array
        nextPoint = 1; //después del punto de 0 el siguiente punto es el 1.
        // transform.LookAt(points[nextPoint].position);
    }

    public void Update()
    {
        if (Vector3.Distance(transform.position, points[nextPoint].position) < 0.1f) //si ya hemos llegado a nuestro objetivo
        {
            nextPoint++; //le decimos cual es el siguiente punto
            if (nextPoint == totalPoints) //si hemos llegado al último punto
            {
                nextPoint = 0; //volvemos al primero
            }
            // transform.LookAt(points[nextPoint].position);
        }

        transform.position = Vector3.MoveTowards(transform.position, points[nextPoint].position, speed * Time.deltaTime); //se mueve al siguiente punto marcado. de la posición en la que estoy a la posición siguiente
    }
}