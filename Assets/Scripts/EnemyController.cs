using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public enum AISTATE { PATROL, ATTACK }

    public Animator enemyAnim;
    private AISTATE enemyState = AISTATE.PATROL;

    [Header("References")]
    public Transform player_drone;
    public NavMeshAgent enemy;

    [Header("Patrol")]
    public List<Transform> wayPoints;
    private Transform currentWayPoint;

    [Header("Attack Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 700f;
    public float fireRate = 0.3f;
    private float lastFireTime;

    public float attackDistance = 25f;

    private void Start()
    {
        if (wayPoints.Count > 0)
            currentWayPoint = wayPoints[Random.Range(0, wayPoints.Count)];

        ChangeState(AISTATE.PATROL);
    }

    public void ChangeState(AISTATE newState)
    {
        StopAllCoroutines();
        enemyState = newState;

        if (newState == AISTATE.PATROL)
            StartCoroutine(PatrolState());
        else
            StartCoroutine(AttackState());
    }

    // ----------------------------------------------------------
    // PATROL STATE
    // ----------------------------------------------------------
    IEnumerator PatrolState()
    {
        enemyAnim.SetBool("Walking", true);
        enemyAnim.SetBool("Shoot", false);

        enemy.isStopped = false;

        while (enemyState == AISTATE.PATROL)
        {
            if (currentWayPoint == null)
                currentWayPoint = wayPoints[Random.Range(0, wayPoints.Count)];

            enemy.SetDestination(currentWayPoint.position);

            // Reached waypoint
            if (Vector3.Distance(transform.position, currentWayPoint.position) < 2f)
            {
                currentWayPoint = wayPoints[Random.Range(0, wayPoints.Count)];
            }

            yield return null;
        }
    }

    // ----------------------------------------------------------
    // ATTACK STATE
    // ----------------------------------------------------------
    IEnumerator AttackState()
    {
        enemyAnim.SetBool("Walking", false);
        enemyAnim.SetBool("Shoot", true);

        enemy.isStopped = false;

        while (enemyState == AISTATE.ATTACK)
        {
            if (player_drone == null) yield break;

            float dist = Vector3.Distance(transform.position, player_drone.position);

            // Exit attack if drone goes too far
            if (dist > attackDistance)
            {
                ChangeState(AISTATE.PATROL);
                yield break;
            }

            // Rotate only on Y axis
            Vector3 look = player_drone.position;
            look.y = transform.position.y;
            transform.LookAt(look);

            // Move towards drone slowly
            enemy.SetDestination(player_drone.position);

            // SHOOT BULLETS
            if (Time.time > lastFireTime + fireRate)
            {
                lastFireTime = Time.time;

                GameObject b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                b.GetComponent<Rigidbody>().AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
            }

            yield return null;
        }
    }

    // ----------------------------------------------------------
    // TRIGGERS
    // ----------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ChangeState(AISTATE.ATTACK);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            ChangeState(AISTATE.PATROL);
    }
}
