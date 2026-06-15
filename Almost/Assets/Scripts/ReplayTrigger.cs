using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayTrigger : MonoBehaviour
{
    public void ReplayGame()
    {
        GameManager.instance.LoadScene("Prototype");
    }
}
