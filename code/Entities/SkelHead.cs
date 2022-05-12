using Sandbox;
using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HICIG 
{
	[Hammer.EditorModel("models/skelhead/skelhead.vmdl")]
	[Library("c_skelhead"), AutoGenerate]
	[Display(Name = "Skeleton Head"), Category("Captures"), Icon("add_reaction")]
	public class SkelHead : Prop
	{
		public enum Flags 
		{
			Blebs = 1,
			Ruds
		}
		
		[Property("teamEnum", Title = "Which team does this skull belong to?")]
		public Flags TeamEnum {get; set;}

		public override void Spawn()
		{
			base.Spawn();

			SetModel("models/skelhead/skelhead.vmdl");
			SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if (other is HICIGPlayer player) 
			{
				if (player.CurrTeam.ToString() != TeamEnum.ToString()) 
				{
					PlayerPickedUpSkel();
				}
			}
		}

		public void PlayerPickedUpSkel() 
		{
			var player = Local.Pawn as HICIGPlayer;

			player.Inventory.DeleteContents();
			player.Inventory.Add(new SkelHeadWep(), true);

			player.IsHoldingSkel = true;
		}
	}
}
