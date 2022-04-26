using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeShallNotPass.Model;

namespace WeShallNotPass.Test
{
    [TestClass]
    public class ModelTest
    {
        private Model.Model model;

        [TestInitialize]
        public void Initialize()
        {
            model = new Model.Model();
            model.GameAreaSize = 14;
            model.NewGame();
        }

        [TestMethod]
        public void NewGameTest()
        { 
            Assert.IsNotNull(model.GameArea);
            Assert.IsNotNull(model.Games);
            Assert.IsNotNull(model.Restaurants);
            Assert.IsNotNull(model.Restrooms);
            Assert.IsNotNull(model.Visitors);
            Assert.IsNotNull(model.Plants);
            Assert.IsNotNull(model.Generators);
            Assert.IsNotNull(model.Plants);

            Assert.AreEqual(model.Generators.Count, 1);
            Assert.AreEqual(model.Money, 15000);
            Assert.AreEqual(model.CampaignTime, 0);
            Assert.AreEqual(model.GameTime, "00:00");
            Assert.AreEqual(model.IsOpen, false);
        }

        [TestMethod]
        public void BuildTest()
        {
            int originalAmount = model.Money;
            Item item = new Item(
                1,
                1,
                "SecondItem",
                2,
                2,
                new Uri("/Images/placeholder.png", UriKind.Relative),
                100,
                0);
            model.Build(item);

            Assert.AreEqual(model.Money, originalAmount - item.Price);
            Assert.AreEqual(model.GameArea[1, 1], item);
            Assert.AreEqual(model.GameArea[1, 2], item);
            Assert.AreEqual(model.GameArea[2, 1], item);
            Assert.AreEqual(model.GameArea[2, 2], item);
        }

        [TestMethod]
        public void BuildItemHasNoSpace()
        {
            int originalAmount = model.Money;
            Item item = new Item(
                model.GameAreaSize-1,
                model.GameAreaSize - 1,
                "TestItem",
                2,
                2,
                new Uri("/Images/placeholder.png", UriKind.Relative),
                0,
                0);
            model.Build(item);

            Assert.AreEqual(model.Money, originalAmount); // The item was not built
        }

        [TestMethod]
        public void BuildSpaceIsOccupied()
        {
            int originalAmount = model.Money;
            Item firstItem = new Item(
                1,
                1,
                "FirstItem",
                2,
                2,
                new Uri("/Images/placeholder.png", UriKind.Relative),
                100,
                0);
            model.Build(firstItem);

            Item secondItem = new Item(
                1,
                2,
                "SecondItem",
                2,
                2,
                new Uri("/Images/placeholder.png", UriKind.Relative),
                200,
                0);
            model.Build(secondItem);

            Assert.AreEqual(model.GameArea[1, 2], firstItem);
            Assert.AreEqual(model.Money, originalAmount - firstItem.Price);
        }

        [TestMethod]
        public void NotEnoughMoneyToBuildTest()
        {
            int originalAmount = model.Money;
            Item item = new Item(
                1,
                1,
                "TestItem",
                2,
                2,
                new Uri("/Images/placeholder.png", UriKind.Relative),
                model.Money + 100,
                0);
            model.Build(item);

            Assert.AreEqual(model.Money, originalAmount);
            Assert.AreNotEqual(model.GameArea[2, 2], item);
        }
    }
}
