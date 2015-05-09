Feature: LeasePolicyValidator
	In order to create a valid lease using Azure blob storage
	As a dev
	I want to ensure the lease policy meets the Azure blob leasing rules

Scenario: Duration is less than 15 seconds
	Given the duration on the lease policy is 10 seconds
	And has a valid name
	When I validate the policy
	Then it should throw a ArgumentOutOfRangeException

Scenario: Duration is greater than 59 seconds
	Given the duration on the lease policy is 60 seconds
	And has a valid name
	When I validate the policy
	Then it should throw a ArgumentOutOfRangeException

Scenario: Duration is between 15 and 59 seconds
	Given the duration on the lease policy is 40 seconds
	And has a valid name
	When I validate the policy
	Then it should not throw any exceptions

Scenario: Duration is null
	Given the duration on the lease policy is null
	And has a valid name
	When I validate the policy
	Then it should not throw any exceptions

Scenario: Name is valid
	Given the name on the lease policy is "SomeValidName"
	And has a valid duration
	When I validate the policy
	Then it should not throw any exceptions

Scenario: Name ends in a dot
	Given the name on the lease policy is "EndsWithADot."
	And has a valid duration
	When I validate the policy
	Then it should throw a ArgumentException

Scenario: Name ends in a forward slash
	Given the name on the lease policy is "EndsWithASlash/"
	And has a valid duration
	When I validate the policy
	Then it should throw a ArgumentException

Scenario: Name is less than 1 character
	Given the name on the lease policy is ""
	And has a valid duration
	When I validate the policy
	Then it should throw a ArgumentOutOfRangeException

Scenario: Name is more than 1024 characters
	Given the name on the lease policy is "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz"
	And has a valid duration
	When I validate the policy
	Then it should throw a ArgumentOutOfRangeException

Scenario: Name is null
	Given the name on the lease policy is null
	And has a valid duration
	When I validate the policy
	Then it should throw a ArgumentNullException
