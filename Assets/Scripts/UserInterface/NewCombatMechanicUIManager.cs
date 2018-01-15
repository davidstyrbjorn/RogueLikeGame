using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCombatMechanicUIManager : MonoBehaviour {

    private PlayerManager playerManager;

    public Text currentPhaseText;
    public Text currentPhaseTimeLeftText;
    public Text currentAttackStance;

    // 0 - Begin
    // 1 - Combat Player
    // 2 - Combat Enemy
    // 3 - End
    public List<Text> phaseListText;

    private Color phaseActiveColor = Color.white;
    private Color phaseNotActiveColor = new Color(1, 1, 1, 0.4f);
    private float countDownTimer;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>(); // Player Manager controls the combat
    }

    private void Update()
    {
        CountDownLogic();
        AttackStanceTextLogic();
    }

    public void NewPhase(PlayerManager.CombatPhase newPhase)
    {
        try
        {
            switch (newPhase)
            {
                case PlayerManager.CombatPhase.BEGIN:
                    currentPhaseText.text = "Phase: <color=green>Begin</color>";
                    countDownTimer = BaseValues.BEGIN_TIME;

                    phaseListText[0].color = phaseActiveColor;
                    phaseListText[1].color = phaseNotActiveColor;
                    phaseListText[2].color = phaseNotActiveColor;
                    phaseListText[3].color = phaseNotActiveColor;

                    break;
                case PlayerManager.CombatPhase.COMBAT_PLAYER:
                    currentPhaseText.text = "Phase: <color=red>Combat</color>";
                    countDownTimer = BaseValues.COMBAT_PLAYER_TIME + 0.3f; // 0.3f - time for attack animation

                    phaseListText[0].color = phaseNotActiveColor;
                    phaseListText[1].color = phaseActiveColor;
                    phaseListText[2].color = phaseNotActiveColor;
                    phaseListText[3].color = phaseNotActiveColor;

                    break;
                case PlayerManager.CombatPhase.COMBAT_ENEMY:
                    currentPhaseText.text = "Phase: <color=red>Combat</color>";
                    countDownTimer = BaseValues.COMBAT_ENEMY_TIME;

                    phaseListText[0].color = phaseNotActiveColor;
                    phaseListText[1].color = phaseNotActiveColor;
                    phaseListText[2].color = phaseActiveColor;
                    phaseListText[3].color = phaseNotActiveColor;

                    break;
                case PlayerManager.CombatPhase.END:
                    currentPhaseText.text = "Phase: <color=green>End</color>";
                    countDownTimer = BaseValues.END_TIME;

                    phaseListText[0].color = phaseNotActiveColor;
                    phaseListText[1].color = phaseNotActiveColor;
                    phaseListText[2].color = phaseNotActiveColor;
                    phaseListText[3].color = phaseActiveColor;

                    break;
            }
        }
        catch (System.ArgumentException e)
        {
            //print(e.StackTrace);
            throw new System.Exception("Invalid Phase");
        }
    }

    private void AttackStanceTextLogic()
    {
        switch (playerManager.getAttackType())
        {
            case PlayerManager.AttackType.NORMAL:
                currentAttackStance.text = "Stance: Normal";
                break;

            case PlayerManager.AttackType.HARD:
                currentAttackStance.text = "Stance: Hard";
                break;
        }
    }

    private void CountDownLogic()
    {
        if(Mathf.Abs(countDownTimer) > 0)
        {
            countDownTimer -= Time.deltaTime;
        }
        else
        {
            countDownTimer = 0;
        }

        currentPhaseTimeLeftText.text = "Phase Time: " + countDownTimer.ToString("F1");
    }
}
