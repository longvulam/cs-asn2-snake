using System;
using UnityEngine;

internal class PlayerState
{
    public string Id;
    public Vector3 position;

    public PlayerState(Guid id, Vector3 pos)
    {
        Id = id.ToString();
        position = pos;
    }

}
