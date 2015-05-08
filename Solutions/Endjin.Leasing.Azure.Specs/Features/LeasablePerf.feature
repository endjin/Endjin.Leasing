@ReleaseLeases @container
Feature: Leasable Perf

@container @storage_emulator
Scenario: Create 100 leasables
	When I run 100 actions using leasable
	Then the result should take less than 20 second(s)

@container @storage_emulator
Scenario: Run 10 mutex actions simultaneously with 5ms action
	Given an action takes 5 ms
	When I run 10 mutex actions simultaneously
	Then the result should take less than 3 second(s)

@container @storage_emulator
Scenario: Run 10 actions consecutively with 5ms action
	Given an action takes 5 ms
	When I run 10 actions consecutively
	Then the result should take less than 3 second(s)

@container @storage_emulator
Scenario: Run 10 actions simultaneously with 5ms action
	Given an action takes 5 ms
	When I run 10 actions simultaneously
	Then the result should take less than 3 second(s)

@container @storage_emulator
Scenario: Run 10 mutex actions simultaneously with 50ms action
	Given an action takes 50 ms
	When I run 10 mutex actions simultaneously
	Then the result should take less than 3 second(s)

@container @storage_emulator
Scenario: Run 10 actions consecutively with 50ms action
	Given an action takes 50 ms
	When I run 10 actions consecutively
	Then the result should take less than 3 second(s)

@container @storage_emulator
Scenario: Run 10 actions simultaneously with 50ms action
	Given an action takes 50 ms
	When I run 10 actions simultaneously
	Then the result should take less than 3 second(s)

@container @storage_emulator
Scenario: Run 10 mutex actions simultaneously with 500ms action
	Given an action takes 500 ms
	When I run 10 mutex actions simultaneously
	Then the result should take less than 10 second(s)

@container @storage_emulator
Scenario: Run 10 actions consecutively with 500ms action
	Given an action takes 500 ms
	When I run 10 actions consecutively
	Then the result should take less than 10 second(s)

@container @storage_emulator
Scenario: Run 10 actions simultaneously with 500ms action
	Given an action takes 500 ms
	When I run 10 actions simultaneously
	Then the result should take less than 10 second(s)

@container @storage_emulator
Scenario: Run 10 mutex actions simultaneously with 1000ms action
	Given an action takes 1000 ms
	When I run 10 mutex actions simultaneously
	Then the result should take less than 20 second(s)

@container @storage_emulator
Scenario: Run 10 actions consecutively with 1000ms action
	Given an action takes 1000 ms
	When I run 10 actions consecutively
	Then the result should take less than 20 second(s)

@container @storage_emulator
Scenario: Run 10 actions simultaneously with 1000ms action
	Given an action takes 1000 ms
	When I run 10 actions simultaneously
	Then the result should take less than 20 second(s)