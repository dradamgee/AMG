using System;
using System.Linq;
using AMG.FySics;
using GraphicsSandbox;
using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace GraphicsSandboxTests
{
    [TestClass]
    public class TestCodeChangeBall
    {
        [TestMethod]
        public void WhenOneItem_MassIsOne()
        {
            var paths = new[]
                {
                    @"OM/Client/Caching/DataSources.cs"
                };

            var asd = ListModule.OfSeq((paths.Select(
                line => ListModule.OfSeq(line.Split('/')
                ))));

            var rootNode = TreeModule.buildRoot(asd);


            Vector location = new Vector(1.0, 1.0);
            Velocity velocity = new Velocity(new Vector(1.0, 1.0));
            
            TreeNodeBall ccb = new TreeNodeBall(rootNode, location, velocity);
            
            Assert.AreEqual(1, ccb.Mass);
        }

        [TestMethod]
        public void When4Items_MassIs4()
        {
            var paths = new[]
            {
                @"WindowManager.cs",
                @"Aggregation.cs",
                @"BrokerRepository.cs",
                @"DataSources.cs"
            };

            var asd = ListModule.OfSeq((paths.Select(
                line => ListModule.OfSeq(line.Split('/')
                ))));

            var rootNode = TreeModule.buildRoot(asd);


            Vector location = new Vector(1.0, 1.0);
            Velocity velocity = new Velocity(new Vector(1.0, 1.0));
            

            TreeNodeBall ccb = new TreeNodeBall(rootNode, location, velocity);

            Assert.AreEqual(4, ccb.Mass);
        }

        [TestMethod]
        public void Give3SubGroups_Split_3ItemsResult()
        {
            var paths = new[]
            {
                @"Client/Application/WindowManagers/WindowManager.cs",
                @"Client/Caching/Aggregation.cs",
                @"OM/Common/MOMGA/DomainHelpers.fs",
                @"OM/Common/MOMGA/DomainHelpers.fs",
                @"Client/Caching/DataSources.cs",
                @"Services/Orders/Common/Test/Helpers/ExecRptBuilder.cs",
                @"Services/Orders/Common/Test/Helpers/SubOrderBuilder.cs",
            };

            var asd = ListModule.OfSeq((paths.Select(
                line => ListModule.OfSeq(line.Split('/')
                ))));

            var rootNode = TreeModule.buildRoot(asd);

            Vector location = new Vector(1.0, 1.0);
            Velocity velocity = new Velocity(new Vector(1.0, 1.0));
            
            TreeNodeBall ccb = new TreeNodeBall(rootNode, location, velocity);

            var count = ccb.Split().Count();

            Assert.AreEqual(3, count);
        }
    }
}
