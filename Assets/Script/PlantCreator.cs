using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlantCreator : MonoBehaviour {

    [SerializeField] GameObject prefab;
    [SerializeField] MinMax timeInterval = new MinMax(1f, 5f);
    [SerializeField] MinMax distanceInterval = new MinMax(1f, 5f);
    [SerializeField] int maxActiveNode = 10;
   

    [Space(10f)]
    [Header("Status")]
    [SerializeField] [ReadOnly] float timer;
    [SerializeField] [ReadOnly] float distancer;
    [SerializeField] [ReadOnly] List<PlantNode> plantNodes = new List<PlantNode>();

    private void Update()
    {
        timer -= Time.deltaTime;
        distancer -= MPlayer.Instance.DeltaPos.magnitude;

        if (timer < 0 || distancer < 0)
        {
            CreatePlant();
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

    public void CreatePlant()
    {
        if (GetActiveNodeNumber() >= maxActiveNode)
            return;

        var plant = Instantiate(prefab) as GameObject;
        var com = plant.GetComponent<PlantNode>();
        plant.transform.position = MPlayer.Instance.Position + Vector3.forward;

        float size = Random.RandomRange(0.5f, 1f);
        Vector3 dir = MPlayer.Instance.DeltaPos.normalized + (Vector3) Random.insideUnitCircle * 0.5f ;

        com.Init( dir, size , null );
        plantNodes.Add(com);
    }

}
