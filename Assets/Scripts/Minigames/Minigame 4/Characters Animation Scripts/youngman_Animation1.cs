using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class youngman_Animation : MonoBehaviour
{

    public GameObject Body;
    public List<Sprite> BodySprites;

    public GameObject Mouth;
    public List<Sprite> MouthSprites;

    public GameObject Eyes;
    public List<Sprite> EyesSprites;

    public GameObject Eyebrows;
    public GameObject LeftArm;
    public GameObject RigthArm;
    public GameObject BothArms;
    public GameObject Glass;

    public GameObject Sweat;
    public List<Sprite> Sweatsprites;

    bool isTalking = false;
    bool isDrinking = false;
    bool isBouncing = false;
    bool isBlinking = false;

    // Update is called once per frame
    void Update()
    {
        if (!isTalking && isDrinking)
        {
            float wiggleAngle = Mathf.Sin(Time.time * 5f) * 0.02f;
            LeftArm.transform.rotation = LeftArm.transform.rotation * Quaternion.Euler(0f, 0f, wiggleAngle);
        }
        else if (isTalking && !isBouncing)
        {
            bodyBouncingAnim();
        }

        if (!isBlinking && isTalking)
        {
            eyesBlinkAnim(Random.Range(0.5f, 1f), Random.Range(0.03f, 0.15f));
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

            Eyes.transform.DOScale(1f, 0).SetDelay(blinTime).OnComplete(() =>
            {
                Eyes.GetComponent<Image>().sprite = EyesSprites[1];
                isBlinking = false;
            });
        });
    }

    public void moveLeftArm()
    {
        Eyebrows.transform.DOLocalMoveY(Eyebrows.transform.localPosition.y - 32f, 0.2f).SetEase(Ease.InOutBack);

        isTalking = true;
        Body.GetComponent<Image>().sprite = BodySprites[0];
        Sweat.GetComponent<Image>().sprite = sweatSprite();
      
        BothArms.SetActive(true);
        Glass.SetActive(true);
        Glass.transform.DOScale(0.35f, 0.15f).From();

        LeftArm.SetActive(false);
        RigthArm.SetActive(false);

    }

    public void changeMouth()
    {
        Mouth.GetComponent<Image>().sprite = mouthSprite();
    }

    public void closeMouth()
    {
        Mouth.GetComponent<Image>().sprite = MouthSprites[0];
        Eyebrows.transform.DOLocalMoveY(Eyebrows.transform.localPosition.y + 32f, 0.2f).SetEase(Ease.OutBack).SetDelay(1.4f);

        LeftArm.transform.DORotate(new Vector3(0, 0, 90), 0f).SetDelay(2f).OnStart(() =>
        {
            Body.GetComponent<Image>().sprite = BodySprites[1];
            LeftArm.SetActive(true);
            RigthArm.SetActive(true);
            BothArms.SetActive(false);
            Glass.SetActive(false);
        }).OnComplete(() => {
            LeftArm.transform.DORotate(new Vector3(0, 0, -15), 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
                isTalking = false;
                isDrinking = true;
            });
        });
    }

    Sprite mouthSprite()
    {
        return MouthSprites[Random.Range(0, MouthSprites.Count-1)];
    }
    Sprite sweatSprite()
    {
        return Sweatsprites[Random.Range(0, Sweatsprites.Count - 1)];
    }
}
