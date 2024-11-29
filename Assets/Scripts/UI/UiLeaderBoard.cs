using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLeaderBoard : MonoBehaviour
{
    [SerializeField] private List<User> m_UsersLeaderBoard;
    [SerializeField] private Button m_CloseButton;
    void Start()
    {
        m_CloseButton.onClick.AddListener(ClickCloseButton);
    }

    void Update()
    {
        
    }

    private void ClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}
