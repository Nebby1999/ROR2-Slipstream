﻿using BepInEx;
using BepInEx.Configuration;
using HG.Reflection;
using Slipstream.Modules;
using Moonstorm;
using Slipstream.Items;
using Slipstream.Scenes;
using Slipstream.Utils;
using R2API;
using R2API.ContentManagement;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using R2API.Utils;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618
[module: UnverifiableCode]
[assembly: SearchableAttribute.OptIn]


namespace Slipstream
{
    //Not all modules are implemented but they will be. If you are working on something make sure to reference off of Lost In Transit.

    //Dependencies
    [BepInDependency("com.bepis.r2api.content_management")]
    [BepInDependency("com.bepis.r2api.dot")]
    [BepInDependency("com.bepis.r2api.damagetype")]
    [BepInDependency("com.bepis.r2api.deployable")]
    [BepInDependency("com.bepis.r2api.networking")]
    [BepInDependency("com.bepis.r2api.prefab")]
    [BepInDependency("com.bepis.r2api.recalculatestats")]
    [BepInDependency("com.bepis.r2api.sound")]
    [BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.TheMysticSword.AspectAbilities", BepInDependency.DependencyFlags.SoftDependency)]

    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(GUID, MODNAME, VERSION)]
    //[R2APISubmoduleDependency(nameof(DotAPI), nameof(DamageAPI), nameof(PrefabAPI), nameof(DeployableAPI), nameof(SoundAPI))]
    public class SlipMain : BaseUnityPlugin
    {
        //Big credit to Starstorm/Lost in Transit devs Swuff and NebNeb, the structure of the mod is purely based off of LIT and without them I wouldn't know what to do.
        //To Swuff and Neb: Don't get made at me for this ^ lol, but seriously if you have any problems with Slipstream's structure being extremely based off of LIT please let me know. -JaceDaDorito

        //Strings for mod details. Very self explainitory.
        internal const string GUID = "com.TeamSlipstream.Slipstream";
        internal const string MODNAME = "Slipstream";
        internal const string VERSION = "0.0.1";

        public static SlipMain instance;

        public static PluginInfo pluginInfo;

        public static ConfigFile config;

        public static bool DEBUG = false;

        public void Awake()
        {
            //Establishes an instance of the mod, config, and console logger.
            instance = this;
            pluginInfo = Info;
            config = Config;
            SlipLogger.logger = Logger;

            new SlipConfig().Init();
            new SlipAssets().Init();
            new SlipContent().Init();
            new SlipLanguage().Init();

            //new CriticalShield().Init();
            new SlipCriticalShield().Init();
            new VoidShieldCatalog().Init();
            new ExpansionUtils().Init();
            //new SlipDirectorUtilsHandler().Init();

            //Allows organized configurable fields of public static fields.
            ConfigurableFieldManager.AddMod(this);

            //Slipstream now loads alongside the game "and will properly show percentage increase in the loading screen" -Neb
        }


    }
}