using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int charsPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogManager Instance { get; private set; }
    public GameObject DialogBox { get { return dialogBox; } }

    public bool IsShowing { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private Dialog dialog;
    private Action onDialogFinished;
    private int currentLine = 0;
    private bool isTyping;

    public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;
        dialogBox.SetActive(true);
        yield return TypeDialog(dialog.Lines[currentLine]);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            currentLine++;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogBox.SetActive(false);
                onDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / charsPerSecond);
        }
        isTyping = false;
    }

}
