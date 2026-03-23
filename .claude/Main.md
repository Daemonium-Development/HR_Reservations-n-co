I've built a few markdown files that contain context for the project.
The question I have is whether it would be reasonable to put the dependency injection container in the DebugDiner.Application, or in the DebuDiner project.

Personally I think putting the container in the application layer is not a terrible idea, though then we'd have to figure out how to set the presentation layer up (DebugDiner project)