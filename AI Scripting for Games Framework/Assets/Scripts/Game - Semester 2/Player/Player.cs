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

    public float m_TimeTillAtack = 0.0f;
    public float m_AttackRate = 1.0f;

    private float xp = 0.0f;

    Rect weaponBox = Rect.zero;

    // AI

    public SteeringBehaviour_Manager m_SB_Manager; 
    public SteeringBehaviour_Wander m_Wander;
    public SteeringBehaviour_Pursuit m_Pursuit;
    public SteeringBehaviour_Evade m_Evade;


    protected override void Awake()
    {
        base.Awake();
        m_AttackTime = m_AttackRate;

        m_SB_Manager = GetComponent<SteeringBehaviour_Manager>();
        if(!m_SB_Manager)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_Wander = GetComponent<SteeringBehaviour_Wander>();
        if (!m_Wander)
            Debug.LogError("Object doesn't have a Steering Behaviour Wander attached", this);

        m_Pursuit = GetComponent<SteeringBehaviour_Pursuit>();
        if (!m_Pursuit)
            Debug.LogError("Object doesn't have a Steering Behaviour Pursuit attached", this);

        m_Evade = GetComponent<SteeringBehaviour_Evade>();
        if(!m_Evade)
            Debug.LogError("Object doesn't have a Steering Behaviour Pursuit attached", this);

    }

    private void Start()
    {
        //InvokeRepeating("Attack", 1.0f, m_AttackRate);

    }

    public void Initialise()
    {
        m_SB_Manager.m_SteeringBehaviours.Add(m_Wander);
        m_SB_Manager.m_SteeringBehaviours.Add(m_Pursuit);
        m_SB_Manager.m_SteeringBehaviours.Add(m_Evade);
    }

    void Update()
    {
        // Attacks if every "attackRate" seconds
        m_TimeTillAtack += Time.deltaTime;
        if (m_TimeTillAtack >= m_AttackRate)
            Attack();

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
        // Resets the timer
        m_TimeTillAtack = 0.0f;

        // Creates a debug box to show the weapon's radius
    }

    public void StopAttack()
	{
        m_Attacking = false;

        // Destorys the weapon's debug box

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
        Vector2 steeringForce = m_SB_Manager.GenerateSteeringForce();

        // Direction
        m_Horizontal = steeringForce.x;
        m_Vertical = steeringForce.y;

        return steeringForce;

        //return new Vector2(m_Horizontal, m_Vertical) * m_Acceleration;
    }
}
