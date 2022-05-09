using Sandbox;

namespace HICIG 
{
	public class HICIGPlayer : Player 
	{
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
	}
}
