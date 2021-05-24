using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleLoader : MonoBehaviour
{

    int currentSceneIndex;
    public bool onBattle;

    public void LoadBattle()
    {
        onBattle = true;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadBattleAsync(1));
    }

    private IEnumerator LoadBattleAsync(int sceneIndex)
    {
        yield return SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void EndBattle()
    {
        StartCoroutine(EndBattleAsync(currentSceneIndex));
    }

    private IEnumerator EndBattleAsync(int index)
    {
        yield return SceneManager.LoadSceneAsync(index);
        onBattle = false;
    }

}
