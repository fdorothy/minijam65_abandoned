using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public Player player;
    public LookAt arrowHint;
    public TextManager textManager;
    public AudioSource music;
    public AudioSource coin;

    public int coins = 0;
    public int totalCoins = 0;

    public float gameStartTime = 0.0f;

    protected DungeonRoom startRoom;

    public static Game singleton;

    public Fungus.Flowchart flowChart;

    protected List<string> introText = new List<string>()
    {
        "Why is there so much junk on the stairs, <i>child</i>?",
        "Pick up all the <b>coins</b>!",
        "Don't fall off and break your head open again. Those medical bills are too much.",
        "Remember, I'm <i>always</i> watching you.",
        "Use the <b>arrow keys</b> or <b>wasd</b> to move, and <b>spacebar</b> to jump"
    };

    protected List<string> failureText = new List<string>()
    {
        "Only <b>xxx</b> coins? And in <b>ttt</b> seconds? Pathetic, child. When I was your age I could have at least picked up <b>yyy</b> coins.",
        "Try again, except this time try to pick up coins instead of bringing shame to this family.",
        "I want those stairs clean!"
    };

    protected List<string> successText = new List<string>()
    {
        "What is this? You got all <b>xxx</b> coins in <b>ttt</b> seconds?",
        "Always doing the bare minimum, I see. Your brother would have done better.",
        "I guess that's enough for now. I'll let you get back to cooking my dinner.",
        "Game over.\nFollow me on @redmountainman1 or https://fredric.itch.io"
    };

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator.OnReady += OnReady;
        singleton = this;
        NewLevel();
        //textManager.ShowText(introText, () => {
        //});
    }

    void NewLevel()
    {
        mapGenerator.PopulateMap();
        gameStartTime = Time.time;
    }

    void OnReady()
    {
        Debug.Log("starting level");
        List<DungeonRoom> rooms = mapGenerator.dungeonGenerator.Rooms;
        rooms.ForEach(room =>
        {
            if (room.IsStartRoom)
            {
                Debug.Log("Found starting room");
                startRoom = room;
                PlaceInRoomCenter(startRoom);
            }
            if (room.IsEndRoom)
            {
                arrowHint.target = mapGenerator.goal.transform;
            }

        });

        flowChart.ExecuteBlock("FadeIn");
    }

    public void PlaceInRoomCenter(DungeonRoom room)
    {
        player.Reset(mapGenerator.ToWorld(room.Center) + Vector3.up);
    }

    public Vector3 swapYZ(Vector3 pt) => new Vector3(pt.x, pt.z, pt.y);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Lose();
        }
    }

    public void Win()
    {
        flowChart.ExecuteBlock("FadeOut");
        Invoke("DoWin", 1.0f);
    }

    public void DoWin()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Finale");
    }

    public void Lose()
    {
        flowChart.ExecuteBlock("FadeOut");
        Invoke("DoReset", 1.0f);
    }

    public void DoReset()
    {
        PlaceInRoomCenter(startRoom);
        Invoke("DoFadeIn", 0.5f);
    }

    public void DoFadeIn()
    {
        flowChart.ExecuteBlock("FadeIn");
    }
}
