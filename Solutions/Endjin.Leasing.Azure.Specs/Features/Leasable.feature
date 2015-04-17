Feature: Leasable
	In order to avoid concurrency issues
	As an actor in the system
	I want to have an exclusive lease on a long running task

@container @storage_emulator
Scenario: A single actor executes a long running task with duration less than the lease policy
	Given the long running task takes 5 seconds to complete
	And the lease name is "long-running-task"
	When I execute the task
	Then it should not throw any exceptions
	And it should return successfully
	And 1 action(s) should have completed successfully

@container @storage_emulator
Scenario: A single actor executes a long running task with duration more than the lease policy
	Given the long running task takes 70 seconds to complete
	And the lease name is "long-running-task"
	When I execute the task
	Then it should not throw any exceptions
	And it should return successfully
	And 1 action(s) should have completed successfully

@container @storage_emulator
Scenario: Actor A attempts to execute a long running task whilst Actor B is currently running the task
	Given the long running task takes 20 seconds to complete
	And the lease name is "long-running-task"
	And actor B is currently running the task
	When Actor A executes the task
	Then it should not throw any exceptions
	And it should return successfully
	And 2 action(s) should have completed successfully

@container @storage_emulator
Scenario: Actor A attempts to execute a long running task with a do not retry policy and a linear retry strategy, whilst Actor B is currently running the task
	Given the long running task takes 20 seconds to complete
	And the lease name is "long-running-task"
	And the lease duration is 40 seconds
	And we use a do not retry policy
	And we use a linear retry strategy with periodicity of 10 seconds and 10 max retries
	And actor B is currently running the task
	When Actor A executes the task with options
	Then it should not throw any exceptions
	And it should return unsuccessfully
	And after 20 seconds
	And 1 action(s) should have completed successfully

@container @storage_emulator
Scenario: Actor A attempts to execute a long running task with a do not retry on lease acquisition unsuccessful policy and a linear retry strategy, whilst Actor B is currently running the task
	Given the long running task takes 20 seconds to complete
	And the lease name is "long-running-task"
	And the lease duration is 40 seconds
	And we use a do not retry on lease acquisition unsuccessful policy
	And we use a linear retry strategy with periodicity of 10 seconds and 10 max retries
	And actor B is currently running the task
	When Actor A executes the task with options
	Then it should not throw any exceptions
	And it should return unsuccessfully
	And after 20 seconds
	And 1 action(s) should have completed successfully

@container @storage_emulator
Scenario: A single actor executes a long running task with a retry until lease acquired policy and no retry strategy
	Given the long running task takes 5 seconds to complete
	And the lease name is "long-running-task"
	And the lease duration is 40 seconds
	And we use a do not retry on lease acquisition unsuccessful policy
	And we use no retry strategy
	When I execute the task with options
	Then it should throw an AggregateException containing NullReferenceException

@container @storage_emulator
Scenario: A single actor executes a long running task with no retry policy and a linear retry strategy
	Given the long running task takes 5 seconds to complete
	And the lease name is "long-running-task"
	And the lease duration is 40 seconds
	And we use no lease policy
	And we use a linear retry strategy with periodicity of 10 seconds and 10 max retries
	When I execute the task with options
	Then it should throw an AggregateException containing NullReferenceException

@container @storage_emulator
Scenario: A single actor executes a long running task with no lease policy
	When I execute the task with options
	Then it should throw an AggregateException containing NullReferenceException