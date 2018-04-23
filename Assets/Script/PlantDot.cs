 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using HutongGames.PlayMaker;

public class PlantDot : PlantBase {

    public enum State
    {
        Show,
        Appear,
        Active,
        Hide,
        Die,
    };

    [SerializeField] float showUpDuration = 2f;
    [SerializeField] float hideDuration = 2f;
    [SerializeField] [ReadOnly]  State m_state ;
    [SerializeField] Transform target;
    [SerializeField] [ReadOnly] float m_size = 1f;
    [SerializeField] float basicSize = 0.4f;
    [SerializeField] List<GameObject> m_petals;
    [SerializeField] PlantCreator.FlowerType m_FlowerType;
    [SerializeField] PlantCreator.FlowerType m_FlowerSubType;
    [SerializeField] PlayMakerFSM fsm;
    [SerializeField] SpriteRenderer center;
    [SerializeField] AudioSource source;

    [SerializeField] AudioClip initClip;
    [SerializeField] AudioClip readyClip;
    [SerializeField] AudioClip ActiveClip;
    [SerializeField] AudioClip DieClip;
    // [SerializeField] Transform[] buds;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Active " + collision.tag);
        if (collision.tag == "Player")
        {
            if ( MPlayer.Instance.isDash )
                fsm.SendEvent("ToActive");
        }
            
    }

    //public void OnTriggerEnter2D(Collider2D other)
    //{
    //    Debug.Log("Active " + other.tag);
    //    if ( other.tag == "Player" )
    //    {
    //        Active();
    //    }
    //}

    public void EnterReady()
    {
        source.clip = readyClip;
        source.Play();
        //var item = PlantCreator.Instance.GetFlowerItem(m_FlowerType);
        //for ( int i = 0; i < buds.Length; ++ i )
        //{
        //    PlayShowUpAnimation(buds[i], showUpDuration, true, i * 0.1f);
        //    buds[i].GetComponent<SpriteRenderer>().color = item.color;
        //}

        var item = PlantCreator.Instance.GetFlowerItem(m_FlowerType);

        for (int i = 0; i < m_petals.Count; ++i)
            PlayShowUpAnimation(m_petals[i].transform, 0.5f, false, i * 0.05f);

        // PlayShowPetalAnimation(item.petalNumber, item.budPrefab, item.color, 0);
    }

    public void EnterActive()
    {
        source.clip = ActiveClip;
        source.Play();
        //for (int i = 0; i < buds.Length; ++i)
        //{
        //    PlayHideAnimation(buds[i], hideDuration * 0.5f , true, i * 0.05f);
        //}

        PlayHidePetalsAnimation();
        m_petals.Clear();

        Debug.Log("Active " + m_state);
        m_state = State.Active;

        if (m_FlowerType != PlantCreator.FlowerType.None)
        {
            var item = PlantCreator.Instance.GetFlowerItem(m_FlowerType);
            PlayShowPetalAnimationFast( item.petalNumber , item.petalPrefab , item.color , 0);

            if ( !GameController.Instance.IsZen)
             CreateAttack(item.AttackPrefab);
        }
        if (m_FlowerSubType != PlantCreator.FlowerType.None)
        {
            var item = PlantCreator.Instance.GetFlowerItem(m_FlowerSubType);
            PlayShowPetalAnimationFast(item.petalNumber, item.petalPrefab, item.color , 1.5f );

            if (!GameController.Instance.IsZen)
                CreateAttack(item.AttackPrefab);
        }

        M_Event.FireLogicEvent(LogicEvents.ActivePlant, new LogicArg(this));
    }

    public void CreateAttack( GameObject obj )
    {
        var att = Instantiate(obj) as GameObject;
        att.transform.parent = transform;
        att.transform.localPosition = Vector3.zero;
    }

    public void EnterDie()
    {
        source.clip = DieClip;
        source.Play();

        m_state = State.Die;
        for( int i = 0; i < m_petals.Count; ++ i )
        {
            var sprite = m_petals[i].GetComponentInChildren<SpriteRenderer>();
            sprite.DOColor(Color.black, hideDuration).SetEase(Ease.InOutCubic);
            sprite.DOFade(0, hideDuration).SetEase(Ease.InOutCubic).SetDelay(hideDuration * 0.5f );
            PlayHideAnimation(m_petals[i].transform, hideDuration, false, i * 0.05f + hideDuration * 0.5f );
            
        }

        center.DOColor(Color.black, hideDuration).SetEase(Ease.InOutCubic);
        PlayHideAnimation(target.transform, hideDuration, false, hideDuration + hideDuration);

    }

    public void Hide()
    {
        m_state = State.Hide;

        PlayHideAnimation(target, hideDuration);
    }

    public void Init(PlantCreator.FlowerType type, float size, PlantCreator.FlowerType subType = PlantCreator.FlowerType.None)
    {
        m_size = size;
        m_FlowerType = type;
        m_FlowerSubType = subType;

        m_state = State.Show;
        PlayShowUpAnimation(target, showUpDuration, true , 0.2f );

        var item = PlantCreator.Instance.GetFlowerItem(m_FlowerType);
        center.color = item.color;

        source.clip = initClip;
        source.Play();


        PlayShowPetalAnimation(item.petalNumber, item.budPrefab, item.color, 0);
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

    public void PlayShowPetalAnimation(int petalNumber, GameObject petalPrefab, Color color, float delay)
    {
        StartCoroutine(PlayShowPetalAnimationCor(petalNumber, petalPrefab, color, delay));
    }
    IEnumerator PlayShowPetalAnimationCor(int petalNumber, GameObject petalPrefab, Color color, float delay)
    { 

        float deltaAngle = 360f / petalNumber;

        float interval = fsm.FsmVariables.GetFsmFloat("GrowTime").Value / petalNumber;


        yield return new WaitForSeconds(delay);

        for( int i = 0; i < petalNumber; ++ i )
        {
            var obj = Instantiate(petalPrefab) as GameObject;
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.zero;
            obj.transform.eulerAngles = new Vector3(0, 0, deltaAngle * i);

            obj.GetComponentInChildren<SpriteRenderer>().color = color;

            m_petals.Add(obj);

            PlayShowUpAnimation(obj.transform, showUpDuration   , true , 0);
            yield return new WaitForSeconds(interval);
        }
    }

    public void PlayShowPetalAnimationFast(int petalNumber, GameObject petalPrefab, Color color, float delay)
    {
        StartCoroutine(PlayShowPetalAnimationFastCor(petalNumber, petalPrefab, color, delay));
    }

    IEnumerator PlayShowPetalAnimationFastCor(int petalNumber, GameObject petalPrefab, Color color, float delay)
    { 
        Sequence seq = DOTween.Sequence();

        float deltaAngle = 360f / petalNumber;

        float interval = 0.1f;

        yield return new WaitForSeconds(delay);

        for (int i = 0; i < petalNumber; ++i)
        {
            var obj = Instantiate(petalPrefab) as GameObject;
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.zero;
            obj.transform.eulerAngles = new Vector3(0, 0, deltaAngle * i);

            obj.GetComponentInChildren<SpriteRenderer>().color = color;

            m_petals.Add(obj);



            PlayShowUpAnimation(obj.transform, showUpDuration * 0.5f , true , 0);
            yield return new WaitForSeconds(interval);
        }
    }

    public void PlayHidePetalsAnimation()
    {
        for (int i = 0; i < m_petals.Count ; ++i )
        {
            PlayHideAnimation(m_petals[i].transform, hideDuration, false, 0.1f * i);
        }
    }

}
