# PrismLog
Log console with channels


![image](https://github.com/user-attachments/assets/7a109f95-9f01-4df3-a725-21a203e9524e)



## Authors
- [@Kévin "Kebab" Reilhac](https://www.github.com/KevinReilhac)


## Installation

Install PrismLog with Unity Package Manager
![prismLog](https://github.com/user-attachments/assets/86d57b2d-93cb-4b94-bdd6-15763d17bbe6)
```bash
  https://github.com/KevinReilhac/PrismLog.git#upm
```

## Initalization

- Setup your channels in your project settings and generate your enum.

![image](https://github.com/user-attachments/assets/21ac0ab1-9600-4294-b79b-1162d54ca340)

- Open Open Prism Log Console `Shift + Alt + C`

![image](https://github.com/user-attachments/assets/9c77e476-0e06-4785-bbdc-52a4deb6b2e0)

## Usage
```csharp
PConsole.Log("Default Log"); //Print a log in the default channel
PConsole.LogWarning("UI LogWarning", PrismLogChanel.UI); //Print a warning in the UI channel
PConsole.LogError("AI LogError", PrismLogChanel.AI); //Print an error in the AI channel

//You can also use a logtype as third parameter
PConsole.Log("Default Log", PrismLogChanel.PlayerController, LogType.Warning); //Print a warning in the PlayerController channel
```

## Documentation

[Read Documentation](https://kevinreilhac.github.io/PrismLog/)
