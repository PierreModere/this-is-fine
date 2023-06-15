using UnityEngine;

public class StartAnimController : MonoBehaviour
{
    public MinigameUI MinigameUI;
    public void OnAnimationEnd(AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter != "")
        {
            MinigameUI.popUpAnimationEnd(animationEvent.stringParameter);
        }
    }
}
