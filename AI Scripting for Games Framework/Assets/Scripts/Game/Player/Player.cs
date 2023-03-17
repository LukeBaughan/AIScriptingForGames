using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingEntity
{
    public float m_Acceleration;

    //input
    public float m_Horizontal;
    public float m_Vertical;

    public bool m_CanMoveWhileAttacking;
    bool m_Attacking;

    private float m_AttackTime;
    public float m_AttackRate = 1.0f;

    protected override void Awake()
    {
        base.Awake();
        m_AttackTime = m_AttackRate;
    }

    private void Start()
    {
        InvokeRepeating("Attack", 1.0f, m_AttackRate);
    }

    void Update()
    {
        m_AttackTime += Time.deltaTime;

        if (!m_Attacking || (m_Attacking && m_CanMoveWhileAttacking))
        {
            //up
            if (m_Vertical > 0)
            {
                m_Animator.SetInteger("Direction", 0);
            }
            //right
            else if (m_Horizontal > 0)
            {
                m_Animator.SetInteger("Direction", 1);
            }
            //down
            else if (m_Vertical < 0)
            {
                m_Animator.SetInteger("Direction", 2);
            }
            //left
            else if (m_Horizontal < 0)
            {
                m_Animator.SetInteger("Direction", 3);
            }
            //idle
            else
            {
                m_Animator.SetInteger("Direction", -1);
            }
        }
    }

    public void Attack()
    {
        m_Animator.SetTrigger("Attack");
        m_Attacking = true;
    }

    public void StopAttack()
	{
        m_Attacking = false;
	}

    // Enemy takes damage if it is in the weapon's collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity ent = collision.gameObject.GetComponent<Entity>();

        if (ent)
        {
            ent.TakeDamage(m_AttackPower);
        }
    }

    // Player takes damage while the enemy is touching them
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (m_AttackTime >= m_AttackRate)
        {
            Entity ent = collision.gameObject.GetComponent<Entity>();

            if (ent)
            {
                TakeDamage(ent.m_AttackPower);
                // Resets the attack timer
                m_AttackTime = 0;
            }
        }
    }

	protected override Vector2 GenerateVelocity()
	{
		return new Vector2(m_Horizontal, m_Vertical) * m_Acceleration;
    }
}
