@data
Feature: Get instrument With Data
	As a Survey Manager
	I want to be able to get raw respondent data (Blaise files)
	So that my team can use the data for further processing

Background:
	Given there is a questionnaire available in a bucket
	When the API is called to deploy the questionnaire
	Then the questionnaire is available to use in the Blaise environment
	And the questionnaire does not contain any correspondent data

@smoke
Scenario: Deliver an instrument with all correspondent data that has been captured so far
	Given we have captured correspondent data for the questionnaire
	When the API is called to retrieve the questionnaire with data
	Then the questionnaire package contains the captured correspondent data
