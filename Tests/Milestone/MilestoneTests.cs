namespace NiftyCore.Tests
{
    public static class MilestoneTests
    {
        public class MilestoneTest : Milestone
        {
            public MilestoneTest()
            {
                
            }
        }

        [Test]
        public static void MilestoneShouldHaveName()
        {
            MilestoneTest = new MilestoneTest();
        }
    }
}