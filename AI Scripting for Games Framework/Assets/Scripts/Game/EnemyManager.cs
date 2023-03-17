using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyToSpawn;
    private MovingEntity enemy;
    public float enemySpawnRate;

    private MovingEntity m_PlayerMovingEntity;

    // How far the enemy can spawn from the camera border
    public float spawnBoundsSize = 2.0f;
    private Vector2 cameraSize;

    Rect spawnZoneLeft = Rect.zero;
    Rect spawnZoneRight = Rect.zero;
    Rect spawnZoneTop = Rect.zero;
    Rect spawnZoneBottom = Rect.zero;


    // Start is called before the first frame update
    void Start()
    {
        m_PlayerMovingEntity = GameObject.Find("Player").GetComponent<MovingEntity>();

        UpdateSpawnZones();
        InvokeRepeating("SpawnEnemyRandom", 1.0f, enemySpawnRate);
    }

    private void Update()
    {
        UpdateSpawnZones();
    }

    private void SpawnEnemyRandom()
    {
        // Picks a random zone to spawn the enemy in
        int spawnZoneValue = Random.Range(0, 4);

        switch (spawnZoneValue)
        {
            case 0:
                SpawnEnemy(GetRandomPositonInZone(spawnZoneLeft), spawnZoneValue);
                break;
            case 1:
                SpawnEnemy(GetRandomPositonInZone(spawnZoneRight), spawnZoneValue);
                break;
            case 2:
                SpawnEnemy(GetRandomPositonInZone(spawnZoneTop), spawnZoneValue);
                break;
            case 3:
                SpawnEnemy(GetRandomPositonInZone(spawnZoneBottom), spawnZoneValue);
                break;
            default:
                Debug.Log("Range Too Large");
                break;
        }
    }

    private Vector2 GetRandomPositonInZone(Rect spawnZone)
    {
        // Gets a random point in the zone's perimeter
        Vector2 spawnPosition = new Vector2(Random.Range(spawnZone.position.x - (spawnZone.size.x / 2), spawnZone.position.x + (spawnZone.size.x / 2)),
            Random.Range(spawnZone.position.y - (spawnZone.size.y / 2), spawnZone.position.y + (spawnZone.size.y / 2)));

        return spawnPosition;
    }

    private void SpawnEnemy(Vector2 spawnLocation, int spawnZoneValue)
    {
        // Spawns the enemy
        GameObject enemyObj = Instantiate(enemyToSpawn, new Vector3(spawnLocation.x, spawnLocation.y, 0), Quaternion.identity);
        enemy = enemyObj.GetComponent<MovingEntity>();

        CircleCollider2D enemyCollider = enemyObj.GetComponent<CircleCollider2D>();
        SpriteRenderer enemySpriteRenderer = enemyObj.GetComponent<SpriteRenderer>();

        bool suitableSpawn = false;

        // Prevents the enemy from spawning on top of another enemy
        while (!suitableSpawn)
        {
            Collider2D[] overlappingEnemies = Physics2D.OverlapCircleAll(enemyObj.transform.position, enemySpriteRenderer.bounds.size.x / 2);
            // If there are no overlapping enemies, it is a suitable spawn
            if (overlappingEnemies.Length == 0)
            {
                suitableSpawn = true;
                break;
            }
            else
            {
                foreach (Collider2D entity in overlappingEnemies)
                {
                    // Keeps suitableSpawn false and breaks the loop if the enemy is colliding with another enemy
                    if (entity.GetComponent<MovingEntity>() && entity.GetComponent<MovingEntity>().gameObject != enemyObj.gameObject)
                    {
                        suitableSpawn = false;
                        break;
                    }
                    else
                        suitableSpawn = true;
                }
            }
            // If the spawn is not suitable, move it along (by its size amount)
            if (suitableSpawn)
                break;
            else
            {
                // Changes the move direction based on which zone the enemy is in
                switch (spawnZoneValue)
                {
                    // Left
                    case 0:
                        enemyObj.transform.position = new Vector3(enemyObj.transform.position.x - enemySpriteRenderer.bounds.size.x, enemyObj.transform.position.y, 0);
                        break;
                    // Right
                    case 1:
                        enemyObj.transform.position = new Vector3(enemyObj.transform.position.x + enemySpriteRenderer.bounds.size.x, enemyObj.transform.position.y, 0);
                        break;
                    // Top
                    case 2:
                        enemyObj.transform.position = new Vector3(enemyObj.transform.position.x, enemyObj.transform.position.y + enemySpriteRenderer.bounds.size.y, 0);
                        break;
                    // Bottom
                    case 3:
                        enemyObj.transform.position = new Vector3(enemyObj.transform.position.x, enemyObj.transform.position.y - enemySpriteRenderer.bounds.size.y, 0);
                        break;
                }
            }
        }


        // Gets the steering manager and steering behaviour/s
        SteeringBehaviour_Manager m_Manager = enemyObj.GetComponent<SteeringBehaviour_Manager>();
        SteeringBehaviour_Pursuit m_Pursuit = enemyObj.AddComponent<SteeringBehaviour_Pursuit>();

        m_Manager.m_SteeringBehaviours.Add(m_Pursuit);

        // Sets the enemy's target to be the player
        m_Pursuit.m_PursuingEntity = m_PlayerMovingEntity;
        m_Manager.EnableExclusive(m_Pursuit);
    }

    private void UpdateSpawnZones()
    {
        // Gets the camera object and its size
        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        cameraSize.y = camera.orthographicSize;
        cameraSize.x = cameraSize.y * Screen.width / Screen.height;

        float cameraPosX = Camera.main.transform.position.x;
        float cameraPosY = Camera.main.transform.position.y;

        // Calculates the size and positions of the spawn zones
        // Left
        spawnZoneLeft.position = new Vector2(cameraPosX - cameraSize.x - spawnBoundsSize, cameraPosY);
        spawnZoneLeft.size = new Vector2(spawnBoundsSize, cameraSize.y * 2.6f);

        // Right
        spawnZoneRight.position = new Vector2(cameraPosX + cameraSize.x + spawnBoundsSize, cameraPosY);
        spawnZoneRight.size = spawnZoneLeft.size;

        // Top
        spawnZoneTop.position = new Vector2(cameraPosX, cameraPosY + cameraSize.y + spawnBoundsSize);
        spawnZoneTop.size = new Vector2(cameraSize.x * 2.1f, spawnBoundsSize);

        // Bottom
        spawnZoneBottom.position = new Vector2(cameraPosX, cameraPosY - cameraSize.y - spawnBoundsSize);
        spawnZoneBottom.size = spawnZoneTop.size;
    }

    private void OnDrawGizmos()
    {
        float cameraPosX = Camera.main.transform.position.x;
        float cameraPosY = Camera.main.transform.position.y;

        ////left
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(new Vector3(spawnZoneLeft.position.x, spawnZoneLeft.position.y, 0), new Vector3(spawnZoneLeft.size.x, spawnZoneLeft.size.y, 1));
        ////Gizmos.DrawCube(new Vector3(cameraPosX - cameraSize.x - spawnBoundsSize, cameraPosY, 0), new Vector3(spawnBoundsSize, cameraSize.y * 2.6f, 1));

        ////right
        //Gizmos.color = Color.blue;
        //Gizmos.DrawCube(new Vector3(spawnZoneRight.position.x, spawnZoneRight.position.y, 0), new Vector3(spawnZoneRight.size.x, spawnZoneRight.size.y, 1));
        ////Gizmos.DrawCube(new Vector3(cameraPosX + cameraSize.x + spawnBoundsSize, cameraPosY, 0), new Vector3(spawnBoundsSize, cameraSize.y * 2.6f, 1));

        ////top
        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(new Vector3(spawnZoneTop.position.x, spawnZoneTop.position.y, 0), new Vector3(spawnZoneTop.size.x, spawnZoneTop.size.y, 1));
        ////Gizmos.DrawCube(new Vector3(cameraPosX, cameraPosY + cameraSize.y + spawnBoundsSize, 0), new Vector3(cameraSize.x * 2.1f, spawnBoundsSize, 1));

        ////bottom
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawCube(new Vector3(spawnZoneBottom.position.x, spawnZoneBottom.position.y, 0), new Vector3(spawnZoneBottom.size.x, spawnZoneBottom.size.y, 1));
        ////Gizmos.DrawCube(new Vector3(cameraPosX, cameraPosY - cameraSize.y - spawnBoundsSize, 0), new Vector3(cameraSize.x * 2.1f, spawnBoundsSize, 1));
    }

}
