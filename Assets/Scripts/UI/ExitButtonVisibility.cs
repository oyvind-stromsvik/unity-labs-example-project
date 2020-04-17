using UnityEngine;

public class ExitButtonVisibility : MonoBehaviour {

    void Awake()
    {
        if (Application.isConsolePlatform || Application.isMobilePlatform || Application.isWebPlayer)
            gameObject.SetActive(false);
    }
}
