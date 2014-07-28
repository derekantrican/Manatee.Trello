﻿/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		Member.cs
	Namespace:		Manatee.Trello
	Class Name:		Member
	Purpose:		Represents a member.

***************************************************************************************/

using System;
using System.Collections.Generic;
using Manatee.Trello.Contracts;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.Synchronization;
using Manatee.Trello.Internal.Validation;
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	/// <summary>
	/// Represents a member.
	/// </summary>
	public class Member : ICanWebhook, ICacheable
	{
		private const string AvatarUrlFormat = "https://trello-avatars.s3.amazonaws.com/{0}/170.png";

		private readonly Field<AvatarSource> _avatarSource;
		private readonly Field<string> _avatarUrl;
		private readonly Field<string> _bio;
		private readonly Field<string> _fullName;
		private readonly Field<string> _initials;
		private readonly Field<bool?> _isConfirmed;
		private readonly Field<MemberStatus> _status;
		private readonly Field<IEnumerable<string>> _trophies;
		private readonly Field<string> _url;
		private readonly Field<string> _userName;
		internal readonly MemberContext _context;

		/// <summary>
		/// Gets the collection of actions performed by the member.
		/// </summary>
		public ReadOnlyActionCollection Actions { get; private set; }
		/// <summary>
		/// Gets the source type for the member's avatar.
		/// </summary>
		public AvatarSource AvatarSource
		{
			get { return _avatarSource.Value; }
			internal set { _avatarSource.Value = value; }
		}
		/// <summary>
		/// Gets the URL to the member's avatar.
		/// </summary>
		public string AvatarUrl { get { return GetAvatar(); } }
		/// <summary>
		/// Gets the member's bio.
		/// </summary>
		public string Bio
		{
			get { return _bio.Value; }
			internal set { _bio.Value = value; }
		}
		/// <summary>
		/// Gets the collection of boards owned by the member.
		/// </summary>
		public ReadOnlyBoardCollection Boards { get; private set; }
		/// <summary>
		/// Gets the member's full name.
		/// </summary>
		public string FullName
		{
			get { return _fullName.Value; }
			internal set { _fullName.Value = value; }
		}
		/// <summary>
		/// Gets the member's ID.
		/// </summary>
		public string Id { get; private set; }
		/// <summary>
		/// Gets or sets the member's initials.
		/// </summary>
		public string Initials
		{
			get { return _initials.Value; }
			internal set { _initials.Value = value; }
		}
		/// <summary>
		/// Gets whether the member has actually join or has merely been invited (ghost).
		/// </summary>
		public bool? IsConfirmed { get { return _isConfirmed.Value; } }
		/// <summary>
		/// Gets the collection of organizations to which the member belongs.
		/// </summary>
		public ReadOnlyOrganizationCollection Organizations { get; private set; }
		/// <summary>
		/// Gets the member's online status.
		/// </summary>
		// TODO: Add forced refresh to support member status.
		public MemberStatus Status { get { return _status.Value; } }
		/// <summary>
		/// Gets the collection of trophies earned by the member.
		/// </summary>
		public IEnumerable<string> Trophies { get { return _trophies.Value; } }
		/// <summary>
		/// Gets the member's URL.
		/// </summary>
		public string Url { get { return _url.Value; } }
		/// <summary>
		/// Gets the member's username.
		/// </summary>
		public string UserName
		{
			get { return _userName.Value; }
			internal set { _userName.Value = value; }
		}

		internal IJsonMember Json { get { return _context.Data; } }

		/// <summary>
		/// Raised when data on the member is updated.
		/// </summary>
		public event Action<Member, IEnumerable<string>> Updated;

		/// <summary>
		/// Creates a new instance of the <see cref="Member"/> object.
		/// </summary>
		/// <param name="id">The member's ID.</param>
		/// <remarks>
		/// The supplied ID can be either the full ID or the username.
		/// </remarks>
		public Member(string id)
			: this(id, false) {}
		internal Member(string id, bool isMe)
			: this(id, isMe, true) {}
		internal Member(IJsonMember json, bool cache)
			: this(json.Id, cache)
		{
			_context.Merge(json);
		}
		private Member(string id, bool isMe, bool cache)
		{
			Id = id;
			_context = new MemberContext(id);
			_context.Synchronized += Synchronized;

			Actions = new ReadOnlyActionCollection(typeof(Member), id);
			_avatarSource = new Field<AvatarSource>(_context, () => AvatarSource);
			_avatarSource.AddRule(EnumerationRule<AvatarSource>.Instance);
			_avatarUrl = new Field<string>(_context, () => AvatarUrl);
			_bio = new Field<string>(_context, () => Bio);
			Boards = isMe ? new BoardCollection(typeof(Member), id) : new ReadOnlyBoardCollection(typeof(Member), id);
			_fullName = new Field<string>(_context, () => FullName);
			_fullName.AddRule(MemberFullNameRule.Instance);
			_initials = new Field<string>(_context, () => Initials);
			_initials.AddRule(MemberInitialsRule.Instance);
			_isConfirmed = new Field<bool?>(_context, () => IsConfirmed);
			Organizations = isMe ? new OrganizationCollection(id) : new ReadOnlyOrganizationCollection(id);
			_status = new Field<MemberStatus>(_context, () => Status);
			_trophies = new Field<IEnumerable<string>>(_context, () => Trophies);
			_url = new Field<string>(_context, () => Url);
			_userName = new Field<string>(_context, () => UserName);
			_userName.AddRule(UsernameRule.Instance);

			if (cache)
				TrelloConfiguration.Cache.Add(this);
		}

		/// <summary>
		/// Applies the changes an action represents.
		/// </summary>
		/// <param name="action">The action.</param>
		public void ApplyAction(Action action)
		{
			if (action.Type != ActionType.UpdateMember || action.Data.Member == null || action.Data.Member.Id != Id) return;
			_context.Merge(action.Data.Member.Json);
		}
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return FullName;
		}

		private void Synchronized(IEnumerable<string> properties)
		{
			Id = _context.Data.Id;
			var handler = Updated;
			if (handler != null)
				handler(this, properties);
		}
		private string GetAvatar()
		{
			var hash = _avatarUrl.Value;
			return hash.IsNullOrWhiteSpace() ? null : string.Format(AvatarUrlFormat, hash);
		}
	}
}