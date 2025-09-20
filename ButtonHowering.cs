using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHowering : MonoBehaviour, IPointerEnterHandler
{
   [SerializeField] private AudioClip buttonHowerSoundEffectClip;

  public void OnPointerEnter(PointerEventData eventData)
  {
    Sounds.Instance.PlaySoundEffect(buttonHowerSoundEffectClip);
  }
}
