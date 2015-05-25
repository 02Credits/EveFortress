# EveFortress
A game I am working on which aims to mix the multiplayer aspects of Eve with the gameplay of Dwarf Fortress. This project represents what I consider my most complex work and is likely my longest term project. Unfortunately it doesn't compile at the moment.

## Interesting Pieces

### Networking

I wrote for this system a networking library which allows me to achieve remote process execution without much of the overhead found in other .net networking solutions.

To achieve this I wrote a [Network Manager](https://github.com/02Credits/EveFortress/blob/master/NetworkLibrary/NetworkManager.cs) on top of the Lidgren system which provides generic methods for calling remote methods. These return a Task encapsulating the return value type of the remote method which can be awaited for the response using C# async await.

The tricky piece is that all of the await code is done on a single thread and is pumped manually using TaskCompletionSources. This allows me to get the benefits of asynchronous programming which we all know and love without the added complexity of dealing with thread safety. I get the best of both worlds similar to the programming model found in node.js.

### Component Entity Systems

I wrote the Client and Server code using a System manager which automatically calls the draw, update, reset, dispose, and input methods based on what [interfaces](https://github.com/02Credits/EveFortress/tree/master/EveFortressModel/Interfaces) each system implement. This way when adding new systems to the game, I only need to add one line at the top of my [Program.cs](https://github.com/02Credits/EveFortress/blob/master/EveFortressServer/Program.cs) or [Game.cs](https://github.com/02Credits/EveFortress/blob/master/EveFortressOpenGL/Game.cs) files which adds the System object to the appropriate lists.

On top of this, I am also working on implementing the gameplay logic using a Component Entity System. The idea is that each entity in the game should be at its core a collection of components and that the processing of the components is done through systems that subscribe to a given type of component. This modular approach allows me to write systems that handle a generic piece of the functionality of a group of elements, for instance a movement system could manage the trajectories of all projectiles reguardless of the other features of a given projectile entity.
