using DG.Tweening;
using UnityEngine;

public class ArcMover : MonoBehaviour
{
    int state = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))  //CCW rotation
        {
            switch (state)
            {
                case 0:
                    transform.DOLocalMoveX(0, .2f).SetEase(Ease.InQuad);
                    transform.DOLocalMoveY(-2, .2f).SetEase(Ease.OutQuad);
                    state = 1;
                    break;
                case 1:
                    transform.DOLocalMoveX(-2, .2f).SetEase(Ease.OutQuad);
                    transform.DOLocalMoveY(0, .2f).SetEase(Ease.InQuad);
                    state = 2;
                    break;
                case 2:
                    transform.DOLocalMoveX(0, .2f).SetEase(Ease.InQuad);
                    transform.DOLocalMoveY(2, .2f).SetEase(Ease.OutQuad);
                    state = 3;
                    break;
                case 3:
                    transform.DOLocalMoveX(2, .2f).SetEase(Ease.OutQuad);
                    transform.DOLocalMoveY(0, .2f).SetEase(Ease.InQuad);
                    state = 0;
                    break;
                default:
                    break;
            }
        }
    }
}
