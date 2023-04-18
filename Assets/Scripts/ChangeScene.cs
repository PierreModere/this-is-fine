using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeScene : MonoBehaviour
{
    public float fadeTime = 0.5f; // Durée du fondu en noir

    private void Start()
    {
        // Désactive l'objet qui contient ce script pour que le fondu ne soit pas visible au début de la scène
        /*        gameObject.SetActive(false);
        */
    }

    // Méthode appelée lorsqu'on clique sur le bouton
    public void changeScene(string sceneName)
    {
        StartCoroutine(FadeToScene(sceneName));
    }

    // Coroutine pour faire un fondu en noir avant de charger la nouvelle scène
    private IEnumerator FadeToScene(string sceneName)
    {
        // Active l'objet qui contient ce script pour que le fondu soit visible
        gameObject.SetActive(true);

        // Crée un objet pour représenter le fondu en noir
        var fade = new GameObject();
        fade.name = "Fade";
        var image = fade.AddComponent<UnityEngine.UI.Image>();
        image.color = Color.black;
        fade.transform.SetParent(transform, false);

        // Fait un fondu en noir en augmentant l'opacité de l'image
        for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
        {
            var alpha = Mathf.Clamp01(t / fadeTime);
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Charge la nouvelle scène
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        // Fait un fondu en noir inverse en diminuant l'opacité de l'image
        for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
        {
            var alpha = Mathf.Clamp01(1 - (t / fadeTime));
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Détruit l'objet qui représentait le fondu en noir
        Destroy(fade);

    }

}
