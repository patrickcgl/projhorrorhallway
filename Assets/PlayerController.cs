//Handwerk Patrick, Cologne Game Lab, 11135936, 7/14/2023

using UnityEngine;

/// <summary>
/// Singleton class for player inputs based off of custom arduino sensor readings
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region properties
    /// <summary>
    /// health value of the Player
    /// </summary>
    public float health = 100.0f;
    
    /// <summary>
    /// Private singleton instance
    /// </summary>
    private static PlayerController _instance;
    
    /// <summary>
    /// Public singleton instance - will consist between scenes. Creates new gameObject and component if not in scene
    /// </summary>
    public static PlayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerController>();

                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(PlayerController).Name;
                    _instance = go.AddComponent<PlayerController>();
                }
            }

            return _instance;
        }
    }
    
    /// <summary>
    /// Delegate declartion for StickRotation event
    /// </summary>
    public delegate void StickRotationDelegate();
    
    /// <summary>
    /// event triggered when one full rotation of the analog stick is being registered
    /// </summary>
    public event StickRotationDelegate StickRotation;
    
    #region input properties
    
    /// <summary>
    /// Input reading for anlog stick pressed
    /// </summary>
    public bool StickButton = false;
    
    /// <summary>
    /// Input reading for anlog axis x
    /// </summary>
    public float XAxis = 0.0f;
    
    /// <summary>
    /// Input reading for anlog axis y
    /// </summary>
    public float YAxis = 0.0f;
    
    /// <summary>
    /// Normalized value for upper right sensor. Values from -1.0f to 1.0f.
    /// Value is being normalized from the "SensorBase". -1.0f to 0.0f are values between 0 and "SensorBase".
    /// 0.0f to 1.0f are values from "SensorBase" to 1000 of the sensor reading.
    /// </summary>
    public float SensorLightUR = 0;
    
    /// <summary>
    /// Normalized value for upper left sensor. Values from -1.0f to 1.0f.
    /// Value is being normalized from the "SensorBase". -1.0f to 0.0f are values between 0 and "SensorBase".
    /// 0.0f to 1.0f are values from "SensorBase" to 1000 of the sensor reading.
    /// </summary>
    public float SensorLightUL = 0;
    
    /// <summary>
    /// Normalized value for bottom sensor. Values from -1.0f to 1.0f.
    /// Value is being normalized from the "SensorBase". -1.0f to 0.0f are values between 0 and "SensorBase".
    /// 0.0f to 1.0f are values from "SensorBase" to 1000 of the sensor reading.
    /// </summary>
    public float SensorLightB = 0;
    
    /// <summary>
    /// Sensor base determines the current rooms brightness - leveling the current sensor reading as comparison point
    /// to determine changes in brights from the room brightness
    /// </summary>
    public float SensorBase = 0;
    
    /// <summary>
    /// Used to only initiate the first sensor reading to "SensorBase"
    /// </summary>
    public bool SensorInitiated = false;
    
    #endregion

    // Helpers for calculating analog stick full rotations
    private float sensitivityThreshold = 0.5f;
    private float neutralPosition = 2.3f;
    private float previousAngle;
    
    #endregion

    private void Awake()
    {
        StickRotation += StickRotationEvent;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        CountAnalogStickRotations();
    }
    
    /// <summary>
    /// Calculates the analogs stick current angle from its X and Y axis sensor readings. Invokes "StickRotation" event
    /// if a full revolution is being detected if the previous angle and the current angle step over the 360 degree mark.
    /// </summary>
    private void CountAnalogStickRotations()
    {
        float angle = Mathf.Atan2(YAxis - neutralPosition, XAxis - neutralPosition) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360;
        }

        if (Mathf.Abs(XAxis - neutralPosition) > sensitivityThreshold || Mathf.Abs(YAxis - neutralPosition) > sensitivityThreshold)
        {
            if (angle < 45 && previousAngle >= 315)
            {
                StickRotation();
            }
            else if (angle >= 315 && previousAngle < 45)
            {
                StickRotation();
            }

            previousAngle = angle;
        }
    }
    
    /// <summary>
    /// Local function bound to "StickRotation" event to avoid event being "null" when invoked. Currently does nothing though
    /// </summary>
    private void StickRotationEvent()
    { }

    public void StickRotationDummy()
    {
        StickRotation();
    }
}
