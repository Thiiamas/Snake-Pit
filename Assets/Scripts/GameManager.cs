using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;

public class GameManager : MonoBehaviour
{

    public GameObject _appleSpawner;
    public Vector2 _horizontalPoint;
    public Vector2 _verticalPoint;

    public Apple CurrentApple;
    public GameObject _snake;
    public Camera _snakeCam;

    public TwitchIRC IRC;
    public Scoreboard scoreboard;

    public Chatter latestChatter;
    public List<Score> _scores = new List<Score>();

    // Start is called before the first frame update
    void Start()
    {
        IRC.newChatMessageEvent.AddListener(NewMessage);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewMessage(Chatter chatter)
    {
        Debug.Log(
            "<color=cyan>New chatter object received!</color>"
            + " Chatter's name: " + chatter.tags.displayName
            + " Chatter's message: " + chatter.message);

        // Here are some examples on how you could use the chatter objects...

        if (chatter.tags.displayName == "Lexone")
            Debug.Log("Chat message was sent by Lexone!");

        if (chatter.HasBadge("subscriber"))
            Debug.Log("Chat message sender is a subscriber");

        if (chatter.HasBadge("moderator"))
            Debug.Log("Chat message sender is a channel moderator");

        if (chatter.MessageContainsEmote("25")) //25 = Kappa emote ID
            Debug.Log("Chat message contained the Kappa emote");

        if (chatter.message == "!join")
            Debug.Log(chatter.tags.displayName + " said !join");

        // Get chatter's name color (RGBA Format)
        Color nameColor = chatter.GetRGBAColor();

        // Check if chatter's display name is "font safe"
        //
        // Most fonts don't support unusual characters
        // If that's the case then you could use their login name instead (chatter.login) or use a fallback font
        // Login name is always lowercase and can only contain characters: a-z, A-Z, 0-9, _
        if (chatter.IsDisplayNameFontSafe())
            Debug.Log("Chatter's displayName is font-safe (only characters: a-z, A-Z, 0-9, _)");


        // Save latest chatter object
        // This is just to show how the Chatter object looks like inside the Inspector
        latestChatter = chatter;
    }

    public bool hasScoreName(string name)
    {
        foreach (Score score in _scores)
        {
            if (score.name.Equals(name))
            {
                return true;
            }
        }
        return false;
    }

    public void resetScore(string name)
    {
        foreach (Score score in _scores)
        {
            if (score.name.Equals(name))
            {
                score.point = 0;
            }
        }
    }

    public void SpawnSnake(string name, Color color)
    {
        GameObject snake = Instantiate(_snake, this.transform.parent.transform, false);
        Debug.Log("Here");
        SnakeMultiClassic snakeScript = snake.GetComponent<SnakeMultiClassic>();
        snakeScript._spawner = _appleSpawner.GetComponent<AppleSpawner>();
        snakeScript._gameManager = this;

        //Set color
        snakeScript._color = color;
        CameraSensorComponent camSensor = snakeScript.GetComponent<CameraSensorComponent>();
        camSensor.Camera = _snakeCam;

        //set score
        if (!hasScoreName(name))
        {
            _scores.Add(new Score(name, 0));
        }
        else
        {
            resetScore(name);
        }
        Debug.Log("spawn");
    }

    public void SpawnSnake()
    {
        SpawnSnake("thiiamas", new Color(0, 0, 0));
        // GameObject snake = Instantiate(_snake,this.transform.parent.transform,false);
        // Debug.Log("Here");
        // SnakeMultiClassic snakeScript = snake.GetComponent<SnakeMultiClassic>();
        // snakeScript._spawner = _appleSpawner.GetComponent<AppleSpawner>();
        // CameraSensorComponent camSensor = snakeScript.GetComponent<CameraSensorComponent>();
        // print(camSensor);
        // camSensor.Camera = _snakeCam;
        // Debug.Log("spawn");
    }
}
