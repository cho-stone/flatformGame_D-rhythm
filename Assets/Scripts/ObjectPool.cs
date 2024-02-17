using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //���̶�Űâ�� ��������
public class ObjectInfo
{
    public GameObject goPrefab; //�ʿ��� ��ŭ ����
    public int count; // ����
    public Transform tfPoolParent; //�����ġ�� ��������
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance; //�����ڿ����� ��ȯ

    [SerializeField] ObjectInfo[] objectInfo = null;
    public Queue<GameObject> noteQueue = new Queue<GameObject>(); //ť�� ���� ������Ʈ��

    public Queue<GameObject> droneQueue = new Queue<GameObject>();

    public Queue<GameObject> bulletQueue = new Queue<GameObject>();

    public List<GameObject> notes = new List<GameObject>(); //��Ʈ ��������Ʈ ������ �ʿ�

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        noteQueue = InsertQueue(objectInfo[0]);
        droneQueue = InsertDroneQueue(objectInfo[1]);
        bulletQueue = InsertBulletQueue(objectInfo[2]);
    }

    Queue<GameObject> InsertQueue(ObjectInfo objInfo)
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        for(int i = 0; i < objInfo.count; i++)
        {
            GameObject clone = Instantiate(objInfo.goPrefab, transform.position, Quaternion.identity);

            notes.Add(clone);

            clone.SetActive(false);
            if (objInfo.tfPoolParent != null)
                clone.transform.SetParent(objInfo.tfPoolParent);
            else
                clone.transform.SetParent(this.transform);
            queue.Enqueue(clone);
        }

        return queue;
    }

    Queue<GameObject> InsertDroneQueue(ObjectInfo objectInfo)
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        for(int i = 0; i < objectInfo.count; i++)
        {
            GameObject clone = Instantiate(objectInfo.goPrefab, transform.position, Quaternion.identity);
            clone.SetActive(false);
            if (objectInfo.tfPoolParent != null)
                clone.transform.SetParent(objectInfo.tfPoolParent);
            else
                clone.transform.SetParent(this.transform) ;
            queue.Enqueue(clone);
        }
        return queue;
    }

    Queue<GameObject> InsertBulletQueue(ObjectInfo objectInfo)
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        for (int i = 0; i < objectInfo.count; i++)
        {
            GameObject clone = Instantiate(objectInfo.goPrefab, transform.position, Quaternion.identity);
            clone.SetActive(false);
            if (objectInfo.tfPoolParent != null)
                clone.transform.SetParent(objectInfo.tfPoolParent);
            else
                clone.transform.SetParent(this.transform);
            queue.Enqueue(clone);
        }
        return queue;
    }
}
