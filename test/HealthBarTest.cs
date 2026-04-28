/*using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

namespace ADC.Tests
{
	[TestSuite]
	public class HealthBarTest
	{
		[TestCase]
		public void TestTakeDamageDecreasesHealthAndClampsAtZero()
		{
			var healthBar = new SeekerHealthBar();
			healthBar.MaxHealth = 100.0f;
			healthBar.CurrentHealth = 100.0f;

			healthBar.TakeDamage(30.0f);
			AssertThat(healthBar.CurrentHealth).IsEqual(70.0f);

			healthBar.TakeDamage(100.0f);
			AssertThat(healthBar.CurrentHealth).IsEqual(0.0f);

			healthBar.QueueFree();
		}

		[TestCase]
		public void TestHealIncreasesHealthAndClampsAtMax()
		{
			var healthBar = new SeekerHealthBar();
			healthBar.MaxHealth = 100.0f;
			healthBar.CurrentHealth = 50.0f;

			healthBar.Heal(20.0f);
			AssertThat(healthBar.CurrentHealth).IsEqual(70.0f);

			healthBar.Heal(50.0f);
			AssertThat(healthBar.CurrentHealth).IsEqual(100.0f);

			healthBar.QueueFree();
		}

		[TestCase]
		public void TestVisibilityLogicBasedOnHealth()
		{
			var healthBar = new SeekerHealthBar();
			healthBar.MaxHealth = 100.0f;
			
			healthBar.CurrentHealth = 100.0f;
			healthBar.UpdateHealthBar();
			AssertThat(healthBar.Visible).IsFalse();

			healthBar.CurrentHealth = 50.0f;
			healthBar.UpdateHealthBar();
			AssertThat(healthBar.Visible).IsTrue();

			healthBar.CurrentHealth = 0.0f;
			healthBar.UpdateHealthBar();
			AssertThat(healthBar.Visible).IsFalse();

			healthBar.QueueFree();
		}
	}
}*/
