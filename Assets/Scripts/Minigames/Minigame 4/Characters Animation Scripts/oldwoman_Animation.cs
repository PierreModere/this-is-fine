using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class oldwoman_Animation : MonoBehaviour
{

    public GameObject Body;

    public GameObject Mouth;
    public List<Sprite> MouthSprites;

    public GameObject Eyes;
    public List<Sprite> EyesSprites;

    public GameObject Eyebrows;
    public GameObject LeftArm;
    public GameObject RigthArm;

    bool isTalking = false;
    bool isBouncing = false;
    bool isBlinking = false;

    public GameObject blinkSFX;
    public List<GameObject> voiceSFX;

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

        if (!isBlinking && isTalking)
        {
            eyesBlinkAnim(Random.Range(0.65f, 1.6f), Random.Range(0.03f, 0.15f));
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
        isBlinking = true;
        Eyes.transform.DOScale(1f, 0).SetDelay(delay).OnComplete(() =>
        {
            Eyes.GetComponent<Image>().sprite = EyesSprites[0];
            
            blinkSFX.GetComponent<AudioSource>().Play();

            Eyes.transform.DOScale(1f, 0).SetDelay(blinTime).OnComplete(() =>
            {
                Eyes.GetComponent<Image>().sprite = EyesSprites[1];
                isBlinking = false;
            });
        });
    }

    public void moveLeftArm()
    {
        voiceSFX[Random.Range(0, voiceSFX.Count - 1)].GetComponent<AudioSource>().Play();
        
        isTalking = true;

        Eyebrows.transform.DOLocalMoveY(Eyebrows.transform.localPosition.y - 30f, 0.2f).SetEase(Ease.InOutBack);

        Sequence leftArm = DOTween.Sequence();

        leftArm.Append(LeftArm.transform.DORotate(new Vector3(0, 0, Random.Range(6,16)), 0.2f).SetEase(Ease.InBack));
        leftArm.Append(LeftArm.transform.DORotate(new Vector3(0, 0, Random.Range(70, 85)), 0.2f));
        leftArm.Append(LeftArm.transform.DORotate(new Vector3(0, 0, Random.Range(6, 25)), 0.3f));
        leftArm.Append(LeftArm.transform.DORotate(new Vector3(0, 0, 85), 0.3f).SetEase(Ease.OutBack));

    }

    public void changeMouth()
    {
        Mouth.GetComponent<Image>().sprite = mouthSprite();
    }

    public void closeMouth()
    {
        isTalking = false;
        Eyebrows.transform.DOLocalMoveY(Eyebrows.transform.localPosition.y + 30f, 0.2f).SetEase(Ease.OutBack).SetDelay(1.4f);
        Mouth.GetComponent<Image>().sprite = MouthSprites[0];
    }
    Sprite mouthSprite()
    {
        return MouthSprites[Random.Range(0, MouthSprites.Count-1)];
    }
}
