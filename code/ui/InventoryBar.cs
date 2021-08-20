using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

public class InventoryBar : Panel
{
	readonly List<InventoryIcon> slots = new();

	public InventoryBar()
	{
		for ( int i = 0; i < 5; i++ )
		{
			var icon = new InventoryIcon( i + 1, this );
			slots.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn;
		if ( player == null ) return;
		if ( player.Inventory == null ) return;

		for ( int i = 0; i < slots.Count; i++ )
		{
			UpdateIcon( player.Inventory.GetSlot( i ), slots[i], i );
		}
	}

	private static void UpdateIcon( Entity ent, InventoryIcon inventoryIcon, int i )
	{
		if ( ent == null )
		{
			inventoryIcon.Clear();
			return;
		}

		inventoryIcon.TargetEnt = ent;
		inventoryIcon.Label.Text = ent.ClassInfo.Title;
		inventoryIcon.SetClass( "active", ent.IsActiveChild() );
	}

	[Event( "buildinput" )]
	public void ProcessClientInput( InputBuilder input )
	{
		var player = Local.Pawn as Player;
		if ( player == null )
			return;

		var inventory = player.Inventory;
		if ( inventory == null )
			return;

		if ( input.MouseWheel != 0 ) SwitchActiveSlot( input, inventory, -input.MouseWheel );
	}

	private static void SetActiveSlot( InputBuilder input, IBaseInventory inventory, int i )
	{
		var player = Local.Pawn;
		if ( player == null )
			return;

		var ent = inventory.GetSlot( i );
		if ( player.ActiveChild == ent )
			return;

		if ( ent == null )
			return;

		input.ActiveChild = ent;
	}

	private static void SwitchActiveSlot( InputBuilder input, IBaseInventory inventory, int idelta )
	{
		var count = inventory.Count();
		if ( count == 0 ) return;

		var slot = inventory.GetActiveSlot();
		var nextSlot = slot + idelta;

		while ( nextSlot < 0 ) nextSlot += count;
		while ( nextSlot >= count ) nextSlot -= count;

		SetActiveSlot( input, inventory, nextSlot );
	}
}