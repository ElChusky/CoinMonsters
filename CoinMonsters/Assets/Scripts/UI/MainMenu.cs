using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private string newSaveName;

    // Start is called before the first frame update
    public void NewGame()
    {
        //Abrir un submenu para introducir nombre del save
        /*
        if (File.Exists(Path.Combine(Application.persistentDataPath, newSaveName)))
        {
            Mostrar aviso de que ya existe y no pasar al juego
        } else
        {
            Cargar la partida con SavingSystem.i.Save(newSaveName, FileMode.Create) y luego Cargar escena 1
        }*/
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameController.Instance.State = GameState.FreeRoam;
    }

    public void LoadGame()
    {
        SavingSystem.i.Load("saveslot1");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        GameController.Instance.State = GameState.FreeRoam;
    }

    public void SetNewSaveName(string newName)
    {
        newSaveName = newName;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
