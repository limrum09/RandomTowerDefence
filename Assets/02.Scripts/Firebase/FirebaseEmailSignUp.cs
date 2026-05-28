using Firebase.Auth;
using System;
using System.Threading.Tasks;

public class FirebaseEmailSignUp : FirebaseEmailAuthBase
{
    public async Task<string> SignUpAsync(string email, string password)
    {
        if (!IsFirebaseReady())
            return "Firebase 초기화 오류"; ;

        try
        {
            AuthResult result = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);

            return "Success";
        }
        catch(Exception e)
        {
            return GetAuthErrorMessage(e);
        }
    }
}
