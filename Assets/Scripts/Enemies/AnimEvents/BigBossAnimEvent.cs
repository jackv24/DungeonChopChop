using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBossAnimEvent : MonoBehaviour {

    private BigBossAttack bigBoss;

    void Start()
    {
        bigBoss = GetComponentInParent<BigBossAttack>();
    }

    public void Spread()
    {
        bigBoss.SpreadAttack();
    }

    public void Sweep()
    {
        bigBoss.SweepAttack();
    }

    public void RecoveredFromKnockout()
    {
        bigBoss.RecoveredFromKnockout();
    }
}
