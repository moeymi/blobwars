using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlobController : MonoBehaviour
{
    #region Attributes

    [SerializeField]
    bool isEnemy;
    [SerializeField]
    bool isSelected = false;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Vector2Int currentPos;
    Tilemap tileMap;

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
    }

    private void Awake()
    {
        Initialize(new Vector2Int(0, 0));
    }

    public void Update()
    {
        if (Input.GetMouseButton(0) && isSelected)
        {
            Debug.Log("Pressed");
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
        if (action == BlobAction.Move)
        {
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
            animationEvent.functionName = "CopyBlob";
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
        Select();
    }
    void MoveSelf()
    {
        float x = (currentPos.y - currentPos.x) * 0.5f;
        float y = (currentPos.y + currentPos.x + 1) * 0.25f;
        transform.position = new Vector2(x, y);
        Debug.Log("ShouldMove");
    }
    void CopyBlob()
    {
        GameManager.CopyBlob();
    }
    void Select()
    {
        isSelected = true;
        GameManager.ShowAvailableMoves(currentPos);
        animator.SetBool("selected", true);
    }

    void UnSelect()
    {
        isSelected = false;
        GameManager.NormalizeTiles();
        animator.SetBool("selected", false);
    }
}
