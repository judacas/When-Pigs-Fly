using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUI : MonoBehaviour
{
    GameObject won, lost, lives, levels, enter, title, help;
    float timeStarted;
    // Start is called before the first frame update
    void Start()
    {
        won = transform.Find("YouWonAnim").gameObject;
        lost = transform.Find("YouLostAnim").gameObject;
        lives = transform.Find("Lives").gameObject;
        levels = transform.Find("Levels").gameObject;
        enter = transform.Find("Enter").gameObject;
        title = transform.Find("TitleScreenAnim").gameObject;
        help = transform.Find("ControlsMenu").gameObject;
        won.SetActive(false);
        lost.SetActive(false);
        lives.SetActive(true);
        levels.SetActive(true);
        enter.SetActive(false);
        title.SetActive(true);
        help.SetActive(true);
        timeStarted = Time.time;
    }



    public void Won()
    {
        lives.SetActive(false);
        levels.SetActive(false);
        won.SetActive(true);
        enter.SetActive(true);
    }
    public void Lost()
    {
        lives.SetActive(false);
        levels.SetActive(false);
        lost.SetActive(true);
        enter.SetActive(true);
    }

    public void UpdateLevel(int lvl)
    {
        levels.SetActive(true);
        levels.GetComponent<Level>().level = lvl;
    }

    public void UpdateLives(int newLives)
    {
        lives.SetActive(true);
        lives.GetComponent<UILives>().lives = newLives;
    }

    public void Playing(){
        won.SetActive(false);
        lost.SetActive(false);
        lives.SetActive(true);
        levels.SetActive(true);
        enter.SetActive(false);
        title.SetActive(true);
        help.SetActive(true);
    }

    public void Help(bool set)
    {
        help.SetActive(set);
    }
    public void Title(bool set){
        title.SetActive(set);
    }

}
