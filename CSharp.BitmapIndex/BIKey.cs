using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapIndex
{
    public class BIKey
    {
        public class BIGroup
        {
            private int _group;
            private string _subGroup;

            public BIGroup(int group)
            {
                _group = group;
                _subGroup = string.Empty;
            }

            public BIGroup(int group, string subGroup)
            {
                _group = group;
                _subGroup = (subGroup != null ? subGroup : string.Empty);
            }

            public int Group
            {
                get
                {
                    return _group;
                }
            }

            public string SubGroup
            {
                get
                {
                    return _subGroup;
                }
            }

            public override bool Equals(object obj)
            {
                if (this == obj)
                    return true;

                BIGroup compare = obj as BIGroup;

                if (compare == null)
                    return false;

                return (compare._group == _group && compare._subGroup.Equals(_subGroup));
            }

            public override int GetHashCode()
            {
                int result = 17;
                result = 31 * result + _group;
                result = (_subGroup.Length > 0 ? 31 * result + _subGroup.GetHashCode() : result);

                return result;
            }
        }

        private BIGroup _group;
        private string _key;

        public BIKey(int group, Object key)
        {
            _group = new BIGroup(group);

            if (key != null)
                _key = key.ToString();
            else
                _key = string.Empty;
        }

        public BIKey(int group, string subGroup, Object key)
        {
            _group = new BIGroup(group, subGroup);

            if (key != null)
                _key = key.ToString();
            else
                _key = string.Empty;
        }

        public BIKey(BIGroup group, Object key)
        {
            _group = group;

            if (key != null)
                _key = key.ToString();
            else
                _key = string.Empty;
        }

        public BIGroup Group
        {
            get
            {
                return _group;
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            BIKey compare = obj as BIKey;

            if (compare == null)
                return false;

            return (compare._group.Equals(_group) && compare._key.Equals(_key));
        }

        public override int GetHashCode()
        {
            int result = _group.GetHashCode();
            result = 31 * result + _key.GetHashCode();

            return result;
        }
    }
}
