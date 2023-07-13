//Handwerk Patrick, Cologne Game Lab, 11135936, 7/14/2023

using UnityEngine;

/// <summary>
/// Script for enemy behaviour in the game. Enemies have a target position and move slowly towards it if not hit by light. If they reached the target
/// they start hurting the players health. Everytime an Enemy gets hit they get stronger, pausing between hits gets shorter, they move faster and
/// do more damage.
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// Target position the enemy moves towards. If reached the enemy starts hurting the player.
    /// </summary>
    public Vector3 targetPosition;
    
    /// <summary>
    /// Rate at which to interpolate towards the target position. Gets faster every time the enemy is hit.
    /// </summary>
    private float interpolationSpeed;
    
    /// <summary>
    /// A pause time that the enemy waits to start moving towards it's target again. Gets shorter every time the enemy is hit.
    /// </summary>
    private float bufferWaitTime;
    
    /// <summary>
    /// Time between ticks of dealing damage when the Enemy has reached it's target position. Gets shorter every time the enemy is hit.
    /// </summary>
    private float damageTickTime;
    
    /// <summary>
    /// Damage value being dealt to the player when target position is reached. Gets greater every time the enemy is hit.
    /// </summary>
    private float damage;
    
    /// <summary>
    /// Storing where the enemy starts out to move back to when the enemy got hit.
    /// </summary>
    private Vector3 startingPosition;
    
    /// <summary>
    /// State if the enemy is moving towards the target or back to starting position.
    /// </summary>
    private bool movingToTarget = true;
    
    /// <summary>
    /// Stores how much Time.deltaTime has passed since the enemy got hit and has reached it's starting position again. Used to check if the time is
    /// greater than "bufferWaitTime" if yes start moving again.
    /// </summary>
    private float timeSinceDisabled;
    
    /// <summary>
    /// Stores how much Time.deltaTime has passed since the enemy has reached it's target position. Used to check if the time is
    /// greater than "damageTickTime" if yes deal damage to player.
    /// </summary>
    private float timeSinceReachedTarget;
    
    void Start()
    {
        interpolationSpeed = Random.Range(.07f, .18f);
        bufferWaitTime = Random.Range(2.0f, 7.0f);
        startingPosition = transform.position;
        damageTickTime = 3.0f;
        damage = .25f;
        movingToTarget = false;
        timeSinceDisabled = .0f;
        timeSinceReachedTarget = .0f;
    }
    
    void Update()
    {
        MoveEnemy();
    }
    
    /// <summary>
    /// Moves enemy towards the target position until hit by light. If hit, the enemy moves back to it's starting position, waits for "bufferWaitTime" and
    /// starts moving towards the target again.
    /// </summary>
    private void MoveEnemy()
    {
        if (movingToTarget)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, interpolationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) <= 0.085f)
            {
                timeSinceReachedTarget += Time.deltaTime;

                if (timeSinceReachedTarget >= damageTickTime)
                {
                    timeSinceReachedTarget = 0;
                    PlayerController.Instance.health -= damage;
                }
            }
        }
        else
        {
            transform.position =
                Vector3.Lerp(transform.position, startingPosition, interpolationSpeed * 10.0f * Time.deltaTime);

            if (Vector3.Distance(transform.position, startingPosition) <= 0.1f)
            {
                timeSinceDisabled += Time.deltaTime;

                if (timeSinceDisabled >= bufferWaitTime)
                {
                    movingToTarget = true;
                    GetComponent<CapsuleCollider>().enabled = true;
                    timeSinceDisabled = 0f;
                }
            }
        }
    }
    
    /// <summary>
    /// Actiopn called when player hits enemy with flashlight. Enemy gets stronger, increases "damage" and "interpolationSpeed", shortens "bufferWaitTime" and
    /// "damageTickTime".
    /// </summary>
    public void GotHit()
    {
        movingToTarget = false;
        GetComponent<CapsuleCollider>().enabled = false;
        interpolationSpeed += Random.Range(.01f, .04f);
        bufferWaitTime -= Random.Range(.25f, 1.0f);
        damage += Random.Range(.15f, .5f);
        damageTickTime -= Random.Range(.05f, .1f);

        if (bufferWaitTime <= 1.0f)
        {
            bufferWaitTime = Random.Range(1.2f, .5f);
        }
        
        if (damage >= 20.0f)
        {
            damage = 20.0f;
        }
        
        if (damageTickTime <= .75f)
        {
            damageTickTime = .75f;
        }
    }
}
