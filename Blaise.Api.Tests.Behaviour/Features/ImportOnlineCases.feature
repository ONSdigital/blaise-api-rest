﻿@onlinedata
Feature: Import online cases
	In order to process cases gathered online
	As a service
	I want to be given cases to import representing the data captured online

@smoke
#Covers all scenarios in one test run
Scenario: There is an online file available that contains cases that already exists in the blaise database, the cases are updated depending on the outcome codes
	Given there is a online file that contains the following cases
	| primarykey | outcome | mode | 
	#scenario 1
	| 900001     | 110     | Web  | 
	#scenario 2
	| 900002     | 210     | Web  | 
	#scenario 3
	| 900003     | 110     | Web  | 
	#scenario 4
	| 900004     | 110     | Web  | 
	| 900005     | 110     | Web  | 
	| 900006     | 110     | Web  | 
	| 900007     | 110     | Web  | 
	| 900008     | 110     | Web  | 
	| 900009     | 110     | Web  | 
	#scenario 5
	| 900010     | 0       | Web  | 
	#scenario 6
	| 900011     | 210     | Web  | 
	#scenario 7
	| 900012     | 210     | Web  | 
	| 900013     | 210     | Web  | 
	| 900014     | 210     | Web  | 
	| 900015     | 210     | Web  | 
	| 900016     | 210     | Web  | 
	| 900017     | 210     | Web  | 
	#scenario 8
	| 900018     | 0       | Web  | 
	#scenario 9
	| 900019     | 210     | Web  | 
	#scenario 10
	| 900020     | 110     | Web  | 
	#scenario 13
	| 900021     | 580     | Web  | 
	##scenario 14
	#| 900022     | 580     | Web  | 
	#| 900023     | 580     | Web  | 
	#| 900024     | 580     | Web  | 
	#| 900025     | 580     | Web  | 
	#| 900026     | 580     | Web  | 
	#| 900027     | 580     | Web  | 
	#| 900028     | 580     | Web  | 
	##scenario 15
	#| 900029     | 580     | Web  | 
	#| 900030     | 580     | Web  | 
	#| 900031     | 580     | Web  | 
	#| 900032     | 580     | Web  | 
	##scenario 16
	#| 900033     | 110     | Web  | 
	##scenario 17
	#| 900034     | 210     | Web  | 
	
	And blaise contains the following cases
	| primarykey | outcome | mode | 
	#scenario 1
	| 900001     | 110     | Tel  | 
	#scenario 2
	| 900002     | 110     | Tel  | 
	#scenario 3
	| 900003     | 210     | Tel  | 
	#scenario 4
	| 900004     | 310     | Tel  | 
	| 900005     | 430     | Tel  | 
	| 900006     | 460     | Tel  | 
	| 900007     | 461     | Tel  | 
	| 900008     | 541     | Tel  | 
	| 900009     | 542     | Tel  | 
	#scenario 5
	| 900010     | 110     | Tel  | 
	#scenario 6
	| 900011     | 210     | Tel  | 
	#scenario 7
	| 900012     | 310     | Tel  | 
	| 900013     | 430     | Tel  | 
	| 900014     | 460     | Tel  | 
	| 900015     | 461     | Tel  | 
	| 900016     | 541     | Tel  | 
	| 900017     | 542     | Tel  | 
	#scenario 8
	| 900018     | 310     | Tel  | 
	#scenario 9
	| 900019     | 562     | Tel  | 
	#scenario 10
	| 900020     | 561     | Tel  | 
	#scenario 13
	| 900021     | 580     | tel  | 
	##scenario 14
	#| 900022     | 310     | tel  | 
	#| 900023     | 430     | tel  | 
	#| 900024     | 440     | tel  | 
	#| 900025     | 460     | tel  | 
	#| 900026     | 461     | tel  | 
	#| 900027     | 541     | tel  | 
	#| 900028     | 542     | tel  | 
	##scenario 15
	#| 900029     | 110     | tel  | 
	#| 900030     | 210     | tel  | 
	#| 900031     | 561     | tel  | 
	#| 900032     | 562     | tel  | 
	##scenario 16
	#| 900033     | 580     | tel  | 
	##scenario 17
	#| 900034     | 580     | tel  | 

	When the online file is processed
	Then blaise will contain the following cases
	| primarykey | outcome | mode | 
	#scenario 1
	| 900001     | 110     | Web  | 
	#scenario 2
	| 900002     | 110     | Tel  | 
	#scenario 3
	| 900003     | 110     | Web  | 
	#scenario 4
	| 900004     | 110     | Web  | 
	| 900005     | 110     | Web  | 
	| 900006     | 110     | Web  | 
	| 900007     | 110     | Web  | 
	| 900008     | 110     | Web  | 
	| 900009     | 110     | Web  | 
	#scenario 5
	| 900010     | 110     | Tel  | 
	#scenario 6
	| 900011     | 210     | Web  | 
	#scenario 7
	| 900012     | 210     | Web  | 
	| 900013     | 210     | Web  | 
	| 900014     | 210     | Web  | 
	| 900015     | 210     | Web  | 
	| 900016     | 210     | Web  | 
	| 900017     | 210     | Web  | 
	#scenario 8
	| 900018     | 310     | Tel  | 
	#scenario 9
	| 900019     | 562     | Tel  | 
	#scenario 10
	| 900020     | 561     | Tel  | 
	#scenario 13
	| 900021     | 580     | Web  | 
	##scenario 14
	#| 900022     | 580     | Web  | 
	#| 900023     | 580     | Web  | 
	#| 900024     | 580     | Web  | 
	#| 900025     | 580     | Web  | 
	#| 900026     | 580     | Web  | 
	#| 900027     | 580     | Web  | 
	#| 900028     | 580     | Web  | 
	##scenario 15
	#| 900029     | 110     | tel  | 
	#| 900030     | 210     | tel  | 
	#| 900031     | 561     | tel  | 
	#| 900032     | 562     | tel  | 
	##scenario 16
	#| 900033     | 110     | Web  | 
	##scenario 17
	#| 900034     | 210     | Web  | 

