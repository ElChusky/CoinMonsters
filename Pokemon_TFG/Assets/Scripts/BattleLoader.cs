using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleLoader : MonoBehaviour
{
    public void LoadBattle(int pokemonID, int pokemonLevel)
    {
        StartCoroutine(LoadAsynchronously(1, pokemonID, pokemonLevel));
    }

    public IEnumerator LoadAsynchronously(int sceneIndex, int pokeID, int pokeLevel)
    {

        yield return PokeApiController.GetLearnableMoves(pokeID, pokeLevel);

        yield return PokeApiController.GetBasePokemonWithID(pokeID);

        yield return SceneManager.LoadSceneAsync(sceneIndex);
    }
}
