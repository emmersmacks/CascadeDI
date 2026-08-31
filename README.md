## Dependency Injection Framework in C#

Implementation of a Dependency Injection (DI) framework for educational purposes. This project should be considered as my understanding of how DI works, rather than a standalone project.

## Let's get started!

### Installation
Installing as a unity module via a git link in the PackageManager or editing `Packages/manifest.json`:

```
"com.cascade.di": "https://github.com/emmersmacks/CascadeDI.git",
```

### Usage
Example of service registration:

```csharp
var container = new Container();
container
    .RegisterSingleton<IService, Service>()
    .RegisterSingleton<IController, Controller>();

var scope = container.CreateScope();
var controller = scope.Resolve(typeof(IController));

// Регистрация возможна и после того, как контейнер уже использовался:
container.RegisterSingleton<IOtherService, OtherService>();
```

Fluent (Zenject-style) registration, when a type needs to be exposed under several service types at once:

```csharp
container
    .Register<ShopWindow>()
    .ToInterface<IWindow>()
    .ToInterface<IInitializable>()
    .AsSingleton();

var scope = container.CreateScope();
var window = scope.Resolve<IWindow>();
var initializable = scope.Resolve<IInitializable>();
// window and initializable share the same ShopWindow instance
```

Any registration can react right after its instance is created via `.OnCreate()`:

```csharp
container
    .Register<ShopWindow>()
    .ToInterface<IWindow>()
    .OnCreate(window => window.Init())
    .AsSingleton();

container.RegisterSingleton<IService, Service>()
    .OnCreate(service => service.Warmup());
```

## API
### Methods
* **RegisterSingleton** - registers an object as a Singleton. This means that the instance of the object will be shared across the entire application.

* **RegisterTransient** - registers an object as a transient object. This means that the instance of the object will be created individually for each request and will not be stored in the container.

* **RegisterScoped (under development)** - registers an object specifically for the current scope.

* **Register\<TImplementation\>()** - starts a fluent registration for a concrete type. The type itself is always registered as a service; chain `.ToInterface<TInterface>()` to expose the same instance under additional service types, and finish with `.AsSingleton()`, `.AsTransient()` or `.AsScoped()` to commit the registration and pick the lifetime.

* **OnCreate(callback)** - chainable off of any registration (fluent or classic). Runs the callback with the resolved instance right after it's created (once per instance, not on every resolve), for post-construction initialization without touching the constructor.

Written thanks to the tutorial: https://www.youtube.com/watch?v=dCkYP03lXOs&t=1919s
