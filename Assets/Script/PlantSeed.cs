 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class PlantSeed : PlantBase {

    public enum State
    {
        Show,
        Appear,
        Active,
        Hide,
    };

    [SerializeField] float showUpDuration = 2f;
    [SerializeField] float hideDuration = 2f;
    [SerializeField] [ReadOnly] State m_state;
    [SerializeField] Transform target;
    [SerializeField] [ReadOnly] float m_size = 1f;
    [SerializeField] float basicSize = 0.4f;
    [SerializeField] GameObject rootPrefab;
	[SerializeField] GameObject activeEffect;

    public State MState {  get { return m_state;  } }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Active " + collision.tag);

		if (collision.tag == "Player") {

            if (m_state != State.Active)
            {
                m_state = State.Active;
                
				Vector3 dir = MPlayer.Instance.DeltaPos;
				CreateRoot (dir.normalized);
            }
		}

//        if (collision.tag == "Plant")
//        {
//
//            if (m_state != State.Active)
//            {
//                m_state = State.Active;
//                
//                Vector3 dir = (transform.position - collision.gameObject.transform.position).normalized;
//                Debug.Log("Dir " + dir);
//                CreateRoot( ( transform.position - collision.gameObject.transform.position ).normalized );
//            }
//		} 
    }

    //public void OnEnable()
    //{
    //    M_Event.RegisterEvent(LogicEvents.MakePlant, OnMakePlant);
    //}
    //public void OnDisable()
    //{
    //    M_Event.UnregisterEvent(LogicEvents.MakePlant, OnMakePlant);
    //}

    //public void OnMakePlant(LogicArg arg)
    //{

    //    CreateRoot();
    //}
    
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

    public void CreateRoot( Vector3 dir )
    {
		// show active effect
		var effect = Instantiate( activeEffect , transform ) as GameObject;
		effect.transform.localPosition = Vector3.zero;

		// create root
		int createNumber = 1 ; //Random.RandomRange(1, 4);



        for (int i = 0; i < createNumber; ++i)
        {
            var plant = Instantiate(rootPrefab) as GameObject;
            var com = plant.GetComponent<PlantNode>();
            plant.transform.parent = transform;
            plant.transform.localPosition = Vector3.zero;

            float size = Random.RandomRange(0.5f, 1f);
            dir = (dir.normalized + (Vector3)Random.insideUnitCircle * 0.1f);
            dir.z = 0;

            com.Init(dir.normalized, size, this);
        }
    }


}
