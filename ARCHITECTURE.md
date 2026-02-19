# Tizen Timer App - Architecture & Design Document

## System Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      NUI C# User Interface                  │
│  ┌──────────────┬──────────────┬──────────────────────────┐ │
│  │  Main Page   │  Timer List  │  Timer Running Page      │ │
│  │              │     Page     │  ┌──────────────────────┐│ │
│  │  - Create    │              │  │ Timer Completion     ││ │
│  │    Timer     │  - View All  │  │ Popup                ││ │
│  │  - Quick     │  - Start/Stop│  │ ┌──────────────────┐ ││ │
│  │    Access    │  - Delete    │  │ │ Dismiss / Reset  │ ││ │
│  │  - Presets   │  - Progress  │  │ └──────────────────┘ ││ │
│  └──────────────┴──────────────┴──────────────────────────┘ │
│                           ▲                                  │
│                           │ Events                           │
│  ┌────────────────────────┴──────────────────────────────┐   │
│  │        TimerManager Service (C# Wrapper)             │   │
│  │  ┌─────────────────────────────────────────────────┐ │   │
│  │  │  - Create/Delete Timers                         │ │   │
│  │  │  - Start/Pause/Resume/Stop                      │ │   │
│  │  │  - Track elapsed time                           │ │   │
│  │  │  - Manage predefined timers                     │ │   │
│  │  │  - Fire OnTimerComplete events                  │ │   │
│  │  └─────────────────────────────────────────────────┘ │   │
│  └────────────────────────┬──────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                           │
                  ┌────────┴────────┐
                  │                 │
           ┌──────▼──────┐   ┌──────▼────────┐
           │  C++ Timer  │   │  Persistent  │
           │   Service   │   │  Storage     │
           │             │   │              │
           │ - Manage    │   │ - Config     │
           │   timers    │   │ - Presets    │
           │ - Thread    │   │ - States     │
           │   workers   │   │              │
           └─────────────┘   └──────────────┘
```

## Components

### 1. UI Layer (NUI C# - Frontend)

#### MainWindow
- Root window container
- Manages page navigation stack
- Handles back button events
- Coordinates theme and styling

#### MainPage
- Application home screen
- Quick action buttons
- Predefined timer shortcuts
- Navigation to other pages

#### TimerListPage
- Displays all active timers
- Shows progress for each timer
- Allows timer selection
- Provides add new timer button
- Refresh on timer updates

#### TimerRunningPage
- Large display of timer countdown
- Play/Pause/Reset controls
- Progress visualization
- Integration with TimerManager
- Handles timer completion events

#### TimerFinishPage
- Modal popup on timer completion
- Shows completion message
- Provides dismiss and reset options
- Visual celebration indicator

### 2. Service Layer (C# - Business Logic)

#### TimerManager
- Singleton instance
- Manages timer collection (max 5)
- Thread-safe operations using lock
- Implements timer lifecycle:
  - Create → Start → (Pause/Resume) → Complete
- Predefined timer management
- File persistence for presets
- Event system for completion

**Key Methods:**
```csharp
// Timer Management
int CreateTimer(string name, int seconds)
bool StartTimer(int timerId)
bool PauseTimer(int timerId)
bool ResumeTimer(int timerId)
bool StopTimer(int timerId)
bool DeleteTimer(int timerId)

// Queries
Timer GetTimer(int timerId)
List<Timer> GetAllTimers()
bool IsTimerRunning(int timerId)
int GetTimerProgress(int timerId)

// Presets
bool SavePredefinedTimer(string name, int seconds)
List<PredefinedTimer> GetPredefinedTimers()
int CreateTimerFromPredefined(string name)
```

### 3. Data Model

#### Timer Class
```csharp
public class Timer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TotalSeconds { get; set; }
    public int ElapsedSeconds { get; set; }
    public bool IsRunning { get; set; }
    public bool IsPaused { get; set; }
}
```

#### PredefinedTimer Class
```csharp
public class PredefinedTimer
{
    public string Name { get; set; }
    public int Seconds { get; set; }
}
```

### 4. Backend Service (C++ - Native)

#### TimerService Class
- Manages time tracking
- Spawns worker threads for each timer
- Handles system time precision
- Provides callback mechanism
- Logging support

**Features:**
- Thread-per-timer model
- Condition variables for pause/resume
- Atomic operation handling
- Durable state management

## Data Flow

### Timer Creation Flow
```
User clicks "Create Timer"
        ↓
MainPage button clicked
        ↓
