# Add developer tests (Unittest file template in teams) file in notion

Status: In progress
Assignee: Lars Werner (1127685)
Priority: 6
Task type: must have
Projects: Debug Diner  (../Projects/Debug%20Diner%2031e3f8f3faef80cbaa75c11de397be90.md)
Time entries: Unit Testing (../Time%20Entries%20Soufian/Unit%20Testing%203333f8f3faef8041ab7ec73197c0872c.md)

## Task description

| User story | As a user I want to be able to create an account so I can save my personal data for next time |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- |
|  |  |  | Give description of what/how you fixed it. | Give the github commit link to your fix |  |  |  |
| ID | Acceptance criteria | Path IDs | Changes made as a result of testing | Github commit link | Snippet |  |  |
| 1 | The user should supply a user name, age and password | H1, H2, S1 |  |  |  |  |  |
| 2 | The name should be at least 2 characters | S5 |  |  |  |  |  |
| 3 | The age should be between 0 and 150 | S2 | EXAMPLE  1.3 Age was set  to 0 when no age was supplied, now empty. (See snippet 1). |  | 1 |  |  |
| 4 | The password should be 10 characters and include a number | S3, S4 | EXAMPLE  Forgot to add a check for the digit. Now we do this. |  | 2 |  |  |
| 5 | If any input is invalid, show an appropriate warning | S2, S3, S4, S5 | EXAMPLE Typo in text fixed <link here> |  | 3 |  |  |
| Happy paths |  |  |  |  |  |  |  |
| ID | Description |  |  |  | Test method |  |  |
| H1 | All inputs are provided and valid |  |  |  | Unit test |  |  |
| H2 | Message is shown that account has been created |  |  |  | System test |  |  |\
| Sad paths |  |  |  |  |  |  |  |
| S1 | Inputs are missing |  |  |  | Unit test |  |  |
| S2 | Age is not between 0 and 150 |  |  |  | Unit test and System test |  |  |
| S3 | Password is less than 10 chars |  |  |  | Unit test and System test |  |  |
| S4 | Password is more than 10 chars but does not contain a number |  |  |  | Unit test and System test |  |  |
| S5 | Username is less then 2 chars |  |  |  | Unit test and System test |  |  |
|  |  |  |  |  |  |  |  |
| Path ID |  |  |  |  |  |  |  |
| H1 | Input data | Expected output | Unit test result | System test result |  |  |  |
