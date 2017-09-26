using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Maze MazePrefab;
    public GameObject Player;
    public bool UseSeed;
    public string Seed;

    private Maze mazeInstance;

    private void Start()
    {
        BeginGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    private void BeginGame()
    {
        mazeInstance = Instantiate(MazePrefab) as Maze;
        int seed = Random.Range(0, 99999999);
        if (UseSeed) seed = Seed.GetHashCode();
        StartCoroutine(mazeInstance.Generate(seed));
    }

    private void RestartGame()
    {
        StopAllCoroutines();
        Destroy(mazeInstance.gameObject);
        BeginGame();
    }
}
