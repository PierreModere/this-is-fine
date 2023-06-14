using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class youngwoman_Animation : MonoBehaviour
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

    public void moveLeftArm()
    {
        isTalking = true;
    }

    public void changeMouth()
    {
        Mouth.GetComponent<Image>().sprite = mouthSprite();
    }

    public void closeMouth()
    {
        isTalking = false;
        Mouth.GetComponent<Image>().sprite = MouthSprites[0];
    }
    Sprite mouthSprite()
    {
        return MouthSprites[Random.Range(0, MouthSprites.Count-1)];
    }
}
