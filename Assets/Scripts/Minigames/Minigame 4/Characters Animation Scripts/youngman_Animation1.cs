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
    bool isDrinking = true;


    // Update is called once per frame
    void Update()
    {
        if (!isTalking && !isDrinking)
        {
            drinkWater();
        }
    }

    public void bodyBouncingAnim()
    {
        Body.transform.DOScaleY(0.34f, 0.15f).OnComplete(() =>
        {
            Body.transform.DOScaleY(0.33f, 0.2f);
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
    
    public void moveLeftArm()
    {

        isTalking = true;
        Body.GetComponent<Image>().sprite = BodySprites[0];
        Sweat.GetComponent<Image>().sprite = sweatSprite();
        Sweat.transform.DOMoveY(80, 0).OnComplete(() =>
        {
            Sweat.transform.DOLocalMoveY(Sweat.transform.localPosition.y - 80, 2f);
        });


        BothArms.SetActive(true);
        Glass.SetActive(true);
        Glass.transform.DOScale(0.35f, 0.15f).From();

        LeftArm.SetActive(false);
        RigthArm.SetActive(false);

    }

    void drinkWater()
    {
        isDrinking = true;

        BothArms.SetActive(false);
        Glass.SetActive(false);

        LeftArm.SetActive(true);
        RigthArm.SetActive(true);

        Sequence leftArm = DOTween.Sequence();

        leftArm.Append(LeftArm.transform.DORotate(new Vector3(0, 0, 3.38f), 0.4f).SetEase(Ease.OutBack));
        leftArm.Append(LeftArm.transform.DORotate(new Vector3(0, 0, Random.Range(-12, -16)), 0.2f));
        leftArm.Append(LeftArm.transform.DORotate(new Vector3(0, 0, Random.Range(-12, -16)), 0.2f));
        leftArm.Append(LeftArm.transform.DORotate(new Vector3(0, 0, Random.Range(-12, -16)), 0.2f));
        leftArm.Append(LeftArm.transform.DORotate(new Vector3(0, 0, Random.Range(-12, -16)), 0.2f));

        leftArm.OnComplete(() => { isDrinking = false; });
    }

    public void changeMouth()
    {
        Mouth.GetComponent<Image>().sprite = mouthSprite();
    }

    public void closeMouth()
    {
        Mouth.GetComponent<Image>().sprite = MouthSprites[0];
        Body.GetComponent<Image>().sprite = BodySprites[1];
        LeftArm.transform.DORotate(new Vector3(0, 0, 100), 0f).SetDelay(2f).OnStart(() => {
            LeftArm.SetActive(true);
        }).OnComplete(() => {
            isTalking = false;
            isDrinking = false;
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
