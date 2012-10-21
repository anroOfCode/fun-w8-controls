using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusao.Controls.Utilities;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
namespace Fusao.Controls.Tests
{
    [TestClass]
    public class SchedulerTest
    {
        private PriorityScheduler _sched = new PriorityScheduler();

        [TestMethod]
        public async Task SchedulerBasicTest()
        {
            bool didRun = false;
            _sched.Add(DateTime.Now + TimeSpan.FromMilliseconds(100), new Action(() =>
                {
                    didRun = true;
                }
            ));

            await Task.Delay(TimeSpan.FromMilliseconds(150));

            Assert.IsTrue(didRun);
        }

        [TestMethod]
        public async Task MoreComplexSchedule()
        {
            int counter = 0;
            _sched.Add(DateTime.Now + TimeSpan.FromMilliseconds(500), new Action(() =>
                {
                    Assert.AreEqual(counter, 4);
                    counter++;
                }
            ));

            _sched.Add(DateTime.Now + TimeSpan.FromMilliseconds(50), new Action(() =>
            {
                Assert.AreEqual(counter, 0);
                counter++;
            }
            ));

            _sched.Add(DateTime.Now + TimeSpan.FromMilliseconds(150), new Action(() =>
            {
                Assert.AreEqual(counter, 1);
                counter++;
            }
            ));

            _sched.Add(DateTime.Now + TimeSpan.FromMilliseconds(250), new Action(() =>
            {
                Assert.AreEqual(counter, 3);
                counter++;
            }
            ));

            _sched.Add(DateTime.Now + TimeSpan.FromMilliseconds(200), new Action(() =>
            {
                Assert.AreEqual(counter, 2);
                counter++;
            }
            ));

            await Task.Delay(TimeSpan.FromMilliseconds(600));

            Assert.AreEqual(counter, 5);
        }

    }
}
