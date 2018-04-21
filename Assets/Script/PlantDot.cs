 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class PlantDot : PlantBase {

    public enum State
    {
        Show,
        Appear,
        Active,
        Hide,
    };

    [SerializeField] float showUpDuration = 2f;
    [SerializeField] float hideDuration = 2f;
    [SerializeField] [ReadOnly]  State m_state ;
    [SerializeField] Transform target;
    [SerializeField] [ReadOnly] float m_size = 1f;
    [SerializeField] float basicSize = 0.4f;
    [SerializeField] GameObject flowerPetals;
    [SerializeField] List<GameObject> m_petals;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Active " + collision.tag);
        if ( collision.tag == "Player" )  
            Active();
    }

    //public void OnTriggerEnter2D(Collider2D other)
    //{
    //    Debug.Log("Active " + other.tag);
    //    if ( other.tag == "Player" )
    //    {
    //        Active();
    //    }
    //}

    public void Active()
    {
        if (m_state != State.Active)
        {
            Debug.Log("Active " + m_state);
            m_state = State.Active;
            PlayShowPetalAnimation(6, flowerPetals);
        }
    }

    public void Hide()
    {
        m_state = State.Hide;

        PlayHideAnimation(target, hideDuration);
    }

    public void Init( float size )
    {
        m_size = size;
        
        m_state = State.Show;
        PlayShowUpAnimation(target, showUpDuration, true , 0.2f );
    }

    public void PlayShowUpAnimation( Transform trans , float duration , bool force = false , float delay = 0 )
    {
        Sequence seq = DOTween.Sequence();

        if ( force )
            seq.Append(trans.DOScale(0, 0));

        seq.AppendInterval(delay);
        seq.Append(trans.DOScale(1.25f * basicSize * m_size, duration * 0.7f).SetEase(Ease.InOutCirc));
        // seq.Append(trans.DOScale(0.85f * basicSize * m_size, duration * 0.25f).SetEase(Ease.InOutCirc));
        seq.Append(trans.DOScale(1.00f * basicSize * m_size, duration * 0.3f).SetEase(Ease.InOutCirc));
        
    }

    public void PlayHideAnimation( Transform trans, float duration , bool force = false , float delay =0 )
    {
        Sequence seq = DOTween.Sequence();
        
        if ( force)
            seq.Append(trans.DOScale(1f * basicSize * m_size, 0));

        seq.AppendInterval(delay);
        seq.Append(trans.DOScale(1.25f * basicSize * m_size, duration * 0.33f).SetEase(Ease.InOutCirc));
        seq.Append(trans.DOScale(0f, duration * 0.66f).SetEase(Ease.InOutCirc));

    }

    public void PlayShowPetalAnimation( int petalNumber , GameObject petalPrefab  )
    {
        Sequence seq = DOTween.Sequence();

        float deltaAngle = 360f / petalNumber;
        
        for( int i = 0; i < petalNumber; ++ i )
        {
            var obj = Instantiate(petalPrefab) as GameObject;
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.zero;
            obj.transform.eulerAngles = new Vector3(0, 0, deltaAngle * i);

            m_petals.Add(obj);

            PlayShowUpAnimation(obj.transform, showUpDuration * 0.5f  , true , 0.05f * i );
        }
    }

}
