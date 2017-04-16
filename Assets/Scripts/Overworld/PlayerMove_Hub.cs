using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMove_Hub : MonoBehaviour {

    enum PLAYER_STATES
    {
        NORMAL,
        STARTING,
    }

    private int[,] currentMap;
    int curr_x, curr_y;
    private Vector2 currentWorldPosition;

    private Animator anim;
    private SpriteRenderer spre;

    private float moveSpeed = 17.5f;
    private bool canMove;
    private const float fadeSpeed = 3.5f;

    private OverworldMapGen mapGenerator;
    private PLAYER_STATES state;

    public Image fadePanel;

    void Awake()
    {
        mapGenerator = FindObjectOfType<OverworldMapGen>();
        spre = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentMap = mapGenerator.getMap();

        curr_x = 4;
        curr_y = 1;
        currentWorldPosition = SetPlayerPos(curr_x, curr_y);

        transform.position = currentWorldPosition;

        state = PLAYER_STATES.NORMAL;

        StartCoroutine("FadeIn");
    }

    void Update()
    {
        { 
            transform.position = Vector2.MoveTowards(transform.position, currentWorldPosition, moveSpeed * Time.deltaTime);
        }

        if (canMove && state == PLAYER_STATES.NORMAL)
        {
            anim.SetBool("WalkVertical", false);
            anim.SetBool("WalkSide", false);
            MovePlayer();
        }else
        {
            CheckCanMove();
        }
    }

    Vector2 SetPlayerPos(int _x, int _y)
    {
        for(int x = 0; x < BaseValues.HUB_WIDTH; x++)
        {
            for(int y = 0; y < BaseValues.HUB_HEIGHT; y++)
            {
                if(x == _x && y == _y)
                {
                    return new Vector2(x * mapGenerator.tileSize, y * mapGenerator.tileSize);
                }
            }
        }
        return new Vector2(0, 0);
    }

    void MovePlayer()
    {
        if (Input.anyKey)
        {
            // Move left
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                anim.SetBool("WalkSide", true);
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x - 1, curr_y] != 1 && currentMap[curr_x - 1, curr_y] != 4)
                {
                    curr_x--;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;
                    spre.flipX = false;

                }
            }

            // Move to the left 
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                anim.SetBool("WalkSide", true);
                spre.flipX = true;
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x + 1, curr_y] != 1 && currentMap[curr_x + 1, curr_y] != 4)
                {
                    curr_x++;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;

                }
            }

            // Move upwards
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                anim.SetBool("WalkVertical", true);
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x, curr_y + 1] != 1 && currentMap[curr_x, curr_y + 1] != 4)
                {
                    curr_y++;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;
                }
            }

            // Move downwards
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                anim.SetBool("WalkVertical", true);
                // Move as long as we arent hitting any wall or an enemy
                if (currentMap[curr_x, curr_y - 1] != 1 && currentMap[curr_x, curr_y - 1] != 4)
                {
                    curr_y--;
                    currentWorldPosition = SetPlayerPos(curr_x, curr_y);
                    canMove = false;

                }
            }

            if(currentMap[curr_x,curr_y] == 2)
            {
                StopAllCoroutines();
                StartCoroutine("StartGame");
            }
        }
    }

    void CheckCanMove()
    {
        if ((Vector2)transform.position == currentWorldPosition)
            canMove = true;
    }

    IEnumerator FadeIn()
    {
        fadePanel.color = Color.black;

        while(fadePanel.color != Color.clear)
        {
            fadePanel.color = Color.Lerp(fadePanel.color, Color.clear, fadeSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
        }

        fadePanel.color = Color.clear;
    }

    IEnumerator StartGame()
    {
        state = PLAYER_STATES.STARTING;
        anim.SetBool("WalkVertical", false);
        anim.SetBool("WalkSide", false);

        fadePanel.color = Color.clear;

        while(fadePanel.color != Color.black)
        {
            fadePanel.color = Color.Lerp(fadePanel.color, Color.black, fadeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        fadePanel.color = Color.black;

        SceneManager.LoadScene("Main");

        yield return null;
    }
}
