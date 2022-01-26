using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RegularZombie : MonoBehaviour
{
    private GameObject player;
    private float mSpeed = 17.5f;

    /// <value>Player script</value>
    private Player ps;
    private STATE mState = STATE.IDLE;
    private NavMeshAgent mAgent;
    private float mHealth = 150.0f;
    private float mDamage = 20.0f;
    private float distanceToPlayer;
    private bool isAttacking = false;
    private bool isDead = false;
    private int layerMask = ~(1 << 7);
    private float mRange = 10.0f;

    private Dictionary<string, GameObject> arms = new Dictionary<string, GameObject>();

    enum STATE { IDLE, CHASE, ATTACK, DEAD }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        ps = player.GetComponent<Player>();
        arms.Add("left", transform.Find("LeftZombieArm").gameObject);
        arms.Add("right", transform.Find("RightZombieArm").gameObject);

        mAgent = gameObject.GetComponent<NavMeshAgent>();
        mAgent.speed = mSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (mState)
        {
            case STATE.IDLE: Idle(); break;
            case STATE.CHASE: Chase(); break;
            case STATE.ATTACK: Attack(); break;
            case STATE.DEAD: Die(); break;
        }
    }

    void Idle()
    {
        if (distanceToPlayer <= 80.0f && ps.PlayerInSight(gameObject, layerMask)) mState = STATE.CHASE;
        if (mHealth <= 0.0f) mState = STATE.DEAD;
        mAgent.isStopped = true;
    }

    void Chase()
    {
        if (distanceToPlayer > 80.0f)
        {
            mState = STATE.IDLE;
            return;
        }
        else if (distanceToPlayer <= mRange)
        {
            mState = STATE.ATTACK;
            return;
        }
        else if (mHealth <= 0.0f)
        {
            mState = STATE.DEAD;
            return;
        }

        mAgent.isStopped = false;
        if(!mAgent.SetDestination(player.transform.position)) mAgent.isStopped = true;
    }

    void Attack()
    {
        if (distanceToPlayer > mRange)
        {
            mState = STATE.CHASE;
            return;
        }

        if (mHealth <= 0.0f)
        {
            mState = STATE.DEAD;
            return;
        }

        mAgent.isStopped = true;
        if (!isAttacking) StartCoroutine(RegularAttackAnimation());
    }

    void Die()
    {
        // Disable and stop the nav mesh agent so the zombie stops moving
        if (mAgent.enabled) mAgent.isStopped = true;
        mAgent.enabled = false;

        // If it hasn't been played already, play the Die animation
        if (!isDead)
        {
            GameManager.killCount++;
            StartCoroutine(DieAnimation());
        }
        isDead = true;

        // Set colliders to trigger, so the player doesn't collide with the dead zombie
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders) c.isTrigger = true;
    }

    void AttackPlayer()
    {
        ps.Damage(mDamage);
    }

    public void TakeDamage(float dmg)
    {
        mHealth -= dmg;
        if (mHealth <= 0.0f) mState = STATE.DEAD;
    }

    IEnumerator RegularAttackAnimation()
    {
        isAttacking = true;

        // Move arms forward
        for (int i = 0; i < 10; ++i)
        {
            arms["left"].transform.Translate(0.0f, 0.0f, 0.1f);
            arms["right"].transform.Translate(0.0f, 0.0f, 0.1f);

            yield return new WaitForSeconds(0.025f);
        }

        // Move arms inward
        for (int i = 0; i < 10; ++i)
        {
            arms["left"].transform.Translate(0.1f, 0.0f, 0.0f);
            arms["right"].transform.Translate(-0.1f, 0.0f, 0.0f);

            yield return new WaitForSeconds(0.025f);
        }

        // Apply damage
        if (mState != STATE.DEAD) ps.Damage(mDamage);

        // Move arms outward
        for (int i = 0; i < 10; ++i)
        {
            arms["left"].transform.Translate(-0.1f, 0.0f, 0.0f);
            arms["right"].transform.Translate(0.1f, 0.0f, 0.0f);

            yield return new WaitForSeconds(0.025f);
        }

        // Move arms backward
        for (int i = 0; i < 10; ++i)
        {
            arms["left"].transform.Translate(new Vector3(0.0f, 0.0f, -0.1f), Space.Self);
            arms["right"].transform.Translate(new Vector3(0.0f, 0.0f, -0.1f), Space.Self);

            yield return new WaitForSeconds(0.025f);
        }

        isAttacking = false;
    }

    IEnumerator DieAnimation()
    {
        for (int i = 0; i < 100; ++i)
        {
            transform.Rotate(Vector3.right, -0.9f);
            transform.Translate(new Vector3(0.0f, 0.01f, 0.0f), Space.World);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
