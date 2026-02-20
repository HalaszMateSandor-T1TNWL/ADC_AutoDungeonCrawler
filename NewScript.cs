using Godot;
using System;

public partial class PlayerTest : CharacterBody2D
{
    [Export]
    public float Speed = 300.0f;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Bemenetek lekérése (alapértelmezett ui_ irányok)
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (direction != Vector2.Zero)
        {
            velocity = direction * Speed;
        }
        else
        {
            velocity = velocity.MoveToward(Vector2.Zero, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();

        // Teszt funkció: Ha megnyomod a Space-t, írjon ki valamit a konzolra
        if (Input.IsActionJustPressed("ui_accept"))
        {
            GD.Print("Teszt: Interakció történt! A játékos pozíciója: " + GlobalPosition);
            PerformQuickTest();
        }
    }

    private void PerformQuickTest()
    {
        // Egy egyszerű logikai ellenőrzés példa
        GD.Print("C# Script futtatása sikeres a Godot-ban.");
    }
}