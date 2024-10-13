Feature: Pet API
As a pet store owner
I want to manage pets in my store
So that I can keep track of available pets

Scenario: Add a new pet
    Given I have a new pet with the following details:
        | Name   | Status    | PhotoUrl                       | CategoryName | TagName  |
        | Fluffy | available | http://example.com/fluffy.jpg  | Dogs         | Friendly |
    When I add the pet to the store
    Then the pet should be successfully added
    And the added pet should have the correct details

Scenario: Retrieve a pet by ID
    Given I have added a pet named "Buddy" to the store
    When I retrieve the pet by its ID
    Then the retrieved pet should have the name "Buddy"

Scenario: Update an existing pet
    Given I have added a pet named "Max" to the store
    When I update the pet's status to "sold"
    Then the pet's status should be updated to "sold"

Scenario: Delete a pet
    Given I have added a pet named "Charlie" to the store
    When I delete the pet
    Then the pet should be successfully deleted
