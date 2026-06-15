# Unit Testing with FluentAssertions

Since we've tested the infrastructure layer, I want to test the whole of the application, each layer in their own unit testing project.
- Presentation Layer -> DD.PresentationTests
- Application Layer -> DD.ApplicationTests
- Domain Layer -> DD.DomainTests
- Infrastructure Layer -> DD.InfraTests (already implemented, no need to change)

Build each of these layer projects, and write full unit testing for each, using xUnit and FluentAssertions

