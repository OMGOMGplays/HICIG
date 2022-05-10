using Sandbox;
using System;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HICIG 
{
	[Hammer.EditorModel("models/citizen_props/crate01.vmdl")]
	[Library("c_cappoint")]
	[Display(Name = "Capture Point"), Category("Captures"), Icon("move_to_inbox")]
	partial class CapturePoint : Prop, IUse 
	{
		[Flags]
		public enum Flags 
		{
			Blebs = 1,
			Ruds
		}

		[Property("teamEnum", Title = "Which team does this belong to?")]
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
					if (player.CurrTeam == TeamList.Ruds) 
					{
						HICIGGame.RudsTotalCaps++;
						using (Prediction.Off())
							HICIGGame.UpdateTeamScoreClient(To.Everyone, TeamList.Ruds, 1);
					}
					else if (player.CurrTeam == TeamList.Blebs) 
					{
						HICIGGame.BlebsTotalCaps++;
						using (Prediction.Off())
							HICIGGame.UpdateTeamScoreClient(To.Everyone, TeamList.Blebs, 1);
					}

					if (HICIGGame.RudsTotalCaps >= HICIGGame.WinGoal) 
					{
						HICIGGame.RudsTotalCaps = HICIGGame.WinGoal;
						HICIGGame.DeclareWinner(TeamList.Ruds);

						HICIGGame.SetTeamScoreClient(To.Everyone, TeamList.Ruds, HICIGGame.WinGoal);
					}
					else if (HICIGGame.BlebsTotalCaps >= HICIGGame.WinGoal) 
					{
						HICIGGame.BlebsTotalCaps = HICIGGame.WinGoal;
						HICIGGame.DeclareWinner(TeamList.Blebs);

						HICIGGame.SetTeamScoreClient(To.Everyone, TeamList.Blebs, HICIGGame.WinGoal);
					}

					player.IsHoldingSkel = false;
				}
			}

			return true;
		}
	}
}
