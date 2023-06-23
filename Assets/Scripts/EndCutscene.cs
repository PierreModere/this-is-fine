using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndCutscene : MonoBehaviour
{
    public GameData GameData;
    public GameObject characterAnim;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("FadePanel").GetComponent<Image>().DOFade(0, 0.2f);
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
}
