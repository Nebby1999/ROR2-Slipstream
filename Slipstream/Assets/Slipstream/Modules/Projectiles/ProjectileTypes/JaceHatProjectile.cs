﻿using Moonstorm;
using R2API;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;


namespace Slipstream.Projectiles
{
    [DisabledContent]
    public class JaceHatProjectile : ProjectileBase
    {
        //public override GameObject ProjectilePrefab { get; set; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("projJaceHat");
        public override GameObject ProjectilePrefab { get; set; } = PrefabAPI.InstantiateClone(SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("projJaceHat"), "HatProjectile", false);
        public static GameObject hatProj;

        //var model = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("projJaceHat");
        //model.
        public override void Initialize()
        {
            if (ProjectilePrefab)
            {
                hatProj = ProjectilePrefab;
            }
        }
    }

}