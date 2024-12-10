using System;
using System.Collections;
using UnityEngine;

public class HumanPlayerController : BasePlayerController
{
    public override void MakeMove()
    {
        StartCoroutine(WaitForPlayerInput());
    }

    private IEnumerator WaitForPlayerInput()
    {
        bool moveMade = false;

        while (!moveMade)
        {
            if (Input.GetMouseButtonDown(0))
            {
                print("hi");
            }
            yield return null;
        }
    }
}

