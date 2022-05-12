using Sandbox;
using System;
using System.Linq;

namespace HICIG 
{
	partial class TestWeapon : WeaponBase 
	{
		public override string ViewModelPath => "models/skelhead/skelhead.vmdl";
		public override string WorldModelPath => "models/skelhead/skelhead.vmdl";

		public override int ClipSize => 16;

		public override void Spawn() 
		{
			base.Spawn();

			AmmoClip = 16;
		}

		public override void Simulate( Client owner )
		{
			base.Simulate( owner );
		}


	}
}
