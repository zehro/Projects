using UnityEngine;
using System.Collections;

public class PauseButton : PhysicalButton
{

    public float raisedHeight = 1f;

    private Vector3 initialPos;

    new void Start()
    {
        base.Start();
        initialPos = transform.localPosition;
    }

    protected override void HandleHover()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialPos + Vector3.up * raisedHeight, Time.deltaTime * 15f);
        base.HandleHover();
    }

    protected override void HandleNormal()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialPos + Vector3.up, Time.deltaTime * 15f);
        base.HandleNormal();
    }

    protected override void HandlePressed()
    {
        base.HandlePressed();
    }
}