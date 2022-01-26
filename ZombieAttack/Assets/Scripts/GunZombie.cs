using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GunZombie : MonoBehaviour
{
    private GameObject player;
    private Player ps;
    private GameObject body;
    private Animator anim;
    private AnimatorStateInfo animInfo;
    private NavMeshAgent mAgent;
    private GameObject arm;
    private GameObject gun;
    private float mHealth = 100.0f;
    private float mDamage = 100.0f;
    private float distanceToPlayer;
    private bool isReloading = false;
    private bool isDead = false;
    private const int layerMask = ~(1 << 7);

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        ps = player.GetComponent<Player>();
        body = transform.Find("ZombieBody").gameObject;
        anim = body.GetComponent<Animator>();
        mAgent = GetComponent<NavMeshAgent>();
        mAgent.speed = 12.5f;
        arm = body.transform.Find("RightZombieArm").gameObject;
        gun = body.transform.Find("ZombieGun").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        anim.SetFloat("DistanceToPlayer", distanceToPlayer);
        anim.SetFloat("Health", mHealth);
        anim.SetBool("PlayerIsInSight", ps.PlayerInSight(gameObject, layerMask));
        anim.SetBool("IsReloading", isReloading);
        anim.SetBool("IsFacingPlayer", IsFacingPlayer());

        animInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (animInfo.IsName("Idle")) Idle();
        if (animInfo.IsName("Chase")) Chase();
        if (animInfo.IsName("AimAndShoot")) AimAndShoot();
        if (animInfo.IsName("Die")) Die();
    }

    void Idle()
    {
        mAgent.isStopped = true;
    }

    void Chase()
    {
        mAgent.isStopped = false;
        mAgent.SetDestination(player.transform.position);
    }

    void AimAndShoot()
    {
        mAgent.isStopped = true;
    }

    void Die()
    {
        mAgent.isStopped = true;

        if (!isDead)
        {
            GameManager.killCount++;
            isDead = true;
        }

        // Make every dead zombie's trigger true, so the player doesn't collide with them
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders) c.isTrigger = true;
    }

    public void TakeDamage(float dmg)
    {
        mHealth -= dmg;
    }

    /// <summary>
    /// Fire the gun in the direction in front of the zombie, then reload for 5 seconds
    /// </summary>
    public void Shoot()
    {
        RaycastHit hit;
        Physics.Raycast(body.transform.position, body.transform.forward, out hit, Mathf.Infinity, layerMask);
        if (hit.collider.gameObject.CompareTag("Player")) ps.Damage(mDamage);

        Debug.Log("Hit: " + hit.collider.gameObject.name);  

        StartCoroutine(Reload());
    }

    bool IsFacingPlayer()
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask)) return false;
        if (hit.collider.gameObject.CompareTag("Player")) return true;
        return false;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(5.0f);
        isReloading = false;
    }
}
