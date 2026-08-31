using UnityEngine;

public class CarBodyCollision : MonoBehaviour
{
    public Transform Avto;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.MovePosition(Avto.position);
        rb.MoveRotation(Avto.rotation);
    }
}