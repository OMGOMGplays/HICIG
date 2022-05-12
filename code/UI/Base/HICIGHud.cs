using Sandbox;
using Sandbox.UI;

namespace HICIG 
{
	public partial class HICIGHud : HudEntity<RootPanel> 
	{
		public HICIGHud() 
		{

			
			// Screens //
			RootPanel.AddChild<TeamSelectScreen>();
		}
	}
}
