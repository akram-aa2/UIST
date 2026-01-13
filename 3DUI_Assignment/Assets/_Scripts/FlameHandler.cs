using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //ToDo: when the lighter flame collides with a candle, activate the candle's flame
        if (other.CompareTag("Candle"))
        {
            Transform candleFlame = other.transform.GetChild(0);
            if (candleFlame != null)
            {
                candleFlame.gameObject.SetActive(true);
            }
        }
    }
}
