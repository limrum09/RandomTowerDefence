using TMPro;
using UnityEngine;

public class SignInController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI messageText;

    [Header("Views")]
    [SerializeField]
    private SignInView emailSinginView;
    [SerializeField]
    private SignUpView emailSignUpView;

    [Header("Sign")]
    [SerializeField]
    private FirebaseEmailSignIn emailSignIn;
    [SerializeField]
    private FirebaseEmailSignUp emailSignUp;

    private void Awake()
    {
        emailSinginView.OnSingIn += SignInEmail;
        emailSignUpView.OnCreateAccount += SignUpEamil;
    }

    private void OnDestroy()
    {
        emailSinginView.OnSingIn -= SignInEmail;
        emailSignUpView.OnCreateAccount -= SignUpEamil;
    }

    private async void SignInEmail(string email, string password)
    {
        string result = await emailSignIn.SignInAsync(email, password);

        if(result == "Success")
        {
            LoadSceneManager.Instance.OnLoadStringScene("LobbyScene");
        }
        else
        {
            messageText.text = "로그인 실패 : " + result;
        }
    }

    private async void SignUpEamil(string email, string password)
    {
        string result = await emailSignUp.SignUpAsync(email, password);

        if(result == "Success")
        {
            emailSignUpView.Hide();
            messageText.text = "회원가입 완료";
            return;
        }

        if (result != "Success")
        {
            messageText.text = "회원가입 실패 : " + result;
            return;
        }
    }
}
