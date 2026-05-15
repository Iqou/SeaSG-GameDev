using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    [SerializeField] private bool goNextLevel;
    [SerializeField] string LevelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (goNextLevel)
            {
                Levels.instance.NextLevel();
            }
            else
            {
                Levels.instance.LoadLevel(LevelName);
            }
        }
    }
}
