using System;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public Transform characters;

    private int currentCharacter;
    private int characterCount;

    private void Start()
    {
        characterCount = characters.childCount;
    }

    public void CycleCharacter()
    {
        characters.GetChild(currentCharacter).gameObject.SetActive(false);
        currentCharacter = (currentCharacter + 1) % characterCount;
        characters.GetChild(currentCharacter).gameObject.SetActive(true);
    }
}