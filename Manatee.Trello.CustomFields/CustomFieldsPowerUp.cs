﻿using System;
using Manatee.Trello.Json;

namespace Manatee.Trello.CustomFields
{
	/// <summary>
	/// Models the Custom Fields plugin.
	/// </summary>
	[Obsolete("Custom fields have been integrated into the main Manatee.Trello library as of version 2.4.")]
	public class CustomFieldsPowerUp : PowerUpBase
	{
		internal const string PluginId = "56d5e249a98895a9797bebb9";

		private static bool _isRegistered;

		private CustomFieldsPowerUp(IJsonPowerUp json, TrelloAuthorization auth)
			: base(json, auth) {}

		/// <summary>
		/// Registers a power-up implementation.
		/// </summary>
		public static void Register()
		{
			if (!_isRegistered)
			{
				_isRegistered = true;
				TrelloConfiguration.RegisterPowerUp(PluginId, (j, a) => new CustomFieldsPowerUp(j, a));
			}
		}
	}
}
