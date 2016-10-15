using UnityEngine;
using System.Collections;


public class ItemBlock : ActionBlock {

    private float _cooldown = 10.0f;
    private float _currentCooldown = 0.0f;
    void DropItem(Vector2 position) //TODO Which item to drop?
    {
        int itemToDrop = 0; //first item (bomb?)
        itemToDrop = Random.Range(0, (int)(ScoreManager.instance.itemPickups.GetLength(0)));
        GameObject item = GameObject.Instantiate(ScoreManager.instance.itemPickups[itemToDrop],transform.position + Vector3.up * 0.32f, Quaternion.identity) as GameObject;
        switch (itemToDrop)
        {
            case 0:
                ScoreManager.gameData.BombsSpawned++;
                break;
            case 1:
                ScoreManager.gameData.ShieldsSpawned++;
                break;
            case 2:
                ScoreManager.gameData.BouncesSpawned++;
                break;
            case 3:
                ScoreManager.gameData.LivesSpawned++;
                break;

        }

    }

    protected override void BlockUpdate()
    {
        if (_currentCooldown <= 0) return;
        _currentCooldown -= Time.deltaTime;
        if(_currentCooldown <= 0)
        {
            //sprite update to Active
            Animator animator = GetComponent<Animator>();
            if (animator != null) animator.enabled = true;
            GetComponent<SpriteRenderer>().sprite = PowerupManager.instance.ExtraSprites[0];
        }
    }


    bool CollisionCheck(Collision2D c)
    {
        PlayerHit hit = c.collider.gameObject.GetComponent<PlayerHit>();
        if (!hit) return false;

        if (hit.gameObject.transform.position.y < this.transform.position.y)
        {
            Activate(hit.gameObject);
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (_currentCooldown <= 0)
        {
            CollisionCheck(c);
        }
        

    }

    public override void Activate(GameObject activator)
    {
         GetComponent<Animation>().Play();
            PlayerHit[] hits = FindObjectsOfType<PlayerHit>();
        //Debug.Log(hits.Length);
        for (int i = hits.GetLength(0) - 1; i >= 0; i--)
        {
            if (Vector2.Distance(hits[i].transform.position, gameObject.transform.position) > 0.40f) continue;
            if (hits[i].transform.position.y > transform.position.y && hits[i].gameObject.GetComponent<PlayerMovement>().Grounded)
            {
                hits[i].OnDeath(activator);
            }
        }
        PlayerMovement activatorMov = activator.GetComponent<PlayerMovement>();
            if(activatorMov != null) activatorMov.ForceStopJump();
            if (_currentCooldown > 0) return;
            DropItem(transform.position);
            _currentCooldown = _cooldown;
            //GameObject.Destroy(gameObject);
            //Sprite update to Inactive
            Animator animator = GetComponent<Animator>();
            if (animator != null) animator.enabled = false;
            GetComponent<SpriteRenderer>().sprite = PowerupManager.instance.ExtraSprites[1];
           // return true;
    }
}
