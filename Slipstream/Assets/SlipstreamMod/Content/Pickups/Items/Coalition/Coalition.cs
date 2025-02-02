﻿using Moonstorm;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using TMPro;
using UnityEngine.UI;
using Slipstream.Utils;
using System.Collections.Generic;

namespace Slipstream.Items
{
    public class Coalition : ItemBase
    {
        private const string token = "SLIP_ITEM_COALITION_DESC";
        public override ItemDef ItemDef { get; } = SlipAssets.Instance.MainAssetBundle.LoadAsset<ItemDef>("Coalition");

        [ConfigurableField(ConfigName = "Allies are immunte to void death", ConfigDesc = "Prevents instant void explosion deaths (like the Void Reaver explosion) for allies.", ConfigSection = "Coalition")]
        //[TokenModifier(token, StatTypes.Default, 0)]
        public static bool preventsVoidDeath = true;

        [ConfigurableField(ConfigName = "Initial health threshold/Hyperbolic Scaling", ConfigDesc = "Initial health threshold percentage at one stack and hyperbolic staling", ConfigSection = "Coalition")]
        [TokenModifier(token, StatTypes.MultiplyByN, 0, "100")]
        public static float amplificationPercentage = 0.25f;

        [ConfigurableField(ConfigName = "Armor increase per stack", ConfigDesc = "Armor increase applied to allies per stack", ConfigSection = "Coalition")]
        [TokenModifier(token, StatTypes.Default, 1)]
        public static float armorIncrease = 100f;

        public static RoR2.UI.HealthBarStyle.BarStyle coalitionBarStyle;

        public class CoalitionIntervalsBetweenDeaths : MonoBehaviour
        {
            public List<CharacterBody> deathList = new List<CharacterBody>();
            private Run.FixedTimeStamp timer;
            private static float totalPhaseTime = 2f;
            private static float pauseTime = 0.5f;
            private float intervalTime;
            private int index = 0;
            private int deathIndex = 0;
            private void Start()
            {
                
                timer = Run.FixedTimeStamp.now;
                intervalTime = totalPhaseTime / deathList.Count;
                SlipLogger.LogD("Timer on coalition started" + intervalTime);

            }

            private void FixedUpdate()
            {
                if(timer.timeSince > 0)
                {
                    if (timer.timeSince <= totalPhaseTime)
                    {
                        if (timer.timeSince >= index * intervalTime)
                        {
                            //put spawn effect deathList[index] here
                            SlipLogger.LogD("index added");
                            index++;
                        }
                    }
                    else if (timer.timeSince <= totalPhaseTime + pauseTime)
                    {
                        return;
                    }
                    else if (timer.timeSince <= totalPhaseTime * 2f + pauseTime)
                    {
                        if (timer.timeSince >= deathIndex * intervalTime + totalPhaseTime + pauseTime)
                        {
                            
                            deathList[0].healthComponent.Suicide();
                            deathList.RemoveAt(0);
                            deathIndex++;
                        }
                    }
                    else if (timer.timeSince >= totalPhaseTime * 2f + pauseTime)
                    {
                        SlipLogger.LogD("deathcomponent destroyed");
                        Destroy(this);
                        
                    }
                }
            }

        }
        public class CoalitionBehavior : BaseItemBodyBehavior
        {
            [ItemDefAssociation(useOnClient = false, useOnServer = true)]
            public static ItemDef GetItemDef() => SlipContent.Items.Coalition;

            private GameObject destroyEffectPrefab;
            private List<CharacterBody> deathList = new List<CharacterBody>();


            private void Start()
            {
                destroyEffectPrefab = SlipAssets.Instance.MainAssetBundle.LoadAsset<GameObject>("CoalitionPreDetonation");

                //Adds Blackhealth item to all your allies
                CharacterMaster master = body.master;
                if (master)
                {
                    foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
                    {
                        if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == master)
                        {
                            characterMaster.inventory.GiveItem(SlipContent.Items.BlackHealth);
                        }
                    }
                }
            }

            private void OnDestroy()
            {
                //When you get rid of Coalition, get rid of Blackhealth from all allies
                CharacterMaster master = body.master;
                if (master)
                {
                    foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
                    {
                        if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == master)
                        {
                            characterMaster.inventory.ResetItem(SlipContent.Items.BlackHealth);
                        }
                    }
                }
            }

