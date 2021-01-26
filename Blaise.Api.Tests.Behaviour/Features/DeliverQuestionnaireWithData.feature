@deliver
Feature: Deliver Instrument With Data
	As a Survey Manager
	I want raw respondent date delivered (Blaise files)
	So that my team can use the data for further processing

Background:
	Given there is a questionnaire available in a bucket
	When the API is called to deploy the questionnaire
	Then the questionnaire is available to use in the Blaise environment
	And the questionnaire does not contain any correspondent data

@smoke
Scenario: Deliver an instrument with all correspondent data that has been captured so far
	Given we have captured correspondent data for the questionnaire
	When the API is called to deliver the questionnaire with data
	Then the questionnaire package is delivered to the bucket
	And the questionnaire is package uses the agreed file name format
	And the questionnaire package contains the captured correspondent data