//Handwerk Patrick, Cologne Game Lab, 11135936, 7/14/2023

using UnityEngine;

/// <summary>
/// Dummy input script to use "G", "H", "B" and "SpaceBar" to imitate hardware controller
/// </summary>
public class DummyInput : MonoBehaviour
{
    void Start()
    {
        PlayerController.Instance.SensorBase = 0.0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerController.Instance.StickRotationDummy();
        }

        PlayerController.Instance.SensorLightUR = Input.GetKey(KeyCode.H) ? 1.0f : PlayerController.Instance.SensorBase;

        PlayerController.Instance.SensorLightUL = Input.GetKey(KeyCode.G) ? 1.0f : PlayerController.Instance.SensorBase;
        
        PlayerController.Instance.SensorLightB = Input.GetKey(KeyCode.B) ? 1.0f : PlayerController.Instance.SensorBase;

    }
}

