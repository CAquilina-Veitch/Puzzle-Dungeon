using System.Collections;
using System;
using Scripts.Items;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class slimeAI : Enemy
{
    private Rigidbody rb;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public float health, kbStrength, delay;
    private BoxCollider colliderBox;
    private Animator animator;

    public bool playerInSight;
    public float sightRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        colliderBox = GetComponent<BoxCollider>();
        base.Start();
    }
    public override void dropItems()
    {
        throw new NotImplementedException();
    }
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void KnockBack()
    {
        StopAllCoroutines();
        Vector3 direction = (transform.position - player.transform.position).normalized;
        rb.AddForce(direction * kbStrength, ForceMode.Impulse);
        StartCoroutine(Reset());
    }

    private void Die()
    {       
        colliderBox.enabled = false;
        animator.SetTrigger("dead");
        reportDeath();
        Invoke(nameof(DeSpawn), 2f);
        DroppedItemManager.Instance.SpawnDroppedItem(transform, ItemType.Health);
    }

    private void DeSpawn()
    {
        Destroy(gameObject);
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        rb.linearVelocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 8)
        {
            health -= 1;
            KnockBack();
        }
        if (health == 0) Die();
    }
    void Update()
    {
       playerInSight = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);


       if (health > 0 && playerInSight) ChasePlayer();
       if (health == 0) agent.SetDestination(transform.position);
    }
}
