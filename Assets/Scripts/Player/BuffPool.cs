using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BuffPool : MonoBehaviour
{
    private static Dictionary<Buff, BuffPool> m_BuffPools = new Dictionary<Buff, BuffPool>();


    public static BuffPool getSingleton(Buff buff)
    {
        if (m_BuffPools.ContainsKey(buff))
        {
            return m_BuffPools[buff];
        }
        else
        {
            var pool = new GameObject($"{buff.name}Pool").AddComponent<BuffPool>();
            pool.m_Prefab = buff;
            m_BuffPools.Add(buff, pool);
            return pool;
        }
    }

    [SerializeField]
    private Buff m_Prefab;
    [SerializeField]
    private int currentCount;

    public Pool<Buff> m_Pool;

    private void Awake()
    {
        if (m_Prefab == null)
        {
            DestroyImmediate(this);
        }
        else if (!m_BuffPools.ContainsKey(m_Prefab))
        {
            m_BuffPools.Add(m_Prefab, this);
        }
        else
        {
            DestroyImmediate(this);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        m_Pool = new Pool<Buff>(CreateNew, 5);
        NetworkClient.RegisterPrefab(m_Prefab.gameObject, SpawnHandler, UnspawnHandler);
    }

    // used by NetworkClient.RegisterPrefab
    GameObject SpawnHandler(SpawnMessage msg) => Get(msg.position, msg.rotation).gameObject;

    // used by NetworkClient.RegisterPrefab
    void UnspawnHandler(GameObject spawned) => Return(spawned.GetComponent<Buff>());

    Buff CreateNew()
    {
        // use this object as parent so that objects dont crowd hierarchy
        Buff next = Instantiate(m_Prefab, transform);
        next.name = $"{m_Prefab.name}_pooled_{currentCount}";
        next.gameObject.SetActive(false);
        currentCount++;
        return next;
    }


    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (m_BuffPools.ContainsKey(m_Prefab))
        {
            m_BuffPools.Remove(m_Prefab);
        }
        NetworkClient.UnregisterPrefab(m_Prefab.gameObject);
    }

    // Used to take Object from Pool.
    // Should be used on server to get the next Object
    // Used on client by NetworkClient to spawn objects
    public Buff Get(Vector3 position, Quaternion rotation)
    {
        Buff next = m_Pool.Get();

        // set position/rotation and set active
        next.transform.position = position;
        next.transform.rotation = rotation;
        next.gameObject.SetActive(true);
        return next;
    }

    // Used to put object back into pool so they can b
    // Should be used on server after unspawning an object
    // Used on client by NetworkClient to unspawn objects
    public void Return(Buff spawned)
    {
        // disable object
        spawned.gameObject.SetActive(false);

        // add back to pool
        m_Pool.Return(spawned);
    }
}
