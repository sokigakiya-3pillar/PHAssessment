@RequiresAdminUser
@RequiresPowerUser
@RequiresRegularUser

Feature: ContactsFeature

A short summary of the feature

@tag1
Scenario: Add a New Contact Without a Customer
	Given I use an Authenticated "admin" User
	When I add a new Contact with data
	 | FirstName | LastName | Email					  | Phone             |
     | Soki		 | Gakiya   | soki.gakiya@outlook.com | +52 1 6671438677  |
  
	Then I verify the returned Http Status Code was "201"
	When I retrieve Contacts with Search Criteria: "soki.gakiya@outlook.com"
	Then I should have retrieved 1 Contact(s)
	Then I check the Contact has First Name: "Soki" and Last Name: "Gakiya"

Scenario: Try to Add a New Contact to a Non Existant Customer
	Given I use an Authenticated "admin" User
	When I add a new Contact with data
	 | FirstName | LastName | Email                   | Phone            | CustomerID |
	 | Soki      | Gakiya   | soki.gakiya@outlook.com | +52 1 6671438677 | 0          |
  
	Then I verify the returned Http Status Code was "404"
	And I verify the Response is "Customer Not Found"
	When I retrieve Contacts with Search Criteria: "soki.gakiya@outlook.com"
	Then I should have retrieved 0 Contact(s)

Scenario: Try to Add a New Contact with no Email or Phone
	Given I use an Authenticated "admin" User
	When I add a new Contact with data
	 | FirstName | LastName | Email | Phone  |
	 | Soki      | Gakiya   |       |        |
  
	Then I verify the returned Http Status Code was "422"
	And I verify the Response is "Email or Phone Required"
	When I retrieve Contacts with Search Criteria: "soki.gakiya@outlook.com"
	Then I should have retrieved 0 Contact(s)

Scenario: Add a New Contact to an existing Customer
	Given I use an Authenticated "admin" User
	When I try to create a Customer with name: "[Test] New Customer With Note" and Status: "prospective"
		#201 = Created
	Then I verify the returned Http Status Code was "201"
	Then I add a new Contact with data for the recently created Customer
	 | FirstName | LastName | Email                   | Phone            |
	 | Soki      | Gakiya   | soki.gakiya@outlook.com | +52 1 6671438677 |
	Then I verify the returned Http Status Code was "201"
	When I retrieve the newly created Customer
	Then I verify it contains a Contact with Email: "soki.gakiya@outlook.com"

Scenario: Add and forcefully Remove Contact from existing Customer
	Given I use an Authenticated "admin" User
	When I try to create a Customer with name: "[Test] New Customer With Contact" and Status: "prospective"
		#201 = Created
	Then I verify the returned Http Status Code was "201"
	Then I add a new Contact with data for the recently created Customer
	 | FirstName | LastName | Email                   | Phone            |
	 | Soki      | Gakiya   | soki.gakiya@outlook.com | +52 1 6671438677 |
	Then I verify the returned Http Status Code was "201"
	When I retrieve the newly created Customer
	Then I verify it contains a Contact with Email: "soki.gakiya@outlook.com"
	When I retrieve a Contact with Email: "soki.gakiya@outlook.com"
	Then I verify only 1 record(s) were retrieved
	When I forcefully remove the recently created Contact
	And I retrieve the newly created Customer
	Then I verify it does not contain a Contact with Email: "soki.gakiya@outlook.com"

Scenario: Try to Delete a Non Existant Contact
	Given I use an Authenticated "admin" User
	When I try to Delete a Contact with Id: 0
		#404 = NotFound
	Then I verify the returned Http Status Code was "404"

Scenario: Try to Remove Contact associated with Customer non forcefully
	Given I use an Authenticated "admin" User
	When I try to create a Customer with name: "[Test] New Customer With Contact" and Status: "prospective"
	Then I verify the returned Http Status Code was "201"
	Then I add a new Contact with data for the recently created Customer
	 | FirstName | LastName | Email                   | Phone            |
	 | Soki      | Gakiya   | soki.gakiya@outlook.com | +52 1 6671438677 |
	Then I verify the returned Http Status Code was "201"
	When I non forcefully try to remove the recently created Contact
	#403 Forbidden
	Then I verify the returned Http Status Code was "403"
	When I retrieve a Contact with Email: "soki.gakiya@outlook.com"
	Then I verify only 1 record(s) were retrieved

Scenario: Try to Create a Contact with no First Name
	Given I use an Authenticated "admin" User
	When I add a new Contact with data
	 | FirstName | LastName | Email					  | Phone             |
     |     		 | Gakiya   | soki.gakiya@outlook.com | +52 1 6671438677  |
	#400 BadRequest
	Then I verify the returned Http Status Code was "400"

