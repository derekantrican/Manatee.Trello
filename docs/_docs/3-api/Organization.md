---
title: Organization
category: API
order: 200
---

Represents an organization.

**Assembly:** Manatee.Trello.dll

**Namespace:** Manatee.Trello

**Inheritance hierarchy:**

- Object
- Organization

## Constructors

### Organization(string id, TrelloAuthorization auth)

Creates a new instance of the Manatee.Trello.Organization object.

**Parameter:** id

The organization&#39;s ID.

**Parameter:** auth

(Optional) Custom authorization parameters. When not provided, Manatee.Trello.TrelloAuthorization.Default will be used.

#### Remarks

The supplied ID can be either the full ID or the organization&#39;s name.

## Properties

### static Organization.Fields DownloadedFields { get; set; }

Specifies which fields should be downloaded.

### IReadOnlyCollection&lt;[IAction](../IAction#iaction)&gt; Actions { get; }

Gets the collection of actions performed on the organization.

### [IBoardCollection](../IBoardCollection#iboardcollection) Boards { get; }

Gets the collection of boards owned by the organization.

### DateTime CreationDate { get; }

Gets the creation date of the organization.

### string Description { get; set; }

Gets or sets the organization&#39;s description.

### string DisplayName { get; set; }

Gets or sets the organization&#39;s display name.

### string Id { get; }

Gets the organization&#39;s ID.

### bool IsBusinessClass { get; }

Gets whether the organization has business class status.

### IReadOnlyCollection&lt;[IMember](../IMember#imember)&gt; Members { get; }

Gets the collection of members who belong to the organization.

### [IOrganizationMembershipCollection](../IOrganizationMembershipCollection#iorganizationmembershipcollection) Memberships { get; }

Gets the collection of members and their priveledges on this organization.

### string Name { get; set; }

Gets the organization&#39;s name.

### IReadOnlyCollection&lt;[IPowerUpData](../IPowerUpData#ipowerupdata)&gt; PowerUpData { get; }

Gets specific data regarding power-ups.

### [IOrganizationPreferences](../IOrganizationPreferences#iorganizationpreferences) Preferences { get; }

Gets the set of preferences for the organization.

### string Url { get; }

Gets the organization&#39;s URL.

### string Website { get; set; }

Gets or sets the organization&#39;s website.

## Events

### Action&lt;[IOrganization](../IOrganization#iorganization), IEnumerable&lt;string&gt;&gt; Updated

Raised when data on the organization is updated.

## Methods

### void ApplyAction(IAction action)

Applies the changes an action represents.

**Parameter:** action

The action.

### Task Delete(CancellationToken ct)

Deletes the organization.

**Parameter:** ct

(Optional) A cancellation token for async processing.

#### Remarks

This permanently deletes the organization from Trello&#39;s server, however, this object will remain in memory and all properties will remain accessible.

### Task Refresh(CancellationToken ct)

Refreshes the organization data.

**Parameter:** ct

(Optional) A cancellation token for async processing.

### string ToString()

Returns a string that represents the current object.

**Returns:** A string that represents the current object.
