using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private Camera m_mainCam;
    [SerializeField] private Text timer;
    [SerializeField] private Text score;
    [SerializeField] private GameObject m_moleTarget;
    [SerializeField] private GameObject m_moleTargetL2;
    [SerializeField] private GameObject m_moleTargetL3;
    [SerializeField] private List<GameObject> m_moleTargets;
    [SerializeField] private List<Vector3> holePosList;
    [SerializeField] private GameObject holes;
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private Animator m_animator;
    private GameObject currentMole;
    private Ray m_ray;
    private RaycastHit m_hit;
    private string m_hited;
    private bool m_hitedObj;
    private float m_lastTarget = -11f;
    private float m_mulFactor = -3f;
    private float timeRemaining = 60;
    private int currentScore;
    
    void Start()
    {
        Application.targetFrameRate = 33; //so fps would be around 32
        m_moleTargets = new List<GameObject>();
        holePosList =  new List<Vector3>();

        //create positions list from holes' positions
        foreach(Transform hole in holes.transform)
        {
            holePosList.Add(hole.transform.position);
        }

        currentScore = 0;
    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;
        if(timeRemaining <= 1)
        {
            gameOverScene();
        }

        timer.text = "0:" + Mathf.FloorToInt(timeRemaining);
        score.text = "" + currentScore;

        if(m_lastTarget-Time.fixedTime < m_mulFactor)
        {
            if (holePosList.Count > 0)
            {
                Vector3 randomHole = holePosList[Random.Range(0, holePosList.Count)]; //choose random hole position
                holePosList.Remove(randomHole); //remove from list so 2 moles won't spawn in the same hole
                m_lastTarget =  Time.fixedTime;

                //generate random number to calculate which mole type to spawn
                int randNum = Random.Range(1, 9);
                if(randNum%3 == 0)
                {
                    currentMole = Instantiate(m_moleTargetL2, new Vector3(randomHole.x, -2.5f, randomHole.z), Quaternion.identity);
                } else if(randNum%5 == 0)
                {
                    currentMole = Instantiate(m_moleTargetL3, new Vector3(randomHole.x, -2.5f, randomHole.z), Quaternion.identity);
                } else
                {
                    currentMole = Instantiate(m_moleTarget, new Vector3(randomHole.x, -2.5f, randomHole.z), Quaternion.identity);
                }
                currentMole.transform.LookAt(m_mainCam.transform);
                m_moleTargets.Add(currentMole);          
            }
        }
        if(currentMole!=null)
            currentMole.transform.position = Vector3.MoveTowards(currentMole.transform.position , new Vector3(currentMole.transform.position.x, -1.1f, currentMole.transform.position.z) , 5f*Time.deltaTime);
            if(m_moleTargets.Count > 1)
            {
                if(m_moleTargets[m_moleTargets.Count-2] != null)
                {
                    m_moleTargets[m_moleTargets.Count-2].transform.position = Vector3.MoveTowards(m_moleTargets[m_moleTargets.Count-2].transform.position , new Vector3(m_moleTargets[m_moleTargets.Count-2].transform.position.x, -2.7f, m_moleTargets[m_moleTargets.Count-2].transform.position.z) , 5f*Time.deltaTime);
                    
                    if(m_moleTargets[m_moleTargets.Count-2].transform.position.y < -2.6)
                    {
                            holePosList.Add(m_moleTargets[m_moleTargets.Count-2].transform.position);
                            Destroy(m_moleTargets[m_moleTargets.Count-2]);
                    }
                }
            }
        if (Input.anyKeyDown)
        {
            m_animator.SetTrigger("hit");
            hitSound.Play();
            m_ray = m_mainCam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            if (Physics.Raycast(m_ray, out m_hit))
            {
                if (m_hit.transform != null)
                {
                    m_hited = m_hit.transform.name.ToLower();
                    m_hitedObj = true;
                }
            }
            if (m_hitedObj)
            {
                m_hitedObj = false;
                if (m_hited.Contains("mole"))
                {
                    if(m_hited.Contains("mole3"))
                    {
                        Destroy(m_hit.transform.Find("lives").transform.Find("life3").gameObject);
                        m_hit.transform.name = "mole2";
                    } else if(m_hited.Contains("mole2"))
                    {
                        Destroy(m_hit.transform.Find("lives").transform.Find("life2").gameObject);
                        m_hit.transform.name = "mole";
                    } else {
                        destroyMole();
                    }
                }
            }
        }
    }

    void destroyMole()
    {
        deathSound.Play();
        currentScore++;
        holePosList.Add(m_hit.transform.position); //add position back to position list so new moles can spawn there
        Destroy(m_hit.transform.gameObject);
        m_mulFactor *= 0.95f;
    }

    void gameOverScene()
    {
        PlayerPrefs.SetInt("currentScore", currentScore);
        PlayerPrefs.SetInt("addedCurrent", 0);
        SceneManager.LoadScene("EndScene");
    }
}
