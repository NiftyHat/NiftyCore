using System;
using System.Collections;
using System.Collections.Generic;
using NiftyFramework.UnityUtils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NiftyFramework.Tests.Pooling
{
    public class MonoPoolProtoypeTest
    {
        private class TestView : MonoBehaviour
        {
            public TestView()
            {
            }
        }
        
        private static TestView GetPrototype()
        {
            GameObject prototypeObject = new GameObject();
            var comp = prototypeObject.AddComponent<TestView>();
            return comp;
        }

        private static MonoPool<TestView> GetPrototypeMonoPool(int maxSize = -1, int initialCount = -1)
        {
            TestView prototype = GetPrototype();
            return new MonoPool<TestView>(prototype, maxSize, initialCount);
        }
        
        [Test]
        public static void Get_Prototype_IsAllocated()
        {
            TestView prototype = GetPrototype();
            Assert.IsNotNull(prototype.gameObject);
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void MonoPool_Should_HaveCountLessThanMaxSize(int maxSize = -1)
        {
            MonoPool<TestView> pool = GetPrototypeMonoPool(maxSize);
            Assert.LessOrEqual(pool.Count, maxSize);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void MonoPool_Should_PrewarmToInitialSize(int initialCount = -1)
        {
            MonoPool<TestView> pool = GetPrototypeMonoPool(-1, initialCount);
            Assert.GreaterOrEqual(pool.Count, initialCount);
        }
        
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void Get_Should_ReturnUniqueInstances(int size = -1)
        {
            MonoPool<TestView> pool = GetPrototypeMonoPool(-1, size);
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
        public static void MonoPool_Should_CreateFromPrototype()
        {
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype);
            testPool.TryGet(out var instance);
            Assert.NotNull(instance);
        }
        
        [Test]
        public static void MonoPool_Should_NotUsePrototypeInstance()
        {
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype);
            testPool.TryGet(out var instance);
            Assert.AreNotSame(instance, prototype);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void Prewarm_Should_NotUsePrototypeInstance(int size)
        {
            var prototype = GetPrototype();
            MonoPool<TestView> pool = new MonoPool<TestView>(prototype, -1, size);
            for (int i = 0; i < size; i++)
            {
                if (pool.TryGet(out var instance))
                {
                    Assert.AreNotSame(instance, prototype);
                }
            }
        }

        [Test]
        public static void TryReturn_Should_ReducePoolCount()
        {
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype);
            if (testPool.TryGet(out var instance))
            {
                testPool.TryReturn(instance);
            }
            Assert.AreEqual(testPool.Count, 1);
        }
        
        [Test]
        public static void TryReturn_Should_NotReturnPooledItem()
        {
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype);
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
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype);
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
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype,size);
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
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype,maxSize);
            testPool.TryGet(out HashSet<TestView> instanceList, getCount);
            Assert.AreEqual( getCount, instanceList.Count);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void Dispose_Clears_Pool(int size)
        {
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype,size);
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
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype);
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
            var prototype = GetPrototype();
            MonoPool<TestView> testPool = new MonoPool<TestView>(prototype,size);
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
