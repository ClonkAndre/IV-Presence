using System.Numerics;

namespace IVPresence.Classes.Json
{
    internal class TriggerRange
    {

        #region Variables
        public Vector3 Position;
        public float Range;
        #endregion

        #region Constructor
        public TriggerRange(Vector3 pos, float range)
        {
            Position = pos;
            Range = range;
        }
        public TriggerRange()
        {
            
        }
        #endregion

    }
}
