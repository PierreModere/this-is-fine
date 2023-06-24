using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class MenuAnimation : MonoBehaviour
{
    public GameObject splashScreen;
    public GameObject logo;
    public GameObject createButton;
    public GameObject joinButton;

    // Start is called before the first frame update
    void Start()
    {
        splashScreen.SetActive(true);
        Sequence startMenuAnim = DOTween.Sequence();

        //Menu elements set to invisible
        startMenuAnim.Append(createButton.GetComponent<Image>().DOFade(0,0));
        startMenuAnim.Join(joinButton.GetComponent<Image>().DOFade(0, 0));

        //Splashscreen BureauCrap
        startMenuAnim.Join(splashScreen.transform.Find("BureauCrapLogo").GetComponent<Image>().DOFade(0, 0.8f).From());

        startMenuAnim.AppendCallback(() => { splashScreen.transform.Find("BureauCrapLogo").GetComponent<AudioSource>().Play();
            splashScreen.transform.Find("BureauCrapLogo").GetComponent<Animator>().Play("bureauCrapLogon"); });
        startMenuAnim.Append(splashScreen.transform.Find("BureauCrapLogo").GetComponent<Image>().DOFade(0, 0.7f).SetDelay(3.5f));
        startMenuAnim.Append(splashScreen.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() => { splashScreen.SetActive(false); }));

        //Menu elements pop-up
        startMenuAnim.AppendCallback(() => { GameObject.Find("StartMenuMusic").GetComponent<AudioSource>().Play(); });
        startMenuAnim.Join(logo.transform.DOScale(0.7f, 0.3f).SetEase(Ease.InOutBack).SetDelay(0.2f).From());
        startMenuAnim.Append(createButton.GetComponent<Image>().DOFade(1f, 0.2f));
        startMenuAnim.Join(createButton.transform.DOLocalMoveY(-130, 0.3f).SetEase(Ease.InOutBack).From());
        startMenuAnim.Join(joinButton.GetComponent<Image>().DOFade(1f, 0.2f).SetDelay(-0.1f));
        startMenuAnim.Join(joinButton.transform.DOLocalMoveY(-200, 0.3f).SetEase(Ease.InOutBack).From());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
