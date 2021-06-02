using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherNPC : MonoBehaviour, Interactable, ISavable
{

    [SerializeField] Dialog dialogBeforeGiveMonster;
    [SerializeField] Dialog dialogAfterGiveMonster;
    [SerializeField] MotherActivator activator;

    private Character character;
    private Vector3 originalPos;
    private HealingParty healing;

    private bool gaveMonster;

    private void Start()
    {
        originalPos = transform.position;
        character = GetComponent<Character>();
        healing = GetComponent<HealingParty>();
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public void Interact(Transform initiator)
    {
        if(!gaveMonster)
            StartCoroutine(OnActivatorTriggered(initiator.GetComponent<PlayerController>()));
        else
        {
            character.LookTowards(initiator.position);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterGiveMonster));
            healing.HealParty(initiator.GetComponent<PlayerController>());
        }
    }

    public IEnumerator OnActivatorTriggered(PlayerController player)
    {

        Vector2 diff = player.transform.position - originalPos;
        Vector2 moveVector;
        if (diff.x != 0)
            moveVector = new Vector2(0, diff.y);
        else
            moveVector = diff - diff.normalized;

        yield return character.Move(moveVector, false);

        player.Character.LookTowards(transform.position);
        character.LookTowards(player.transform.position);

        StartCoroutine(DialogManager.Instance.ShowDialog(dialogBeforeGiveMonster, () =>
        {
            activator.gameObject.SetActive(false);
            StartCoroutine(DialogManager.Instance.ShowDialog(new Dialog(new List<string> { "Has recibido un Empig de manos de tu madre."}), () =>
            {
                BaseMonster baseMonster = MonstersDB.GetMonsterByName("Empig");
                Monster monster = new Monster(baseMonster, 5);
                player.GetComponent<MonsterParty>().AddMonster(monster);
                gaveMonster = true;
                GameController.Instance.LastHealPosition = player.transform.position;
            }));
        }));

        yield return new WaitUntil(() => gaveMonster);

        yield return character.Move(new Vector2(moveVector.x, -moveVector.y), false);
    }

    public object CaptureState()
    {
        return gaveMonster;
    }

    public void RestoreState(object state)
    {
        gaveMonster = (bool)state;
        if (gaveMonster)
            activator.gameObject.SetActive(false);
    }
}
