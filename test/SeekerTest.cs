using GdUnit4;
using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using static GdUnit4.Assertions;

namespace ADC.Tests
{
    [TestSuite]
    public class SeekerTest
    {
        [TestCase]
        public void TestAcquireTargetLogic()
        {
            var seeker = new Seeker();

            var enemyContainer = new Godot.Collections.Array<Node>();
            var actualEnemy = new Node2D();
            enemyContainer.Add(actualEnemy);

            seeker.AcquireTarget(enemyContainer);

            AssertThat(seeker.CurrentTarget).IsNotNull();
            AssertThat(seeker.CurrentTarget).IsEqual(actualEnemy);

            seeker.QueueFree();
            foreach (Node node in enemyContainer)
            {
                node.QueueFree();
            }
        }

        [TestCase]
        public void TestMovementVelocityPointsToTarget()
        {
            var seeker = new Seeker();

            seeker.GlobalPosition = new Vector2(0, 0);
            Vector2 dummyTargetPosition = new Vector2(50, 0);

            seeker.Velocity = seeker.CalculateVelocityToTarget(seeker.GlobalPosition, dummyTargetPosition);

            AssertThat(seeker.Velocity.X).IsGreater(0);
            AssertThat(seeker.Velocity.Y).IsEqual(0);

            seeker.QueueFree();
        }

        [TestCase]
        public void TestMovement2VelocityPointsToTarget()
        {
            var seeker = new Seeker();

            seeker.GlobalPosition = new Vector2(0, 0);
            Vector2 dummyTargetPosition = new Vector2(-50, -50);

            seeker.Velocity = seeker.CalculateVelocityToTarget(seeker.GlobalPosition, dummyTargetPosition);

            AssertThat(seeker.Velocity.X).IsLess(0);
            AssertThat(seeker.Velocity.Y).IsLess(0);

            seeker.QueueFree();
        }

        [TestCase]
        public void testWhatIfThereIsNoTarget()
        {
            var seeker = new Seeker();
            var enemyContainer = new Godot.Collections.Array<Node>();

            seeker.AcquireTarget(enemyContainer);

            AssertThat(seeker.CurrentTarget).IsNull();

            seeker.QueueFree();
            foreach (Node node in enemyContainer)
            {
                node.QueueFree();
            }
        }

        [TestCase]
        public void testWhatIfTheTargetPosIsTheCurrentPos()
        {
            var seeker = new Seeker();

            seeker.GlobalPosition = new Vector2(10, 10);
            Vector2 dummyTargetPosition = new Vector2(10, 10);

            seeker.Velocity = seeker.CalculateVelocityToTarget(seeker.GlobalPosition, dummyTargetPosition);

            AssertThat(seeker.Velocity.X).IsEqual(0);
            AssertThat(seeker.Velocity.Y).IsEqual(0);

            seeker.QueueFree();
        }

        [TestCase]
        public void testTargetSelectionByDistance()
        {
            var seeker = new Seeker();

            seeker.GlobalPosition = new Vector2(0, 0);

            var enemyContainer = new Godot.Collections.Array<Node>();

            var unreachableEnemy = new Node2D();
            unreachableEnemy.GlobalPosition = new Vector2(100000000000, 100000000000);
            unreachableEnemy.Name = "Unreachable";
            enemyContainer.Add(unreachableEnemy);

            var reachableEnemy = new Node2D();
            reachableEnemy.GlobalPosition = new Vector2(50, 0);
            reachableEnemy.Name = "Reachable";
            enemyContainer.Add(reachableEnemy);

            seeker.AcquireTarget(enemyContainer);

            AssertThat(seeker.CurrentTarget).IsEqual(reachableEnemy);
            AssertThat(seeker.CurrentTarget).IsNotEqual(unreachableEnemy);

            seeker.QueueFree();
            foreach (Node node in enemyContainer)
            {
                node.QueueFree();
            }
        }

        [TestCase]
        public void testTargetSelectionLogicWithDistance()
        {
            var seeker = new Seeker();
            seeker.GlobalPosition = new Vector2(0, 0);

            var enemyContainer = new Godot.Collections.Array<Node>();

            var enemyFar = new Node2D();
            enemyFar.GlobalPosition = new Vector2(200, 0);
            enemyContainer.Add(enemyFar);

            var enemyClose = new Node2D();
            enemyClose.GlobalPosition = new Vector2(50, 0);
            enemyContainer.Add(enemyClose);

            seeker.AcquireTarget(enemyContainer);

            AssertThat(seeker.CurrentTarget).IsEqual(enemyClose);
            AssertThat(seeker.CurrentTarget).IsNotEqual(enemyFar);

            seeker.QueueFree();
            enemyFar.QueueFree();
            enemyClose.QueueFree();
        }
    }
}