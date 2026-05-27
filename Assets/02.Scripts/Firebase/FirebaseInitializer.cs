using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer Instance { get; private set; }

    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore Firestore { get; private set; }
    public FirebaseDatabase Realtime {  get; private set; }

    public bool IsReady { get; private set; }
    private async void Awake()
    {
        IsReady = false;

        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeFirebase();
    }
    
    private async System.Threading.Tasks.Task InitializeFirebase()
    {
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

        if(status == DependencyStatus.Available)
        {
            Auth = FirebaseAuth.DefaultInstance;
            Firestore = FirebaseFirestore.DefaultInstance;
            Realtime = FirebaseDatabase.GetInstance(FirebaseApp.DefaultInstance, "https://d-random-tower-defense-default-rtdb.firebaseio.com/");

            IsReady = true;
        }
        else
        {
            IsReady = false;
        }
    }
}
