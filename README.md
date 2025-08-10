# 🐍 Unity Snake Game - Professional Architecture Implementation

A modern Unity implementation of the classic Snake game showcasing **enterprise-level architecture patterns**, **clean code principles**, and **professional game development practices**.

## 🌟 Key Features

### 🏗️ **Professional Architecture**
- **Clean Architecture** with separation of concerns
- **Dependency Injection** using Zenject
- **Reactive Programming** with UniRx
- **Event-Driven Architecture** for decoupled systems
- **MVC Pattern** implementation for game entities

### 🎮 **Game Features**
- Classic Snake gameplay mechanics
- Cross-platform input support (PC/Mobile)
- Dynamic difficulty through remote configuration
- High score persistence system
- Professional UI/UX with game states management

### ☁️ **Cloud Integration**
- **Firebase Remote Config** for live parameter updates
- A/B testing capabilities
- Real-time configuration without app updates
- Fallback mechanisms for offline scenarios

### 🔊 **Advanced Systems**
- **Modular Sound System** with SFX and music channels
- **Scene Management** with Addressable Assets
- **Memory Management** with automatic disposable cleanup
- **Persistence System** with JSON serialization

## 🛠️ Technologies & Frameworks

| Technology | Purpose | Version |
|------------|---------|---------|
| **Unity** | Game Engine | 2022.3.16f1 |
| **Zenject** | Dependency Injection | Latest |
| **UniRx** | Reactive Programming | Latest |
| **UniTask** | Async Operations | Latest |
| **Addressables** | Asset Management | Unity Built-in |
| **Firebase** | Remote Config | Latest |

## 📁 Project Structure

```
Assets/
├── _Scripts/
│   ├── Entities/          # Game entities (Snake, Food, Score)
│   │   ├── Snake/
│   │   │   ├── Model/     # Data and business logic
│   │   │   ├── View/      # Visual representation
│   │   │   ├── Controller/ # Input handling and coordination
│   │   │   ├── Config/    # ScriptableObject configurations
│   │   │   └── Factory/   # Object creation patterns
│   │   ├── Food/          # Food system implementation
│   │   ├── Score/         # Scoring system
│   │   └── GameOverAndPause/ # Game state management
│   ├── Services/          # Core application services
│   │   ├── EventBus/      # Event system implementation
│   │   ├── SoundSystem/   # Audio management
│   │   ├── SceneFlowManagementSystem/ # Scene transitions
│   │   ├── Persistence/   # Save/load system
│   │   ├── RemoteConfig/  # Firebase integration
│   │   └── InputSystem/   # Cross-platform input
│   ├── GlobalConfigs/     # Application-wide configurations
│   ├── Events/            # Event definitions
│   ├── Enums/            # Type-safe enumerations
│   └── HelperClasses/    # Utility classes
└── Installers/           # Zenject dependency injection setup
```


## 🚀 Getting Started

### Prerequisites
- Unity 2022.3.16f1 or later
- .NET Framework 4.7.1 support
- Firebase project (optional, for remote config)

### Installation

1. **Clone the repository**

2. **Open in Unity**
   - Launch Unity Hub
   - Add project from disk
   - Select the cloned repository folder

3. **Install Dependencies**
   - Dependencies are managed through Unity Package Manager
   - All required packages should auto-resolve

4. **Firebase Setup (Optional)**
   - Create a Firebase project
   - Download `google-services.json`
   - Place in `Assets/StreamingAssets/`

### Quick Start

1. Open the **Bootstrap** scene
2. Press Play in Unity Editor
3. Game will automatically load the main menu
4. Use arrow keys (PC) or touch controls (mobile) to play

## 🏛️ Architecture Highlights

### Dependency Injection Pattern
```csharp
// Automatic service registration and injection
Container.BindInterfacesTo<SoundManager>().AsSingle();
Container.BindInterfacesTo<SceneFlowManager>().AsSingle();
```


### Event-Driven Communication
```csharp
// Decoupled event handling
_eventBus.OnEvent<PlaySfxEvent>()
    .Subscribe(e => PlaySfx(e.ClipName))
    .AddTo(_disposables);
```


### Memory Management
```csharp
// Automatic subscription cleanup
private readonly CompositeDisposable _disposables = new();

public void Dispose() => _disposables.Dispose();
```


### Remote Configuration
```csharp
// Live parameter updates without app deployment
var remoteValue = FirebaseRemoteConfig.DefaultInstance.GetValue("gameSpeed");
```


## 🎯 Design Patterns Implemented

- **🏗️ Dependency Injection** - Loose coupling and testability
- **🔄 Observer Pattern** - Event-driven architecture
- **🏭 Factory Pattern** - Object creation abstraction
- **📦 Repository Pattern** - Data access abstraction
- **🎭 MVC Pattern** - Clear separation of concerns
- **🧹 Disposable Pattern** - Automatic resource cleanup
- **⚙️ Service Layer Pattern** - Business logic encapsulation

## 🔧 Configuration Management

### ScriptableObject Configurations
- **Game Config** - Core gameplay parameters
- **Sound Config** - Audio clip mappings
- **Scene Config** - Scene reference management
- **Entity Configs** - Individual component settings

### Remote Configuration
- Real-time parameter updates via Firebase
- A/B testing for game balancing
- Environment-specific configurations
- Graceful fallback to local settings

## 🎮 Cross-Platform Support

### Input System
```csharp
#if UNITY_EDITOR || UNITY_STANDALONE
    Container.BindInterfacesTo<PCInput>().AsSingle();
#elif UNITY_ANDROID || UNITY_IOS
    Container.BindInterfacesTo<MobileTouchInput>().AsSingle();
#endif
```


### Supported Platforms
- ✅ Windows (Standalone)
- ✅ macOS (Standalone)
- ✅ Android
- ✅ iOS
- ✅ Unity Editor

## 📊 Performance Features

- **Memory Leak Prevention** with automatic disposable management
- **Efficient Asset Loading** using Addressable system
- **Object Pooling** for frequently created objects
- **Optimized Event System** with reactive programming
- **Resource Cleanup** on scene transitions

## 🧪 Testing Architecture

The project is designed with testability in mind:
- **Interface-based design** for easy mocking
- **Dependency injection** for isolated unit testing
- **Separation of concerns** for focused testing
- **Event-driven architecture** for integration testing

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow the established architecture patterns
- Maintain separation of concerns
- Include proper disposable cleanup
- Document complex systems
- Write meaningful commit messages

## 🙏 Acknowledgments

- **Zenject** for dependency injection framework
- **UniRx** for reactive programming support
- **UniTask** for advanced async operations
- **Firebase** for cloud configuration services
- **Unity Technologies** for the incredible game engine

## 📈 Project Stats

- **Lines of Code**: ~2000+
- **Architecture Patterns**: 7+
- **Services Implemented**: 6+
- **Entities**: 4 (Snake, Food, Score, GameState)
- **Cross-Platform Compatibility**: 5 platforms
- **Memory Leak Prevention**: ✅ Complete

---

*This project serves as a demonstration of professional Unity development practices and can be used as a reference for commercial game development projects.*

**⭐ If you find this project useful, please consider giving it a star!**
