using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    private Player ps;
    private float mDamage;

    private void Start()
    {
        ps = GameObject.Find("Player").GetComponent<Player>();
        mDamage = transform.GetComponentInParent<WeaponZombie>().GetDamage();
    }

    public void AttackPlayer()
    {
        ps.Damage(mDamage);
    }
}
