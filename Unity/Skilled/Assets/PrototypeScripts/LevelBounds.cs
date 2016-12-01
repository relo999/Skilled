using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelBounds : MonoBehaviour {

    public Bounds bounds;
    GameObject[] players;
    List<GameObject> objects;
    bool _requestFindPlayers = true;
    List<WrapClones> cloneList;

    public static LevelBounds instance;

    class WrapClones
    {
        public GameObject[] clones;
        int originalLayer;
        public GameObject original;
        Bounds levelBounds;
        bool renderingClones = false;
        public bool isStatic = false;
        public WrapClones(GameObject original, Bounds levelBounds, bool isStatic = false)
        {
            this.original = original;
            this.levelBounds = levelBounds;
            this.isStatic = isStatic;
            originalLayer = original.layer;
            clones = new GameObject[3]; //max 3 clones for each corner +original
            for (int i = 0; i < clones.Length; i++)
            {
                GameObject clone = GameObject.Instantiate(this.original);

       

                Component[] components = clone.GetComponentsInChildren(typeof(Component));

                foreach(Component comp in components)
                {
                    if(comp is SpriteRenderer && (comp.gameObject.name == "TAGobj" || comp.gameObject.name == "Chicken" || comp.gameObject.name == "OwnedRenderer"))
                    {
                        if (comp.gameObject.name == "OwnedRenderer") GameObject.Destroy(comp.gameObject.transform.parent.gameObject);
                        else GameObject.Destroy(comp.gameObject);
                    }

                    if (comp is PlayerHit)
                    {
                        PlayerHit hit = comp as PlayerHit;
                        hit.isClone = true;
                    }
                    if (!(comp is Transform) && !(comp is SpriteRenderer) && !(comp is Collider2D) && !(comp is PlayerHit))
                    {
                        
                        if(comp is LoopOutLevel)
                        {
                            LoopOutLevel loop = comp as LoopOutLevel;
                            loop.isClone = true;
                        }
                        if (comp is Bounce)
                        {
                            Bounce bounce = comp as Bounce;
                            bounce.isClone = true;
                        }
                        //if (comp is PlayerHit) clone.AddComponent<PlayerHitClone>();
                        Destroy(comp);


                    }
                }


            

        

        clones[i] = clone;
            }
            foreach (GameObject clone in clones)
            {
                clone.transform.parent = original.transform;
            }
        }
        public void UpdateClones()
        {
            
            for (int i = 0; i < 3; i++)
            {
                GameObject clone = clones[i];
                if (clone == null) continue;
                if(Mathf.Abs(clone.transform.position.x - levelBounds.center.x) - 0.16f > levelBounds.size.x / 2f  ||
                   Mathf.Abs(clone.transform.position.y - levelBounds.center.y) - 0.16f > levelBounds.size.y / 2f)
                {
                    clone.layer = LayerMask.NameToLayer("IgnoreCollisions");
                }
                else
                {
                    clone.layer = originalLayer;
                }
            }
            /*
            if (original.name.Contains("splat"))
            {
                Debug.Log(true);
            }*/

            //update sprite changes made by the original
            if (original == null) return;
            SpriteRenderer origRen = original.GetComponent<SpriteRenderer>();
            
            foreach (GameObject clone in clones)
            {
                SpriteRenderer cloneRen = clone.GetComponent<SpriteRenderer>();
                cloneRen.flipX = origRen.flipX;
                if (cloneRen.sprite == origRen.sprite) continue;
                cloneRen.sprite = origRen.sprite;
                
            }

            Vector2 diff = original.transform.position - levelBounds.center;
            float newXpos = diff.x < 0 ? levelBounds.size.x : -levelBounds.size.x;
            float newYpos = diff.y < 0 ? levelBounds.size.y : -levelBounds.size.y;


            clones[0].transform.localPosition = new Vector2(newXpos, 0);


            clones[1].transform.localPosition = new Vector2(newXpos, newYpos);
        

            clones[2].transform.localPosition = new Vector2(0, newYpos);



        }
        public void DestroyClones()
        {
            for (int i = clones.Length-1; i > 0; i--)
            {
                GameObject.Destroy(clones[i]);
            }
        }
    }

    public static LevelBounds Instance { private set; get; }
    public void RegisterObject(GameObject g, bool isStatic = false)
    {
        
        objects.Add(g);

        WrapClones newClones = new WrapClones(g, bounds, isStatic);
            cloneList.Add(newClones);
        newClones.UpdateClones();
    }

    public void StopClonesUpdate(GameObject g)
    {
        GameObject.Destroy(g.GetComponent<SheetAnimation>());
        WrapClones clones = cloneList.Find(x => x.original == g);
        for (int i = clones.clones.Length-1; i >= 0; i--)
        {
            GameObject clone = clones.clones[i];
            if (clone == null) continue;
            if (Mathf.Abs(clone.transform.position.x - bounds.center.x) - 0.16f > bounds.size.x / 2f ||
               Mathf.Abs(clone.transform.position.y - bounds.center.y) - 0.16f > bounds.size.y / 2f)
            {
                GameObject.Destroy(clone);
            }
        }
        objects.Remove(g);
        cloneList.Remove(clones);
    }

    public void UnRegisterObject(GameObject g)
    {
        objects.Remove(g);
        WrapClones clones = cloneList.Find(x => x.original == g);
        if (clones == null) return;
        clones.DestroyClones();
        cloneList.Remove(clones);
        
    }

    bool CheckBounds(GameObject obj, bool checkonly = false)
    {
        if (obj == null) return false;
        bool inbounds = false;

        if (obj.transform.position.y <= bounds.center.y - bounds.size.y / 2f)
        {
            if (!checkonly)
            {
                obj.transform.position += new Vector3(0, bounds.size.y, 0);
            }
        }else
        //above bounds
        if (obj.transform.position.y >= bounds.center.y + bounds.size.y / 2f)
        {
            if (!checkonly)
            {
                obj.transform.position += new Vector3(0, -bounds.size.y, 0);
            }
        }

        //left of bounds
        if (obj.transform.position.x <= bounds.center.x - bounds.size.x / 2f)
        {
            if (!checkonly)
            {
                obj.transform.position += new Vector3(bounds.size.x, 0, 0);
            }
        }else
        //right of bounds
        if (obj.transform.position.x >= bounds.center.x + bounds.size.x / 2f)
        {
            if (!checkonly)
            {
                obj.transform.position += new Vector3(-bounds.size.x, 0, 0);
            }
        }
        //if(newTemps.Count > 1)
       //     tempObjects.Add(newTemps);
        return inbounds;
    }
    void Update()
    {
        //TODO OPTIMIZE THIS
        foreach(GameObject obj in objects)
        {
            CheckBounds(obj);
        }
        foreach(WrapClones clones in cloneList)
        {
            //if (clones.isStatic) continue;
            clones.UpdateClones();
        }
    }

    public void FindPlayers()
    {
        PlayerHit[] hits = GameObject.FindObjectsOfType<PlayerHit>();
        players = new GameObject[hits.GetLength(0)];
        for (int i = 0; i < hits.GetLength(0); i++)
        {
            players[i] = hits[i].gameObject;
        }
        _requestFindPlayers = false;
    }

    void Awake()
    {
        //FindPlayers();
        instance = this;
        bounds = GetComponent<BoxCollider2D>().bounds;
        objects = new List<GameObject>();
        cloneList = new List<WrapClones>();
        Instance = this;
    }
	
}
