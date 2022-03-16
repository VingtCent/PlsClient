namespace PlsClient.Tests.Models
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PlsClient.Models;

    [TestClass]
    public class GameModelTests
    {
        private readonly GameModel game;

        public GameModelTests()
        {
            this.game = new GameModel();
        }

        [TestMethod]
        public void TotalPlayerCards_ShouldBeCalculated()
        {
            this.game.Cities = 48;
            this.game.Objectives = 1;
            this.game.Aggravations = 5;
            this.game.Refuelings = 3;
            this.game.Players = 4;
            
            int expectedTotalCards = 48 + 5 + 3 - 1 - 4 * 2;
            Assert.AreEqual(expectedTotalCards, this.game.TotalPlayerCards);
        }

        [TestMethod]
        public void Start_ShouldStartGame()
        {
            this.game.Start();

            Assert.IsTrue(this.game.IsStarted);
        }

        [TestMethod]
        public void Start_ShouldCreateHeaps()
        {
            this.game.Aggravations = 3;

            this.game.Start();
            Assert.AreEqual(3, this.game.PlayerCardHeaps.Count);
        }

        [TestMethod]
        public void Start_With4Players_ShouldCreateHeapsWithInitialNumber()
        {
            StartGame();

            Assert.AreEqual(10, this.game.PlayerCardHeaps[0].InitialNumberOfCards);
            Assert.AreEqual(10, this.game.PlayerCardHeaps[1].InitialNumberOfCards);
            Assert.AreEqual(9, this.game.PlayerCardHeaps[2].InitialNumberOfCards);
            Assert.AreEqual(9, this.game.PlayerCardHeaps[3].InitialNumberOfCards);
            Assert.AreEqual(9, this.game.PlayerCardHeaps[4].InitialNumberOfCards);
        }

        [TestMethod]
        public void Start_With3Players_ShouldCreateHeapsWithInitialNumber()
        {
            this.game.Cities = 33;
            this.game.Objectives = 0;
            this.game.Aggravations = 3;
            this.game.Refuelings = 0;
            this.game.Players = 3;

            // Total = 36 => 10 10 10

            this.game.Start();
            Assert.AreEqual(30, this.game.TotalPlayerCards);
            Assert.AreEqual(3, this.game.PlayerCardHeaps.Count);
            Assert.AreEqual(10, this.game.PlayerCardHeaps[0].InitialNumberOfCards);
            Assert.AreEqual(10, this.game.PlayerCardHeaps[1].InitialNumberOfCards);
            Assert.AreEqual(10, this.game.PlayerCardHeaps[2].InitialNumberOfCards);
        }

        [TestMethod]
        [DataRow(PlayerCardType.Aggravation)]
        [DataRow(PlayerCardType.City)]
        [DataRow(PlayerCardType.Refueling)]
        public void DrawCard_ShouldAddCardToDrawnCards(PlayerCardType card)
        {
            StartGame();

            this.game.Draw(card);

            Assert.AreEqual(card, this.game.DrawnCards.Last());
        }

        [TestMethod]
        public void DrawCard_ShouldKeepOrder()
        {
            StartGame();

            var cards = new []{PlayerCardType.City, PlayerCardType.City, PlayerCardType.City, PlayerCardType.Aggravation, PlayerCardType.Refueling};
            foreach (var card in cards)
            {
                this.game.Draw(card);
            }

            CollectionAssert.AreEqual(cards, this.game.DrawnCards.ToList());
        }

        [TestMethod]
        public void DrawCard_ShouldDecreaseLastNotEmptyHeap()
        {
            StartGame();

            for (int i = 0; i < 10; i++)
            {
                this.game.Draw(i == 0 ? PlayerCardType.Aggravation : PlayerCardType.City);
                Assert.AreEqual(this.game.PlayerCardHeaps[0].InitialNumberOfCards - (i + 1), this.game.PlayerCardHeaps[0].RemainingCards);
                Assert.AreEqual(i + 1, this.game.PlayerCardHeaps[0].DrawnCards);

                Assert.AreEqual(this.game.PlayerCardHeaps[1].InitialNumberOfCards, this.game.PlayerCardHeaps[1].RemainingCards);
                Assert.AreEqual(this.game.PlayerCardHeaps[2].InitialNumberOfCards, this.game.PlayerCardHeaps[2].RemainingCards);
                Assert.AreEqual(this.game.PlayerCardHeaps[3].InitialNumberOfCards, this.game.PlayerCardHeaps[3].RemainingCards);
                Assert.AreEqual(this.game.PlayerCardHeaps[4].InitialNumberOfCards, this.game.PlayerCardHeaps[4].RemainingCards);
            }

            for (int i = 0; i < 10; i++)
            {
                this.game.Draw(i == 0 ? PlayerCardType.Aggravation : PlayerCardType.City);

                Assert.AreEqual(0, this.game.PlayerCardHeaps[0].RemainingCards);

                Assert.AreEqual(this.game.PlayerCardHeaps[1].InitialNumberOfCards - (i + 1), this.game.PlayerCardHeaps[1].RemainingCards);
                Assert.AreEqual(i + 1, this.game.PlayerCardHeaps[1].DrawnCards);

                Assert.AreEqual(this.game.PlayerCardHeaps[2].InitialNumberOfCards, this.game.PlayerCardHeaps[2].RemainingCards);
                Assert.AreEqual(this.game.PlayerCardHeaps[3].InitialNumberOfCards, this.game.PlayerCardHeaps[3].RemainingCards);
                Assert.AreEqual(this.game.PlayerCardHeaps[4].InitialNumberOfCards, this.game.PlayerCardHeaps[4].RemainingCards);
            }

            for (int i = 0; i < 9; i++)
            {
                this.game.Draw(i == 0 ? PlayerCardType.Aggravation : PlayerCardType.City);

                Assert.AreEqual(0, this.game.PlayerCardHeaps[0].RemainingCards);
                Assert.AreEqual(0, this.game.PlayerCardHeaps[1].RemainingCards);

                Assert.AreEqual(this.game.PlayerCardHeaps[2].InitialNumberOfCards - (i + 1), this.game.PlayerCardHeaps[2].RemainingCards);
                Assert.AreEqual(i + 1, this.game.PlayerCardHeaps[2].DrawnCards);

                Assert.AreEqual(this.game.PlayerCardHeaps[3].InitialNumberOfCards, this.game.PlayerCardHeaps[3].RemainingCards);
                Assert.AreEqual(this.game.PlayerCardHeaps[4].InitialNumberOfCards, this.game.PlayerCardHeaps[4].RemainingCards);
            }

            for (int i = 0; i < 9; i++)
            {
                this.game.Draw(i == 0 ? PlayerCardType.Aggravation : PlayerCardType.City);

                Assert.AreEqual(0, this.game.PlayerCardHeaps[0].RemainingCards);
                Assert.AreEqual(0, this.game.PlayerCardHeaps[1].RemainingCards);
                Assert.AreEqual(0, this.game.PlayerCardHeaps[2].RemainingCards);

                Assert.AreEqual(this.game.PlayerCardHeaps[3].InitialNumberOfCards - (i + 1), this.game.PlayerCardHeaps[3].RemainingCards);
                Assert.AreEqual(i + 1, this.game.PlayerCardHeaps[3].DrawnCards);

                Assert.AreEqual(this.game.PlayerCardHeaps[4].InitialNumberOfCards, this.game.PlayerCardHeaps[4].RemainingCards);
            }

            for (int i = 0; i < 9; i++)
            {
                this.game.Draw(i == 0 ? PlayerCardType.Aggravation : PlayerCardType.City);

                Assert.AreEqual(0, this.game.PlayerCardHeaps[0].RemainingCards);
                Assert.AreEqual(0, this.game.PlayerCardHeaps[1].RemainingCards);
                Assert.AreEqual(0, this.game.PlayerCardHeaps[2].RemainingCards);
                Assert.AreEqual(0, this.game.PlayerCardHeaps[3].RemainingCards);

                Assert.AreEqual(this.game.PlayerCardHeaps[4].InitialNumberOfCards - (i + 1), this.game.PlayerCardHeaps[4].RemainingCards);
                Assert.AreEqual(i + 1, this.game.PlayerCardHeaps[4].DrawnCards);
            }
        }

        /// <summary>
        /// 10 10 9 9 9
        /// </summary>
        private void StartGame()
        {
            this.game.Cities = 48;
            this.game.Objectives = 1;
            this.game.Aggravations = 5;
            this.game.Refuelings = 3;
            this.game.Players = 4;

            this.game.Start();
        }
    }
}
