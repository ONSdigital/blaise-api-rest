@ignore @deploy
Feature: Deploy Questionnaire
	As a stakeholder
	I want to be able to deploy a questionnaire to Blaise
	So that we can capture respondents data

@ignore @smoke
Scenario: Deploy a questionnaire
	Given I have a questionnaire I want to install
	When the API is called to install the questionnaire
	Then the questionnaire is available to use