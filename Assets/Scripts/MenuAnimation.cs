using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class MenuAnimation : MonoBehaviour
{
    public GameObject logo;

    // Start is called before the first frame update
    void Start()
    {

        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(logo.transform.DOScale(0.8f, 0.3f).SetEase(Ease.InOutBack).From());
 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
