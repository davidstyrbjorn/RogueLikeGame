using UnityEngine;
using System.Collections;
using UnityEngine.UI;

enum WeaponInHandTransformStates
{
    Hide,
    Show,
}

public class Inventory : MonoBehaviour {

    private void Awake()
    {
        StartCoroutine("playerIdleRun");

        playerManger = FindObjectOfType<PlayerManager>();

        weaponInHandOrgPos = WeaponInHandTransform.position;
        weaponInHandShowPos = WeaponInHandTransform.position + Vector3.up * 55;
        weaponInHandState = WeaponInHandTransformStates.Hide;
    }

    void Update()
    {
        if(weaponInHandState == WeaponInHandTransformStates.Hide)
        {
            WeaponInHandTransform.position = Vector2.MoveTowards(WeaponInHandTransform.position, weaponInHandOrgPos, 125 * Time.deltaTime);
        }
        if(weaponInHandState == WeaponInHandTransformStates.Show)
        {
            WeaponInHandTransform.position = Vector2.MoveTowards(WeaponInHandTransform.position, weaponInHandShowPos, 125 * Time.deltaTime);
        }
    }

    [Header("Player Data")]
    public Sprite[] playerIdleSprites;
    public Image playerSpriteRenderer;
    private PlayerManager playerManger;

    [Header("Inventory Rect Transforms")]
    public RectTransform WeaponInHandTransform;
    private Vector3 weaponInHandOrgPos, weaponInHandShowPos;
    WeaponInHandTransformStates weaponInHandState;

    public void Toggled()
    {
        float healthRatio = (playerManger.getHealth() / playerManger.getMaxHealth());
        playerSpriteRenderer.color = new Color(1, healthRatio, healthRatio);

        weaponInHandState = WeaponInHandTransformStates.Hide;
        WeaponInHandTransform.position = weaponInHandOrgPos;
    }

    public void MouseEnterEquipedWeapon()
    {
        weaponInHandState = WeaponInHandTransformStates.Show;
    }

    public void MouseExitEquipedWeapon()
    {
        weaponInHandState = WeaponInHandTransformStates.Hide;
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