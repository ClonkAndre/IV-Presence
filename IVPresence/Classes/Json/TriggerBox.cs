using System.Numerics;

namespace IVPresence.Classes.Json
{
    internal class TriggerBox
    {

        #region Variables
        public Vector3 Pos1;
        public Vector3 Pos2;
        #endregion

        #region Constructor
        public TriggerBox(Vector3 pos1, Vector3 pos2)
        {
            Pos1 = pos1;
            Pos2 = pos2;
        }
        public TriggerBox()
        {
            
        }
        #endregion

    }
}
