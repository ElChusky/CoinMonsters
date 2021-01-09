using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    public int charsPerSecond;
    public Color highlightedColor;

    public Text dialogText;
    public GameObject actionSelector;
    public GameObject moveSelector;
    public GameObject moveDetails;

    public List<Text> actionTexts;
    public Text[] moveTexts = new Text[4];

    public Text ppText;
    public Text typeText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        for(int i = 0; i < dialog.Length; i++)
        {
            dialogText.text += dialog.ToCharArray()[i];
            yield return new WaitForSeconds(1f / charsPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UdateActionSelection(int selectedAction)
    {
        for(int i = 0; i < actionTexts.Count; i++)
        {
            if(i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            } else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Length; i++)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP {move.CurrentPP}/{move.BasePp}";
        typeText.text = move.Type.ToString();
    }

    public void SetMoveNames(Move[] learnedMoves)
    {
        for (int i = 0; i < learnedMoves.Length; i++)
        {
            if(learnedMoves[i] == null)
            {
                moveTexts[i].text = "-";
            } else
            {
                moveTexts[i].text = learnedMoves[i].Name;
            }
        }
    }
}
