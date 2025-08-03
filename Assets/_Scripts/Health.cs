using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHits = 3;
    private int hit;

    private bool dead;

    public void GetHit()
    {
        if(dead) return;
        
        hit++;
        
        if(hit >= maxHits){
            GameManager.Instance.GameOver();
            dead = true;
        }
    }
}