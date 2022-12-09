using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Heals the entity to maximum health
        Entity ent = collision.gameObject.GetComponent<Entity>();
        DecisionMakingEntity entDecision = ent.GetComponent<DecisionMakingEntity>();

        Debug.Log(ent);
        if (ent)
        {
            ent.TakeDamage((ent.m_MaxHealth - ent.m_CurrentHealth) * -1);
            if(entDecision != null)
            {
                entDecision.onHealthPickedUp();
            }

            Debug.Log(ent.m_CurrentHealth);
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
