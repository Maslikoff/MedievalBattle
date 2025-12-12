using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _speed = 50f;
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private LayerMask _collisionMask = ~0;
    [SerializeField] private LayerMask _ignoreLayerMask = 0;

    private Rigidbody _rigidbody;
    private bool _hasCollided = false;

    public GameObject Owner { get; set; }

    public event Action<Vector3, Vector3> Hit;

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
        if (_hasCollided)
            return;

        if (ShouldIgnoreCollision(collision.gameObject))
            return;

        if (IsInCollisionLayer(collision.gameObject) == false)
            return;

        _hasCollided = true;
        ProcessCollision(collision);
    }

    public void SetIgnoreLayerMask(LayerMask layerMask)
    {
        _ignoreLayerMask = layerMask;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;

        if (_rigidbody != null)
            _rigidbody.velocity = transform.forward * _speed;
    }

    private bool ShouldIgnoreCollision(GameObject otherObject)
    {
        if (otherObject == Owner)
            return true;

        if (((1 << otherObject.layer) & _ignoreLayerMask) != 0)
            return true;

        return false;
    }

    private bool IsInCollisionLayer(GameObject otherObject) => ((1 << otherObject.layer) & _collisionMask) != 0;

    private void ProcessCollision(Collision collision)
    {
        if (collision.collider.TryGetComponent<Health>(out var health))
            health.TakeDamage(_damage);

        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            Hit?.Invoke(contact.point, contact.normal);
        }
    }
}