using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSimple : MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private float _offset;

    [SerializeField] private GameObject _frontWheel;

    [SerializeField] private float _angle;
    [SerializeField] private float _angleLimit = 30;

    [Header("Keyboard control")]
    public bool KeyboardEnable = false;
    public bool IsCameraFollow = false;

    public float accelSpeed = 2;
    public float turningSpeed = 2;

    [Header("Test")]
    [SerializeField] private float a = 1f;
    [SerializeField] private float b = 1f;

    [SerializeField] private float x_dot = 1;
    private float x_ddot = 0;

    private float y_dot;
    private float y_ddot;

    private float thetta_dot;
    private float thetta_ddot;

    [SerializeField] private float mass = 100;
    [SerializeField] private float c1 = 200;
    [SerializeField] private float c2 = 200;

    [SerializeField] private float inertia = 1;

    [Header("PD-gains")]
    [SerializeField] private float Kp = 2;
    [SerializeField] private float Kd = 4;


    private void Physics()
    {
        y_dot = y_dot + y_ddot * Time.deltaTime;
        thetta_dot = thetta_dot + thetta_ddot * Time.deltaTime;

        float frontSlip = (y_dot + a * thetta_dot) / x_dot;
        float backSlip = (y_dot - b * thetta_dot) / x_dot;

        float delta = _angle * Mathf.Deg2Rad;

        y_ddot = - x_dot * thetta_dot + 1 / mass * (c1 * Mathf.Cos(delta) * (delta - frontSlip) - c2 * backSlip);
        thetta_ddot = 1 / inertia * (a * c1 * Mathf.Cos(delta) * (delta - frontSlip) + b * c2 * backSlip);
    }

    private void UpdateVehicle()
    {
        // Position
        float carRotation = transform.eulerAngles.z;
        x_dot = x_dot + x_ddot * Time.deltaTime;

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
        if (!KeyboardEnable)
            UpdateControl();
        else
            UpdateInput();
        
        WheelUpdate();

        Physics();
        UpdateVehicle();

        if(IsCameraFollow)
            _camera.transform.position = new Vector3(transform.position.x + _offset, _camera.transform.position.y, _camera.transform.position.z);
    }

    private float deltaPosError = 0;
    private float prevY;

    private void UpdateControl() 
    {
        deltaPosError = (prevY - transform.position.y) * 1 / Time.deltaTime;
        prevY = transform.position.y;

        float direction = -Mathf.Sign(transform.position.y);
        float posError = Mathf.Abs(transform.position.y);


        _angle = _angleLimit * (direction * posError * Kp + deltaPosError * Kd);

        Debug.LogError($"P: {direction * posError * Kp}; D: {deltaPosError * Kd}; Angle: {_angle}");
    }

    private void WheelUpdate()
    {
        _angle = Mathf.Clamp(_angle, -_angleLimit, _angleLimit);
        _frontWheel.transform.localEulerAngles = new Vector3(0, 0, _angle);
    }

    private void UpdateInput()
    {
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
