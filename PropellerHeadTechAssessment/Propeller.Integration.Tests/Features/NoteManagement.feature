@RequiresAdminUser
@RequiresPowerUser
@RequiresRegularUser

Feature: NoteManagement

Verifies different Scenarios for Customer Notes

Background: I have successfully Authenticated as "admin,user"

@tag1
Scenario: Add a new Note to a Customer then Delete it
	Given I use an Authenticated "admin" User
	#Given I am Authenticated as "admin"
	When I try to create a Customer with name: "[Test] New Customer With Note" and Status: "prospective"
	#201 = Created
	Then I verify the returned Http Status Code was "201"
	Then I add a new Note with text: "[Test] New Note2"
	When I retrieve the Customer with it's notes
	Then I verify the note with text: "[Test] New Note2" exists
	Then I delete the Note with text: "[Test] New Note2"
	And I verify the Customer does not have any Notes
	Then I delete the Customer with name: "[Test] New Customer With Note"
	And I Verify the Customer does not exist anymore

Scenario: Try to Add a Note to a Non Existant Client
	Given I use an Authenticated "admin" User
	When I try to add a new Note with Text: "[Test] New Note" to a Client with Id: -1
	Then I verify the returned Http Status Code was "400"

Scenario: Try to Add an Empty Note
Given I use an Authenticated "admin" User
	When I try to create a Customer with name: "[Test] New Customer With Empty Note" and Status: "prospective"
	#201 = Created
	Then I verify the returned Http Status Code was "201"
	Then I try to add a New Note with Text: ""
	Then I verify the returned Http Status Code was "422"
	And I verify the Response is "Note Text Required"
	Then I delete the Customer with name: "[Test] New Customer With Empty Note"
	And I Verify the Customer does not exist anymore

Scenario: Try to Add a Note with too much Text
Given I use an Authenticated "admin" User
When I try to create a Customer with name: "[Test] New Customer With Long Note" and Status: "prospective"
	Then I try to add a New Note with 600 characters
	Then I verify the returned Http Status Code was "422"
	And I verify the Response is "Length Exceeded"
	Then I delete the Customer with name: "[Test] New Customer With Long Note"
	And I Verify the Customer does not exist anymore