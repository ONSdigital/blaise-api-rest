@questionnaires
Feature: Get Questionnaire Details
	As a stakeholder
	I want to see a list of all questionnaires in Blaise
	So that I can see that the restful API is working

@smoke
Scenario: Return a list of available questionnaires where a questionnaire is active
	Given there is a questionnaire installed on a Blaise environment
	And the questionnaire is active
	When the API is queried to return all active questionnaires
	Then the details of the questionnaire is returned

