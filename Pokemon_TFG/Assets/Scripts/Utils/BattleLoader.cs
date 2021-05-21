using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleLoader : MonoBehaviour
{

    Scene currentScene;

    public void LoadBattle()
    {
        currentScene = SceneManager.GetActiveScene();
        StartCoroutine(LoadBattleAsync(1));
    }

    public IEnumerator LoadBattleAsync(int sceneIndex)
    {

        yield return SceneManager.SetActiveScene(SceneManager.GetSceneAt(sceneIndex));
    }

    public void EndBattle()
    {
        StartCoroutine(EndBattleAsynchronously(currentScene));
    }

    public IEnumerator EndBattleAsynchronously(Scene initialScene)
    {
        yield return SceneManager.SetActiveScene(initialScene);
    }

}
