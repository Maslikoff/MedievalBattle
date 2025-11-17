using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private float _amplitude = 0.5f;
    [SerializeField] private float _frequency = 1f;

    private Vector3 _startPosition;

    public int HealAmount => healAmount;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);

        float newY = _startPosition.y + Mathf.Sin(Time.time * _frequency) * _amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health player = other.GetComponent<Health>();

        if (player != null)
        {
            if (player.CurrentHealth < player.MaxHealth)
            {
                player.Heal(healAmount);
                gameObject.SetActive(false);
            }
        }
    }
}