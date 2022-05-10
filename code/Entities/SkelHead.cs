using Sandbox;
using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HICIG 
{
	[Hammer.EditorModel("models/skelhead/skelhead.vmdl")]
	[Library("c_skelhead")]
	[Display(Name = "Skeleton Head"), Category("Captures"), Icon("add_reaction")]
	public class SkelHead : Prop, IUse
	{
		[Flags]
		public enum Flags 
		{
			Blebs = 1,
			Ruds
		}
		
		public override void Spawn()
		{
			base.Spawn();

			SetModel("models/skelhead/skelhead.vmdl");
		}

		public bool OnUse(Entity ent)
		{
			return true;
		}

		public bool IsUsable(Entity ent) 
		{
			return true;
		}
	}
}
