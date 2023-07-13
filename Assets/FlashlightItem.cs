//Handwerk Patrick, Cologne Game Lab, 11135936, 7/14/2023

using UnityEngine;
using DG.Tweening;


/// <summary>
/// Flashlight item the player is using. Intensity and direction can be controlled
/// </summary>
public class FlashlightItem : MonoBehaviour
{
    /// <summary>
    /// Reference to the hardware interface
    /// </summary>
    public GameObject ArduinoHardwareInterface;
    
    /// <summary>
    /// Information state if the flashlight is turned on.
    /// </summary>
    public bool bFlashlightOn = false;
    
    #region properties
    /// <summary>
    /// Current energy level of the flashlight. Only if "EnergyLevel" is above 0.0f the flashlight can be used
    /// </summary>
    public float EnergyLevel = 100.0f;
    
    /// <summary>
    /// Value that will being subtracted from "EnergyLevel" when power is drained
    /// </summary>
    private float EnergyTickValueSub = 5.0f;
    
    /// <summary>
    /// Value that will being added to "EnergyLevel" when power is increased by rotating analog stick
    /// </summary>
    private float EnergyTickValueAdd = 10.0f;
    
    /// <summary>
    /// Current upticking timer value
    /// </summary>
    private float EnergyTimerCurrent = 0.0f;
    
    /// <summary>
    /// Timer tick rate every X seconds
    /// </summary>
    private float EnergyTimerReset = 0.75f;
    
    /// <summary>
    /// Buffer value the sonsor readings need to surpass before triggering the flashlight
    /// </summary>
    private float SensorTriggerBuffer = .22f;

    /// <summary>
    /// Light component to change flashlight intensity
    /// </summary>
    private Light Flashlight;
    
    /// <summary>
    /// Reference to the hardware script to put output for leds.
    /// </summary>
    private ArduionoReader _arduionoReader;
    
    #endregion
    
    void Start()
    {
        Flashlight = GetComponent<Light>();
        
        PlayerController.Instance.StickRotation += AddEnergy;
        
        _arduionoReader = ArduinoHardwareInterface.GetComponent<ArduionoReader>();
    }
    
    void Update()
    {
        UpdateRotation();

        UpdateUsage();

        if (bFlashlightOn)
        {
            DetectHit();
        }
    }
    
    /// <summary>
    /// Casts an array of rays in the direction of the light to determine if an enemy was hit
    /// </summary>
    void DetectHit()
    {
        float angle = -20.0f;
        
        for (int i = 0; i < 9; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, transform.up);
            Vector3 direction = rotation * transform.forward;
            
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;

                if (hit.collider.gameObject.name.Contains("figure"))
                {
                    hit.collider.gameObject.GetComponent<Enemy>().GotHit();
                }

                Debug.DrawLine(ray.origin, hitPoint, Color.red);
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.green);
            }
            
            angle += 5.0f;
        }
    }

    /// <summary>
    /// Dependend on the current sensor readings, of the a sensor is higher than the set "SensorTriggerBuffer" it means
    /// the flashlight should turn on and start consuming energy. If energy levels are at 0.0f the flashlight will not turn
    /// on anymore.
    ///
    /// Consumption is solved by having a timer tick ever "EnergyTimerReset" seconds and reduces "EnergyLevel" by "EnergyTickValueSub"
    /// </summary>
    private void UpdateUsage()
    {
        if ((PlayerController.Instance.SensorLightUR > SensorTriggerBuffer
             || PlayerController.Instance.SensorLightUL > SensorTriggerBuffer
             || PlayerController.Instance.SensorLightB > SensorTriggerBuffer)
            && EnergyLevel > 0.0f)
        {
            bFlashlightOn = true;
            EnergyTimerCurrent += Time.deltaTime;
            Flashlight.intensity += 0.5f;

            if (Flashlight.intensity > 2.0f)
            {
                Flashlight.intensity = 2.0f;
            }
        }
        else
        {
            bFlashlightOn = false;
            Flashlight.intensity -= 0.5f;
            
            
            if (Flashlight.intensity < 0.0f)
            {
                Flashlight.intensity = 0.0f;
            }
        }

        if (EnergyTimerCurrent >= EnergyTimerReset)
        {
            EnergyLevel -= EnergyTickValueSub;
            EnergyTimerCurrent = 0.0f;
        }

        if (EnergyLevel <= 0.0f)
        {
            _arduionoReader.SwitchLEDState(true);
        }
        else
        {
            _arduionoReader.SwitchLEDState(false);
        }
    }
    
    /// <summary>
    /// Solving sensor readings if the right sensor value is higher the light starts tilting to the right - if the
    /// left sensor reading is higher the light starts tilting to the left.
    ///
    /// Using tween for smoother transition. If the reading difference between right and left sensors are below ".075f" it lerps
    /// to 0.0f relative rotation. This was done to be able to have a steady straight forward flashlight isntead of snapping
    /// from left to right.
    ///
    /// "newRotationPitch" is dependend on the bottom sensor reading - if the bottom sensor detects more light the flashight
    /// is pitch rotated down.
    /// </summary>
    private void UpdateRotation()
    {
        var newRotationYaw = 0.0f;

        if (PlayerController.Instance.SensorLightUR > PlayerController.Instance.SensorLightUL)
        {
            newRotationYaw = Mathf.Lerp(0.0f, 35.0f, PlayerController.Instance.SensorLightUR);
        }
        else
        {
            newRotationYaw = Mathf.Lerp(0.0f, -35.0f, PlayerController.Instance.SensorLightUL);
        }

        if (Mathf.Abs(PlayerController.Instance.SensorLightUR - PlayerController.Instance.SensorLightUL) <= .075f)
        {
            newRotationYaw = 0.0f;
        }

        var newRotationPitch = Mathf.Lerp(0.0f, 35.0f, PlayerController.Instance.SensorLightB);
        transform.DORotate(new Vector3(newRotationPitch, newRotationYaw, transform.eulerAngles.z), 0.8f, RotateMode.Fast);
    }
    
    /// <summary>
    /// Bound to the "playerController.Instance.StickRotation" event - everytime the player revolves a rotation of the analog stick
    /// "EnergyTickValueAdd" is being added to the current "EnergyLevel".
    /// </summary>
    private void AddEnergy()
    {
        EnergyLevel += EnergyTickValueAdd;

        if (EnergyLevel > 100.0f)
        {
            EnergyLevel = 100.0f;
        }
    }
    
    
}
