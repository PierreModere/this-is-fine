using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeScene : MonoBehaviour
{
    public float fadeTime = 0.5f; // Dur�e du fondu en noir

    private void Start()
    {
        // D�sactive l'objet qui contient ce script pour que le fondu ne soit pas visible au d�but de la sc�ne
        /*        gameObject.SetActive(false);
        */
    }

    // M�thode appel�e lorsqu'on clique sur le bouton
    public void changeScene(string sceneName)
    {
        StartCoroutine(FadeToScene(sceneName));
    }

    // Coroutine pour faire un fondu en noir avant de charger la nouvelle sc�ne
    private IEnumerator FadeToScene(string sceneName)
    {
        // Active l'objet qui contient ce script pour que le fondu soit visible
        gameObject.SetActive(true);

        // Cr�e un objet pour repr�senter le fondu en noir
        var fade = new GameObject();
        fade.name = "Fade";
        var image = fade.AddComponent<UnityEngine.UI.Image>();
        image.color = Color.black;
        fade.transform.SetParent(transform, false);

        // Fait un fondu en noir en augmentant l'opacit� de l'image
        for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
        {
            var alpha = Mathf.Clamp01(t / fadeTime);
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Charge la nouvelle sc�ne
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        // Fait un fondu en noir inverse en diminuant l'opacit� de l'image
        for (float t = 0.0f; t < fadeTime; t += Time.deltaTime)
        {
            var alpha = Mathf.Clamp01(1 - (t / fadeTime));
            image.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // D�truit l'objet qui repr�sentait le fondu en noir
        Destroy(fade);

    }

}
