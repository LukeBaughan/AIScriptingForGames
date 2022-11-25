using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Entity ent = collision.gameObject.GetComponent<Entity>();
        Debug.Log(ent);
        if (ent)
        {
            print("E");
            ent.TakeDamage((ent.m_MaxHealth - ent.m_CurrentHealth) * -1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
