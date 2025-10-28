using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
public class slimeAI : MonoBehaviour
{
    private Rigidbody rb;
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public float health, kbStrength, delay;
    private BoxCollider colliderBox;
    private Animator animator;
    
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
        Invoke("DeSpawn", 2f);
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
       if (health > 0) ChasePlayer();
       if (health == 0) agent.SetDestination(transform.position);
    }
}
