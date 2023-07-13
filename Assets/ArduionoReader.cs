//Handwerk Patrick, Cologne Game Lab, 11135936, 7/14/2023

using System.IO.Ports;
using UnityEngine;
using TMPro;

/// <summary>
/// Arduino hardware interface to collect the current serial port stream and split into sepearte sensor readings and then normalized
/// to useable data.
/// </summary>
public class ArduionoReader : MonoBehaviour
{
    private SerialPort _serialPort;
    private bool _ledOn;

    void Start()
    {
        _serialPort = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);
        _serialPort.Open();
    }

    void Update()
    {
        if (_serialPort.IsOpen)
        {
            string data = _serialPort.ReadLine();
            
            ReadSerialIntoProperties(data, out var sensorLight1, out var sensorLight2, out var sensorLight3);

            Normalize(sensorLight1, sensorLight2, sensorLight3);
        }

    }
    
    /// <summary>
    /// Takes the read string from the serial port and splits up the message and reads them into proper
    /// datatypes into the respective variables. Also initiates the sensor base first tick
    /// </summary>
    /// <param name="data">serial port string message</param>
    /// <param name="sensorLight1">out float value sensor reading photo resistor upper right</param>
    /// <param name="sensorLight2">out float value sensor reading photo resistor bottom</param>
    /// <param name="sensorLight3">out float value sensor reading photo resistor upper left</param>
    private void ReadSerialIntoProperties(string data, out float sensorLight1, out float sensorLight2,
        out float sensorLight3)
    {
        sensorLight1 = 0.0f;
        sensorLight2 = 0.0f;
        sensorLight3 = 0.0f;
        
        string[] dataarray = data.Split('|');

        if (dataarray.Length > 0)
        {
            if (!float.TryParse(dataarray[0], out PlayerController.Instance.XAxis))
            {
                Debug.Log("Error on PlayerController.XAxis: " + dataarray[0]);
            }
        }

        if (dataarray.Length > 1)
        {
            if (!float.TryParse(dataarray[1], out PlayerController.Instance.YAxis))
            {
                Debug.Log("Error on PlayerController.YAxis");
            }
        }

        if (dataarray.Length > 2)
        {
            PlayerController.Instance.StickButton = dataarray[2] == "1";
        }
        

        if (dataarray.Length > 3)
        {
            if (!float.TryParse(dataarray[3], out sensorLight1))
            {
                Debug.Log("Error on PlayerController.SensorLight1");
            }
        }

        if (dataarray.Length > 4)
        {
            if (!float.TryParse(dataarray[4], out sensorLight2))
            {
                Debug.Log("Error on PlayerController.SensorLight2");
            }
        }

        if (dataarray.Length > 5)
        {
            if (!float.TryParse(dataarray[5], out sensorLight3))
            {
                Debug.Log("Error on PlayerController.SensorLight3");
            }
        }

        if (!PlayerController.Instance.SensorInitiated)
        {
            PlayerController.Instance.SensorBase = sensorLight1;
            PlayerController.Instance.SensorInitiated = true;
        }
    }

    /// <summary>
    /// Normalizes raw sensor readings - maps sensor reading 0 - 1000 to -1.0f to 1.0f
    /// Value is being normalized from the "SensorBase". -1.0f to 0.0f are values between 0 and "SensorBase".
    /// 0.0f to 1.0f are values from "SensorBase" to 1000 of the sensor reading.
    /// </summary>
    void Normalize(float sensorLight1, float sensorLight2, float sensorLight3)
    {
        float normalizedValueUpperRight;
        float normalizedValueUpperLeft;
        float normalizedValueBottom;
        float differenceUpperRight = sensorLight1 - PlayerController.Instance.SensorBase;
        float differenceBottom = sensorLight2 - PlayerController.Instance.SensorBase;
        float differenceUpperLeft = sensorLight3 - PlayerController.Instance.SensorBase;
        
        if (differenceUpperRight < 0.0f)
        {
            normalizedValueUpperRight = (PlayerController.Instance.SensorBase + differenceUpperRight) / PlayerController.Instance.SensorBase;
            normalizedValueUpperRight = -1.0f * (1.0f - normalizedValueUpperRight);
        }
        else
        {
            normalizedValueUpperRight = differenceUpperRight / (1000.0f - PlayerController.Instance.SensorBase);
        }
        
        if (differenceUpperLeft < 0.0f)
        {
            normalizedValueUpperLeft = (PlayerController.Instance.SensorBase + differenceUpperLeft) / PlayerController.Instance.SensorBase;
            normalizedValueUpperLeft = -1.0f * (1.0f - normalizedValueUpperLeft);
        }
        else
        {
            normalizedValueUpperLeft = differenceUpperLeft / (1000.0f - PlayerController.Instance.SensorBase);
        }
        
        if (differenceBottom < 0.0f)
        {
            normalizedValueBottom = (PlayerController.Instance.SensorBase + differenceBottom) / PlayerController.Instance.SensorBase;
            normalizedValueBottom = -1.0f * (1.0f - normalizedValueBottom);
        }
        else
        {
            normalizedValueBottom = differenceBottom / (1000.0f - PlayerController.Instance.SensorBase);
        }

        PlayerController.Instance.SensorLightUR = normalizedValueUpperRight;
        PlayerController.Instance.SensorLightUL = normalizedValueUpperLeft;
        PlayerController.Instance.SensorLightB = normalizedValueBottom;
        
        // Debug.Log("SensorLightUR:" + PlayerController.Instance.SensorLightUR);
        // Debug.Log("SensorLightUL:" + PlayerController.Instance.SensorLightUL);
        // Debug.Log("SensorLightB:" + PlayerController.Instance.SensorLightB);
        // Debug.Log("SensorBase:" + PlayerController.Instance.SensorBase);
    }
    
    void OnApplicationQuit()
    {
        SwitchLEDState(false);
        _serialPort.Close();
    }
    
    public void SwitchLEDState(bool newState)
    {
        if (_ledOn != newState)
        {
            _ledOn = newState;
            _serialPort.WriteLine("L" + (_ledOn ? "1" : "0"));
        }
    }
}

