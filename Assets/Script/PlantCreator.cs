using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlantCreator : MonoBehaviour {

    [SerializeField] GameObject seedPrefab;
    [SerializeField] MinMax timeInterval = new MinMax(1f, 5f);
    [SerializeField] MinMax distanceInterval = new MinMax(1f, 5f);
    [SerializeField] int maxActiveNode = 10;
    [SerializeField] float createDistance = 10f;

    [SerializeField] GameObject plantEffect;

    [Space(10f)]
    [Header("Status")]
    [SerializeField] [ReadOnly] float timer;
    [SerializeField] [ReadOnly] float distancer;
    [SerializeField] [ReadOnly] List<PlantNode> plantNodes = new List<PlantNode>();
    [SerializeField] [ReadOnly] List<PlantSeed> plantSeeds = new List<PlantSeed>();

    private void Update()
    {
        timer -= Time.deltaTime;
        distancer -= MPlayer.Instance.DeltaPos.magnitude;

        if (timer < 0 || distancer < 0)
        {
            CreateSeed();
            timer = timeInterval.Rand;
            distancer = distanceInterval.Rand;
        }

    }

    public int GetActiveNodeNumber()
    {
        int sum = 0;
        for (int i = 0; i < plantNodes.Count; ++i)
            if (plantNodes[i].MState == PlantNode.State.Grow)
                sum++;

        return sum;
    }

    public int GetActiveSeedNumber()
    {
        int sum = 0;
        for (int i = 0; i < plantSeeds.Count; ++i)
            if (plantSeeds[i].MState == PlantSeed.State.Active )
                sum++;

        return sum;
    }

    public void CreateSeed()
    {
        if (GetActiveSeedNumber() >= maxActiveNode)
            return;

        var seed = Instantiate(seedPrefab) as GameObject;

        var pos = MPlayer.Instance.Position + (Vector3)Random.insideUnitCircle * createDistance;
        seed.transform.position = pos;

        var com = seed.GetComponent<PlantSeed>();

        com.Init(1f);

        plantSeeds.Add(com);

    }

    public void CreatePlantEffect()
    {
        var eff = Instantiate(plantEffect) as GameObject;
        eff.transform.position = MPlayer.Instance.Position;


    }

    //public void CreatePlant()
    //{
    //    if (GetActiveNodeNumber() >= maxActiveNode)
    //        return;

    //    var plant = Instantiate(prefab) as GameObject;
    //    var com = plant.GetComponent<PlantNode>();
    //    plant.transform.position = MPlayer.Instance.Position + Vector3.forward;

    //    float size = Random.RandomRange(0.5f, 1f);
    //    Vector3 dir = MPlayer.Instance.DeltaPos.normalized + (Vector3) Random.insideUnitCircle * 0.5f ;

    //    com.Init( dir, size , null );
    //    plantNodes.Add(com);
    //}

}
