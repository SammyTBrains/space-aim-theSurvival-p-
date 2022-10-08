using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float maxHealth { get; set; }
    public float health { get; set; }

    public void Damage(float damage, GameObject damager);
}
