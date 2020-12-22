using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private Camera m_mainCam;
    [SerializeField] private GameObject howToPlay;
    [SerializeField] private GameObject highScores;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private AudioSource music;
    private Ray m_ray;
    private RaycastHit m_hit;
    private string m_hited;
    private bool   m_hitedObj;

    void Start()
    {
        music.Play();
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            m_ray = m_mainCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            if(Physics.Raycast(m_ray,out m_hit))
            {
                if (m_hit.rigidbody != null)
                {
                    m_hited = m_hit.rigidbody.transform.name.ToLower();
                    m_hitedObj = true;
                }
            }
            if(m_hitedObj)
            {
                m_hitedObj = false;
                switch (m_hited)
                {
                    case "play":
                        SceneManager.LoadScene("GameScene");
                        break;
                    case "howtoplay":
                        howToPlay.SetActive(true);
                        mainUI.SetActive(false);
                        break;
                    case "x_btn_htp":
                        mainUI.SetActive(true);
                        howToPlay.SetActive(false);
                        break;
                    case "highscores":
                        highScores.SetActive(true);
                        mainUI.SetActive(false);
                        break;
                    case "x_btn_hs":
                        mainUI.SetActive(true);
                        highScores.SetActive(false);
                        break;
                    case "exit":
                        Application.Quit();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
