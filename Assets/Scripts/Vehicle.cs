using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] private float _angle;
    [SerializeField] private float _angleLimit = 25;

    [SerializeField] private float _wheelForce = 2;

    [SerializeField] private float _mass = 1000;

    [Header("Visual")]
    [SerializeField] private GameObject _frontWheel;

    public float al = 1;
    public float bl = 1;

    public float inertia = 1;

    public float fx1 = 0;
    public float fx2 = 0;
    public float fy1 = 0;
    public float fy2 = 0;


    [Header("Debug")]
    public float CurrentSpeedX = 0;
    public float CurrentSpeedY = 0;

    public float CurrentAcccelerationX = 0;
    public float CurrentAcccelerationY = 0;

    public float time = 0;

    // TODO: Посмотреть код чела!

    private void LateUpdate()
    {
        _angle = Mathf.Clamp(_angle, -_angleLimit, _angleLimit);
        _frontWheel.transform.localEulerAngles = new Vector3(0, 0, -_angle);

        // This is local acceleration
        CurrentAcccelerationX = (fx1 * Mathf.Cos(_angle) + fx2 - fy1 * Mathf.Sin(_angle)) / _mass;
        CurrentAcccelerationY = (fx1 * Mathf.Sin(_angle) + fy1 * Mathf.Cos(_angle) + fy2) / _mass;

        CurrentSpeedX = CurrentSpeedX + CurrentAcccelerationX * Time.deltaTime;
        CurrentSpeedY = CurrentSpeedY + CurrentAcccelerationY * Time.deltaTime;

        float offsetX = CurrentSpeedX * Time.deltaTime + CurrentAcccelerationX * Mathf.Pow(Time.deltaTime, 2) / 2;
        float offsetY = CurrentSpeedY * Time.deltaTime + CurrentAcccelerationY * Mathf.Pow(Time.deltaTime, 2) / 2;

        // Convert acceleration from car space to global cords
        float carRotation = -transform.eulerAngles.z;

        float a = Mathf.Sin((carRotation * Mathf.PI) / 180);
        float b = Mathf.Cos((carRotation * Mathf.PI) / 180);

        float worldOffsetX = offsetX * a + offsetY * a;
        float worldOffsetY = offsetX * b + offsetY * b;

        float newPosX = transform.position.x + worldOffsetX;
        float newPosY = transform.position.y + worldOffsetY;

        transform.position = new Vector3(newPosX, newPosY, 0);

        float inertia = 1;

        float addRotationAngle = (al * fx1 * Mathf.Sin(_angle) + al * fy1 * Mathf.Cos(_angle) - bl * fy2) / 2;
        addRotationAngle *= Time.deltaTime;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + addRotationAngle);
    }
}
