using Firebase.Auth;
using System;
using System.Threading.Tasks;

public class FirebaseEmailSignIn : FirebaseEmailAuthBase
{ 
    public async Task<string> SignInAsync(string email, string password)
    {
        if (!IsFirebaseReady())
            return "초기화 되지 않았습니다.";

        try
        {
            AuthResult result = await FirebaseInitializer.Instance.Auth.SignInWithEmailAndPasswordAsync(email, password);

            CurrentUser = result.User;

            return "Success";
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
