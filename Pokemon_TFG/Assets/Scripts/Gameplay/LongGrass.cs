using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        MapArea mapArea = GetComponent<MapArea>();

        float probability = Random.Range(0.0f, 100.0f);

        float veryCommon = 10 / 187.5f;
        float common = 8.5f / 187.5f;
        float semiRare = 6.75f / 187.5f;
        float rare = 3.33f / 187.5f;
        float veryRare = 1.25f / 187.5f;

        bool encountered = false;

        if (probability <= veryRare * 100)
        {
            mapArea.Rarity = 5;
            encountered = true;
        }
        else if (probability <= rare * 100)
        {
            mapArea.Rarity = 4;
            encountered = true;

        }
        else if (probability <= semiRare * 100)
        {
            mapArea.Rarity = 3;
            encountered = true;

        }
        else if (probability <= common * 100)
        {
            mapArea.Rarity = 2;
            encountered = true;

        }
        else if (probability <= veryCommon * 100)
        {
            mapArea.Rarity = 1;
            encountered = true;
        }
        if (encountered)
        {
            player.Character.Animator.IsMoving = false;
            player.Character.Animator.IsRunning = false;
            GameController.Instance.StartBattle(mapArea);
        }
    }
}
