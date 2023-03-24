using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EnemyManager enemyManager;
    public Player player;


    // Start is called before the first frame update
    void Start()
    {
        enemyManager.Initialise();
        // Calls the AwardXP function when an enemy is killed
        enemyManager.m_OnEnemyDead.AddListener(AwardXP);
    }

    private void AwardXP()
    {
        // Gives player XP
    }
}
