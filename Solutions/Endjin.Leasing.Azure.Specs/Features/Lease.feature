@ReleaseLeases
Feature: Lease
	In order to allow an actor in the system to perform an exclusive operation 
	As an actor in the system
	I want to obtain an exclusive lease for the duration of the operation I have to perform

@storage_emulator
Scenario: Acquire a lease with valid policy
	Given I am the only actor trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 15 seconds
	When I acquire the lease
	Then the lease should expire in the future
	Then it should retain the lease for 15 seconds
	And the lease should be expired after 15 seconds

@storage_emulator
Scenario: Acquire a lease with invalid duration
	Given I am the only actor trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 80 seconds
	When I acquire the lease
	Then it should throw an AggregateException containing ArgumentOutOfRangeException

@storage_emulator
Scenario: Acquire a lease with invalid name
	Given I am the only actor trying to perform an operation called "long-running-task."
	And I want to acquire a lease for 15 seconds
	When I acquire the lease
	Then it should throw an AggregateException containing ArgumentException

@storage_emulator
Scenario: A single actor reacquires a lease that it already has acquired
	Given I am the only actor trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 15 seconds
	And I have already acquired the lease
	When I acquire the lease
	Then the lease should expire in the future
	Then it should retain the lease for 15 seconds
	And the lease should be expired after 15 seconds

@storage_emulator
Scenario: Actor B tries to acquire a lease after Actor A has already acquired it
	Given I am actor B trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 15 seconds
	And Actor A has already acquired a lease for an operation called "long-running-task"
	When I acquire the lease
	Then I should not have acquired the lease
	And the lease ID should not be set
	And the lease acquired date should not be set
	And the lease expires date should not be set

@storage_emulator
Scenario: A single actor renews a lease that it already has acquired
	Given I am the only actor trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 15 seconds
	And I have already acquired the lease
	When I renew the lease
	Then the lease should expire in the future
	Then it should retain the lease for 15 seconds
	And the lease should be expired after 15 seconds

@storage_emulator
Scenario: A single actor renews a lease that has expired
	Given I am the only actor trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 15 seconds
	And the lease has expired
	When I renew the lease
	Then it should throw an AggregateException containing InvalidOperationException

@storage_emulator
Scenario: A single actor releases a lease that it already has acquired
	Given I am the only actor trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 15 seconds
	And I have already acquired the lease
	When I release the lease
	Then the lease should no longer be acquired
	And the lease should be expired
	And the lease expiration date should be null
	And the lease last acquired date should be null

@storage_emulator
Scenario: A single actor releases a lease that has expired
	Given I am the only actor trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 15 seconds
	And the lease has expired
	When I release the lease
	Then it should not throw an exception

@storage_emulator
Scenario: A single actor disposes a lease that exists
	Given I am the only actor trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 15 seconds
	And I have already acquired the lease
	When I dispose the lease
	Then it should not throw an exception
	Then the lease should no longer be acquired
	And the lease should be expired
	And the lease expiration date should be null
	And the lease last acquired date should be null

@storage_emulator
Scenario: A single actor disposes a lease that has expired
	Given I am the only actor trying to perform an operation called "long-running-task"
	And I want to acquire a lease for 15 seconds
	And the lease has expired
	When I dispose the lease
	Then it should not throw an exception