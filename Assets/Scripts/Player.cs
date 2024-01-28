using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 5.0f; // how fast the player moves
    public int lives = 3; // number of lives remaining
    private int pelletsCollected = 0; // number of pellets the player has collected
    public Image[] lifeSprites = new Image[3]; // sprites showing the number of lives remaining
    public TMP_Text pelletsDisplay; // displays the number of collected pellets in the UI
    public GameObject retryMessage; // text that appears when the player loses a life
    public GameObject winPanel; // panel that appears if the player wins
    public GameObject losePanel; // panel that appears if the player loses
    public Ghost[] ghosts = new Ghost[4]; // the ghosts
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource sfx;
    [SerializeField] AudioClip ghostSound;
    bool super;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        retryMessage.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var y = Input.GetAxisRaw("Vertical");

        if (x != 0) // if the player is pressing left or right
        {
            rb.velocity = Vector3.right * x * speed; //move horizontally
        }
        else if (y != 0) // if the player is pressing up or down
        {
            rb.velocity = Vector3.forward * y * speed; // move vertically
        }
        
        // if the player leaves the maze on the left or right, they wrap around to the other side
        if (gameObject.transform.position.x >= 8.5)
        {
            gameObject.transform.SetPositionAndRotation(new Vector3(-8.25f, 0, 0), Quaternion.identity);
        }
        if (gameObject.transform.position.x <= -8.5)
        {
            gameObject.transform.SetPositionAndRotation(new Vector3(8.25f, 0, 0), Quaternion.identity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Pellet") // if the player touches a pellet
        {
            // destroy the pellet, increase the score by one, and display the score
            Destroy(other.gameObject);
            pelletsCollected++;
            pelletsDisplay.text = $"{pelletsCollected}/176";
            // if every pellet is collected, show the win panel and stop the game
            if (pelletsCollected == 176)
            {
                winPanel.SetActive(true);
                Time.timeScale = 0;
            }
        }
        else if (other.GetComponent<Collider>().tag == "Ghost") // if the player touches a ghost
        {
            if(super)
            {
                Destroy(other.gameObject);
                sfx.PlayOneShot(ghostSound);
            }
            else
            {
                lives--; // the player loses a life
                if (lives > 0 && lives < 3)
                {
                    // if the player has one or two lives left, display the remaining lives in the UI, then display a message telling the player to click the screen
                    lifeSprites[lives].enabled = false;
                    retryMessage.SetActive(true);
                }
                else if (lives == 0)
                {
                    // if the player has no lives left, show the game over panel
                    losePanel.SetActive(true);
                }
                Time.timeScale = 0; // stop the player and ghosts
            }
        }
        else if(other.gameObject.name == "Powerup")
        {
            StartCoroutine(Powerup());
        }
    }

    // resets the player and ghost's positions without changing the score or pellets
    public void OnClickRespawn()
    {
        Time.timeScale = 1;
        retryMessage.SetActive(false);
        gameObject.transform.SetPositionAndRotation(new Vector3(0, 0, -4.5f), Quaternion.identity);
        for (int i = 0; i < 4; i++)
        {
            if (ghosts[i].active)
            {
                ghosts[i].gameObject.transform.SetPositionAndRotation(new Vector3(0, 0, 1.5f), Quaternion.identity);
            }
        }
    }
    
    // disables, then re-enables powerup
    IEnumerator Powerup()
    {
        super = true;
        music.pitch = 2;
        GameObject powerup = GameObject.Find("Powerup");
        powerup.SetActive(false);
        yield return new WaitForSeconds(12);
        super = false;
        music.pitch = 1;
        yield return new WaitForSeconds(Random.Range(0, 12));
        powerup.SetActive(true);
    }
    // resets the scene
    public void OnClickRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    // ends the game
    public void OnClickQuitButton()
    {
        Application.Quit();
    }
}
