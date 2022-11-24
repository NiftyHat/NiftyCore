using System.Collections.Generic;
using NiftyFramework.UnityUtils;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Pooling
{
    public class MonoPoolListTests
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
        
        private static List<TestView> GetPrototypeList(int itemCount)
        {
            List<TestView> list = new List<TestView>();
            for (int i = 0; i < itemCount; i++)
            {
                GameObject prototypeObject = new GameObject();
                var comp = prototypeObject.AddComponent<TestView>();
                list.Add(comp);
            }
            return list;
        }
        
        private static TestView[] GetPrototypeArray(int itemCount)
        {
            TestView[] list = new TestView[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                GameObject prototypeObject = new GameObject();
                var comp = prototypeObject.AddComponent<TestView>();
                list[i] = comp;
            }
            return list;
        }

        private static MonoPool<TestView> GetIListMonoPool(IList<TestView> items, int maxSize = -1)
        {
            return new MonoPool<TestView>(items, maxSize);
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void MonoPool_Should_InitializeFromList(int listSize = -1)
        {
            List<TestView> viewList = GetPrototypeList(listSize);
            MonoPool<TestView> pool = GetIListMonoPool(viewList);
            Assert.AreEqual(pool.Count,viewList.Count);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void MonoPool_Should_InitializeFromArray(int listSize = -1)
        {
            TestView[] viewList = GetPrototypeList(listSize).ToArray();
            MonoPool<TestView> pool = GetIListMonoPool(viewList);
            Assert.AreEqual(pool.Count,viewList.Length);
        }
        
        [TestCase(1,1)]
        [TestCase(5,50)]
        [TestCase(100, 50)]
        [TestCase(1, 0)]
        public static void MonoPool_MaxSize_GreaterOrEqualToListSize(int listSize = -1, int maxSize = -1)
        {
            List<TestView> viewList = GetPrototypeList(listSize);
            MonoPool<TestView> pool = GetIListMonoPool(viewList, maxSize);
            Assert.GreaterOrEqual(pool.MaxSize,viewList.Count);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public static void Get_ListShould_ReturnUniqueInstances(int size = -1)
        {
            List<TestView> viewList = GetPrototypeList(size);
            MonoPool<TestView> pool = GetIListMonoPool(viewList);
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

    }
}