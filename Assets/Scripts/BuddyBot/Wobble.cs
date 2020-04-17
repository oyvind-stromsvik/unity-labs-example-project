using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class Wobble : MonoBehaviour
{
    public float Period;
    public float Scale;
    public float MaxOffset = 0.2f;

    private Vector3 _direction;
    private Vector3 _offset;
    private float _thisPeriod;
    private float _magnitude;
    private Rigidbody _rb;

    public void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        _rb.AddForceAtPosition(_direction*_magnitude*Scale, transform.position + _offset, ForceMode.Force);
    }

    public void OnEnable()
    {
        ChangeForceDirection();
    }

    private void ChangeForceDirection()
    {
        _thisPeriod = Period + Random.Range(-0.5f, 0.5f);
        _direction = Random.onUnitSphere;
        _offset = Random.onUnitSphere*MaxOffset;
        Invoke("ChangeForceDirection", _thisPeriod);
    }

    public void Update()
    {
        _magnitude = 0.5f*Mathf.Sin(Time.time*2*Mathf.PI/Period) + 0.5f;
    }
}