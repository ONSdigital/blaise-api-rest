@questionnaires
Feature: Get Questionnaire Details
	As a stakeholder
	I want to see a list of all questionnaires in Blaise
	So that I can see that the restful API is working

Scenario: Return a list of available questionnaires where there is a single questionnaire
	Given there is a questionnaire installed on a Blaise environment
	And the questionnaire is active
	When the API is queried to return all active questionnaires
	Then the details of the questionnaire is returned

Scenario: Return a an empty list of questionnaires when there are no active questionnaires are available
	Given there is a questionnaire installed on a Blaise environment
	And the questionnaire is inactive
	When the API is queried to return all active questionnaires
	Then an empty list is returned

@smoke
Scenario: Return a an empty list of questionnaires when none are available
	Given there are no questionnaires installed
	When the API is queried to return all active questionnaires
	Then an empty list is returned