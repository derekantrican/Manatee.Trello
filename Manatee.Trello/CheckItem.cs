﻿/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		CheckItem.cs
	Namespace:		Manatee.Trello
	Class Name:		CheckItem
	Purpose:		Represents an item in a checklist on Trello.com.

***************************************************************************************/
using System;
using System.Linq;
using Manatee.Json;
using Manatee.Json.Enumerations;
using Manatee.Trello.Implementation;
using Manatee.Trello.Rest;

namespace Manatee.Trello
{
	//   "checkItems":[
	//      {
	//         "state":"incomplete",
	//         "id":"514463bfd02ebee350000d1c",
	//         "name":"Test development",
	//         "pos":16703
	//      },
	public class CheckItem : JsonCompatibleExpiringObject, IEquatable<CheckItem>
	{
		private static readonly OneToOneMap<CheckItemStateType, string> _stateMap;

		private string _apiState;
		private string _name;
		private int? _position;
		private CheckItemStateType _state;

		public string Name
		{
			get
			{
				VerifyNotExpired();
				return _name;
			}
			set
			{
				_name = value;
				Parameters.Add("value", value);
				Put("name");
			}
		}
		public int? Position
		{
			get
			{
				VerifyNotExpired();
				return _position;
			}
			set
			{
				_position = value;
				Parameters.Add("value", value);
				Put("pos");
			}
		}
		public CheckItemStateType State
		{
			get { return _state; }
			set
			{
				_state = value;
				UpdateApiState();
				Parameters.Add("value", value);
				Put("state");
			}
		}

		static CheckItem()
		{
			_stateMap = new OneToOneMap<CheckItemStateType, string>
						{
							{CheckItemStateType.Incomplete, "incomplete"},
							{CheckItemStateType.Complete, "complete"},
						};
		}
		public CheckItem() {}
		internal CheckItem(TrelloService svc, CheckList owner)
			: base(svc, owner) {}

		public void Delete()
		{
			Svc.DeleteFromCache(new Request<CheckItem>(new[] {Owner, this}));
		}

		public override void FromJson(JsonValue json)
		{
			if (json == null) return;
			if (json.Type != JsonValueType.Object) return;
			var obj = json.Object;
			Id = obj.TryGetString("id");
			_name = obj.TryGetString("name");
			_position = (int?) obj.TryGetNumber("pos");
			_apiState = obj.TryGetString("state");
			UpdateState();
		}
		public override JsonValue ToJson()
		{
			var json = new JsonObject
						{
							{"id", Id},
							{"name", _name},
							{"pos", _position.HasValue ? _position.Value : JsonValue.Null},
							{"state", _apiState}
						};
			return json;
		}
		public bool Equals(CheckItem other)
		{
			return Id == other.Id;
		}

		internal override void Refresh(ExpiringObject entity)
		{
			var checkItem = entity as CheckItem;
			if (checkItem == null) return;
		}
		internal override bool Match(string id)
		{
			return Id == id;
		}

		protected override void Get()
		{
			var entity = Svc.Api.Get(new Request<CheckItem>(new[] {Owner, this}));
			Refresh(entity);
		}
		protected override void PropigateSerivce() {}

		private void Put(string extension)
		{
			Svc.PutAndCache(new Request<CheckItem>(new[] {((CheckList) Owner).Card, Owner, this}, this, extension));
		}
		private void UpdateState()
		{
			_state = _stateMap.Any(kvp => kvp.Value == _apiState) ? _stateMap[_apiState] : CheckItemStateType.Unknown;
		}
		private void UpdateApiState()
		{
			if (_stateMap.Any(kvp => kvp.Key == _state))
				_apiState = _stateMap[_state];
		}
	}
}
