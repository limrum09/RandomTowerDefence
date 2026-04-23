using UnityEngine;

public class NewEmptyCSharpScript : MonoBehaviour
{
    private void Start()
    {
        string format = "용인 타워 보유: <color=#FF4545>[{0} / {1} / {2} / {3} / {4} / {5}]</color>";

        string result = string.Format(format, 10, 20, 30, 40, 50, 60);

        Debug.Log(result);
    }
}
