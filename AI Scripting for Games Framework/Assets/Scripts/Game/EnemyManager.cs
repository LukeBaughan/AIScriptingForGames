using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyToSpawn;
    private MovingEntity enemy;
    private MovingEntity m_PlayerMovingEntity;
    private float spawnBoundsSize = 2.0f;
    public float x;
    public float y;
    public float width;
    public float height;

    Rect spawnZoneLeft = Rect.zero;
    Rect spawnZoneRight = Rect.zero;
    Rect spawnZoneTop = Rect.zero;
    Rect spawnZoneBottom = Rect.zero;


    // Start is called before the first frame update
    void Start()
    {
        m_PlayerMovingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();

        SpawnEnemyGroup();
    }

    private void SpawnEnemyGroup()
    {
        SetUpspawnZones();
        //SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        // Spawns the enemy
        GameObject enemyObj = Instantiate(enemyToSpawn, new Vector3(10,10,0), Quaternion.identity);
        enemy = enemyObj.GetComponent<MovingEntity>();

        // Gets the steering manager and steering behaviour/s
        SteeringBehaviour_Manager m_Manager = enemyObj.GetComponent<SteeringBehaviour_Manager>();
        SteeringBehaviour_Pursuit m_Pursuit = enemyObj.AddComponent<SteeringBehaviour_Pursuit>();

        m_Manager.m_SteeringBehaviours.Add(m_Pursuit);

        // Sets the enemy's target to be the player
        m_Pursuit.m_PursuingEntity = m_PlayerMovingEntity;
        m_Manager.EnableExclusive(m_Pursuit);
    }

    private Vector3 SetUpspawnZones()
    {
        Vector2 cameraSize;
        // How far the enemy can spawn from the camera border
        //float spawnBoundsSize = 7;

        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraSize.y = camera.orthographicSize;

        cameraSize.x = cameraSize.y * Screen.width / Screen.height;

        x = cameraSize.x;
        y = cameraSize.y;

        //float SpawnX = Random.Range(((cameraSize.x * -1) - spawnBoundsSize), (cameraSize.x  + spawnBoundsSize));
        // if(SpawnX > cameraSize.x)

        float cameraPosX = Camera.main.transform.position.x;
        float cameraPosY = Camera.main.transform.position.y;

        
        spawnZoneLeft.position = new Vector2(cameraPosX + x + spawnBoundsSize, cameraPosY);

        //float spawnX = spawnZone.x;

        return new Vector3(0, 0, 0);

    }

    private void OnDrawGizmos()
    {
        float cameraPosX = Camera.main.transform.position.x;
        float cameraPosY = Camera.main.transform.position.y;

        //right
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3(cameraPosX + x + spawnBoundsSize, cameraPosY, 0), new Vector3(spawnBoundsSize, y * 2.6f, 1));

        //left
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(cameraPosX - x - spawnBoundsSize, cameraPosY, 0), new Vector3(spawnBoundsSize, y * 2.6f, 1));

        //top
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(cameraPosX, cameraPosY + y + spawnBoundsSize, 0), new Vector3(x * 2.1f, spawnBoundsSize, 1));

        //bottom
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(new Vector3(cameraPosX, cameraPosY - y - spawnBoundsSize, 0), new Vector3(x * 2.1f, spawnBoundsSize, 1));
    }

}
