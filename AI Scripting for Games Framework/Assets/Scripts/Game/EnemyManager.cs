using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyToSpawn;
    private MovingEntity enemy;
    private MovingEntity m_PlayerMovingEntity;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerMovingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        GameObject enemyObj = Instantiate(enemyToSpawn, new Vector3(0, 0, 0), Quaternion.identity);
        enemy = enemyObj.GetComponent<MovingEntity>();

        SteeringBehaviour_Manager m_Manager = enemyObj.GetComponent<SteeringBehaviour_Manager>();
        SteeringBehaviour_Pursuit m_Pursuit = enemyObj.AddComponent<SteeringBehaviour_Pursuit>();
        m_Manager.m_SteeringBehaviours.Add(m_Pursuit);

        m_Pursuit.m_PursuingEntity = m_PlayerMovingEntity;

    }

}
