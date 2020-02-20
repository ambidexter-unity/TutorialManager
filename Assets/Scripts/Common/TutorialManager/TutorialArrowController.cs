using DG.Tweening;
using UnityEngine;

namespace Common.TutorialManager
{
    public class TutorialArrowController : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private GameObject arrow;
#pragma warning restore 649

        private Sequence _sequence;
        
        private void OnEnable()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(arrow.transform.DOLocalMoveY(-15, 1f).SetEase(Ease.InExpo));
            _sequence.Append(arrow.transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutExpo));
            _sequence.SetLoops(-1);
            _sequence.Play();
        }

        private void OnDisable()
        {
            arrow.transform.localPosition = Vector3.zero;
            _sequence?.Kill();
            _sequence = null;
        }
    }
}