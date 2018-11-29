﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;

    [SerializeField] private GunType projectileType;

    private void Update()
    {
        transform.Translate(transform.up * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageableObject = other.GetComponent<IDamageable>(); // Checks if the 
            if (damageableObject != null)
            {
                Debug.Log("hit");

                switch (GameManager.instance.weaponSystem.currentFireMode)
                {
                    case WeaponSystem.FireMode.BULLET:
                        Destroy(gameObject);
                        BulletBehavior(other);
                        break;

                    case WeaponSystem.FireMode.EXPLOSIVE:
                        Destroy(gameObject);
                        ExplosiveBulletBehavior(other);
                        break;
                }
            }
        }
    }

    private void BulletBehavior(Collider2D other)
    {
        other.GetComponent<IDamageable>().TakeDamage(projectileType.damage);
    }

    private void ExplosiveBulletBehavior(Collider2D other)
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(other.transform.position, projectileType.damageRange);

        if (nearby.Length > 0)
        {
            foreach (Collider2D enemy in nearby)
            {
                EnemyController e = enemy.GetComponent<EnemyController>();

                if(e != null)
                {
                    float proximity = (other.transform.position - e.transform.position).magnitude;
                    float damageAmount = 1 - (proximity / projectileType.damageRange);

                    enemy.GetComponent<IDamageable>().TakeDamage(projectileType.damage * damageAmount + 0.5f);
                }
            }

            Debug.Log(nearby.Length);
        }
    }
}
