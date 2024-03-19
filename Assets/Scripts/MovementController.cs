using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController: MonoBehaviour
{
    // Start is called before the first frame update
    public Joystick joystick;
    public float speed = 2f;
    public float maxVelocityChange = 2f;
    public float tiltAmount = 10f;
    private Vector3 initVelocityVector = Vector3.zero; //
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        float _InputX = joystick.Horizontal;
        float _InputZ = joystick.Vertical;

        Vector3 _MovHorizontal = transform.right * _InputX;
        Vector3 _MovVertical = transform.forward * _InputZ;

        Vector3 _MovVelocity = (_MovHorizontal+_MovVertical).normalized * speed;

        Move(_MovVelocity);

        transform.rotation = Quaternion.Euler(joystick.Vertical * speed * tiltAmount, 0, -1 * joystick.Horizontal * speed * tiltAmount);
    }

    void Move(Vector3 moveVelocity)
    {
        initVelocityVector = moveVelocity;
    }

    private void FixedUpdate()
    {
        if(initVelocityVector!= Vector3.zero)
        {
            Vector3 velocity = _rb.velocity;
            Vector3 velocityChange = (initVelocityVector - velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

            _rb.AddForce(velocityChange, ForceMode.Acceleration);
        }
    }
}
