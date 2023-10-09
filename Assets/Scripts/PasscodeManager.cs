using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasscodeManager : Singleton<PasscodeManager>
{
    public TMP_Text outcomeText;
    public string Passcode { get; private set; }

    private string enteredPasscode = string.Empty;

    private void Start()
    {
        Passcode = "1234";
    }

    public void Validate()
    {
        if (enteredPasscode == Passcode)
        {
            outcomeText.text = "Correct!";
        }
        else
        {
            outcomeText.text = "Wrong!";
        }
    }

    public void EnterDigit(string entry)
    {
        enteredPasscode += entry;
        if (enteredPasscode.Length == Passcode.Length)
        {
            Validate();
        }
    }

}
