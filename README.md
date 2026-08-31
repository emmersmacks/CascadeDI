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

Every registration method accepts an optional `onCreate` callback, invoked with the created instance as soon as it is resolved for the first time (once per Singleton/Scoped instance, every time for Transient):

```csharp
container.RegisterSingleton<IService, Service>(instance => instance.Init());
container.RegisterTransientFor<Logger>(logger => logger.SetContext("startup"), typeof(ILogger));
container.RegisterSingleton<IConfig>(existingConfigInstance, config => config.Reload());
```

## API
### Methods
* **RegisterSingleton** - registers an object as a Singleton. This means that the instance of the object will be shared across the entire application.

* **RegisterTransient** - registers an object as a transient object. This means that the instance of the object will be created individually for each request and will not be stored in the container.

* **RegisterScoped (under development)** - registers an object specifically for the current scope.

* **onCreate** - optional callback parameter on any Register* method, invoked with the newly created/resolved instance right after it comes into being (on first Resolve).

Written thanks to the tutorial: https://www.youtube.com/watch?v=dCkYP03lXOs&t=1919s
