using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusao.Controls.Utilities
{
    public class WeakKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private Dictionary<int, List<KeyPair>> _dict;

        public WeakKeyDictionary()
        {
            _dict = new Dictionary<int, List<KeyPair>>();
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            int hash = key.GetHashCode();

            if (!_dict.ContainsKey(hash))
            {
                _dict.Add(hash, new List<KeyPair>());
            }

            List<KeyPair> list = _dict[hash];

            List<KeyPair> toDelete = new List<KeyPair>();
            foreach (KeyPair kp in list)
            {
                TKey keyPairKey;
                if (!kp.TryGetKey(out keyPairKey))
                {
                    toDelete.Add(kp);
                    continue;
                }

                if (EqualityComparer<TKey>.Default.Equals(keyPairKey, key))
                {
                    throw new ArgumentException("Key already exists.");
                }
            }

            foreach (KeyPair kp in toDelete)
            {
                list.Remove(kp);
            }

            list.Add(new KeyPair(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            int hash = key.GetHashCode();

            List<KeyPair> list;
            if (_dict.TryGetValue(hash, out list))
            {
                foreach (KeyPair kp in list)
                {
                    TKey keyPairKey;
                    if (kp.TryGetKey(out keyPairKey))
                    {
                        if (EqualityComparer<TKey>.Default.Equals(keyPairKey, key))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }

            int hash = key.GetHashCode();

            List<KeyPair> list;
            if (_dict.TryGetValue(hash, out list))
            {
                KeyPair toRemove = null;
                for (int i = 0; i < list.Count; i++)
                {
                    KeyPair kp = list[i];
                    TKey keyPairKey;
                    if (kp.TryGetKey(out keyPairKey))
                    {
                        if (EqualityComparer<TKey>.Default.Equals(keyPairKey, key))
                        {
                            toRemove = kp;
                            break;
                        }
                    }
                }

                if (toRemove != null)
                {
                    return list.Remove(toRemove);
                }
            }

            return false;
        }

        public ICollection<TKey> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public ICollection<TValue> Values
        {
            get { throw new NotImplementedException(); }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException();
                }

                int hash = key.GetHashCode();

                List<KeyPair> list;
                if (_dict.TryGetValue(hash, out list))
                {
                    foreach (KeyPair kp in list)
                    {
                        TKey keyPairKey;
                        if (kp.TryGetKey(out keyPairKey))
                        {
                            if (EqualityComparer<TKey>.Default.Equals(keyPairKey, key))
                            {
                                return kp.Value;
                            }
                        }
                    }
                }

                throw new KeyNotFoundException();
            }

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException();
                }

                int hash = key.GetHashCode();

                List<KeyPair> list;
                if (!_dict.TryGetValue(hash, out list))
                {
                    list = new List<KeyPair>();
                    _dict.Add(hash, list);
                }

                foreach (KeyPair kp in list)
                {
                    TKey keyPairKey;
                    if (kp.TryGetKey(out keyPairKey))
                    {
                        if (EqualityComparer<TKey>.Default.Equals(keyPairKey, key))
                        {
                            kp.Value = value;
                            return;
                        }
                    }
                }

                list.Add(new KeyPair(key, value));
                
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private class KeyPair : EqualityComparer<KeyPair>
        {
            private WeakReference<Object> _keyReference;
            private TValue _value;
            public KeyPair(TKey key, TValue value)
            {
                _keyReference = new WeakReference<Object>((Object)key);
                _value = value;
            }

            public bool TryGetKey(out TKey key)
            {
                Object refParam;
                bool didSucceed = _keyReference.TryGetTarget(out refParam);
                key = (TKey)refParam;
                return didSucceed;
            }

            public TValue Value 
            { 
                get 
                { 
                    return _value; 
                }
                set
                {
                    _value = value;
                }
            }

            public override bool Equals(KeyPair x, KeyPair y)
            {
                TKey key1, key2;
                x.TryGetKey(out key1);
                y.TryGetKey(out key2);

                if (key1 == null || key2 == null)
                {
                    return false;
                }

                return EqualityComparer<TKey>.Default.Equals(key1, key2);
            }

            public override int GetHashCode(KeyPair obj)
            {
                TKey key;
                if (obj.TryGetKey(out key))
                {
                    return key.GetHashCode();
                }
                throw new ArgumentException();
            }
        }
    }
}
