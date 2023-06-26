using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class oldman_Animation : MonoBehaviour
{

    public GameObject Body;
    public List<Sprite> BodySprites;

    public GameObject Mouth;
    public List<Sprite> MouthSprites;

    public GameObject Eyes;
    public List<Sprite> EyesSprites;

    public GameObject Eyebrows;
    public GameObject ShockWave;
    public GameObject Cigar;
    public GameObject Smoke;

    bool isTalking = false;
    bool isBouncing = false;

    public List<GameObject> punchWoodSFX;

    // Start is called before the first frame update
    void Start()
    {
        isTalking = false;
        isBouncing = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (isTalking && !isBouncing)
        {
            bodyBouncingAnim();
        }
    }

    public void bodyBouncingAnim()
    {
        isBouncing = true;
        Body.transform.DOScaleY(0.34f, 0.15f).OnComplete(() =>
        {
            Body.transform.DOScaleY(0.33f, 0.2f).OnComplete(() => { isBouncing = false; });
        });

    }

    public void eyesBlinkAnim(float delay, float blinTime)
    {
        Eyes.transform.DOScale(1f, 0).SetDelay(delay).OnComplete(() =>
        {
            Eyes.GetComponent<Image>().sprite = EyesSprites[0];

            Eyes.transform.DOScale(1f, 0).SetDelay(blinTime).OnComplete(() =>
            {
                Eyes.GetComponent<Image>().sprite = EyesSprites[1];
            });
        });
    }

    public void moveArms()
    {
        Smoke.GetComponent<Image>().DOFade(0, 0.4f).OnComplete(() => { Smoke.SetActive(false); });

        Sequence hitLectern = DOTween.Sequence();

        hitLectern.Append(Eyebrows.transform.DOLocalMoveY(Eyebrows.transform.localPosition.y - 50f, 0.2f).SetEase(Ease.OutBack).OnStart(() => { Body.GetComponent<Image>().sprite = BodySprites[1]; }));
        hitLectern.Join(ShockWave.GetComponent<Image>().DOFade(1, 0));
        hitLectern.Append(Body.transform.DOScaleY(0.35f, 0.2f).SetDelay(0.2f));
        hitLectern.Append(Body.transform.DOScaleY(0.32f, 0.1f).SetEase(Ease.OutBack).OnComplete(() => {
            Body.GetComponent<Image>().sprite = BodySprites[2];
            ShockWave.SetActive(true);

            punchWoodSFX[Random.Range(0, punchWoodSFX.Count - 1)].GetComponent<AudioSource>().Play();
            punchWoodSFX[Random.Range(0, punchWoodSFX.Count - 1)].GetComponent<AudioSource>().Play();
        }));
        hitLectern.Append(ShockWave.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() => {
            ShockWave.SetActive(false);
            isTalking = true;
        }));
    }

    public void changeMouth()
    {
        Mouth.GetComponent<Image>().sprite = mouthSprite();
    }

    public void closeMouth()
    {
        isTalking = false;
        Mouth.GetComponent<Image>().sprite = MouthSprites[0];
        Eyebrows.transform.DOLocalMoveY(Eyebrows.transform.localPosition.y + 50f, 0.2f).SetEase(Ease.OutBack);
        Smoke.GetComponent<Image>().DOFade(1, 0.5f).SetDelay(1f).OnComplete(() => { Smoke.SetActive(true); });
    }
    Sprite mouthSprite()
    {
        return MouthSprites[Random.Range(0, MouthSprites.Count-1)];
    }
}
