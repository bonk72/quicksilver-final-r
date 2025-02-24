using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerGold : MonoBehaviour
{
    public static int Gold;
    public TMP_Text goldAmnt;
    private bool canPurchase;
    // Start is called before the first frame update
    void Start()
    {
        goldAmnt.text = Gold.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void gainGold(int amnt){
        Gold += amnt;
        goldAmnt.text = Gold.ToString();
    }
    public void loseGold(int amnt){
        if (Gold - amnt >= 0){
            Gold-= amnt;
            goldAmnt.text = Gold.ToString();
        }

    }
}
