using UnityEngine;
using System.Collections;

public class EnemySpike : EnemyBase {

    bool _spikes = false;
   
	void Start () {
        Init();
        

	}

    protected override void OnPlayerHit(PlayerHit player, bool above)
    {
        if (_spikes || !above)
        {
            player.OnDeath(gameObject);      
        }
    }
    protected override void HitCallback(PlayerHit player)
    {
        player.BounceUp(player.gameObject);
        if (!_spikes) OnDeath(player);
    }

    protected override void EnemyUpdate()
    {
        int currentFrame = _sheetAni.GetFrame();
        _spikes = currentFrame >= 3 && currentFrame <= 5;
    }

}
