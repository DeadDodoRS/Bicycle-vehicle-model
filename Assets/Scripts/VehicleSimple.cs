using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSimple : MonoBehaviour
{
    [SerializeField] private GameObject _frontWheel;

    [SerializeField] private float _length = 1;

    [SerializeField] private float lr = 0.5f;

    [SerializeField] private float _acceleration;
    [SerializeField] private float _velocity;
    [SerializeField] private float _angle;
    [SerializeField] private float _angleLimit = 30;

    [Header("Keyboard control")]
    public float accelSpeed = 2;
    public float turningSpeed = 2;

    private void LateUpdate()
    {
        UpdateInput();
        WheelUpdate();

        // Calculate slip angle
        float radiansAngle = _angle * Mathf.Deg2Rad;
        float slipAngle = Mathf.Atan(lr * Mathf.Tan(radiansAngle) / _length);

        // Calculate position
        float carRotation = transform.eulerAngles.z;

        _velocity = _velocity + _acceleration * Time.deltaTime;

        float velX = _velocity * Mathf.Cos((carRotation + slipAngle) * Mathf.Deg2Rad);
        float velY = _velocity * Mathf.Sin((carRotation + slipAngle) * Mathf.Deg2Rad);

        float worldOffsetX = velX * Time.deltaTime;
        float worldOffsetY = velY * Time.deltaTime;

        float newPosX = transform.position.x + worldOffsetX;
        float newPosY = transform.position.y + worldOffsetY;
        transform.position = new Vector3(newPosX, newPosY, 0);


        // Calculate orientation
        float s = _length / Mathf.Tan(radiansAngle);
        float turningRadius = s / Mathf.Cos(slipAngle * Mathf.Deg2Rad);
        float angularVelocity = _velocity / turningRadius;

        float addRotationAngle = angularVelocity * Time.deltaTime * Mathf.Rad2Deg;

        Debug.LogError($"radius: {turningRadius}; vel: {_velocity}; angVel:{angularVelocity}");

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + addRotationAngle);
    }

    private void WheelUpdate()
    {
        _angle = Mathf.Clamp(_angle, -_angleLimit, _angleLimit);
        _frontWheel.transform.localEulerAngles = new Vector3(0, 0, _angle);
    }

    private void UpdateInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _acceleration += accelSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            _acceleration -= accelSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            _angle += turningSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _angle -= turningSpeed * Time.deltaTime;
        }
    }
}
