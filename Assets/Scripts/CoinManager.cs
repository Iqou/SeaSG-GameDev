using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CoinManager : MonoBehaviour
{
    public int coinCount;
    public TextMeshProUGUI coinText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         coinText.text  = coinCount.ToString();
    }
}
