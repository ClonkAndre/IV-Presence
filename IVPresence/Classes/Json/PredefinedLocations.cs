using System;
using System.Numerics;

namespace IVPresence.Classes.Json
{
    internal class PredefinedLocations
    {

        #region Variables
        public string ID;
        public TriggerRange RangeTrigger;
        //public TriggerBox BoxTrigger;
        public CustomRichPresence CustomRichPresence;
        #endregion

        #region Constructor
        /// <summary>
        /// A constructor which sets up this <see cref="PredefinedLocations"/> for a <see cref="TriggerRange"/>.
        /// </summary>
        /// <param name="id">Just an indentifier for this location so its easier to debug.</param>
        /// <param name="pos">The target position.</param>
        /// <param name="triggerRange">The target range around the given position.</param>
        public PredefinedLocations(string id, Vector3 pos, float triggerRange)
        {
            ID = id;
            RangeTrigger = new TriggerRange(pos, triggerRange);
        }
        ///// <summary>
        ///// A constructor which sets up this <see cref="PredefinedLocations"/> for a <see cref="TriggerBox"/>.
        ///// </summary>
        ///// <param name="id">Just an indentifier for this location so its easier to debug.</param>
        ///// <param name="pos1">The upper-left position for a 3D box.</param>
        ///// <param name="pos2">The lower-right position for a 3D box.</param>
        //public PredefinedLocations(string id, Vector3 pos1, Vector3 pos2)
        //{
        //    ID = id;
        //    BoxTrigger = new TriggerBox(pos1, pos2);
        //}
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PredefinedLocations()
        {
            
        }
        #endregion

    }
}
