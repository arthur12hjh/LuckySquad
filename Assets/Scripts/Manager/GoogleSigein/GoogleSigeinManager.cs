using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GoogleSigeinManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;

    void Start()
    {
        InitFirebase();
    }

    private void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log($"Firebase Auth initialized successfully.");
            }
            else
            {
                Debug.LogError($"Could not resolve Firebase dependencies: {task.Result}");
            }
        });
    }

    public void GoogleSignInClick()
    {
        try
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration()
            {
                WebClientId = "YourClientId",
                RequestIdToken = true,
                UseGameSignIn = false,
                RequestEmail = true
            };

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
            {
                if (task.IsFaulted)
                    Debug.Log("Faulted");
                else if (task.IsCanceled)
                    Debug.Log("Canceled");
                else
                    OnGoogleAuthenticatedFinished(task);
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"GoogleSignInClick Exception: {e.Message}");
        }
    }

    private void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
            Debug.Log("Faulted");
        else if (task.IsCanceled)
            Debug.Log("Canceled");
        else
        {
            Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                    return;

                if (task.IsFaulted)
                {
                    Debug.Log("Faulted");
                    return;
                }

                user = auth.CurrentUser;

                Debug.Log($"UserName: {user.DisplayName}");
                Debug.Log($"UserEmail: {user.Email}");
            });
        }
    }

    public void SignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();

        if (auth != null)
            auth.SignOut();

        user = null;
    }
}
