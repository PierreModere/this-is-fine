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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    

    public void changeMouth()
    {
        Mouth.GetComponent<Image>().sprite = mouthSprite();
    }

    public void closeMouth()
    {
        Mouth.GetComponent<Image>().sprite = MouthSprites[0];
    }
    Sprite mouthSprite()
    {
        return MouthSprites[Random.Range(0, MouthSprites.Count-1)];
    }
}
