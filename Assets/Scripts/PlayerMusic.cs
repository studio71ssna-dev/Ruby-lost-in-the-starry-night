using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerMusic : MonoBehaviour
{
    private void OnEnable()
    {
        InputHandler.Instance.OnLane1 += Lane1;
        InputHandler.Instance.OnLane2 += Lane2;
        InputHandler.Instance.OnLane3 += Lane3;
        InputHandler.Instance.OnFret1 += Fret1;
        InputHandler.Instance.OnFret2 += Fret2;
        InputHandler.Instance.OnFret3 += Fret3;
    }

    private void OnDisable()
    {
        InputHandler.Instance.OnLane1 -= Lane1;
        InputHandler.Instance.OnLane2 -= Lane2;
        InputHandler.Instance.OnLane3 -= Lane3;
        InputHandler.Instance.OnFret1 -= Fret1;
        InputHandler.Instance.OnFret2 -= Fret2;
        InputHandler.Instance.OnFret3 -= Fret3;
    }

    private void Lane1()
    {
        Debug.Log("Lane 1 Triggered!");
    }
    private void Lane2()
    {
        Debug.Log("Lane 2 Triggered!");
    }
    private void Lane3()
    {
        Debug.Log("Lane 3 Triggered!");
    }

    private void Fret1(bool isPressed)
    {
        if (isPressed) Debug.Log("Fret 1 Pressed!");
        else Debug.Log("Fret 1 Released!");
    }
    private void Fret2(bool isPressed)
    {
        Debug.Log($"Fret 2 {(isPressed ? "Pressed" : "Released")}!");
    }
    private void Fret3(bool isPressed)
    {
        Debug.Log($"Fret 3 {(isPressed ? "Pressed" : "Released")}!");
    }
}
