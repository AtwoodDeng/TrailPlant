using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlantEffect : MonoBehaviour {
    [SerializeField] Transform circle1;
    [SerializeField] SpriteRenderer sprite1;
    //[SerializeField] Transform circle2;
    //[SerializeField] Transform circle3;
    [SerializeField] float time = 2f;
    [SerializeField] float lastTime = 2f;

    private void Start()
    {
        circle1.DOScale(0, time).SetEase(Ease.InOutCubic).From();
        sprite1.DOFade(0, time).SetEase(Ease.InOutCubic).SetDelay(lastTime).OnComplete(delegate ()
        {
            var collider = circle1.GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;
        });
    }

}
