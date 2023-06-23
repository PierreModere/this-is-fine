using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

public class EndCutscene : MonoBehaviour
{
    public GameData GameData;
    public GameObject characterAnim;
    public GameObject bgFade;
    public GameObject replayButton;

    [DllImport("__Internal")]
    private static extern void restartGame();

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("FadePanel")) GameObject.Find("FadePanel").GetComponent<Image>().DOFade(0, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void launchCharacterAnim()
    {
        if (GameData.winnerID !="")
        {
            string winnerCharacter = GameData.playersList.Find(cl => cl.id.ToString() == GameData.winnerID).selectedCharacter;
            characterAnim.GetComponent<Animator>().Play(winnerCharacter+"Anim");
        }
    }

    public void onCharacterAnimEnd()
    {
        characterAnim.GetComponent<Animator>().Play("emptyAnim");

    }

    public void onBackgroundAnimEnd()
    {
        bgFade.SetActive(true);
        bgFade.GetComponent<Image>().DOFade(1, 1f).OnComplete(() => { replayButton.GetComponent<Image>().DOFade(1, 1f); });
    }

    public void replay()
    {
        Debug.Log("Game has been reloaded !");
        if (!Application.isEditor)
        {
            restartGame();
        }
    }
}
