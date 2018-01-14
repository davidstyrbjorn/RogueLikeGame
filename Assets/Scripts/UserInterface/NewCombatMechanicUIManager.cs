using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCombatMechanicUIManager : MonoBehaviour {

    public Text currentPhaseText;
    public Text currentPhaseTimeLeftText;

    // 0 - Begin
    // 1 - Combat Player
    // 2 - Combat Enemy
    // 3 - End
    public List<Text> phaseListText;

    private Color phaseActiveColor = Color.white;
    private Color phaseNotActiveColor = new Color(1, 1, 1, 0.4f);

    public void NewPhase(PlayerManager.CombatPhase newPhase)
    {
        try
        {
            switch (newPhase)
            {
                case PlayerManager.CombatPhase.BEGIN:
                    currentPhaseText.text = "Phase: <color=green>Begin</color>";

                    phaseListText[0].color = phaseActiveColor;
                    phaseListText[1].color = phaseNotActiveColor;
                    phaseListText[2].color = phaseNotActiveColor;
                    phaseListText[3].color = phaseNotActiveColor;

                    break;
                case PlayerManager.CombatPhase.COMBAT_PLAYER:
                    currentPhaseText.text = "Phase: <color=red>Combat</color>";

                    phaseListText[0].color = phaseNotActiveColor;
                    phaseListText[1].color = phaseActiveColor;
                    phaseListText[2].color = phaseNotActiveColor;
                    phaseListText[3].color = phaseNotActiveColor;

                    break;
                case PlayerManager.CombatPhase.COMBAT_ENEMY:
                    currentPhaseText.text = "Phase: <color=red>Combat</color>";

                    phaseListText[0].color = phaseNotActiveColor;
                    phaseListText[1].color = phaseNotActiveColor;
                    phaseListText[2].color = phaseActiveColor;
                    phaseListText[3].color = phaseNotActiveColor;

                    break;
                case PlayerManager.CombatPhase.END:
                    currentPhaseText.text = "Phase: <color=green>End</color>";

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
}
