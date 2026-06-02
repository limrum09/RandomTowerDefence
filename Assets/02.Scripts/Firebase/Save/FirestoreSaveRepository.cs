using Firebase.Firestore;
using System.Threading.Tasks;
using Firebase;
using System;

public enum FirestoreLoadStat
{
    Success,
    DocumentMissing,
    NetworkError,
    DataCorrupted,
    Timeout,
    PermissionError,
    UnknownError
}

public class FireStoreLoadResult<T>
{
    public FirestoreLoadStat Stat;
    public bool IsSuccess => Stat == FirestoreLoadStat.Success;
    public T Data;
    public string ErrorMessage;
}

public class FirestoreSaveRepository
{
    public async Task<FireStoreLoadResult<T>> LoadAsync<T>(DocumentReference docRef, int timeoutMs = 10000) where T : class
    {
        var result = new FireStoreLoadResult<T>();

        try
        {
            Task<DocumentSnapshot> loadTask = docRef.GetSnapshotAsync();
            Task timeoutTask = Task.Delay(timeoutMs);

            Task completedTask = await Task.WhenAny(loadTask, timeoutTask);

            if(completedTask == timeoutTask)
            {
                result.Stat = FirestoreLoadStat.Timeout;
                result.ErrorMessage = "Firestore Load Timeout";
                return result;
            }

            DocumentSnapshot snapShot = await loadTask;

            if (!snapShot.Exists)
            {
                result.Stat = FirestoreLoadStat.DocumentMissing;
                result.ErrorMessage = "New User";
                return result;
            }

            T data = snapShot.ConvertTo<T>();

            if (data == null)
            {
                result.Stat = FirestoreLoadStat.DataCorrupted;
                result.ErrorMessage = "Data Corrupted";
                return result;
            }

            if (data is IValidSaveData validator)
            {
                if (!validator.IsValid())
                {
                    result.Stat = FirestoreLoadStat.DataCorrupted;
                    result.ErrorMessage = "Validation Failed";
                    return result;
                }
            }

            result.Stat = FirestoreLoadStat.Success;
            result.Data = data;
            return result;
        }
        catch (FirebaseException e)
        {
            result.ErrorMessage = e.Message;

            switch ((FirestoreError)e.ErrorCode)
            {
                case FirestoreError.PermissionDenied:
                    result.Stat = FirestoreLoadStat.PermissionError;
                    break;
                case FirestoreError.Unavailable:
                    result.Stat = FirestoreLoadStat.NetworkError;
                    break;
                default:
                    result.Stat = FirestoreLoadStat.UnknownError;
                    break;
            }

            return result;
        }
        catch(Exception e)
        {
            result.Stat = FirestoreLoadStat.UnknownError;
            result.ErrorMessage = e.Message;
            return result;
        }
    }
}
