namespace Villeon.Components
{
    public class Ladder : IComponent
    {
        private bool _canClimb = false;

        public Ladder(bool canClimb)
        {
            CanClimb = canClimb;
        }

        public bool CanClimb;
    }
}