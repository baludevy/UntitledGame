using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBullet : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 5f;

    [SerializeField] private ParticleSystem impactPrefab;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.LookRotation(contact.normal);
        ParticleSystem fx = Instantiate(impactPrefab, contact.point, rot);
        Destroy(fx.gameObject, 3f);

        Destroy(gameObject);
    }
}