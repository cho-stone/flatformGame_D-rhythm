using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform player; //�÷��̾�
    [SerializeField] Vector3 cameraPosition; //ī�޶� ��ġ

    [SerializeField]float cameraMoveSpeed;
    float height;
    float width;

    [SerializeField] Vector2 center;
    [SerializeField] Vector2 mapSize;

    private Vector3 originPos;

    // Start is called before the first frame update
    void Start()
    {
        //size�� ���� ���μ��� ���
        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;
    }

    void FixedUpdate()
    {
        UpdateCameraMove();
    }

    void UpdateCameraMove()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + cameraPosition, Time.deltaTime * cameraMoveSpeed);

        float t_x = mapSize.x - width; //���� ���� ���� ���ϱ�
        float clamp_x = Mathf.Clamp(transform.position.x, -t_x + center.x, t_x + center.x);

        float t_y = mapSize.y - height; //���� ���� ���� ���ϱ�
        float clamp_y = Mathf.Clamp(transform.position.y, -t_y + center.y, t_y + center.y);

        transform.position = new Vector3(clamp_x, clamp_y, -5f); //ī�޶� ����
        originPos = new Vector3(clamp_x, clamp_y, -5f);
    }

    public IEnumerator CameraShakeCo(float amount, float duration)
    {
        float timer = 0;
        while(timer < duration)
        {
            transform.position = (Vector3)Random.insideUnitCircle * amount + originPos;

            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = originPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, mapSize * 2);
    }
}
