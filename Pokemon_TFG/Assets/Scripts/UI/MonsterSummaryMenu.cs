using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSummaryMenu : MonoBehaviour
{

    [SerializeField] Text monsterName;
    [SerializeField] Image monsterImage;
    [SerializeField] Text level;
    [SerializeField] Text type1;
    [SerializeField] Text type2;
    [SerializeField] Text currentHP;
    [SerializeField] Text maxHP;
    [SerializeField] Text attack;
    [SerializeField] Text defense;
    [SerializeField] Text spAttack;
    [SerializeField] Text spDefense;
    [SerializeField] Text speed;
    [SerializeField] Text[] moveTexts;
    [SerializeField] Text power;
    [SerializeField] Text accuracy;
    [SerializeField] Text moveType;
    [SerializeField] Text moveCategory;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color normalColor;

    public event Action OnBack;

    private List<Move> moves;
    private int selectedMove = 0;

    public Monster CurrentMonster { get; set; }

    public void SetData(Monster monster)
    {
        moves = new List<Move>();
        monsterName.text = monster.BaseMonster.Name;
        monsterImage.sprite = monster.BaseMonster.Sprite;
        level.text = monster.Level.ToString();

        type1.text = monster.BaseMonster.Type1.ToString();
        if (monster.BaseMonster.Type2 == MonsterType.Ninguno)
            type2.text = "-";
        else
            type2.text = monster.BaseMonster.Type2.ToString();

        currentHP.text = monster.CurrentHP.ToString();
        maxHP.text = monster.MaxHp.ToString();
        attack.text = monster.Attack.ToString();
        defense.text = monster.Defense.ToString();
        spAttack.text = monster.SpAttack.ToString();
        spDefense.text = monster.SpDefense.ToString();
        speed.text = monster.Speed.ToString();

        for (int i = 0; i < moveTexts.Length; i++)
        {
            if (i < monster.LearntMoves.Count)
            {
                moves.Add(monster.LearntMoves[i]);
                moveTexts[i].text = moves[i].Name;
            }
            else
                moveTexts[i].text = "-";
        }

        power.text = moves[0].Power.ToString();
        accuracy.text = moves[0].Accuracy.ToString();
        moveType.text = moves[0].Type.ToString();
        moveCategory.text = moves[0]._MoveCategory.ToString();
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            selectedMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedMove -= 2;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            selectedMove++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            selectedMove--;

        selectedMove = Mathf.Clamp(selectedMove, 0, moves.Count - 1);

        if (Input.GetKeyDown(KeyCode.X))
        {
            OnBack?.Invoke();
        }

        UpdateMoveSelection();

    }

    private void UpdateMoveSelection()
    {
        for (int i = 0; i < moveTexts.Length; i++)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = normalColor;
        }

        SetMoveData();

    }

    private void SetMoveData()
    {
        power.text = moves[selectedMove].Power.ToString();
        accuracy.text = moves[selectedMove].Accuracy.ToString();
        moveType.text = moves[selectedMove].Type.ToString();
        moveCategory.text = moves[selectedMove]._MoveCategory.ToString();
    }

}
