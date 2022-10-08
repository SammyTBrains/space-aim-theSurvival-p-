using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : PowerUp
{
    private bool _hit;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player) && !_hit)
        {
            _hit = true;
            player.NoOfEnemyHits = 0;
            PowerUpCollectedFeedback(other);
            if (SpawnManager.Instance.PlayerWeaponLevel < 4)
            {
                SpawnManager.Instance.PlayerWeaponLevel++;
                SpawnManager.Instance.ResetPlayerWeaponLevel();
            }
            _model.SetActive(false);
            player.NoOfUpgradesCollected++;
            Destroy(gameObject, 1.6f);
        }
    }
}
