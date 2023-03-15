using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject menuPanel;
    public Button dayBtn;
    public Button nightBtn;
    public Button exitBtn;

    private void Start() {
        dayBtn.onClick.AddListener(delegate {ChangeScene("Day"); });
        nightBtn.onClick.AddListener(delegate {ChangeScene("Night"); });

        SceneManager.LoadScene("Night", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Day"));
        exitBtn.onClick.AddListener(() => Application.Quit());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuPanel.gameObject.SetActive (!menuPanel.gameObject.active);
        }
    }

    void ChangeScene(string scene) {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
        menuPanel.gameObject.SetActive (false);
    }
}
