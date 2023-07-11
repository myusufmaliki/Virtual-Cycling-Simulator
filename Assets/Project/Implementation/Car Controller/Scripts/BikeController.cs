using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Globalization;
using System;

public class BikeController : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM7", 115200); //SERIAL PORT
    public string strReceived;
    public string[] strData = new string[2];
    public string[] strData_received = new string[2];
    public float yawData;
    public float rps;
    public float inersia = 0.125f;
    

    //byte[] Data = new byte[7];
    public Transform BikeTransform; // kemiringan terhadap x

    public const string HORIZONTAL_INPUT = "Horizontal";
    public const string VERTICAL_INPUT = "Vertical";

    [Header("Properties")]
    private float _brakeForce = 300f; //atas dasar apa ? kok bisa 300
    public float maxSteerAngle;
    public float motorForce = 0;
    public float motorTorque = 0;

    

    [Header("Wheel Collider")]
    [SerializeField] private WheelCollider _frontWheelCollider = null;
    [SerializeField] private WheelCollider _backWheelCollider = null;


    [Header("Wheel Transform")]
    [SerializeField] private Transform _frontWheelModel = null;
    [SerializeField] private Transform _backWheelModel = null;

    // private Vector2 _inputDirection = Vector2.zero;
    private float _currentSteerAngle = 0f;
    private float _currentBrakeForce = 0f;
    private bool _isBraking = false;

    private void Start()
    {
        sp.Open();
    }

    private void Update()
    {
       DataS();
       //Degree();
        
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();

    }

    private void HandleMotor()
    {
        _backWheelCollider.motorTorque = inersia * motorForce * 628f;
        motorTorque = _backWheelCollider.motorTorque;
        _currentBrakeForce = _isBraking || motorForce == 0 ? _brakeForce : 0f;
        ApplyBrake();
    }
    private void ApplyBrake()
    {
        if (_currentBrakeForce == 0)
        {
           // _frontWheelCollider.brakeTorque = 0;
            _backWheelCollider.brakeTorque = 0;
            return;
        }
        //_frontWheelCollider.brakeTorque += Time.deltaTime * _currentBrakeForce * 1f;
        _backWheelCollider.brakeTorque += Time.deltaTime * _currentBrakeForce * 0.1f;
    }


    private void DataS()
    {
        strReceived = sp.ReadLine();
        strData = strReceived.Split(',');
        if (strData[0] != "" && strData[1] != "")
        {
            strData_received[0] = strData[0];
            strData_received[1] = strData[1];

            yawData = float.Parse(strData_received[0]);
            rps = float.Parse(strData_received[1]);

            maxSteerAngle = yawData;
            motorForce = rps;

        }

    }

    private void Degree()

    {
        if (sp.IsOpen == false)
        {
            sp.Open();
        }
         
        float degree = (float)Math.Round(BikeTransform.position.y, 1);
        float min = 0;
        float max = 1;
        if (degree < min)
            degree = min;
        if(degree > max)
            degree = max;
        float percent = (degree - min)/ (max - min) * 100;
        sp.WriteLine(percent.ToString());
        Debug.Log(percent);


        /**degree *= 10;
        int intValue = (int)degree;

        int ValH = intValue / 1000 % 10;
        int ValHH = intValue / 100 % 10;
        int ValHHH = intValue / 10 % 10;
        int ValHHHH = intValue / 1 % 10;


        byte BValH = (byte)ValH;
        byte BValHH = (byte)ValHH;
        byte BValHHH = (byte)ValHHH;
        byte BValHHHH = (byte)ValHHHH;



        Data[0] = 255;
        Data[1] = 254;
        Data[2] = BValH;
        Data[3] = BValHH;
        Data[4] = BValHHH;
        Data[5] = BValHHHH;
        Data[6] = (byte)(BValH ^ BValHH ^ BValHHH ^ BValHHHH);



        for (int a = 0; a < 7; a++)
        {
            sp.Write(Data, 0, 7);
        }

        Debug.Log(intValue);*/
    }

private void HandleSteering()
    {

        _currentSteerAngle = maxSteerAngle;
        _frontWheelCollider.steerAngle = _currentSteerAngle;
    }


    private void UpdateWheels()
    {
        UpdateSingleWheel(_frontWheelCollider, _frontWheelModel);
        UpdateSingleWheel(_backWheelCollider, _backWheelModel);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
    void OnDestroy()
    {
        sp.Close();
    }
}
