using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
// Namespace necesario para utilizar IA en Unity
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent; //para acceder a los navMeshAgent de los enemigos
    //private FollowRoute followRouteScript; //el script para decirle los puntos a recorrer en su ruta (no lo usa, lo ha implementado dentro de este script desde 0)
    private Transform player; //el transform del Player para poder controlar la position del jugador

    private float visionRange = 20f; //el rango de visión del enemigo para detectar si nos ve
    private float attackRange = 10f; //rango de ataque por si nos acercamos mucho

    private bool playerInVisionRange; //saber si estamos dentro del rango de vision
    private bool playerInAttackRange; //saber si estamos dentro del rango de ataque

    // Capa que debe tenerse en cuenta para campos de visión y ataque
    [SerializeField] private LayerMask playerLayer;
    
    // Patrulla (FollowRoute pero creado aquí)
    [SerializeField] private Transform[] points;
    private float speed = 5f;
    private int totalPoints;
    private int nextPoint;

    // Ataque
    [SerializeField] private GameObject bullet; //bala que va a disparar el enemigo
    private float timeBetweenAttacks = 2f; //cuanto cooldown tiene
    private bool canAttack = true; //puede atacar o no, para el cooldown
    private float upAttackForce = 5f; //fuerza arriba que tendrá la bola
    private float forwardAttackForce = 8f; //fueraza hacia alante, para que junto a la de arriba haga una parábola

    private void Awake()
    {
        // Guardamos la referencia a la componente NavMeshAgent del Game Object
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // Localizamos al player por código, ya que al ser un prefab, no podemos asignarlo por inspector
        player = GameObject.Find("Player").transform;
        
        // Calculamos el total de puntos de la ruta de patrulla del agente
        totalPoints = points.Length;
        // Como empezamos en el punto 0, el siguiente objetivo es el punto 1
        nextPoint = 1;
    }

    private void Update()
    {

        // Campos de visión y ataque del agente
        Vector3 pos = transform.position;
        playerInVisionRange = Physics.CheckSphere(pos, visionRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(pos, attackRange, playerLayer);

        
        // Activamos la lógica de patrulla
        if (!playerInVisionRange && !playerInAttackRange)
        {
            FollowPatrolRoute();
        }
        

        
        // Activamos la lógica de persecución 
        if (playerInVisionRange && !playerInAttackRange)
        {
            Chase();
        }
        

        
        // Activamos la lógica de ataque
        if (playerInVisionRange && playerInAttackRange)
        {
            Attack();
        }
        
    }

    
    // Lógica de patrulla dle agente
    private void FollowPatrolRoute()
    {
        // Si hemos llegado al siguiente punto
        if (Vector3.Distance(transform.position, points[nextPoint].position) < 0.5f)
        {
            // Cambiamos el objetivo al siguiente punto
            nextPoint++;
            
            // Si hemos llegado al último punto, volvemos a empezar fijando como objetivo el primer punto de la ruta
            if (nextPoint == totalPoints)
            {
                nextPoint = 0;
            }
            
            // Miramos siempre al punto al que vamos
            transform.LookAt(points[nextPoint].position);
        }

        // Versión sin IA: Fijamos el objetivo al que queremos llegar
        //transform.position = Vector3.MoveTowards(transform.position, points[nextPoint].position, speed * Time.deltaTime);
        
        // Versión con IA: Fijamos el objetivo al que queremos llegar
        agent.SetDestination(points[nextPoint].position);
    }

    
    // Lógica de persecución del agente
    private void Chase()
    {
        // Fijamos como objetivo al player
        agent.SetDestination(player.position); //con IA ya no es MoveTowards, es Set destination para que no atraviese las paredes si hay algo por enmedio que le impide pasar
        transform.LookAt(player);
    }
    

    
    
    // Lógica de ataque del agente
    private void Attack()
    {
        // Paramos al agente
        agent.SetDestination(transform.position);
        
        // Si hemos finalizado el Attack Cooldown
        if (canAttack)
        {
            // Disparamos bala con físicas
            Rigidbody rb = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * forwardAttackForce, ForceMode.Impulse);
            rb.AddForce(transform.up * upAttackForce, ForceMode.Impulse);
            
            // Activamos Attack Cooldown
            canAttack = false;
            StartCoroutine(AttackCooldown());
        }


    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }
    
    

    
    // Esta función nos permite dibujar Gizmos
    private void OnDrawGizmos()
    {
        // Rango de visión del agente una esfera verde
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        // Rango de ataque del agente una esfera magenta
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
