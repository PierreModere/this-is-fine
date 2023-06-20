using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    public GameData GameData;

    public void playTransitionAnim()
    {
        transform.Find("TransitionAnim").gameObject.GetComponent<Animator>().Play("screenTransitionStart");
    }
    public void changeScene()
    {
        if (GameData.currentScene != null && GameData.currentScene != "") {
            // Charge la nouvelle scène
            SceneManager.LoadScene(GameData.currentScene, LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameObject.GetComponent<Animator>().Play("screenTransitionEnd");
    }




}
