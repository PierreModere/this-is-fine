using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleAnimation : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 originalPos;
    private Vector2 oringalScale;
    void Start()
    {
        originalPos = transform.position;
        oringalScale = transform.localScale;
        Sequence anim = DOTween.Sequence();

        anim.SetLoops(-1, LoopType.Restart);
        anim.Append(transform.DOScale(1.005f, 0.1f).SetEase(Ease.OutBack).SetDelay(0.1f));
        anim.Append(transform.DOScale(1, 0.1f).SetEase(Ease.OutBack).SetDelay(0.1f));


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
