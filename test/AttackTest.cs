using GdUnit4;
using Godot;
using System.Collections.Generic;
using static GdUnit4.Assertions;

namespace ADC.Tests
{
    [TestSuite]
    public class AttackTest
    {
        [TestCase]
        public void AttackAppliesDamageWhenTargetIsInRange()
        {
            var attacker = new Entity();
            attacker.damage = 15;
            attacker.attackRange = 2;
            attacker.attackspeed = 1.0f;

            var pathfinder = new Pathfinder();
            pathfinder.Name = "Pathfinder";
            pathfinder._parent = attacker;
            pathfinder._currentIdPath = new Godot.Collections.Array<Vector2I> { new Vector2I(0, 0) };

            var target = new Entity();
            target.CurrentHealth = 100;

            var targetContainer = new Godot.Collections.Array<Node> { target };
            pathfinder.AcquireTarget(targetContainer);

            var attack = new Attack();

            attacker.AddChild(pathfinder);
            attacker.AddChild(attack);

            attack._Ready();
            attack._PhysicsProcess(0.1f);

            AssertThat(target.CurrentHealth).IsEqual(85.0f);

            attacker.QueueFree();
            target.QueueFree();
        }

        [TestCase]
        public void AttackIsIgnoredWhenTargetIsOutOfRange()
        {
            var attacker = new Entity();
            attacker.damage = 20;
            attacker.attackRange = 1;
            attacker.attackspeed = 1.0f;

            var pathfinder = new Pathfinder();
            pathfinder.Name = "Pathfinder";
            pathfinder._parent = attacker;
            pathfinder._currentIdPath = new Godot.Collections.Array<Vector2I>
            {
                new Vector2I(0, 0),
                new Vector2I(1, 1),
                new Vector2I(2, 2)
            };

            var target = new Entity();
            target.CurrentHealth = 100;

            var targetContainer = new Godot.Collections.Array<Node> { target };
            pathfinder.AcquireTarget(targetContainer);

            var attack = new Attack();

            attacker.AddChild(pathfinder);
            attacker.AddChild(attack);

            attack._Ready();
            attack._PhysicsProcess(0.1f);

            AssertThat(target.CurrentHealth).IsEqual(100.0f);

            attacker.QueueFree();
            target.QueueFree();
        }

        [TestCase]
        public void CooldownPreventsMultipleAttacksInTheSameTimeframe()
        {
            var attacker = new Entity();
            attacker.damage = 10;
            attacker.attackRange = 5;
            attacker.attackspeed = 1.0f;

            var pathfinder = new Pathfinder();
            pathfinder.Name = "Pathfinder";
            pathfinder._parent = attacker;
            pathfinder._currentIdPath = new Godot.Collections.Array<Vector2I> { new Vector2I(0, 0) };

            var target = new Entity();
            target.CurrentHealth = 100;

            var targetContainer = new Godot.Collections.Array<Node> { target };
            pathfinder.AcquireTarget(targetContainer);

            var attack = new Attack();

            attacker.AddChild(pathfinder);
            attacker.AddChild(attack);

            attack._Ready();

            attack._PhysicsProcess(0.1f);

            attack._PhysicsProcess(0.1f);
            attack._PhysicsProcess(0.1f);

            AssertThat(target.CurrentHealth).IsEqual(90.0f);

            attacker.QueueFree();
            target.QueueFree();
        }

        [TestCase]
        public void SequentialAttacksProcessMultipleTargets()
        {
            var attacker = new Entity();
            attacker.damage = 10;
            attacker.attackRange = 5;
            attacker.attackspeed = 1.0f;

            var pathfinder = new Pathfinder();
            pathfinder.Name = "Pathfinder";
            pathfinder._parent = attacker;
            pathfinder._currentIdPath = new Godot.Collections.Array<Vector2I> { new Vector2I(0, 0) };

            var attack = new Attack();

            attacker.AddChild(pathfinder);
            attacker.AddChild(attack);

            attack._Ready();

            var enemies = new List<Entity>();
            for (int i = 0; i < 3; i++)
            {
                var enemy = new Entity();
                enemy.CurrentHealth = 50;
                enemies.Add(enemy);
            }

            foreach (var enemy in enemies)
            {
                var targetContainer = new Godot.Collections.Array<Node> { enemy };
                pathfinder.AcquireTarget(targetContainer);

                attack._PhysicsProcess(0.1f);

                AssertThat(enemy.CurrentHealth).IsEqual(40.0f);

                var timer = attack.GetChild<Timer>(0);
                timer.EmitSignal(Timer.SignalName.Timeout);
            }

            attacker.QueueFree();
            foreach (var enemy in enemies)
            {
                enemy.QueueFree();
            }
        }


        //TDD
        [TestCase]
        public void FriendlyTargetsAreIgnored()
        {
            var attacker = new Entity();
            attacker.damage = 20;
            attacker.attackRange = 3;
            attacker.attackspeed = 1.0f;

            var pathfinder = new Pathfinder();
            pathfinder.Name = "Pathfinder";
            pathfinder._parent = attacker;
            pathfinder._currentIdPath = new Godot.Collections.Array<Vector2I> { new Vector2I(0, 0) };

            var friendlyTarget = new Entity();
            friendlyTarget.CurrentHealth = 100;
            friendlyTarget.AddToGroup("Friendly");

            var targetContainer = new Godot.Collections.Array<Node> { friendlyTarget };
            pathfinder.AcquireTarget(targetContainer);

            var attack = new Attack();

            attacker.AddChild(pathfinder);
            attacker.AddChild(attack);

            attack._Ready();
            attack._PhysicsProcess(0.1f);

            AssertThat(friendlyTarget.CurrentHealth).IsEqual(100.0f);

            attacker.QueueFree();
            friendlyTarget.QueueFree();
        }
    }
}