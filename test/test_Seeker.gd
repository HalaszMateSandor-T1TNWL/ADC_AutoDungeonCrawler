extends "res://addons/gut/test.gd"

var SeekerClass = load("res://Scripts/Seeker.cs")

func test_acquire_target_sets_correct_enemy():
	var seeker = SeekerClass.new()
	var agent = NavigationAgent2D.new()
	agent.name = "NavigationAgent2D"
	seeker.add_child(agent)
	
	var enemy_container = Node2D.new()
	enemy_container.add_to_group("enemy")
	var actual_enemy = Node2D.new()
	enemy_container.add_child(actual_enemy)
	
	add_child_autofree(seeker)
	add_child_autofree(enemy_container)
	
	seeker.AcquireTarget()
	
	assert_not_null(seeker.CurrentTarget)
	assert_eq(seeker.CurrentTarget, actual_enemy)
