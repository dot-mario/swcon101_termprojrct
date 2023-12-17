using UnityEngine;
using UnityEngine.UI;

public class BoxCollector : MonoBehaviour
{
    public Text countText; // UI 텍스트
    public TriggerDoorController door; // 열리는 문
    private int count; // 수집한 박스 수

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
            other.gameObject.SetActive(false); // 박스 비활성화
            count++;
            UpdateCountText();

            if (count == 5) // 모든 박스를 수집했다면
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
        countText.text = "남은 선물 상자: " + (5 - count).ToString();
    }
}
