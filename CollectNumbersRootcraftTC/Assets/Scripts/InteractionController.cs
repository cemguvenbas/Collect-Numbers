using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    bool canClick = true;
    [SerializeField] float clickCooldown;

    public static InteractionController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }

    public bool TryClick()
    {
        if (canClick)
        {
            StartCoroutine(ClickCooldown());
            return true;
        }

        return false;
    }

    private IEnumerator ClickCooldown()
    {
        canClick = false;
        yield return new WaitForSecondsRealtime(clickCooldown);
        canClick = true;
    }
}
