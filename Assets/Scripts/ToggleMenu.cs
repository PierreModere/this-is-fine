using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToggleMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject menuToDisable;
    [SerializeField]
    private GameObject menuToEnable;
    private RectTransform transitionPanel;

    void Start()
    {
        transitionPanel = GameObject.Find("TransitionPanel").GetComponent<RectTransform>();
    }

    public void disableMenu()
    {
     menuToDisable.SetActive(false);
    }
    public void enableMenu()
    {
        menuToEnable.SetActive(true);
    }

    void playPanelTransition()
    {
        Sequence mySequence = DOTween.Sequence();

        //Your code here  
        mySequence.Append(transitionPanel.DOSizeDelta(new Vector2(1080f, 1920), 0.4f))
        .Append(transitionPanel.DOMoveX(1080f, 0))
        .Append(transitionPanel.DORotate(new Vector3(1080,0, -180), 0))
        .Append(transitionPanel.DOSizeDelta(new Vector2(0f, 1920), 0.4f))

        .OnComplete(() => {
            Debug.Log("Done");
        });

    }
}
