using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAlert : MonoBehaviour
{
    private List<EnemyAI> enemiesInRadius = new List<EnemyAI>();

    private void Start()
    {
        // Add a sphere collider and set it as a trigger
        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider has an EnemyAI component
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null && !enemiesInRadius.Contains(enemy))
        {
            enemiesInRadius.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove the EnemyAI from the list when it exits the radius
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemiesInRadius.Remove(enemy);
        }
    }

    public void Death()
    {
        foreach (EnemyAI enemy in enemiesInRadius)
        {
            enemy.CheckSound();
        }
    }
}
