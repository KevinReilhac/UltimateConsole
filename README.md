# PrismLog
LogConsole with chanels

[//]: # (TODO: Add a gif of the console)


## Authors
- [@Kévin "Kebab" Reilhac](https://www.github.com/KevinReilhac)


## Installation

Install PrismLog with Unity Package Manager

[//]: # (TODO: Replace install gif)
![pkgmngr](https://github.com/user-attachments/assets/7165bb65-8738-4d3c-9c2a-fb31acae2e56)


```bash
  https://github.com/KevinReilhac/PrismLog.git#upm
```

## Initalization

- Setup your chanels in your project settings.
[//]: # (TODO: Add a gif of the chanels)

- Generate your enum
[//]: # (TODO: Add a generate enum gif)

- Open Open Prism Log Console `Alt + Shift + C`
[//]: # (TODO: Add a open console gif)


## Usage
```csharp
PConsole.Log("Default Log"); //Print a log in the default chanel
PConsole.LogWarning("UI LogWarning", PrismLogChanel.UI); //Print a warning in the UI chanel
PConsole.LogError("AI LogError", PrismLogChanel.AI); //Print an error in the AI chanel

//You can also use a logtype as thrid parameter
PConsole.Log("Default Log", PrismLogChanel.PlayerController, LogType.Warning); //Print a warning in the PlayerController chanel
```

## Documentation

[Read Documentation](https://kevinreilhac.github.io/PrismLog/)
