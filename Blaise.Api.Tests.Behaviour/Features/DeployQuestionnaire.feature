@deploy
Feature: Deploy Questionnaire
	As a stakeholder
	I want to be able to deploy a questionnaire to Blaise
	So that we can capture respondents data

@smoke
Scenario: Deploy a questionnaire from a file held in a bucket
	Given there is a questionnaire available in a bucket
	When the API is called to deploy the questionnaire
	Then the questionnaire is available to use in the Blaise environment 