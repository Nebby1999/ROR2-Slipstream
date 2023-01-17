﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using Slipstream;
using Slipstream.Buffs;
using AK;
using Slipstream.Orbs;
using RoR2.Orbs;

public class DieOnCollision : MonoBehaviour
{
    private CharacterBody body;
    public CharacterBody attackerBody;
    private bool collided = false;

    private void Start()
    {
        body = gameObject.GetComponent<CharacterBody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        SlipLogger.LogD(body.name + " collided with something");
        if(collision.gameObject.layer == LayerIndex.world.intVal && !collided)
        {
            SlipLogger.LogD(body.name + " died");
            collided = true;
            AffixSandswept.FireKBBlast(body);
            if (attackerBody.HasBuff(Grainy.buff))
            {
                SandsweptDeathOrb sandsweptDeathOrb = new SandsweptDeathOrb();
                sandsweptDeathOrb.origin = body.corePosition;
                sandsweptDeathOrb.target = Util.FindBodyMainHurtBox(attackerBody);
                OrbManager.instance.AddOrb(sandsweptDeathOrb);
            }
            body.healthComponent.Suicide(attackerBody?.gameObject);
            
        }
    }
}