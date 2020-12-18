using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSimple : MonoBehaviour
{
    [SerializeField] private GameObject _frontWheel;

    [SerializeField] private float _length = 1;

    [SerializeField] private float lr = 0.5f;

    [SerializeField] private float _acceleration;
    //[SerializeField] private float _velocity;
    [SerializeField] private float _angle;
    [SerializeField] private float _angleLimit = 30;

    [Header("Keyboard control")]
    public float accelSpeed = 2;
    public float turningSpeed = 2;

    [Header("Test")]
    private float x_dot = 1;
    private float x_ddot = 0;

    private float y_dot;
    private float y_ddot;

    private float thetta_dot;
    private float thetta_ddot;

    [SerializeField] private float mass = 100;
    [SerializeField] private float c1 = 200;
    [SerializeField] private float c2 = 200;

    [SerializeField] private float inertia = 1;


    private void Physics()
    {
        float a = 1f;
        float b = 1f;

        y_dot = y_dot + y_ddot * Time.deltaTime;
        thetta_dot = thetta_dot + thetta_ddot * Time.deltaTime;

        float frontSlip = (y_dot + a * thetta_dot) / x_dot;
        float backSlip = (y_dot - b * thetta_dot) / x_dot;

        float delta = _angle * Mathf.Deg2Rad;

        y_ddot = - x_dot * thetta_dot + 1 / mass * (c1 * Mathf.Cos(delta) * (delta - frontSlip) - c2 * backSlip);
        thetta_ddot = 1 / inertia * (a * c1 * Mathf.Cos(delta) * (delta - frontSlip) + b * c2 * backSlip);

        Debug.Log($"{y_ddot}; {thetta_ddot}");
    }

    private void UpdateVehicle()
    {
        // Position
        float carRotation = transform.eulerAngles.z;
        x_dot = x_dot + _acceleration * Time.deltaTime;

        float velX = x_dot * Mathf.Cos(carRotation * Mathf.Deg2Rad) + y_dot * Mathf.Sin(carRotation * Mathf.Deg2Rad);
        float velY = x_dot * Mathf.Sin(carRotation * Mathf.Deg2Rad) + y_dot * Mathf.Cos(carRotation * Mathf.Deg2Rad);

        float worldOffsetX = velX * Time.deltaTime;
        float worldOffsetY = velY * Time.deltaTime;

        float newPosX = transform.position.x + worldOffsetX;
        float newPosY = transform.position.y + worldOffsetY;
        transform.position = new Vector3(newPosX, newPosY, 0);

        // Rotation

        float addRotationAngle = thetta_dot * Time.deltaTime * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + addRotationAngle);
    }

    private void LateUpdate()
    {
        WheelUpdate();
        UpdateInput();

        Physics();
        UpdateVehicle();

        //WheelUpdate();

        //// Calculate slip angle
        //float radiansAngle = _angle * Mathf.Deg2Rad;
        //float slipAngle = Mathf.Atan(lr * Mathf.Tan(radiansAngle) / _length);

        //// Calculate position
        //float carRotation = transform.eulerAngles.z;

        //x_dot = x_dot + _acceleration * Time.deltaTime;

        //float velX = x_dot * Mathf.Cos((carRotation + slipAngle) * Mathf.Deg2Rad);
        //float velY = x_dot * Mathf.Sin((carRotation + slipAngle) * Mathf.Deg2Rad);

        //float worldOffsetX = velX * Time.deltaTime;
        //float worldOffsetY = velY * Time.deltaTime;

        //float newPosX = transform.position.x + worldOffsetX;
        //float newPosY = transform.position.y + worldOffsetY;
        //transform.position = new Vector3(newPosX, newPosY, 0);


        //// Calculate orientation
        //float s = _length / Mathf.Tan(radiansAngle);
        //float turningRadius = s / Mathf.Cos(slipAngle * Mathf.Deg2Rad);
        //float angularVelocity = x_dot / turningRadius;

        //float addRotationAngle = angularVelocity * Time.deltaTime * Mathf.Rad2Deg;

        //Debug.LogError($"radius: {turningRadius}; vel: {x_dot}; angVel:{angularVelocity}");

        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + addRotationAngle);
    }

    private void WheelUpdate()
    {
        _angle = Mathf.Clamp(_angle, -_angleLimit, _angleLimit);
        _frontWheel.transform.localEulerAngles = new Vector3(0, 0, _angle);
    }

    private void UpdateInput()
    {
        //if (Input.GetKey(KeyCode.W))
        //{
        //    _acceleration += accelSpeed * Time.deltaTime;
        //}

        //if (Input.GetKey(KeyCode.S))
        //{
        //    _acceleration -= accelSpeed * Time.deltaTime;
        //}

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
