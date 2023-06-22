using System.Numerics;
using System.Runtime.Serialization;

namespace Ortus.Models.Representation
{
    public class Bone
    {
        // for serialization
        private string parentName;
        private Vector3 local;
        private Vector3 world;
        private bool worldIsDirty = true;
        private Bone parent;

        public string Name { get; set; }
        public int Id { get; set; }
        public int Flags { get; set; }

        [IgnoreDataMember]
        public Bone Parent
        {
            get { return parent; }
            set
            {
                worldIsDirty = true;
                parent = value;
            }
        }

        public string ParentName
        {
            get
            {
                if ( Parent == null )
                    return parentName;
                else
                    return Parent.Name;
            }

            set
            {
                if ( Parent == null )
                    parentName = value;
                else
                    Parent.Name = value;
            }
        }

        public int Field08 { get; set; }

        public Vector3 Local
        {
            get { return local; }
            set
            {
                local = value;
                worldIsDirty = true;
            }
        }

        [IgnoreDataMember]
        public Vector3 World
        {
            get
            {
                if ( Parent == null )
                    return Local;
                else
                {
                    if ( worldIsDirty )
                        world = Parent.World * Local;
                    return world;
                }
            }
        }
    }
}
