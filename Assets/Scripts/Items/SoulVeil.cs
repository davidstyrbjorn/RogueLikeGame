using UnityEngine;
using System.Collections;

public class SoulVeil : MonoBehaviour {

    private SpriteRenderer spre;

    private bool fadeOut;
    private int moneyReward;

    [System.NonSerialized]
    public Vector2 groundPos;

    private void Start()
    {
        spre = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (fadeOut)
            spre.color = new Color(spre.color.r, spre.color.g, spre.color.b, spre.color.a - 1f * Time.deltaTime);
        if (spre.color.a <= 0)
        {
            Destroy
            (gameObject);
        }
    }

    public void Activate()
    {
        fadeOut = true;
    }

    public void setMoneyReward(int amount)
    {
        moneyReward = amount;
    }

    public int getMoneyReward()
    {
        return moneyReward;
    }
}
