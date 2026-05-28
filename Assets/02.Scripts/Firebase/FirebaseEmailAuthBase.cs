using Firebase;
using Firebase.Auth;
using System;
using UnityEngine;

public abstract class FirebaseEmailAuthBase : MonoBehaviour
{
    public FirebaseUser CurrentUser { get; protected set; }

    protected FirebaseAuth Auth => FirebaseInitializer.Instance.Auth;

    protected bool IsFirebaseReady()
    {
        return FirebaseInitializer.Instance != null &&
            FirebaseInitializer.Instance.IsReady &&
            FirebaseInitializer.Instance.Auth != null;
    }

    protected string GetAuthErrorMessage(Exception e)
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
}
