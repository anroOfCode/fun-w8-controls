using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using Fusao.Controls.Utilities;

namespace Fusao.Controls.Tests
{
    [TestClass]
    public class HeapTest
    {
        class IdContainer
        {
            public int Id { get; private set; }
            public IdContainer(int id)
            {
                this.Id = id;
            }
        }

        private LightPriorityQueue<DateTime, IdContainer> _queue = new LightPriorityQueue<DateTime, IdContainer>();
        private KeyValuePair<DateTime, IdContainer>[] _list;
        [TestInitialize]
        public void BuildList()
        {
            _list = new KeyValuePair<DateTime, IdContainer>[]
            {
                new KeyValuePair<DateTime, IdContainer>(DateTime.Now, new IdContainer(0)),
                new KeyValuePair<DateTime, IdContainer>(DateTime.Now + TimeSpan.FromSeconds(1), new IdContainer(0)),
                new KeyValuePair<DateTime, IdContainer>(DateTime.Now + TimeSpan.FromSeconds(2), new IdContainer(1)),
                new KeyValuePair<DateTime, IdContainer>(DateTime.Now + TimeSpan.FromSeconds(2), new IdContainer(1)),
                new KeyValuePair<DateTime, IdContainer>(DateTime.Now + TimeSpan.FromSeconds(3), new IdContainer(2)),
                new KeyValuePair<DateTime, IdContainer>(DateTime.Now + TimeSpan.FromSeconds(4), new IdContainer(3))
            };
        }

        [TestMethod]
        public void HeapTestBasicAddPopOperation()
        {
            Assert.AreEqual(_queue.Count, 0);

            _queue.Add(_list[0].Key, _list[0].Value);
            _queue.Add(_list[3].Key, _list[3].Value);
            _queue.Add(_list[1].Key, _list[1].Value);
            _queue.Add(_list[5].Key, _list[5].Value);
            _queue.Add(_list[2].Key, _list[2].Value);
            _queue.Add(_list[4].Key, _list[4].Value);

            for (int i = 0; i < 6; i++)
            {
                Assert.IsTrue(_queue.Min.Id == _list[i].Value.Id);
                _queue.RemoveMin();
            }

            Assert.AreEqual(_queue.Count, 0);
        }

        [TestMethod]
        public void HeapTestOutOfOrderInsert()
        {
            _queue.Add(_list[5].Key, _list[5].Value);
            _queue.Add(_list[4].Key, _list[4].Value);
            _queue.Add(_list[3].Key, _list[3].Value);
            _queue.Add(_list[2].Key, _list[2].Value);
            _queue.Add(_list[1].Key, _list[1].Value);
            _queue.Add(_list[0].Key, _list[0].Value);

            for (int i = 0; i < 6; i++)
            {
                Assert.IsTrue(_queue.Min.Id == _list[i].Value.Id);
                _queue.RemoveMin();
            }
        }

        [TestMethod]
        public void HeapTestRemoval()
        {
            _queue.Add(_list[5].Key, _list[5].Value);
            _queue.Add(_list[4].Key, _list[4].Value);
            _queue.Add(_list[3].Key, _list[3].Value);
            _queue.Add(_list[2].Key, _list[2].Value);
            _queue.Add(_list[1].Key, _list[1].Value);
            _queue.Add(_list[0].Key, _list[0].Value);

            _queue.Remove(_list[3].Value);
            _queue.Remove(_list[0].Value);
            _queue.Remove(_list[2].Value);

            int[] remaining = new int[] { 1, 4, 5 };
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(_queue.Min.Id == _list[remaining[i]].Value.Id);
                _queue.RemoveMin();
            }

        }
    }
}
