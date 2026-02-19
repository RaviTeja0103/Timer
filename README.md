# Tizen Timer App

A multi-timer application for Tizen devices with C++ service backend and NUI C# UI frontend.

## Features

1. **Basic Timer Functionality** - Create and manage timers with custom durations
2. **Multi-Timer Support** - Run up to 5 timers in parallel simultaneously
3. **Pre-defined Timers** - Save and manage pre-defined timer templates (Quick Timer, 5 Min, 10 Min, 30 Min)
4. **Timer Controls** - Play/Pause, Reset, and Dismiss operations
5. **Completion Notifications** - Popup notification when timer completes with dismiss and reset options
6. **Persistent Storage** - Save pre-defined timers to disk

## Project Structure

```
TimerApp/
├── TimerService/           # C++ Service Component
│   ├── inc/
│   │   └── timer_service.h
│   ├── src/
│   │   ├── timer_service.cpp
│   │   ├── TimerManager.cs (C# Wrapper)
│   │   └── AssemblyInfo.cs
│   ├── CMakeLists.txt
│   └── TimerService.csproj
├── TimerUI/                # NUI C# UI Component
│   ├── src/
│   │   ├── Program.cs
│   │   ├── MainWindow.cs
│   │   ├── MainPage.cs
│   │   ├── TimerListPage.cs
│   │   ├── TimerRunningPage.cs
│   │   ├── TimerFinishPage.cs
│   │   └── AssemblyInfo.cs
│   ├── res/
│   └── TimerUI.csproj
├── CMakeLists.txt
├── tizen-manifest.xml
├── TimerApp.sln
└── README.md
```

## Build Instructions

### Prerequisites

- Tizen Studio IDE installed
- Tizen SDK 9.0 or newer
- .NET SDK for Tizen (Tizen.NET)

### Building the Project

1. **Open the project in Tizen Studio:**
   ```bash
   cd /Users/ritadas/Desktop/Ravi/TimerApp
   ```

2. **Build using Tizen Studio:**
   - File → Open Project from File System
   - Select the TimerApp directory
   - Build → Clean Project
   - Build → Build Project

3. **Or build from command line:**
   ```bash
   tizen build-native -a x86 -c Release
   tizen build-web
   ```

## Project Features Breakdown

### Main Page (`MainPage.cs`)
- Welcome screen with app title
- "Create New Timer" button
- "My Timers" button to view active timers
- Quick access to pre-defined timers
  - Quick Timer (60 seconds)
  - 5 Minutes (300 seconds)
  - 10 Minutes (600 seconds)
  - 30 Minutes (1800 seconds)

### Timer List Page (`TimerListPage.cs`)
- Shows all active timers with their status
- Progress bars for each timer
- Time remaining display
- Add new timer button
- Click to open timer details

### Timer Running Page (`TimerRunningPage.cs`)
- Large timer display (HH:MM:SS format)
- Play/Pause button for timer control
- Reset button to restart timer
- Dismiss button to close timer
- Visual progress indicator
- Integrates with TimerManager service

### Timer Finish Page (`TimerFinishPage.cs`)
- Completion popup with visual indicator
- Timer name and completion message
- "Dismiss" button to go back to list
- "Reset" button to restart timer
- Semi-transparent overlay

### TimerManager Service (`TimerManager.cs`)
- Singleton service managing all timers
- Create, start, pause, resume, stop timers
- Retrieve timer status and progress
- Manage pre-defined timers
- Persistent storage for pre-defined timers
- Event callbacks for timer completion

## Testing Instructions

### Unit Testing (Manual)

1. **Test Timer Creation:**
   - Launch app
   - Click "Create New Timer"
   - Verify timer UI displays correctly

2. **Test Multiple Timers:**
   - Go to "My Timers"
   - Create 5 different timers
   - Verify maximum limit (cannot create 6th timer)
   - Start multiple timers simultaneously

3. **Test Timer Controls:**
   - Create a 60-second timer
   - Click Play and verify timer counts down
   - Click Pause and verify timer stops
   - Click Play again to resume
   - Click Reset to restart timer

4. **Test Pre-defined Timers:**
   - On main page, click one of the pre-defined timer buttons
   - Verify timer is created with correct duration
   - Complete the timer

5. **Test Timer Completion:**
   - Create a short timer (5-10 seconds)
   - Start the timer
   - Wait for completion
   - Verify popup appears
   - Test Dismiss and Reset buttons

6. **Test Timer Persistence:**
   - Create and start multiple timers
   - Exit application
   - Relaunch application
   - Verify pre-defined timers are available

### Integration Testing

1. **Parallel Timers:**
   - Create 3 timers with different durations
   - Start all timers
   - Verify they count down independently
   - Complete one timer while others run

2. **UI Responsiveness:**
   - Navigate between pages while timers run
   - Verify timers continue to count
   - Check that UI updates correctly

3. **Error Handling:**
   - Try to create more than 5 timers
   - Verify appropriate error/limitation message
   - Try to resume non-existent timer

## Debugging

### Common Issues and Solutions

1. **Timer not starting:**
   - Check TimerManager singleton initialization
   - Verify OnTimerComplete event is registered
   - Check system time for correct countdown

2. **UI not updating:**
   - Verify MainThread.BeginInvokeOnMainThread is used for UI updates
   - Check timer tick interval is set correctly

3. **Timers not persisting:**
   - Verify data directory exists
   - Check file permissions in app data folder
   - Review predefined_timers.txt content

4. **Maximum timers not enforced:**
   - Check MAX_TIMERS constant in TimerManager.cs
   - Verify timer creation returns -1 when limit reached

## API Documentation

### TimerManager Class

#### Methods

- `CreateTimer(string name, int seconds)` - Create new timer, returns timer ID
- `StartTimer(int timerId)` - Start running a timer
- `PauseTimer(int timerId)` - Pause a running timer
- `ResumeTimer(int timerId)` - Resume paused timer
- `StopTimer(int timerId)` - Stop and reset timer
- `DeleteTimer(int timerId)` - Delete timer completely
- `GetTimer(int timerId)` - Get timer object
- `GetAllTimers()` - Get list of all timers
- `IsTimerRunning(int timerId)` - Check if timer is running
- `GetTimerProgress(int timerId)` - Get progress 0-100
- `SavePredefinedTimer(string name, int seconds)` - Save template
- `DeletePredefinedTimer(string name)` - Delete template
- `GetPredefinedTimers()` - Get all templates
- `CreateTimerFromPredefined(string name)` - Create timer from template

#### Events

- `OnTimerComplete` - Fired when timer reaches zero

## Known Limitations

1. Maximum 5 simultaneous timers
2. Timer precision is limited to 1-second accuracy
3. Timers only run while application is in foreground
4. No sound notifications (only visual popup)
5. No background execution of timers

## Future Enhancements

1. Add sound and vibration notifications
2. Implement background timer service
3. Add timer categories/groups
4. Implement timer history/statistics
5. Add custom timer templates UI
6. Add lock screen timer widget
7. Implement timer synchronization with system clock
8. Add multiple notification options

## Building Release Package

```bash
# Create release build
tizen build-native -a x86 -c Release
tizen package --type tpk -o ./bin/release/

# Install on device
tizen install -n TimerApp-1.0.0-x86.tpk -s device_serial
```

## Support and Contact

For issues or questions, please refer to Tizen SDK documentation or file an issue in the project repository.

---

**Last Updated:** February 18, 2026
**Version:** 1.0.0
