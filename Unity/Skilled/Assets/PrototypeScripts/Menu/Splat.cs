using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Splat : MonoBehaviour{

    Vector2 SCREEN_CENTER_WORLD = new Vector2(0, 2.5f);
    List<SplatPart> splatParts = new List<SplatPart>();
    SpriteRenderer _spriteRenderer = null;
    Texture2D _bakeTexture = null;
    const int _bakeTextureWidth = 16*20;
    const int _bakeTextureHeight = 16*16;
    Sprite[][] _splatSprites = null;
    public static Splat instance = null;

    //public List<SplatPart.SpriteData[]> splatData = new List<SplatPart.SpriteData[]>();

    void LoadSprites()
    {
        int sheetAmount = 5;
        _splatSprites = new Sprite[sheetAmount][];
        for (int i = 0; i < sheetAmount; i++) //5 different spritesheets
        {
            char colorChar = ((SheetAnimation.PlayerColor)i).ToString().ToUpper()[0];
            _splatSprites[i] = Resources.LoadAll<Sprite>("Feedback/Splat_" + colorChar);
        }
        
    }

    void Start()
    {
        instance = this;
        LoadSprites();
        transform.localPosition = new Vector2(-3.36f, -0.16f);
        _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        _bakeTexture = new Texture2D(_bakeTextureWidth, _bakeTextureHeight);
        Color[] pixels = _bakeTexture.GetPixels();
        Color fillColor = new Color(0, 0, 0, 0.01f);
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = fillColor;
        }
        _bakeTexture.SetPixels(pixels);
        _bakeTexture.Apply();
        //_spriteRenderer.sprite.texture = _bakeTexture;
        _spriteRenderer.sprite = Sprite.Create(_bakeTexture, new Rect(Vector2.zero, new Vector2(_bakeTextureWidth, _bakeTextureHeight)),Vector2.zero,50);
        _bakeTexture = _spriteRenderer.sprite.texture;
    }

	public void DoSplat(Vector2 startPos, float delayS = 0, int color = -1)
    {
        int splatID = color == -1? Random.Range(0, 5) : color;
        //char colorChar = ((SheetAnimation.PlayerColor)splatID).ToString().ToUpper()[0];
        Sprite[] splats = _splatSprites[splatID];// = Resources.LoadAll<Sprite>("Feedback/Splat_" + colorChar);

        StartCoroutine(MakeSplat(startPos, splats, delayS));
    }

    IEnumerator MakeSplat(Vector2 startPos, Sprite[] sprites, float delayS)
    {
        yield return new WaitForSeconds(delayS);

        int splatAmount = Random.Range(15, 30);  
        //splatData.Add(new SplatPart.SpriteData[splatAmount]);
        //int splatDataID = splatData.Count - 1;

        for (int i = 0; i < splatAmount; i++)
        {
            GameObject newSplat = new GameObject("Splat");
            newSplat.transform.parent = gameObject.transform;
            newSplat.transform.localPosition = startPos;
            int size = Random.Range(0, sprites.Length);
            SpriteRenderer SR = newSplat.AddComponent<SpriteRenderer>();
            SR.sprite = sprites[size];
            SR.sortingOrder = -14;
            SplatPart part = newSplat.AddComponent<SplatPart>();
            //part.listID = splatDataID;
            //part.arrayID = i;
            part.SetBakeTexture(_spriteRenderer);
            part.Initialize(size, Random.insideUnitCircle, (Vector3)startPos);
            splatParts.Add(part);
        }
    }

    public void RemoveSplats()
    {
        for (int i = splatParts.Count-1; i >= 0; i--)
        {
            SplatPart part = splatParts[i];
            splatParts.RemoveAt(i);
            GameObject.Destroy(part.gameObject);
        }
    }


    public class SplatPart : MonoBehaviour
    {
        Vector2 startPos;
        int size;
        public bool doneMoving { get; private set; }
        float DistanceToMove;
        const int MAX_SIZE = 15;
        Vector2 direction;

        float timer = 0;
        float maxTime = 0.2f;
        bool initialized = false;
       // Sprite _bakeSprite = null;
        SpriteRenderer _bakedRenderer = null;
        SpriteRenderer _spriteRenderer = null;

        //public int listID = -1;
        //public int arrayID = -1;

        const float OFFSET_X = 1.2f;
        const float OFFSET_Y = 0.5f;


        #region threading
        public struct SpriteData
        {
            public Color[] pixels;
            public Vector2 worldPosition;
            public Vector2 parentWorldPosition;
            public Vector2 worldSize;
            public Vector2 textureSize;

            public SpriteData(Color[] pixels, Vector2 worldPosition, Vector2 parentWorldPosition, Vector2 worldSize, Vector2 textureSize)
            {
                this.pixels = pixels;
                this.worldPosition = worldPosition;
                this.parentWorldPosition = parentWorldPosition;
                this.worldSize = worldSize;
                this.textureSize = textureSize;
            }
        }

        void ThreadTest(SpriteData spritedata)
        {
            /*

            float bMinX = spritedata.
            float bMinY =

            float xPerc = 
            float yPerc = 

            float texX =
            float texY = 




            float bMinX = _bakedRenderer.sprite.bounds.center.x - _bakedRenderer.sprite.bounds.size.x / 2 + transform.parent.position.x;
            float bMinY = _bakedRenderer.sprite.bounds.center.y - _bakedRenderer.sprite.bounds.size.y / 2 + transform.parent.position.y;

            float xPerc = ((position.x - _spriteRenderer.sprite.bounds.size.x / 2) - bMinX) / _bakedRenderer.sprite.bounds.size.x;
            float yPerc = ((position.y - _spriteRenderer.sprite.bounds.size.y / 2) - bMinY) / _bakedRenderer.sprite.bounds.size.y;

            float texX = Mathf.Min(Mathf.Max(0, _bakedRenderer.sprite.texture.width * xPerc), _bakedRenderer.sprite.texture.width - _spriteRenderer.sprite.texture.width / 4);
            float texY = Mathf.Min(Mathf.Max(0, _bakedRenderer.sprite.texture.height * yPerc), _bakedRenderer.sprite.texture.height - _spriteRenderer.sprite.texture.height / 4);

            Vector2 textureCoords = new Vector2(texX, texY);

    */


            Debug.Log(9999);
        }
        #endregion

        public void SetBakeTexture(SpriteRenderer bakedRenderer)
        {
            _bakedRenderer = bakedRenderer;
        }

        public void Initialize(int size, Vector2 direction, Vector3 startPos)
        {
            initialized = true;

            _spriteRenderer = GetComponent<SpriteRenderer>();

            this.size = size;
            this.direction = direction;

            transform.position = startPos + (Vector3)(direction * Random.Range(0.0f, (float)size * 0.08f)) ;


            BakeToTexture();
            GameObject.Destroy(gameObject);
        }

        Vector2 PositionToTextureCoords(Vector2 position)
        {
           
            float bMinX = _bakedRenderer.sprite.bounds.center.x - _bakedRenderer.sprite.bounds.size.x / 2 + transform.parent.position.x;
            float bMinY = _bakedRenderer.sprite.bounds.center.y - _bakedRenderer.sprite.bounds.size.y / 2 + transform.parent.position.y;

            float xPerc = ((position.x - _spriteRenderer.sprite.bounds.size.x / 2) - bMinX) / _bakedRenderer.sprite.bounds.size.x;
            float yPerc = ((position.y - _spriteRenderer.sprite.bounds.size.y / 2) - bMinY) / _bakedRenderer.sprite.bounds.size.y;

            float texX = Mathf.Min(Mathf.Max(0, _bakedRenderer.sprite.texture.width  * xPerc), _bakedRenderer.sprite.texture.width - _spriteRenderer.sprite.texture.width / 4);
            float texY = Mathf.Min(Mathf.Max(0, _bakedRenderer.sprite.texture.height * yPerc), _bakedRenderer.sprite.texture.height - _spriteRenderer.sprite.texture.height / 4);

            Vector2 textureCoords = new Vector2(texX, texY);

            return textureCoords;
        }

        void BakeToTexture()
        {

            //SpriteData data = new SpriteData(_spriteRenderer.sprite.texture.GetPixels(), transform.position, transform.parent.position, new Vector2(_spriteRenderer.sprite.bounds.size.x, _spriteRenderer.sprite.bounds.size.y), new Vector2(_spriteRenderer.sprite.texture.width, _spriteRenderer.sprite.texture.height));
            //new System.Threading.Thread(() => ThreadTest(data)).Start();

            Vector2 bakePos = PositionToTextureCoords(transform.position);

            Color[] newPixels = _spriteRenderer.sprite.texture.GetPixels(size % 4 * (_spriteRenderer.sprite.texture.width / 4), ((15 - size) / 4) * (_spriteRenderer.sprite.texture.height / 4), _spriteRenderer.sprite.texture.width / 4, _spriteRenderer.sprite.texture.height / 4);
            Color[] oldPixels = _bakedRenderer.sprite.texture.GetPixels((int)bakePos.x, (int)bakePos.y, _spriteRenderer.sprite.texture.width / 4, _spriteRenderer.sprite.texture.height / 4);
            for (int i = 0; i < oldPixels.Length; i++)
            {
                if (newPixels[i].a == 0) newPixels[i] = oldPixels[i];
            }
            _bakedRenderer.sprite.texture.SetPixels((int)bakePos.x, (int)bakePos.y, _spriteRenderer.sprite.texture.width / 4, _spriteRenderer.sprite.texture.height / 4, newPixels);
            _bakedRenderer.sprite.texture.Apply();
        }

        
    }
}
