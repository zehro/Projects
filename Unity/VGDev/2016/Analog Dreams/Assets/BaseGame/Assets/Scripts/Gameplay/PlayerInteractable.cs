using UnityEngine;
using System.Collections;

public interface PlayerInteractable
{
    void interact(int data);
    bool isInteractable();
}