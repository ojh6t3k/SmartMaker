using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SmartMaker;

public class RCCar : MonoBehaviour
{
    public ArduinoApp arduinoApp;
    public MPU9150 mpu9150;
    public GenericServo servo;
    public AnalogOutput motorForward;
    public AnalogOutput motorBackward;

    public UiJoystick handle;
    public UiJoystick throttle;

    private Quaternion _gyroRotation = Quaternion.identity;
    private Quaternion _gyroCalibration = Quaternion.identity;
    private bool _gyroReady = false;
    private float _gyroInitTime = 0f;
    private float _userAngle = 0f;
    private float _carAngle = 0f;
    private float _goalAngle = 0f;

    void Awake()
    {
        arduinoApp.OnConnected.AddListener(OnConnected);
    }

    // Use this for initialization
    void Start ()
    {
        Input.gyro.enabled = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Quaternion gyro = Input.gyro.attitude * Quaternion.Inverse(_gyroCalibration);
        _gyroRotation = Quaternion.Slerp(_gyroRotation, Quaternion.Inverse(gyro), 0.5f);
        if(_gyroReady)
        {
            Vector3 gyroUp = Vector3.ProjectOnPlane(_gyroRotation * Vector3.up, Vector3.forward);
            _userAngle = Vector3.Angle(Vector3.up, gyroUp);
            if (Vector3.Dot(Vector3.forward, Vector3.Cross(Vector3.up, gyroUp)) < 0f)
                _userAngle = -_userAngle;
        }
        else
        {
            if (_gyroInitTime > 3f)
                _gyroReady = true;
            else
                _gyroInitTime += Time.deltaTime;
        }

        if (arduinoApp.connected)
        {
            Vector3 imuUp = Vector3.ProjectOnPlane(mpu9150.Rotation * Vector3.forward, Vector3.up);
            _carAngle = Vector3.Angle(Vector3.forward, imuUp);
            if (Vector3.Dot(Vector3.up, Vector3.Cross(Vector3.forward, imuUp)) < 0f)
                _carAngle = -_carAngle;

            Vector3 goalUp = handle.Axis;
            if (goalUp != Vector3.zero)
            {
                _goalAngle = Vector3.Angle(Vector3.up, goalUp);
                if (Vector3.Dot(Vector3.forward, Vector3.Cross(Vector3.up, goalUp)) > 0f)
                    _goalAngle = -_goalAngle;

                float diff = _goalAngle - _carAngle + _userAngle;
                if (diff > 180f)
                    diff = diff - 360f;
                else if (diff < -180f)
                    diff = 360f - diff;

                float dir = 0f;
                float speed = throttle.Axis.magnitude;
                if (Mathf.Abs(diff) < 90f)
                {
                    dir = diff;
                    motorForward.value = speed;
                    motorBackward.value = 0f;
                }                    
                else
                {
                    if (diff > 90f)
                        dir = diff - 90f;
                    else
                        dir = 90f + diff;

                    motorForward.value = 0f;
                    motorBackward.value = speed;
                }
                servo.angle = -Mathf.Clamp(dir, -45f, 45f);
            }
            else
            {
                _goalAngle = 0f;
                servo.angle = 0f;
                motorForward.value = 0f;
                motorBackward.value = 0f;
            }            
        }
    }

    public float userAngle
    {
        get
        {
            return _userAngle;
        }
    }

    public float carAngle
    {
        get
        {
            return _carAngle;
        }
    }

    public float goalAngle
    {
        get
        {
            return _goalAngle;
        }
    }

    public void Calibration()
    {
        if (!_gyroReady)
            return;

        _gyroCalibration = Input.gyro.attitude;
        mpu9150.Calibration();
    }

    private void OnConnected()
    {
    }
}
