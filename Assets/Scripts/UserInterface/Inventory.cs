using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    private void Start()
    {
        StartCoroutine("playerIdleRun");

        playerManger = FindObjectOfType<PlayerManager>();
    }

    [Header("Player Data")]
    public Sprite[] playerIdleSprites;
    public Image playerSpriteRenderer;
    private PlayerManager playerManger;

    public void Toggled()
    {
        float healthRatio = (playerManger.getHealth() / playerManger.getMaxHealth());
        playerSpriteRenderer.color = new Color(1, healthRatio, healthRatio);
    }

    public IEnumerator playerIdleRun()
    {
        int index = 0;
        const float animationSpeed = 0.1f;

        while (true)
        {
            index++;
            if (index == playerIdleSprites.Length)
                index = 0;

            //print(index);
            playerSpriteRenderer.sprite = playerIdleSprites[index];

            yield return new WaitForSecondsRealtime(animationSpeed);
        }
    }

}
