using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class GetPlayerAction : ActionTask {

    public BBParameter<List<PlayerInformation>> players;

    protected override void OnExecute()
    {
        players = GameManager.Instance.players;

        if (players.value.Count > 0)
            EndAction(true);
        else
            EndAction(false);
    }
}
