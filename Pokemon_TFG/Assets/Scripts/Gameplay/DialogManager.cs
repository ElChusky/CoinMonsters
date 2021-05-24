using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int charsPerSecond;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public static Dialog dialog;
    public static int currentLine = 0;

    public void ShowDialog(Dialog dialog)
    {
        dialogBox.SetActive(true);
        DialogManager.dialog = dialog;
        StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        for (int i = 0; i < line.Length; i++)
        {
            dialogText.text += line.ToCharArray()[i];
            yield return new WaitForSeconds(1f / charsPerSecond);
        }
    }

}
