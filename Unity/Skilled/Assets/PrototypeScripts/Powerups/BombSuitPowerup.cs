using UnityEngine;
using System.Collections;

public class BombSuitPowerup : PowerupBase
{
    public float explosionRadius = 1f;    //slightly bigger than regular bomb

    public BombSuitPowerup(GameObject owner) : base(owner)
    {
        owner.GetComponent<SpriteOverlay>().SetSprite("PowerUps/Powers/BombSuit/BombSuit", owner.GetComponent<PlayerHit>().color);
    }

    public override void End()
    {
        owner.GetComponent<SpriteOverlay>().DestroySprite();
    }

    protected override void Activate()
    {
        GameObject ExplosionSprite = new GameObject("bombsuit explosion");
        ExplosionSprite.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("PowerUps/Powers/BombSuit/Explosion");
        ExplosionSprite.transform.position = owner.transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(owner.transform.position, explosionRadius);
        for (int i = 0; i < colliders.GetLength(0); i++)
        {
            PlayerHit hit = colliders[i].gameObject.GetComponent<PlayerHit>();
            if (hit == null) continue;
            if (hit.gameObject == owner) continue;
            if (colliders[i] is BoxCollider2D == false) continue;
            hit.OnDeath(owner);
        }
        ExplosionSprite.transform.parent = null;
        ExplosionSprite.transform.rotation = Quaternion.identity;
        DestroyAfterSeconds DAS = ExplosionSprite.AddComponent<DestroyAfterSeconds>();
        DAS.Seconds = 0.5f;
        ScoreManager.gameData.BombSuitUsed++;

        PowerupUser PU = owner.GetComponent<PowerupUser>();
        PU.EndPowerup();
        PU.SetPowerup(new EmptyPowerup(owner));

    }




}
