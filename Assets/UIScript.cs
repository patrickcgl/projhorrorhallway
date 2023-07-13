//Handwerk Patrick, Cologne Game Lab, 11135936, 7/14/2023

using TMPro;
using UnityEngine;

/// <summary>
/// References to data for UI information showing
/// </summary>
public class UIScript : MonoBehaviour
{
    /// <summary>
    /// UI Text value for current enery level
    /// </summary>
    public TextMeshProUGUI TextEnergy;
    
    /// <summary>
    /// UI Text value for current health
    /// </summary>
    public TextMeshProUGUI TextHealth;
    
    /// <summary>
    /// Reference to the Flashlight game Object to get it's current EnergyLevel
    /// </summary>
    public GameObject Flashlight;

    // Update is called once per frame
    void Update()
    {
        TextEnergy.SetText(Flashlight.GetComponent<FlashlightItem>().EnergyLevel.ToString());
        TextHealth.SetText(PlayerController.Instance.health.ToString());
    }
}
