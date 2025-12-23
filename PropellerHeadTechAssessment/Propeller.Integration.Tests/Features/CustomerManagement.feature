@RequiresAdminUser
@RequiresPowerUser
@RequiresRegularUser

Feature: CustomerManagement

A short summary of the feature

@tag1
Scenario: Add a new Customer
	Given I use an Authenticated "admin" User
    When I try to create a Customer with name: "[Test] New Customer Test" and Status: "prospective"
    Then I search for Customers with name: "[Test] New Customer Test"
    And I should have 1 record(s) found
    Then I delete the Customer with name: "[Test] New Customer Test"
    And I Verify the Customer does not exist anymore


Scenario: Try to add a duplicated Customer
	Given I use an Authenticated "admin" User
    When I try to create a Customer with name: "New Customer DupTest" and Status: "prospective"
    Then I search for Customers with name: "New Customer DupTest"
    And I should have 1 record(s) found
    When I try to create a Customer with name: "New Customer DupTest" and Status: "prospective"
    Then I verify the returned Http Status Code was "409"
    #Then I expect to get a "409" Http Status Code
    Then I delete the Customer with name: "New Customer DupTest"
    And I Verify the Customer does not exist anymore

Scenario: Change a Customer's status
	Given I use an Authenticated "admin" User
    When I try to create a Customer with name: "[Test] Customer Status Change" and Status: "prospective"
    	Then I verify the returned Http Status Code was "201"
    Then I change the Customer Status to: "current"
    Then I retrieve a Single Customer with name: "[Test] Customer Status Change"
    And I verify the Customer Status is "current"
    Then I delete the Customer with name: "[Test] Customer Status Change"

    Scenario: Try to set an invalid Customer Status
	Given I use an Authenticated "admin" User
    When I try to create a Customer with name: "[Test] New Customer Invalid Status" and Status: "prospective"
    Then I verify the returned Http Status Code was "201"
    Then I try to change the Customer Status to and Invalid value
    #400 BadRequest
    Then I verify the returned Http Status Code was "400"