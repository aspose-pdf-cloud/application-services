using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Aspose.Cloud.Marketplace.Common
{
    /// <summary>
    /// Provides ability to measure elapsed time for code
    /// </summary>
    public class TaskMeasurer
    {
        private List<StatisticalDocument> _stat;
        public List<StatisticalDocument> Stat => Get();
        public List<StatisticalDocument> Get()
        {
            using (_statLock.Lock())
            {
                return new List<StatisticalDocument>(_stat);
            }
        }
        private readonly AsyncLock _statLock;
        private async Task finish(Stopwatch stopwatch, string call, string comment = null, dynamic commentObj = null)
        {
            stopwatch.Stop();
            var stat = new StatisticalDocument()
            {
                Call = call,
                ElapsedSeconds = stopwatch?.Elapsed.TotalSeconds,
                Comment = string.IsNullOrEmpty(comment) ? (commentObj != null ? JsonConvert.SerializeObject(commentObj) : null) : comment
            };

            using (await _statLock.LockAsync())
            {
                _stat.Add(stat);
            }

        }

        public TaskMeasurer()
        {
            _stat = new List<StatisticalDocument>();
            _statLock = new AsyncLock();
        }
        public async Task<R> Run<R>(Func<Task<R>> func, string call, string comment = null, dynamic commentObj = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return await func().ContinueWith((t) =>
            {
                finish(stopwatch, call, comment, commentObj);
                return t.Result;
            });
        }

        public async Task Run(Func<Task> func, string call, string comment = null, dynamic commentObj = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await func().ContinueWith((t) =>
            {
                finish(stopwatch, call, comment, commentObj);
            });
        }

        public T RunSync<T>(Func<T> func, string call, string comment = null, dynamic commentObj = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            T result = func();
            finish(stopwatch, call, comment, commentObj);
            return result;
        }

        public void RunSync(Action func, string call, string comment = null, dynamic commentObj = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            func();
            finish(stopwatch, call, comment, commentObj);
        }
    }
}
