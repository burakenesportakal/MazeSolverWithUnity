using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static string nextScene;

    void Start()
    {
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        yield return new WaitForSeconds(2f);
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);

        while (!op.isDone)
        {
            yield return null;
        }
    }
}
