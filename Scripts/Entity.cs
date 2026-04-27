using Godot;
using System;

public partial class Entity : CharacterBody2D
{
    public float damage = 0.0f;
    public float maxHealth = 0.0f;
    public float CurrentHealth = 0.0f;
    public float movementSpeed = 100.0f;
    public int attackRange = 0;
    public Pathfinder pathfinding;

    public virtual void OnQueueForFree(){}
}