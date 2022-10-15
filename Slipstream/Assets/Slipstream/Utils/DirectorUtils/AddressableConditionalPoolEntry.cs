﻿using Moonstorm;
using UnityEngine.AddressableAssets;
using Slipstream.Buffs;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using RoR2.Items;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using RoR2.ExpansionManagement;
using UnityEngine.Serialization;

namespace Slipstream.Scenes
{
		[Serializable]
		public class AddressableConditionalPoolEntry : DccsPool.ConditionalPoolEntry, IAddressableKeyProvider<DirectorCardCategorySelection>, IAddressableKeyArrayProvider<ExpansionDef>
		{
			[Tooltip("An optional addressable key to load a vanilla dccs")]
			public string dccsKey;
			[Tooltip("Optional addressable keys to load vanilla expansions as additional required expansions")]
			[SerializeField]
			[SerializeReference]
			public string[] requiredExpansionsKeys = Array.Empty<string>();
			public string Key => dccsKey;
			public DirectorCardCategorySelection Addressable { set => dccs = value; }
            string[] IAddressableKeyArrayProvider<ExpansionDef>.Key => requiredExpansionsKeys;
            ExpansionDef IAddressableKeyProvider<ExpansionDef>.Addressable {
				set 
				{ 
					if( requiredExpansions == null)
                    {
						SlipLogger.LogD("Deez Nuts");
						requiredExpansions = Array.Empty<ExpansionDef>();
                    }
					HG.ArrayUtils.ArrayAppend(ref requiredExpansions, value);
				}
			}
        }
}