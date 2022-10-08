using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : PowerUp
{
    [SerializeField]
    private float _shieldTime;

    private bool _hit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) && !_hit)
        {
            _hit = true;
            PowerUpCollectedFeedback(other);
            player._shield.SetActive(true);
            player._isSheilded = true;
            player.CallShieldRoutine(_shieldTime, gameObject);
            _model.SetActive(false);
            player.NoOfUpgradesCollected++;
        }
    }
}
