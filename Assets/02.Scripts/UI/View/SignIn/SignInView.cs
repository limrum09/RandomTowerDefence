using System;
using TMPro;
using UnityEngine;

public class SignInView : SignInBase
{
    [SerializeField]
    private TMP_InputField emailInputField;
    [SerializeField]
    private TMP_InputField passwordInputField;

    private string email;
    private string password;
    private bool canSingIn;

    public event Action<string, string> OnSingIn;

    private void CheckEmailAndPassword()
    {
        ResetUI(emailInputField.image, passwordInputField.image);
        email = emailInputField.text;
        password = passwordInputField.text;
        canSingIn = true;

        if (!IsFieldDataEmpty(emailInputField.image, emailInputField, email))
        {
            canSingIn = false;
            return;
        }

        if (!CheckFormatEmail(emailInputField))
        {
            canSingIn = false;
            return;
        }

        if (!IsFieldDataEmpty(passwordInputField.image, passwordInputField, password))
        {
            canSingIn = false;
            return;
        }
    }

    public void OnClickSignInButton()
    {
        CheckEmailAndPassword();

        if (!canSingIn)
            return;

        OnSingIn?.Invoke(email, password);
    }
}
