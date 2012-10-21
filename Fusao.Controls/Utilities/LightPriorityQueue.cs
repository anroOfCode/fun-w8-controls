using System;
using System.Collections.Generic;

namespace Fusao.Controls.Utilities
{
    /// <summary>
    /// A lightweight Priority Queue which stores entries
    /// internally using a heap. ty
    /// </summary>
    /// <typeparam name="TValue">The type of object being stored.</typeparam>
    /// <typeparam name="TKey">The key of the object being stored, should
    /// implement Comparer or Comparer.Default will be used. </typeparam>
    public class LightPriorityQueue<TKey, TValue>
    {
        private List<KeyValuePair<TKey, TValue>> _list;
        
        // We keep an internal dictionary for O(1)
        // removal of items by storing their index
        // position.
        private Dictionary<TValue, int> _lookup;

        /// <summary>
        /// Initializes a new LightPriorityQueue.
        /// </summary>
        public LightPriorityQueue()
        {
            _list = new List<KeyValuePair<TKey, TValue>>();
            _lookup = new Dictionary<TValue, int>();
        }

        /// <summary>
        /// Gets the current element count of the queue.
        /// </summary>
        public int Count 
        { 
            get 
            { 
                return _list.Count; 
            } 
        }

        /// <summary>
        /// Gets the current Minimum member of
        /// the priority queue. Throws IndexOutOfRange
        /// exception when the queue is empty.
        /// </summary>
        public TValue Min 
        { 
            get 
            {
                return _list[0].Value; 
            } 
        }

        /// <summary>
        /// Adds the given Key Value pair to the queue, preserving
        /// sorting order.
        /// </summary>
        /// <param name="key">The key of the pair.</param>
        /// <param name="value">The value of the pair.</param>
        public void Add(TKey key, TValue value)
        {
            if (Equals(key, default(TKey)))
            {
                throw new ArgumentNullException("Key cannot be null or of the default value.");
            }

            if (_lookup.ContainsKey(value))
            {
                throw new ArgumentException("Cannot insert duplicate value");
            }

            _list.Add(new KeyValuePair<TKey, TValue>(key, value));
            _lookup.Add(value, _list.Count - 1);
            BubbleUp();
        }

        /// <summary>
        /// Removes the first instance of the specified value from the priority queue.
        /// </summary>
        /// <param name="value">The value to be removed.</param>
        /// <returns>Whether or not the element exists and was successfully removed.</returns>
        public bool Remove(TValue value)
        {
            int i;
            bool didSucceed = _lookup.TryGetValue(value, out i);

            if (didSucceed)
            {
                // Place the last item in this
                // item's place and bubble it
                // down the heap.
                _list[i] = _list[_list.Count - 1];
                _lookup[_list[i].Value] = i;

                _list.RemoveAt(_list.Count - 1);
                _lookup.Remove(value);

                // Only bubble down if more than one item is left
                // in the queue.
                if (_list.Count > 1)
                {
                    BubbleDown(i);
                }
            }

            return didSucceed;
        }

        /// <summary>
        /// Removes the smallest element from the priority queue and returns it.
        /// </summary>
        /// <returns>The smallest key's value.</returns>
        public TValue RemoveMin()
        {
            if (_list.Count == 0)
            {
                throw new IndexOutOfRangeException("The heap is empty.");
            }

            TValue toReturn = _list[0].Value;

            _lookup.Remove(toReturn);

            if (_list.Count > 1)
            {
                _list[0] = _list[_list.Count - 1];
                _lookup[_list[0].Value] = 0;
            }

            _list.RemoveAt(_list.Count - 1);

            if (_list.Count > 1)
            {
                BubbleDown(0);
            }

            return toReturn;
        }

        /// <summary>
        /// Removes all items from the priority queue.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            _lookup.Clear();
        }

        /// <summary>
        /// Returns a boolean indicating whether the given value is
        /// a member of this collection.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(TValue value)
        {
            return _lookup.ContainsKey(value);
        }

        #region Helpers
        private void BubbleUp()
        {
            int i = _list.Count - 1;
            while (i > 0 && Less(i, (i - 1) / 2))
            {
                Swap((i - 1) / 2, i);
                i = (i - 1) / 2;
            }
        }

        private void BubbleDown(int i)
        {
            while (true)
            {
                int left = i * 2 + 1;
                int right = i * 2 + 2;

                int smallest = i;
                if (left < _list.Count && Less(left, i))
                {
                    smallest = left;
                }

                if (right < _list.Count && Less(right, smallest))
                {
                    smallest = right;
                }

                if (smallest != i)
                {
                    Swap(i, smallest);
                    i = smallest;
                }
                else
                {
                    break;
                }
            }
        }

        private void Swap(int i, int j)
        {
            KeyValuePair<TKey, TValue> temp = _list[i];
            _list[i] = _list[j];
            _list[j] = temp;

            _lookup[_list[i].Value] = i;
            _lookup[_list[j].Value] = j;
        }

        private bool Less(int i, int j)
        {
            return Comparer<TKey>.Default.Compare(_list[i].Key, _list[j].Key) <= 0;
        }
        #endregion
    }
}
