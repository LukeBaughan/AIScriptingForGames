using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    Player playerCharacter;

    enum state
    {
        wander,
        pursuit,
        evade
    }
    state m_currentState;

    MovingEntity m_PursuingEnemy;
    MovingEntity m_EvadingEnemy;
    float m_SafeDistance = 5.0f;
    float m_EvadeEnterDistance = 2.0f;

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
                    m_currentState = state.pursuit;
                break;

            case state.pursuit:
                m_PursuingEnemy = GetClosestEnemyOnScreen();
                playerCharacter.m_Pursuit.m_PursuingEntity = m_PursuingEnemy;
                // Keeps a safe distance while waiting for the player to attack
                if (playerCharacter.m_TimeTillAtack <= 0.4)
                {
                    playerCharacter.m_Pursuit.m_SafeDistance = m_SafeDistance;
                }
                else
                    playerCharacter.m_Pursuit.m_SafeDistance = m_EvadeEnterDistance;
                playerCharacter.m_SB_Manager.EnableExclusive(playerCharacter.m_Pursuit);

                // If the enemy  has been killed, wander
                if (!playerCharacter.m_Pursuit.m_PursuingEntity)
                    m_currentState = state.wander;
                // If the player gets too close to the enemy, evade it
                if (Vector2.Distance(playerCharacter.transform.position, m_PursuingEnemy.transform.position) <= m_EvadeEnterDistance)
                    m_currentState = state.evade;
                break;

            case state.evade:
                // Evades the closest enemy if they are within evading distance
                m_EvadingEnemy = GetClosestEnemyOnScreen();
                if (Vector2.Distance(playerCharacter.transform.position, m_EvadingEnemy.transform.position) <= m_SafeDistance && playerCharacter.m_TimeTillAtack <= 0.5f)
                {
                    playerCharacter.m_Evade.m_EvadingEntity = m_EvadingEnemy;
                    playerCharacter.m_SB_Manager.EnableExclusive(playerCharacter.m_Evade);
                }
                else
                    m_currentState = state.wander;
                break;
        }
    }
}
