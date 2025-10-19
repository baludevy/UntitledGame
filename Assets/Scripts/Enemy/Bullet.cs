using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 5f;
    
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStatistics.Instance.TakeDamage(5f);
        }
        
        Destroy(gameObject);
    }
}