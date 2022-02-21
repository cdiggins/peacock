# Peacock

Peacock is a pure functional UI library written in C#.

It is being developed to validate the hypothesis that pure functional programming 
is a valid approach for building an entire UI component library from the ground up. 

The current implementation is built on top of WPF, but it is done so using a thin layer,
that in theory can be easily ported to different platforms (e.g., Unity, HTML, et.) 

# Inspiration

The Elm programming language is a big inspiration for Peacock, as are many of the UI
frameworks and best practices being developed by the JavaScript community. 

# How does it work

## Control and IControl

Central to Peack is the `IControl` interface, and the generic `Control` class.
A control is parameterized on a class derived from `IView` which represents 
the current view state. A view the underlying data model
plus some styling and layout information). 

A control also recieves in its constructor two functions:

1. How to render (draw) that particular state type to a canvas
2. A function for converting the state type to child-controls
3. Optionally: zero or more behaviors

## IBehavior

A Behavior is a class that holds its own state object and can provide additional drawing logic for a control,
can transform its state object in response to input events, and can aggregate proposed changes to the views. 

## IUpdates 

The `IUpdates` class aggregates a set of proposed changes to all views managed by a control. 
These changes are managed as a list of functions associated with a particular view ID. 

# FAQ

## Why not just use WPF

WPF is a pretty good library, but there are some areas for improvement:

* It does not work well with immutable data structures:
* Requires separate UI description language (XAML) from code logic 
* Bindings and data-sources are complicate  
* Hard to apply theming 
* Tooling support has gotten worse over the years 
* Layout is complicated 
* Hard to learn 
* Doesn't work well with other UI rendering frameworks.
* Could be more efficient 
* Easy to create resource leaks. 

## Why not use F#

F# is a great language, but I find [the syntax hostile to new users](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/symbol-and-operator-reference/).

## Why not MVU

I don't think MVU is a viable architecture to large scale software development. 
I am not coninved it can be used in an effective way to separate the different 
software concerns: theming, styles, UI logic, animation, and domain models.  

# A Platonic C# Library 

Peacock is being written as a *Platonic* C# library. This means that it 
is designed to be compliant with the [Plato Language](https://github.com/cdiggins/plato)
and compatible with the Plato tools (optimizer, translators).

Peacock is a key part of validating the design and implementation of Plato as a 
general purpose programming language. 

# Peacock in Action 

To validate the Peacock language I am building a visual programming language editor called 
Bohr. 
