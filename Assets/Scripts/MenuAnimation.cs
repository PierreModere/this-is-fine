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

    public GameObject instruction;

    // Start is called before the first frame update
    void Start()
    {
        splashScreen.SetActive(true);
        Sequence splashScreenAnim = DOTween.Sequence();

        //Menu elements set to invisible
        splashScreenAnim.Append(createButton.GetComponent<Image>().DOFade(0,0));
        splashScreenAnim.Join(joinButton.GetComponent<Image>().DOFade(0, 0));

        //Splashscreen BureauCrap
        splashScreenAnim.Join(splashScreen.transform.Find("BureauCrapLogo").GetComponent<Image>().DOFade(0, 0.8f).From());

        splashScreenAnim.AppendCallback(() => {
            splashScreen.transform.Find("BureauCrapLogo").GetComponent<Animator>().Play("bureauCrapLogon"); 
        });
        splashScreenAnim.Append(instruction.GetComponent<TextMeshProUGUI>().DOFade(1, 0.3f).SetDelay(2f));
        splashScreenAnim.AppendCallback(() => {
            splashScreen.transform.Find("BureauCrapLogo").GetComponent<Button>().interactable = true;
        });
    }

    public void tapScreenToStart()
    {
        Sequence startMenuAnim = DOTween.Sequence();

        startMenuAnim.Append(splashScreen.transform.Find("BureauCrapLogo").GetComponent<Image>().DOFade(0, 0.7f));
        startMenuAnim.Join(instruction.GetComponent<TextMeshProUGUI>().DOFade(0, 0.5f).OnComplete(() => { instruction.SetActive(false); }));
        startMenuAnim.Append(splashScreen.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() => { splashScreen.SetActive(false); }));
        startMenuAnim.AppendCallback(() => { GameObject.Find("StartMenuMusic").GetComponent<AudioSource>().Play(); });

        //Menu elements pop-up
        startMenuAnim.Join(logo.transform.DOScale(0.7f, 0.3f).SetEase(Ease.InOutBack).SetDelay(0.2f).From());
        startMenuAnim.Append(createButton.GetComponent<Image>().DOFade(1f, 0.2f));
        startMenuAnim.Join(createButton.transform.DOLocalMoveY(-130, 0.3f).SetEase(Ease.InOutBack).From());
        startMenuAnim.Join(joinButton.GetComponent<Image>().DOFade(1f, 0.2f).SetDelay(-0.1f));
        startMenuAnim.Join(joinButton.transform.DOLocalMoveY(-200, 0.3f).SetEase(Ease.InOutBack).From());
    }
}
