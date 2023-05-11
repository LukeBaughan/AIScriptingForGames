using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    Player playerCharacter;


    enum state
    {
        wander,
        pursuit
    }
    state m_currentState;


    public void Initialise()
    {
        playerCharacter = GetComponent<Player>();
        playerCharacter.Initialise();

        // Sets the initial state to wander
        m_currentState = state.wander;
    }

    // Finds the closest enemy to the player if they are on the screen
    MovingEntity GetClosestEnemyOnScreen()
    {
        GameAI[] enemies = FindObjectsOfType<GameAI>();
        GameAI closestEnemy = null;
        float closestDistance = float.MaxValue;

        if (enemies.Length > 0)
        {
            // Sets the closest enemy/distance to be the first enemy in the array
            foreach (GameAI enemy in enemies)
            {
                // If the next enemy is closer than the current closest enemy and is on screen, make it the new closest enemy
                float distance = Vector2.Distance(playerCharacter.transform.position, enemy.transform.position);
                if (distance <= closestDistance && enemy.m_OnScreen)
                {
                    closestEnemy = enemy;
                    closestDistance = distance;
                }
            }
        }

        return closestEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_currentState)
        {
            case state.wander:
                playerCharacter.m_SB_Manager.EnableExclusive(playerCharacter.m_Wander);
                // If there are no enemies to pursuit, wander
                if (GetClosestEnemyOnScreen() != null)
                {
                    playerCharacter.m_Pursuit.m_PursuingEntity = GetClosestEnemyOnScreen();
                    m_currentState = state.pursuit;
                }
                break;
            case state.pursuit:
                playerCharacter.m_SB_Manager.EnableExclusive(playerCharacter.m_Pursuit);
                // If the enemy  has been killed, wander
                if (!playerCharacter.m_Pursuit.m_PursuingEntity)
                    m_currentState = state.wander;
                break;
        }
    }
}
