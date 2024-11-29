/*
 * SkinnedModelBone.cs
 * Author: Bruno Evangelista
 * Copyright (c) 2008 Bruno Evangelista. All rights reserved.
 *
 * THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XNAnimation
{
    /// <summary>
    /// Represents a bone from a skeleton
    /// </summary>
    public class SkinnedModelBone
    {
        private readonly ushort index;
        private readonly string name;

        private SkinnedModelBone parent;
        private SkinnedModelBoneCollection children;

        private readonly Pose bindPose;
        private readonly Matrix inverseBindPoseTransform;

        #region Properties

        /// <summary>
        /// Gets the index of this bone in depth-first order.
        /// </summary>
        public ushort Index
        {
            get { return index; }
        }

        /// <summary>
        /// Gets the name of this bone.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the parent of this bone.
        /// </summary>
        public SkinnedModelBone Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }

        /// <summary>
        /// Gets a collection of bones that are children of this bone.
        /// </summary>
        public SkinnedModelBoneCollection Children
        {
            get { return children; }
            internal set { children = value; }
        }

        /// <summary>
        /// Gets the pose of this bone relative to its parent.
        /// </summary>
        public Pose BindPose
        {
            get { return bindPose; }
        }

        /// <summary>
        /// Gets a matrix used to transform model's mesh vertices putting them in the same 
        /// coordinate system of this bone.
        /// </summary>
        public Matrix InverseBindPoseTransform
        {
            get { return inverseBindPoseTransform; }
        }

        #endregion

        internal SkinnedModelBone(ushort index, string name, Pose bindPose,
            Matrix inverseBindPoseTransform)
        {
            this.index = index;
            this.name = name;
            this.bindPose = bindPose;
            this.inverseBindPoseTransform = inverseBindPoseTransform;
        }

        public void CopyBindPoseTo(Pose[] destination)
        {
            int boneIndex = 0;
            CopyBindPoseTo(destination, ref boneIndex);
        }

        private void CopyBindPoseTo(Pose[] destination, ref int boneIndex)
        {
            destination[boneIndex++] = bindPose;
			for (int i = 0; i < children.Count; i++)
			{
				children[i].CopyBindPoseTo(destination, ref boneIndex);
			}
		}

        internal static SkinnedModelBone Read(ContentReader input)
        {
            // Read bone data
            ushort index = input.ReadUInt16();
            string name = input.ReadString();

            // Read bind pose
            Pose bindPose;
            bindPose.Translation = input.ReadVector3();
            bindPose.Orientation = input.ReadQuaternion();
            bindPose.Scale = input.ReadVector3();

            Matrix inverseBindPoseTransform = input.ReadMatrix();
            SkinnedModelBone skinnedBone =
                new SkinnedModelBone(index, name, bindPose, inverseBindPoseTransform);

            // Read bone parent
            input.ReadSharedResource<SkinnedModelBone>(
                delegate(SkinnedModelBone parentBone) { skinnedBone.parent = parentBone; });

            // Read bone children
            int numChildren = input.ReadInt32();
            List<SkinnedModelBone> childrenList = new List<SkinnedModelBone>(numChildren);
            for (int i = 0; i < numChildren; i++)
            {
                input.ReadSharedResource<SkinnedModelBone>(
                    delegate(SkinnedModelBone childBone) { childrenList.Add(childBone); });
            }
            skinnedBone.children = new SkinnedModelBoneCollection(childrenList);

            return skinnedBone;
        }

        /*
        public void UpdateHierarchy()
        {
            Update();
            foreach (SkinnedModelBone bone in children)
                bone.UpdateHierarchy();
        }

        public void UpdateHierarchy(ref Matrix[] hierarchy)
        {
            int index = 0;
            UpdateHierarchy(ref hierarchy, ref index);
        }

        private void UpdateHierarchy(ref Matrix[] hierarchy, ref int index)
        {
            Update();
            hierarchy[index++] = absoluteTransform;

            foreach (SkinnedModelBone bone in children)
                bone.UpdateHierarchy(ref hierarchy, ref index);
        }

        private void Update()
        {
//            if (controllers.Count > 0)
//            {
//                // Calcule the wright of each animation controller
//                float weight = 1.0f / controllers.Count;
//                transform = controllers[0].GetCurrentKeyframeTransform(name) * weight;
//
//                for (int i = 1; i < controllers.Count; i++)
//                    transform += controllers[i].GetCurrentKeyframeTransform(name) * weight;
//            }
            
            if (controller != null)
            {
                Matrix keyframeTransfom;
                if (controller.GetCurrentKeyframeTransform(name, out keyframeTransfom))
                    transform = keyframeTransfom;
            }

            absoluteTransform = transform * customUserTransform;
            if (parent != null)
                absoluteTransform *= parent.AbsoluteTransform;
        }
         */
    }
}