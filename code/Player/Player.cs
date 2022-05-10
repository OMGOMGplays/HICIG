using Sandbox;

namespace HICIG 
{
	public partial class HICIGPlayer : Player 
	{
		public bool LockControls = false;

		public bool IsHoldingSkel = false;

		public override void Spawn()
		{
			base.Spawn();

			CurrTeam = TeamList.Unassigned;

			Respawn();
		}

		public override void Respawn()
		{
			base.Respawn();

			SetModel("models/citizen/citizen.vmdl");

			Animator = new StandardPlayerAnimator();
			CameraMode = new FirstPersonCamera();
			Controller = new WalkController();

			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if (LockControls && IsServer)
				return;

			SimulateActiveChild(cl, ActiveChild);

			TickPlayerUse();
		}
	}
}
