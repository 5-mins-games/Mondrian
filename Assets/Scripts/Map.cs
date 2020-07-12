using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Map : MonoBehaviour
{
    public static Map instance;

    [Range(1, 10)]
    public uint step = 1;
    [Range(1, 5)]
    public uint maxNeighbours = 5;
    [Range(0, 1)]
    public float edgeRatio = 0.2f;
    [Range(1, 5)]
    public int nState = 3;
    [Range(1, 10)]
    public float baseDistance = 5f;

    public GameObject nodePrefab;

    private readonly List<GameObject> nodes = new List<GameObject>();
    private Slider slider;

    void Start()
    {
        if (instance != null && this != instance)
        {
            Destroy(this.gameObject);
        }
        instance = this;

        slider = GameObject.FindWithTag("DistanceSlider").GetComponent<Slider>();

        Debug.Log("Map iniatialized. instance: " + instance);

        GameManager.instance.ProgressLevel();
    }

    void Update()
    {
        foreach (GameObject n in nodes)
        {
            Node node = n.GetComponent<Node>();

            node.baseDistance = baseDistance;
            node.nState = nState;
        }
    }



    #region Level Up

    public void ProgressLevel(LevelConf config)
    {
        this.nState = config.nState;
        this.maxNeighbours = (uint)config.maxNeighbours;
        this.step = (uint)config.step;
        this.edgeRatio = config.edgeRatio;

        Generate();

        float dist = Mathf.Clamp(baseDistance + 1.5f, 1, 10);

        if (slider)
        {
            slider.value = dist;
        }
        else
        {
            baseDistance = dist;
        }
    }

    // win condition
    public bool ClearLevel()
    {
        int status = nodes[0].GetComponent<Node>().status;

        foreach (GameObject n in nodes)
        {
            if (n.GetComponent<Node>().status != (uint)status)
                return false;
        }
        return true;
    }
    #endregion



    #region Draw Grapgh

    public void SetBaseDistance(float value)
    {
        baseDistance = Mathf.Clamp(value, 1, 10);
    }

    private void StepGenerate()
    {
        GameObject node = CreateNode(NextPos(), NextID());

        nodes.Add(node);

        // link to last node
        if (nodes.Count > 1)
        {
            GameObject lastNode = nodes[nodes.Count - 2];

            node.GetComponent<Node>().AddNeighbour(lastNode);
        }

        // neighbours
        int nNearby = (int) Mathf.FloorToInt(Random.value * 100f) % (int) maxNeighbours;

        nNearby = (int) Mathf.Clamp(nNearby, 1, maxNeighbours);

        GameObject[] tmp = new GameObject[nNearby];

        for (int i = 0; i < nNearby; i++)
        {
            GameObject newNode = CreateNode(NextPos(), NextID());

            tmp[i] = newNode;

            // link to master node
            node.GetComponent<Node>().AddNeighbour(newNode);

            nodes.Add(newNode);
        }

        // neighbours may have links too
        foreach (GameObject n in tmp)
        {
            foreach (GameObject m in tmp)
            {
                if (n == m) continue;

                if (Random.value > edgeRatio)
                {
                    n.GetComponent<Node>().AddNeighbour(m);                
                }
            }
        }

        // change state
        foreach (GameObject n in nodes)
        {
            RandomizeState(n);
        }
    }

    private void Generate()
    {
        for (int i = 0; i < step; i++)
        {
            StepGenerate();
        }
    }

    private Vector3 NextPos()
    {
        if (nodes.Count == 0)
            return Vector3.zero;

        Vector3 prevPos = nodes[nodes.Count - 1].transform.position;
        Vector3 pos = new Vector3(prevPos.x + Random.value, prevPos.y + Random.value, prevPos.z);
        return pos;
    }

    private int NextID()
    {
        if (nodes.Count == 0)
            return 0;

        return nodes[nodes.Count - 1].GetComponent<Node>().id + 1;
    }

    private int CurState()
    {
        if (nodes.Count == 0) return 0;

        // assume it's already level clear, so all colors are the same
        return nodes[0].GetComponent<Node>().status;
    }

    private GameObject CreateNode(Vector3 pos, int id)
    {
        GameObject newNode = Instantiate(nodePrefab, pos, nodePrefab.transform.rotation);
        newNode.transform.parent = transform;
        newNode.name = "Node_" + id;

        Node node = newNode.GetComponent<Node>();
        node.id = id;
        node.nState = nState;
        node.baseDistance = baseDistance;
        // node.status = Mathf.RoundToInt(Random.value * 100) % nState;
        node.status = CurState();

        return newNode;
    }

    private void RandomizeState(GameObject node)
    {
        for (int i = 0; i < 3; i++)
        {
            if (Random.value > .5f)
                node.GetComponent<Node>().StepQuiet();
        }
    }
    #endregion
}
