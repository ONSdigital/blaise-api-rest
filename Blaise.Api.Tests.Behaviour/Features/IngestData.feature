@ingest
Feature: Ingest Questionnaire Data
	In order to ingest data from other organisations
	As a service
	I want to be given cases to ingest representing the data captured by other organisations

@smoke
Scenario: There is an ingest file available in the bucket that we wish to merge with an existing blaise dataset
	Given there is an ingest file that contains the following cases
		| primarykey | outcome | mode |
		| 500001     | 110     | Web  |
		| 500002     | 210     | Web  |
		| 500003     | 110     | Web  |	
	
	And blaise contains the existing cases
		| primarykey | outcome | mode |
		| 900001     | 110     | Web  |
		| 900002     | 110     | Web  |
		| 900003     | 210     | Web  |
	
	When the ingest file is processed
	Then blaise will contain the cases
		| primarykey | outcome | mode |
		| 500001     | 110     | Web  |
		| 500002     | 210     | Web  |
		| 500003     | 110     | Web  |	
		| 900001     | 110     | Web  |
		| 900002     | 110     | Web  |
		| 900003     | 210     | Web  |

