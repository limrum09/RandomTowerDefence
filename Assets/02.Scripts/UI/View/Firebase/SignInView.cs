using System;
using TMPro;
using UnityEngine;

public class SignInView : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField emailInputField;
    [SerializeField]
    private TMP_InputField passwordInputField;

    private string email;
    private string password;

    public event Action<string, string> OnSingIn;

    private void CheckEmailAndPassword()
    {
        email = emailInputField.text;

        if(email == string.Empty)
        {
            emailInputField.text = "InputEamil";
        }

        password = passwordInputField.text;            
    }

    public void OnClickSignInButton()
    {
        CheckEmailAndPassword();

        OnSingIn?.Invoke(email, password);
    }
}
