using Sandbox;
using System;
using System.Linq;

namespace HICIG 
{
	partial class Inventory : BaseInventory 
	{
		public Inventory(Player player) : base(player) 
		{
			
		}

		public override bool Add( Entity ent, bool makeActive = false )
		{
			var player = Owner as HICIGPlayer;
			var weapon = ent as WeaponBase;

			return base.Add( ent, makeActive );
		}

		public bool IsCarryingType(Type t) 
		{
			return List.Any(x => x.GetType() == t);
		}
	}
}