            private void OnEnable()
            {
                MasterSummon.onServerMasterSummonGlobal += MasterSummon_onServerMasterSummonGlobal;
                On.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime;
            }

            private void OnDisable()
            {
                MasterSummon.onServerMasterSummonGlobal -= MasterSummon_onServerMasterSummonGlobal;
                On.RoR2.HealthComponent.UpdateLastHitTime -= HealthComponent_UpdateLastHitTime;
            }
            private void HealthComponent_UpdateLastHitTime(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
            {
                //call orig to make sure that Elixr is activated first. This only matters at 1 stack though.
                orig(self, damageValue, damagePosition, damageIsSilent, attacker);

                //Kill all allies when player is below the current threshold.
                float threshold = Moonstorm.MSUtil.InverseHyperbolicScaling(amplificationPercentage, amplificationPercentage, 1, stack);
                if (body == self.body && (body.healthComponent.health + body.healthComponent.shield)/body.healthComponent.fullCombinedHealth < threshold && body.gameObject.GetComponent<CoalitionIntervalsBetweenDeaths>() == null) //Character body check and ratio of curent health compared to the threshold
                {
                    CharacterMaster master = body.master;
                    CoalitionIntervalsBetweenDeaths deathComponent = body.gameObject.AddComponent<CoalitionIntervalsBetweenDeaths>();
                    foreach (CharacterMaster characterMaster in CharacterMaster.instancesList)
                    {
                        if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == master)
                        {
                            CharacterBody allybody = characterMaster.GetBody();
                            //EffectData effectData = new EffectData { origin = allybody.corePosition, scale = allybody.radius};

                            //EffectManager.SpawnEffect(destroyEffectPrefab, effectData, true);
                            //characterMaster.GetBody()?.healthComponent.Suicide();

                            deathComponent.deathList.Add(allybody);
                        }
                    }
                }
            }

            private void MasterSummon_onServerMasterSummonGlobal(MasterSummon.MasterSummonReport report)
            {
                //If allies are gained while having Coalition, give them Blackhealth
                if (body.master && body.master == report.leaderMasterInstance)
                {
                    CharacterMaster summonMasterInstance = report.summonMasterInstance;
                    if (summonMasterInstance)
                    {
                        summonMasterInstance.inventory.GiveItem(SlipContent.Items.BlackHealth);
                    }
                }
            }

        }

        public class CoalitionBarData : ExtraHealthbarSegment.BarData
        {
            private bool enabled;
            private Material barMat = SlipAssets.Instance.MainAssetBundle.LoadAsset<Material>("matCriticalShield");

            private float threshold;


            public override RoR2.UI.HealthBarStyle.BarStyle GetStyle()
            {
                var style = coalitionBarStyle;
                style.sprite = bar.style.barrierBarStyle.sprite;
                style.sizeDelta = bar.style.barrierBarStyle.sizeDelta;
                style.baseColor = ExtraHealthbarSegment.defaultBaseColor;
                style.imageType = Image.Type.Simple;
                style.enabled = true;

                return style;
            }

            public override void CheckInventory(ref RoR2.UI.HealthBar.BarInfo info, RoR2.CharacterBody body)
            {
                base.CheckInventory(ref info, body);

                //if there are any critical shield items in the inventory, enable healthbar

                //enabled = body.gameObject.GetComponent<ICriticalShield>() != null;

                enabled = false;

                int count = body.GetItemCount(SlipContent.Items.Coalition);
                if (count > 0)
                {
                    enabled = true;
                    threshold = Moonstorm.MSUtil.InverseHyperbolicScaling(amplificationPercentage, amplificationPercentage, 1, count);
                }
            }

            public override void UpdateInfo(ref RoR2.UI.HealthBar.BarInfo info, HealthComponent healthSource)
            {
                //instanceMat = image.canvasRenderer.GetMaterial();

                base.UpdateInfo(ref info, healthSource);

                info.enabled = enabled; //&& healthSource.shield > 0;

                info.normalizedXMin = threshold - 0.2f;
                info.normalizedXMax = threshold + 0.2f;
            }

            public override void ApplyBar(ref RoR2.UI.HealthBar.BarInfo info, Image image, HealthComponent source, ref int i)
            {

                //image.material = barMat;
                base.ApplyBar(ref info, image, source, ref i);
            }


        }
    }
}