TimerRunningPage.SetTimer()
        ↓
TimerManager.CreateTimer()
        ↓
Timer object created in list
        ↓
UI displays timer ready to start
```

### Timer Start Flow
```
User clicks Play
        ↓
TimerRunningPage.TogglePlayPause()
        ↓
TimerManager.StartTimer(id)
        ↓
Background thread created for timer
        ↓
Timer counts down every second
        ↓
UI updates via timer tick
```

### Timer Completion Flow
```
Timer reaches 0 seconds
        ↓
C++ service invokes callback
        ↓
TimerManager fires OnTimerComplete event
        ↓
TimerRunningPage event handler invoked
        ↓
OnTimerComplete?.Invoke(timerId, name)
        ↓
MainWindow navigates to TimerFinishPage
        ↓
Completion popup displayed
```

## Threading Model

### Main Thread
- UI updates
- Event handling
- Page navigation

### Timer Worker Threads
- One thread per active timer
- Background countdown
- Independent of UI thread
- Thread pool managed by .NET

### Synchronization
- `List<Timer>` protected by lock
- `OnTimerComplete` event thread-safe
- MainThread.BeginInvokeOnMainThread for UI updates

## File Structure

### Data Storage
- Location: `~/.config/TimerApp/predefined_timers.txt`
- Format: Plain text (name seconds pairs)
- One timer per line
- Auto-created on first launch

### Configuration Files
- `tizen-manifest.xml` - App manifest
- `TimerUI.csproj` - C# project file
- `TimerService.csproj` - Service library
- `CMakeLists.txt` - C++ build

## State Management

### Timer States
```
CREATED (initial)
    ↓
RUNNING (user pressed Play)
    ↓
PAUSED (user pressed Pause while running)
    ↓
COMPLETED (reached 0 seconds)
    ↓
RESET (user pressed Reset, back to CREATED)
    ↓
DELETED (user deleted timer)
```

### App States
```
SPLASH/INIT
    ↓
MAIN_PAGE
    ↓
TIMER_LIST_PAGE or TIMER_RUNNING_PAGE
    ↓
COMPLETION_POPUP (on timer done)
    ↓
Back to appropriate page
```

## Performance Considerations

### Memory
- Max 5 timers × ~200 bytes = 1KB active timers
- Predefined timers: 4-8 entries
- Total memory footprint: < 1MB

### CPU
- Single-second update interval
- Minimal background processing
- No constant polling
- Event-driven architecture

### Battery
- Only runs when app in foreground
- Minimal CPU usage between ticks
- No lock-screen operation
- System sleep not prevented

## Security

### Data Protection
- Predefined timers stored locally
- No internet connectivity
- No sensitive data

### Permission Model
- Notification privilege (for completion alert)
- App manager privilege (for app launching)

## Error Handling

### Invalid Operations
- Creating 6th timer: Return -1, show message
- Invalid timer ID: Return null/false
- Negative duration: Reject at creation
- Race conditions: Protected by locks

### Recovery
- Graceful degradation
- App continues after error
- No unhandled exceptions
- Proper cleanup on exit

## Future Extensibility

### Potential Enhancements
1. **Sound Notifications**
   - Add audio file loading
   - Play on completion
   - User volume control

2. **Background Service**
   - Service app component
   - Run timers when UI closed
   - System notification

3. **Lock Screen Widget**
   - Quick timer access from lock screen
   - Show active timers
   - Quick control buttons

4. **Custom Schedules**
   - Interval timers
   - Recurring timers
   - Timer sequences

5. **Statistics**
   - Timer history
   - Usage analytics
   - Most used presets

## Testing Strategy

### Unit Tests (Code Level)
- TimerManager methods
- State transitions
- Edge cases

### Integration Tests (Component Level)
- UI → Service communication
- Multi-timer coordination
- Completion event flow

### System Tests (End-to-End)
- Full user workflows
- Performance benchmarks
- Battery impact analysis

### Manual QA
- UI responsiveness
- Visual polish
- User experience

## Deployment

### Pre-Release Checklist
- [ ] All tests passing
- [ ] Code review complete
- [ ] Performance profiled
- [ ] Documentation updated
- [ ] Manifest version bumped
- [ ] Release notes prepared

### Distribution
- Tizen Store submission
- Version: 1.0.0
- Minimum SDK: Tizen 9.0
- Target: All Tizen devices

---

**Document Version**: 1.0
**Last Updated**: February 18, 2026
**Maintainer**: Development Team
