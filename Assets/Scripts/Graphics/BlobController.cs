using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlobController : MonoBehaviour
{
    #region Attributes

    [SerializeField] bool isEnemy;
    [SerializeField] bool isSelected = false;
    [SerializeField] bool shouldMove = false;
    [SerializeField] private GameObject arrowObject;
    
    Animator animator;
    SpriteRenderer spriteRenderer;
    Vector2Int currentPos;
    Tilemap tileMap;
    BlobAction? action;
    static bool gotSelectedOnce;

    #endregion

    
    public void Initialize(Vector2Int position)
    {
        currentPos = position;
        tileMap = GameObject.FindGameObjectWithTag("TileMap").GetComponent<Tilemap>();
        float x = (currentPos.y - currentPos.x) * 0.5f;
        float y = (currentPos.y + currentPos.x + 1) * 0.25f;
        transform.position = new Vector2(x, y);
        animator = GetComponent<Animator>();
        animator.speed = 1.5f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 2000 - (int)(currentPos.y / 0.25f);
        action = null;
    }

    private void Awake()
    {
        Initialize(new Vector2Int(0, 0));
        if(gotSelectedOnce && arrowObject != null) arrowObject?.SetActive(false);
    }

    public void Update()
    {
        if (shouldMove && action != null)
        {
            //AIAction();
            //Debug.Log("Enter1");
            shouldMove = false;
            action = null;
        }
        if (Input.GetMouseButton(0) && isSelected)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int newPos = tileMap.WorldToCell(mousePosition);
            GameManager.TryMove(currentPos, new Vector2Int(newPos.y, newPos.x));
            UnSelect();
        }
    }

    public void MakeMove(Vector2Int position, BlobAction? action)
    {
        if (action == null)
        {
            return;
        }
        currentPos = position;
        AnimationEvent animationEvent = new AnimationEvent();

        this.action = action;
        if (action == BlobAction.Move)
        {
            if(isEnemy)
                animationEvent.functionName = "AIAction";
            else
                animationEvent.functionName = "MoveSelf";
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
                if (clip.name == "Squash")
                {
                    animationEvent.time = clip.length;
                    clip.events = new AnimationEvent[] { animationEvent };
                }
        }
        else
        {
            if (isEnemy)
                animationEvent.functionName = "AIAction";
            else
                animationEvent.functionName = "CopySelf";
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
                if (clip.name == "Squash")
                {
                    animationEvent.time = clip.length;
                    clip.events = new AnimationEvent[] { animationEvent };
                }
        }
        animator.SetTrigger("squash");
    }
    private void OnMouseUp()
    {
        if (GameManager.CanMove)
            Select();
    }
    async void AIAction()
    {
        if (action == BlobAction.Copy)
        {
            CopySelf();
        }
        else
            MoveSelf();
        await UniTask.Delay(340);
        GameManager.FinishAI();
    }
    void MoveSelf()
    {
        float x = (currentPos.y - currentPos.x) * 0.5f;
        float y = (currentPos.y + currentPos.x + 1) * 0.25f;
        transform.position = new Vector2(x, y);
        GameManager.TakeOver();
    }
    void CopySelf()
    {
        GameManager.CopyBlob();
        GameManager.TakeOver();
    }
    void Select()
    {
        isSelected = true;
        GameManager.ShowAvailableMoves(currentPos);
        animator.SetBool("selected", true);
        gotSelectedOnce = true;
        arrowObject?.SetActive(false);
    }

    void UnSelect()
    {
        isSelected = false;
        GameManager.NormalizeTiles();
        animator.SetBool("selected", false);
    }
}
