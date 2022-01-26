using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WeaponZombie : MonoBehaviour
{
    private GameObject player;
    private Player ps;
    private NavMeshAgent mAgent;
    private Animator anim;
    private AnimatorStateInfo animInfo;

    private float mHealth = 500.0f;
    private float mDamage = 50.0f;
    private float mSpeed = 14.0f;
    private float distanceToPlayer;
    private int layerMask = ~(1 << 7);
    private bool isDead = false;

    private Dictionary<string, GameObject> arms = new Dictionary<string, GameObject>();
    private GameObject mWeapon;
    private Collider[] colliders;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        ps = player.GetComponent<Player>();

        anim = transform.Find("ZombieBody").gameObject.GetComponent<Animator>();

        mAgent = gameObject.GetComponent<NavMeshAgent>();
        mAgent.speed = mSpeed;

        colliders = GetComponentsInChildren<Collider>(true);
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Set animator parameters
        anim.SetFloat("DistanceToPlayer", distanceToPlayer);
        anim.SetFloat("Health", mHealth);
        anim.SetBool("PlayerIsInSight", ps.PlayerInSight(gameObject, layerMask));

        // Execute AI based on current state
        animInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (animInfo.IsName("Idle")) Idle();
        if (animInfo.IsName("Chase")) Chase();
        if (animInfo.IsName("Attack")) Attack();
        if (animInfo.IsName("Die")) Die();
    }

    void Idle()
    {
        
    }

    void Chase()
    {
        mAgent.isStopped = false;
        mAgent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        mAgent.isStopped = true;
    }

    void Die()
    {
        if (!isDead)
        {
            GameManager.killCount++;
            isDead = true;
        }

        mAgent.isStopped = true;
        foreach (Collider c in colliders) c.isTrigger = true;
    }

    void AttackPlayer()
    {
        mAgent.isStopped = true;
        ps.Damage(mDamage);
    }

    public void TakeDamage(float dmg)
    {
        mHealth -= dmg;
    }

    bool PlayerInSight()
    {
        RaycastHit hit;

        if (!Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, Mathf.Infinity, layerMask)) return false;

        if (hit.collider.gameObject.CompareTag("Player")) return true;
        return false;
    }

    public float GetDamage()
    {
        return mDamage;
    }
}
