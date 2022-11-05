using System;
using System.Collections;
using System.Collections.Generic;
using NiftyFramework.UnityUtils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NiftyFramework.Tests.Pooling
{
    public class MonoPoolFactoryTests
    {
        private class TestView : MonoBehaviour
        {
            public TestView()
            {
            }
        }
        
        private static TestView CreateTestViewInstance()
        {
            GameObject prototypeObject = new GameObject();
            var comp = prototypeObject.AddComponent<TestView>();
            return comp;
        }
        
        private static MonoPool<TestView> GetFactoryMonoPool(int maxSize = -1, int initialCount = -1)
        {
            return new MonoPool<TestView>(CreateTestViewInstance, maxSize, initialCount);
        }

        [Test]
        public static void MonoPool_Should_CreateFromFactoryMethod()
        {
            var pool = new MonoPool<TestView>(CreateTestViewInstance);
            pool.TryGet(out var instance);
            Assert.IsNotNull(instance);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void MonoPool_Should_HaveCountLessThanMaxSize(int maxSize = -1)
        {
            MonoPool<TestView> pool = GetFactoryMonoPool(maxSize);
            Assert.LessOrEqual(pool.Count, maxSize);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void MonoPool_Should_PrewarmToInitialSize(int initialCount = -1)
        {
            MonoPool<TestView> pool = GetFactoryMonoPool(-1, initialCount);
            Assert.GreaterOrEqual(pool.Count, initialCount);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void Get_Should_ReturnUniqueInstances(int size = -1)
        {
            MonoPool<TestView> pool = GetFactoryMonoPool(-1, size);
            HashSet<TestView> allInstanced = new HashSet<TestView>();
            for (int i = 0; i < size; i++)
            {
                if (pool.TryGet(out var instance))
                {
                    Assert.That(!allInstanced.Contains(instance));
                    allInstanced.Add(instance);
                }
            }
        }
        
        [Test]
        public static void TryReturn_Should_ReducePoolCount()
        {
            MonoPool<TestView> testPool = GetFactoryMonoPool();
            if (testPool.TryGet(out var instance))
            {
                testPool.TryReturn(instance);
            }
            Assert.AreEqual(testPool.Count, 1);
        }
        
        [Test]
        public static void TryReturn_Should_NotReturnPooledItem()
        {
            MonoPool<TestView> testPool = GetFactoryMonoPool();
            if (testPool.TryGet(out var instance))
            {
                testPool.TryReturn(instance);
                testPool.TryReturn(instance);
            }
            Assert.AreEqual(testPool.Count, 1);
        }
        
        [Test]
        public static void TryReturn_ReturnFalse_OnPooledItems()
        {
            MonoPool<TestView> testPool = GetFactoryMonoPool();
            if (testPool.TryGet(out var instance))
            {
                testPool.TryReturn(instance);
                Assert.IsFalse(testPool.TryReturn(instance));
            }
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void TryReturn_ReturnFalse_OnPoolFull(int size)
        {
            MonoPool<TestView> testPool = GetFactoryMonoPool(size);
            //overflow the pool size
            List<TestView> instanceItems = new List<TestView>();
            for (int i = 0; i <= size + 1; i++)
            {
                if (testPool.TryGet(out var instance))
                {
                    instanceItems.Add(instance);
                }
            }
            for (int i = 0; i < instanceItems.Count; i++)
            {
                var instance = instanceItems[i];
                if (i == size + 1)
                {
                    Assert.IsFalse(testPool.TryReturn(instance));
                }
                testPool.TryReturn(instance);
            }
        }
        
        [TestCase(0,1)]
        [TestCase(1,1)]
        [TestCase(5,5)]
        [TestCase(5,3)]
        [TestCase(100,99)]
        [TestCase(100,1)]
        public static void TryGet_MaxSize_ReturnsCountItems(int maxSize, int getCount)
        {
            MonoPool<TestView> testPool = GetFactoryMonoPool(maxSize);
            testPool.TryGet(out HashSet<TestView> instanceList, getCount);
            Assert.AreEqual( getCount, instanceList.Count);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void Dispose_Clears_Pool(int size)
        {
            MonoPool<TestView> testPool = GetFactoryMonoPool(size);
            testPool.TryGet(out HashSet<TestView> instanceList, size);
            foreach (var item in instanceList)
            {
                testPool.TryReturn(item);
            }
            testPool.Dispose();
            Assert.AreEqual(0, testPool.Count);
        }
        
        [Test]
        public static void Dispose_Should_ExceptOnNullCallback()
        {
            MonoPool<TestView> testPool = GetFactoryMonoPool();
            void DisposePool()
            {
                testPool.Dispose(null);
            }
            Assert.Throws<ArgumentException>(DisposePool);
        }
        
        [UnityTest]
        public static IEnumerator Dispose_Destroys_GameObjects()
        {
            int size = 100;
            MonoPool<TestView> testPool = GetFactoryMonoPool(size);
            testPool.TryGet(out HashSet<TestView> instanceList, size);
            foreach (var item in instanceList)
            {
                testPool.TryReturn(item);
            }
            testPool.Dispose();
            yield return new WaitForEndOfFrame();
            foreach (var item in instanceList)
            {
                Assert.IsTrue((item == null));
            }
        }

    }
}