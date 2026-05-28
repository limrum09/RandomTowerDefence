using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SignInBase : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI messageText;

    protected EventSystem system;

    private void Awake()
    {
        system = EventSystem.current;
    }

    protected void ResetUI(params Image[] images)
    {
        messageText.text = string.Empty;

        for(int i = 0; i < images.Length; i++)
        {
            images[i].color = Color.white;
        }
    }

    protected bool CheckFormatEmail(TMP_InputField email)
    {
        if (!email.text.Contains("@"))
        {
            GuideForIncorrectlyEnteredData(email.image, "이메일 형식이 아닙니다.");
            return false;
        }

        return true;
    }

    protected void GuideForIncorrectlyEnteredData(Image image, string msg)
    {
        messageText.text = msg;
        image.color = Color.red;
    }

    protected bool IsFieldDataEmpty(Image image, TMP_InputField field, string result)
    {
        if (string.IsNullOrWhiteSpace(field.text))
        {
            GuideForIncorrectlyEnteredData(image, $"{result} 값을 채워주세요.");
            return false;
        }

        return true;
    }

    protected void SetOnlyMessage(string msg) => messageText.text = msg;
}
