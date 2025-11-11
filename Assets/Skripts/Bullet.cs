using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _speed = 50f;
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private LayerMask _collisionMask = ~0;

    private Rigidbody _rigidbody;
    private bool _hasCollided = false;

    public GameObject Owner { get; set; }
    public int Damage => _damage;

    public event Action<Vector3, Vector3> OnHit;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigidbody.velocity = transform.forward * _speed;

        Destroy(gameObject, _lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasCollided) return;
        _hasCollided = true;

        if (((1 << collision.gameObject.layer) & _collisionMask) != 0)
        {
            Health health = collision.collider.GetComponent<Health>();
            if (health != null && collision.gameObject != Owner)
            {
                health.TakeDamage(_damage);
            }

            ContactPoint contact = collision.contacts[0];
            OnHit?.Invoke(contact.point, contact.normal);
        }

        Destroy(gameObject);
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;

        if (_rigidbody != null)
            _rigidbody.velocity = transform.forward * _speed;
    }

    public void IgnoreCollision(Collider collider)
    {
        Collider bulletCollider = GetComponent<Collider>();
        if (bulletCollider != null && collider != null)
        {
            Physics.IgnoreCollision(bulletCollider, collider);
        }
    }
}