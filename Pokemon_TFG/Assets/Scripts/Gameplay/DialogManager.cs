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
    public GameObject DialogBox { get { return dialogBox; } }

    private void Awake()
    {
        Instance = this;
    }

    public Dialog dialog;
    public int currentLine = 0;
    public bool isTyping;

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        dialogBox.SetActive(true);
        this.dialog = dialog;
        StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        for (int i = 0; i < line.Length; i++)
        {
            dialogText.text += line.ToCharArray()[i];
            yield return new WaitForSeconds(1f / charsPerSecond);
        }
        isTyping = false;
    }

}
