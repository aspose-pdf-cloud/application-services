using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aspose.Cloud.Marketplace.Common.Tests
{
    [Trait("AppCommon", "AsyncLockTests")]
    public class AsyncLockTests
    {
        class Result
        {
            public double ElapsedMills { get; set; }
            public string Name { get; set; }
        }

        [Fact]
        public async void AsyncLockTests_SyncTest()
        {
            int expectedDelay = 200;
            var locker = new AsyncLock();
            Result task1Result = null, task2Result = null;
            List<Task> tasks = new List<Task>();
            Stopwatch start = new Stopwatch();
            start.Start();
            tasks.Add(Task.Run(() => 
            {
                using (locker.Lock())
                {
                    task1Result = new Result()
                    {
                        Name = "Task1",
                        ElapsedMills = start.Elapsed.TotalMilliseconds
                    };
                    Task.Delay(expectedDelay * 2).Wait();
                }
            }));
            tasks.Add(Task.Run(() => 
            {
                Task.Delay(expectedDelay).Wait();
                using (locker.Lock())
                {
                    task2Result = new Result()
                    {
                        Name = "Task2",
                        ElapsedMills = start.Elapsed.TotalMilliseconds
                    };
                }
            }));
            await Task.WhenAll(tasks.ToArray());
            Assert.NotNull(task1Result);
            Assert.Equal("Task1", task1Result.Name);
            Assert.NotNull(task2Result);
            Assert.Equal("Task2", task2Result.Name);
            var actualMills = task2Result.ElapsedMills;
            Assert.True(actualMills > 2 * expectedDelay, $"Task2 should complete at least in {2 * expectedDelay} mills, actualMills {actualMills}");
        }


        [Fact]
        public async void AsyncLockTests_AsyncTest()
        {
            int expectedDelay = 200;
            var locker = new AsyncLock();
            Result task1Result = null, task2Result = null;
            List<Task> tasks = new List<Task>();
            Stopwatch start = new Stopwatch();
            start.Start();
            tasks.Add(Task.Run( async () =>
            {
                using (await locker.LockAsync())
                {
                    task1Result = new Result()
                    {
                        Name = "Task1",
                        ElapsedMills = start.Elapsed.TotalMilliseconds
                    };
                    await Task.Delay(expectedDelay * 2);
                }
            }));
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(expectedDelay);
                using (await locker.LockAsync())
                {
                    task2Result = new Result()
                    {
                        Name = "Task2",
                        ElapsedMills = start.Elapsed.TotalMilliseconds
                    };
                }
            }));
            await Task.WhenAll(tasks.ToArray());
            Assert.NotNull(task1Result);
            Assert.Equal("Task1", task1Result.Name);
            Assert.NotNull(task2Result);
            Assert.Equal("Task2", task2Result.Name);
            var actualMills = task2Result.ElapsedMills;
            Assert.True(actualMills > 2 * expectedDelay, $"Task2 should complete at least in {2 * expectedDelay} mills, actualMills {actualMills}");
            //tasks[0].re
        }
    }
}
