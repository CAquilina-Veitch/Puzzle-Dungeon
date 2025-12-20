using System;
using System.Collections;
using Runtime.Extensions;
using Unity.VisualScripting;
using UnityEngine;

public class health : MonoBehaviour
{
    public PlayerController pc;
    public float hp;
    [SerializeField]
    private float kbStrength;
    [SerializeField]
    private float delay;

    public readonly RORP<int> currentHealth = new();
    public Transform enemyPosition;

    private void Awake()
    {
        currentHealth.Set((int)hp);
    }

    private void KnockBack()
    {
        StopAllCoroutines();
        Vector3 direction = (transform.position - enemyPosition.transform.position).normalized;
        pc.rb.AddForce(direction * kbStrength, ForceMode.Impulse);
        StartCoroutine(Reset());
    }
    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        pc.rb.linearVelocity = Vector3.zero;
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 3)
        {
            enemyPosition = collision.transform;
            hp -= 1f;
            currentHealth.NewValue--;
            KnockBack();
        }
                
    }
}
