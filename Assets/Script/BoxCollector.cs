using UnityEngine;
using UnityEngine.UI;

public class BoxCollector : MonoBehaviour
{
    public Text countText; // UI �ؽ�Ʈ
    public TriggerDoorController door; // ������ ��
    private int count; // ������ �ڽ� ��

    public GameObject endPannel;

    void Start()
    {
        count = 0;
        UpdateCountText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GiftBox"))
        {
            other.gameObject.SetActive(false); // �ڽ� ��Ȱ��ȭ
            count++;
            UpdateCountText();

            if (count == 5) // ��� �ڽ��� �����ߴٸ�
            {
                door.OpenDoor();
            }
        }
        else if (other.gameObject.CompareTag("End"))
        {
            endPannel.SetActive(true);
        }
    }

    void UpdateCountText()
    {
        countText.text = "���� ���� ����: " + (5 - count).ToString();
    }
}
