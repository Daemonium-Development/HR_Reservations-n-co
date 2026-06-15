# Infrastructure tests

The current unit test for the infrastructure layer of the application are outdated. They once worked before the finalizations of the
actual infrastructure.
So the tests need to be checked, stabilized, updated, and working. For the purposes of writing the tests we should use the application's
context as much as we can, but adjusting the application to fit tests is not done today.
How things stand at this point is how things work.

The application has more layers:
- Presentation
- Application
- Domain
- Infrastructure
- Cross-Cutting Concerns

WE're only testing Infrastructure at this point
