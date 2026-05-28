using System;
using TMPro;
using UnityEngine;

public class SignUpView : SignInBase
{
    [SerializeField]
    private TMP_InputField emailInputField;
    [SerializeField]
    private TMP_InputField passwordInputField;
    [SerializeField]
    private TMP_InputField checkPasswordInputField;

    private string email;
    private string password;
    private string checkPassword;
    private bool canCreateAccount;

    public event Action<string, string> OnCreateAccount;

    private void CheckEmailAndPasswords()
    {
        ResetUI(emailInputField.image, passwordInputField.image, checkPasswordInputField.image);
        email = emailInputField.text;
        password = passwordInputField.text;
        checkPassword = checkPasswordInputField.text;
        canCreateAccount = true;

        if (!IsFieldDataEmpty(emailInputField.image, emailInputField, email))
        {
            canCreateAccount = false;
            return;
        }

        if (!IsFieldDataEmpty(passwordInputField.image, passwordInputField, password))
        {
            canCreateAccount = false;
            return;
        }

        if (!IsFieldDataEmpty(checkPasswordInputField.image, checkPasswordInputField, checkPassword))
        {
            canCreateAccount = false;
            return;
        }

        if (!CheckFormatEmail(emailInputField))
        {
            canCreateAccount = false;
            return;
        }

        if (password != checkPassword)
        {
            canCreateAccount = false;
            return;
        }
    }

    public void Hide()
    {
        ResetUI(emailInputField.image, passwordInputField.image, checkPasswordInputField.image);
        transform.gameObject.SetActive(false);
    }

    public void OnClickCreateAccountButton()
    {
        CheckEmailAndPasswords();

        if (!canCreateAccount)
            return;

        OnCreateAccount?.Invoke(email, password);
    }
}