#Scenario 1 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario: A case in the online file is complete and in Blaise it is complete, we take the online case
	Given there is a online file that contains a case that is complete
	And the same case exists in Blaise that is complete
	When the online file is processed
	Then the existing blaise case is overwritten with the online case

#Scenario 2 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario:  A case in the online file is partially complete and in Blaise it is complete, we keep the existing blaise case
	Given there is a online file that contains a case that is partially complete
	And the same case exists in Blaise that is complete
	When the online file is processed
	Then the existing blaise case is kept

#Scenario 3 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario: A case in the online file is complete and in Blaise it is partially complete, we take the online case
	Given there is a online file that contains a case that is complete
	And the same case exists in Blaise that is partially complete
	When the online file is processed
	Then the existing blaise case is overwritten with the online case

#Scenario 4 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario Outline: A case in the online file is complete and in Blaise it is between the range 210-542, we take the online case
	Given there is a online file that contains a case that is complete
	And the same case exists in Blaise with the outcome code '<existingOutcome>'
	When the online file is processed
	Then the existing blaise case is overwritten with the online case
	Examples: 
	| existingOutcome | description                                                 |
	| 210             | Partially completed survey                                  |
	| 310             | Non-contact                                                 |
	| 430             | HQ refusal                                                  |
	| 440             | Person not available                                        |
	| 460             | Refuses cooperation - hard refusal                          |
	| 461             | Refuses cooperation - soft refusal could be contacted again |
	| 541             | Language difficulties - notified by Head Office             |
	| 542             | Language difficulties - notified to interviewer             |

#Scenario 5 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario: A case in the online file that has not started and in Blaise it is complete, we keep the existing blaise case
	Given there is a online file that contains a case that has not been started
	And the same case exists in Blaise that is complete
	When the online file is processed
	Then  the existing blaise case is kept

#Scenario 6  https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario: A case in the online file is partially complete and in Blaise it is partially complete, we take the online case
	Given there is a online file that contains a case that is partially complete
	And the same case exists in Blaise that is partially complete
	When the online file is processed
	Then the existing blaise case is overwritten with the online case

#Scenario 7  https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario Outline: A case in the online file is partially complete and in Blaise and it is between the range 310-542, we take the online case
	Given there is a online file that contains a case that is complete
	And the same case exists in Blaise with the outcome code '<existingOutcome>'
	When the online file is processed
	Then the existing blaise case is overwritten with the online case
	Examples: 
	| existingOutcome | description                                                 |
	| 310             | Non-contact                                                 |
	| 430             | HQ refusal                                                  |
	| 440             | Person not available                                        |
	| 460             | Refuses cooperation - hard refusal                          |
	| 461             | Refuses cooperation - soft refusal could be contacted again |
	| 541             | Language difficulties - notified by Head Office             |
	| 542             | Language difficulties - notified to interviewer             |

#Scenario 8 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario: A case in the online file that has not started and in Blaise it is non-contact, we keep the existing blaise case
	Given there is a online file that contains a case that has not been started
	And the same case exists in Blaise with the outcome code '310'
	When the online file is processed
	Then the existing blaise case is kept

#Scenario 9 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario: A case in the online file that is partially complete and in Blaise it marked as respondent request for data to be deleted, we keep the existing blaise case
	Given there is a online file that contains a case that is partially complete
	And the same case exists in Blaise with the outcome code '562'
	When the online file is processed
	Then the existing blaise case is kept

#Scenario 10 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario: A case in the online file that is complete and in Blaise it marked as respondent request for data to be deleted, we keep the existing blaise case
	Given there is a online file that contains a case that is complete
	And the same case exists in Blaise with the outcome code '561'
	When the online file is processed
	Then the existing blaise case is kept

#Scenario 11 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario: A case in the online file has a better outcome but the case has been updated recently so may be open in Cati, do not update
	Given there is a online file that contains a case that is complete
	And the same case exists in Blaise that is partially complete
	And the case has been updated within the past 30 minutes
	When the online file is processed
	Then the existing blaise case is kept

#Scenario 12 https://collaborate2.ons.gov.uk/confluence/display/QSS/OPN+NISRA+Case+Processing+Scenarios
Scenario: A case in the online file has already been processed
	Given there is a online file that contains a case that has previously been imported
	When the online file is processed
	Then the online case is not imported again
