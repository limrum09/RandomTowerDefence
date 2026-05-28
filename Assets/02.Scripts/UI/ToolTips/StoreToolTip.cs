using Unity.VisualScripting;
using UnityEngine;

public class StoreToolTip : MonoBehaviour
{
    private void Start()
    {
        Hide();
    }

    private void SetPosition()
    {
        Vector2 pos = Input.mousePosition;

        pos += new Vector2(25f, 25f);

        transform.position = pos;
    }

    public void Show(StoreProduct product)
    {
        gameObject.SetActive(true);




        SetPosition();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
