using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseEmailSignIn : MonoBehaviour
{
    public FirebaseUser CurrentUser { get; private set; }

    private bool IsFirebaseReady() => FirebaseInitializer.Instance.IsReady;

    private string GetAuthErrorMessage(Exception e)
    {
        FirebaseException firebaseException = e.GetBaseException() as FirebaseException;

        if (firebaseException == null)
            return e.Message;

        AuthError error = (AuthError)firebaseException.ErrorCode;

        switch (error)
        {
            case AuthError.InvalidEmail:
                return "이메일 형식이 올바르지 않습니다.";
            case AuthError.MissingEmail:
                return "이메일을 입력해주세요.";
            case AuthError.MissingPassword:
                return "비밀번호를 입력해주세요";
            case AuthError.WrongPassword:
                return "비밀번호가 틀렸습니다.";
            case AuthError.UserNotFound:
                return "존재하지 않는 계정입니다.";
            case AuthError.UserDisabled:
                return "비활성화된 계정입니다.";
            case AuthError.NetworkRequestFailed:
                return "네트워크 연결을 확인해 주세요.";
            default:
                return $"로그인 오류{error}";
        }
    }

    public async Task<string> SignInAsync(string email, string password)
    {
        if (!IsFirebaseReady())
            return "초기화 되지 않았습니다.";

        try
        {
            AuthResult result = await FirebaseInitializer.Instance.Auth.SignInWithEmailAndPasswordAsync(email, password);

            CurrentUser = result.User;

            return "로그인 성공";
        }
        catch (Exception e)
        {
            return GetAuthErrorMessage(e);
        }
    }

    public void SignOut()
    {
        if (!IsFirebaseReady())
            return;

        FirebaseInitializer.Instance.Auth.SignOut();
        CurrentUser = null;
    }
}
