using Sandbox;
using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HICIG 
{
	[Hammer.EditorModel("models/citizen_props/crate01.vmdl")]
	[Library("c_cappoint"), AutoGenerate]
	[Display(Name = "Capture Point"), Category("Captures"), Icon("move_to_inbox")]
	partial class CapturePoint : Prop, IUse 
	{
		[Flags]
		public enum Flags 
		{
			Blebs = 1,
			Ruds
		}

		[Property, Display(Name = "Which team does this CapPoint belong to?", Description = "Changes which team that this capture point belongs to.")]
		public Flags TeamEnum {get; set;}

		public override void Spawn()
		{
			base.Spawn();
			SetModel("models/citizen_props/crate01.vmdl");
		}

		public bool IsUsable(Entity user) 
		{
			if (user is HICIGPlayer player) 
			{
				if (player.IsHoldingSkel)
					return true;
			}

			return false;
		}

		public bool OnUse(Entity user) 
		{
			if (user is HICIGPlayer player) 
			{
				if (player.CurrTeam.ToString() != TeamEnum.ToString())
					return false;

				if (player.IsHoldingSkel) 
				{
					if (player.CurrTeam == HICIGPlayer.TeamList.Ruds) 
					{
						HICIGGame.RudsTotalCaps++;
						using (Prediction.Off())
							HICIGGame.UpdateTeamScoreClient(To.Everyone, HICIGPlayer.TeamList.Ruds, 1);
					}
					else if (player.CurrTeam == HICIGPlayer.TeamList.Blebs) 
					{
						HICIGGame.BlebsTotalCaps++;
						using (Prediction.Off())
							HICIGGame.UpdateTeamScoreClient(To.Everyone, HICIGPlayer.TeamList.Blebs, 1);
					}

					if (HICIGGame.RudsTotalCaps >= HICIGGame.WinGoal) 
					{
						HICIGGame.RudsTotalCaps = HICIGGame.WinGoal;
						HICIGGame.DeclareWinner(HICIGPlayer.TeamList.Ruds);

						HICIGGame.SetTeamScoreClient(To.Everyone, HICIGPlayer.TeamList.Ruds, HICIGGame.WinGoal);
					}
					else if (HICIGGame.BlebsTotalCaps >= HICIGGame.WinGoal) 
					{
						HICIGGame.BlebsTotalCaps = HICIGGame.WinGoal;
						HICIGGame.DeclareWinner(HICIGPlayer.TeamList.Blebs);

						HICIGGame.SetTeamScoreClient(To.Everyone, HICIGPlayer.TeamList.Blebs, HICIGGame.WinGoal);
					}

					player.IsHoldingSkel = false;
				}
			}

			return true;
		}
	}
}
