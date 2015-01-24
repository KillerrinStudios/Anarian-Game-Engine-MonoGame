using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarian.DataStructures.Animation.Aux
{
    /// <summary>
    /// Class that contains additional information attached to the model and
    /// shared with the runtime.
    /// </summary>
    public class ModelExtra
    {
        #region Fields

        /// <summary>
        /// The bone indices for the skeleton associated with any
        /// skinned model.
        /// </summary>
        private List<int> skeleton = new List<int>();

        /// <summary>
        /// Any associated animation clips
        /// </summary>
        public List<AnimationClip> clips = new List<AnimationClip>();
        #endregion

        #region Properties

        /// <summary>
        /// The bone indices for the skeleton associated with any
        /// skinned model.
        /// </summary>
        public List<int> Skeleton { get { return skeleton; } set { skeleton = value; } }

        /// <summary>
        /// Animation clips associated with this model
        /// </summary>
        public List<AnimationClip> Clips { get { return clips; } set { clips = value; } }

        #endregion

        public ModelExtra() { }
        public ModelExtra(AnimationAux.ModelExtra oldModelExtra)
        {
            foreach (var oldSkeleton in oldModelExtra.Skeleton) {
                skeleton.Add(oldSkeleton);
            }

            foreach (var oldClip in oldModelExtra.Clips) {
                clips.Add(new AnimationClip(oldClip));
            }
        }

        public ModelExtra(ModelExtra oldModelExtra)
        {
            foreach (var oldSkeleton in oldModelExtra.Skeleton) {
                skeleton.Add(oldSkeleton);
            }

            foreach (var oldClip in oldModelExtra.Clips) {
                clips.Add(new AnimationClip(oldClip));
            }
        }
    }
}