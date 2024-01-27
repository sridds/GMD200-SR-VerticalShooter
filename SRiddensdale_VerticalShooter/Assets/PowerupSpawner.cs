using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : ObjectSpawner
{
    protected override bool CanTick()
    {
        if (GameManager.instance.ActivePlayer.ActivePowerup != null) return false;

        return true;
    }
}
