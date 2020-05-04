using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aspose.Cloud.Marketplace.Common.Tests
{
    [Trait("AppCommon", "TaskMeasurer")]
    public class TaskMeasurerTests
    {
        public class Result
        {
            public int Dummy { get; set; }
        }
        public Result expectedResult;
        public TaskMeasurer _measurer;
        public TaskMeasurerTests()
        {
            _measurer = new TaskMeasurer();
            expectedResult = new Result() { Dummy = 55 };
        }
        [Fact]
        public async void TaskMeasurerTests_RunAsyncTest()
        {
            await _measurer.Run(async () =>
            {
                await Task.Delay(200);
            }, "test01", "comment01");
            IList<StatisticalDocument> list = _measurer.Get();
            Assert.Single(list);
            Assert.Equal("test01", list[0]?.Call);
            Assert.Equal("comment01", list[0]?.Comment);
        }

        [Fact]
        public async void TaskMeasurerTests_RunAsyncResultTest()
        {
            var result = await _measurer.Run(async () =>
            {
                await Task.Delay(200);
                return expectedResult;
            }, "test01", "comment01");
            Assert.Equal(expectedResult.Dummy, result.Dummy);
            IList<StatisticalDocument> list = _measurer.Get();
            Assert.Single(list);
            Assert.Equal("test01", list[0]?.Call);
            Assert.Equal("comment01", list[0]?.Comment);
        }

        [Fact]
        public async void TaskMeasurerTests_RunAsyncWithCustomObjTest()
        {
            var customObj = new { prop = 1 };

            await _measurer.Run(async () =>
            {
                await Task.Delay(200);
            }, "test01", commentObj: customObj);
            IList<StatisticalDocument> list = _measurer.Get();
            Assert.Single(list);
            Assert.Equal("test01", list[0]?.Call);

            string expectedStr = JsonConvert.SerializeObject(customObj);
            JToken expected = JToken.Parse(expectedStr);
            JToken actual = JToken.Parse(list[0]?.Comment);

            Assert.True(JToken.DeepEquals(expected, actual), $"Object should be '{expectedStr}' not '{actual.ToString(Formatting.None)}'");
        }

        [Fact]
        public void TaskMeasurerTests_RunSyncTest()
        {
            _measurer.RunSync(() =>
            {
                Task.Delay(200).Wait();
            }, "test01", "comment01");
            IList<StatisticalDocument> list = _measurer.Get();
            Assert.Single(list);
            Assert.Equal("test01", list[0]?.Call);
            Assert.Equal("comment01", list[0]?.Comment);
        }

        [Fact]
        public void TaskMeasurerTests_RunSyncTestResult()
        {
            var result = _measurer.RunSync(() =>
            {
                Task.Delay(200).Wait();
                return expectedResult;
            }, "test01", "comment01");
            Assert.Equal(expectedResult.Dummy, result.Dummy);
            IList<StatisticalDocument> list = _measurer.Get();
            Assert.Single(list);
            Assert.Equal("test01", list[0]?.Call);
            Assert.Equal("comment01", list[0]?.Comment);
        }

        [Fact]
        public void TaskMeasurerTests_RunSyncWithCustomObjTest()
        {
            var customObj = new { prop = 1 };

            _measurer.RunSync(() =>
            {
                Task.Delay(200).Wait();
            }, "test01", commentObj: customObj);
            IList<StatisticalDocument> list = _measurer.Get();
            Assert.Single(list);
            Assert.Equal("test01", list[0]?.Call);

            string expectedStr = JsonConvert.SerializeObject(customObj);
            JToken expected = JToken.Parse(expectedStr);
            JToken actual = JToken.Parse(list[0]?.Comment);

            Assert.True(JToken.DeepEquals(expected, actual), $"Object should be '{expectedStr}' not '{actual.ToString(Formatting.None)}'");
        }
    }
}
