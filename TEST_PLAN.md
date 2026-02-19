# Comprehensive Test Plan for Tizen Timer App

## Test Summary
- **Application**: Tizen Timer App
- **Version**: 1.0.0
- **Test Date**: February 18, 2026
- **Test Level**: Integration & System Testing

## Test Scenarios

### 1. APPLICATION LAUNCH & INITIALIZATION

#### Test Case 1.1: App Launch
**Steps:**
1. Launch TimerApp from device home screen
2. Observe initial screen

**Expected Result:**
- Main page displays with "Timer App" title
- "Create New Timer" and "My Timers" buttons visible
- Predefined timers list displayed
- App is responsive

---

### 2. TIMER CREATION

#### Test Case 2.1: Create Timer from Main Page
**Steps:**
1. From main page, click "Create New Timer"
2. Timer UI page opens

**Expected Result:**
- Timer display shows "00:00:00"
- Play button is visible and active
- Timer name is displayed

#### Test Case 2.2: Create Timer from Predefined
**Steps:**
1. From main page, click any predefined timer button
2. Timer page opens with selected timer

**Expected Result:**
- Timer created with correct duration
- Timer display shows full duration
- Timer is ready to start

#### Test Case 2.3: Create Maximum Timers (5)
**Steps:**
1. Create 5 timers one by one
2. Attempt to create 6th timer

**Expected Result:**
- First 5 timers created successfully
- 6th timer creation fails with appropriate message
- Maximum limit of 5 is enforced

---

### 3. TIMER OPERATIONS

#### Test Case 3.1: Start Timer
**Steps:**
1. Open a timer
2. Click Play button
3. Observe countdown

**Expected Result:**
- Timer starts counting down
- Display updates every second
- Play button changes to Pause

#### Test Case 3.2: Pause Timer
**Steps:**
1. Start a running timer
2. Click Pause button

**Expected Result:**
- Timer stops counting
- Display remains at current time
- Pause button changes back to Play

#### Test Case 3.3: Resume Timer
**Steps:**
1. Pause a running timer
2. Click Play button

**Expected Result:**
- Timer resumes from paused time
- Countdown continues
- Button shows Pause again

#### Test Case 3.4: Reset Timer
**Steps:**
1. Start a timer and let it run for a few seconds
2. Click Reset button

**Expected Result:**
- Timer resets to original duration
- Countdown stops
- Display shows full time
- Play button is ready

#### Test Case 3.5: Dismiss Timer
**Steps:**
1. Open a running timer
2. Click Dismiss button
3. Navigate to timer list

**Expected Result:**
- Timer stops and closes
- Returns to timer list page
- Timer still exists in list if not deleted

---

### 4. COMPLETION BEHAVIOR

#### Test Case 4.1: Timer Completes Successfully
**Steps:**
1. Create a 10-second timer
2. Start the timer
3. Wait for completion (monitor countdown)

**Expected Result:**
- Timer counts down to 0
- Completion popup appears
- Popup shows timer name and completion message
- Visual indicator (checkmark) displayed

#### Test Case 4.2: Dismiss on Completion
**Steps:**
1. Timer completes and popup shown
2. Click "Dismiss" button on popup

**Expected Result:**
- Popup closes
- Returns to timer list
- Timer status updated

#### Test Case 4.3: Reset from Completion
**Steps:**
1. Timer completes and popup shown
2. Click "Reset" button on popup

**Expected Result:**
- Popup closes
- Timer running page opens with same timer
- Timer resets to original duration
- Ready to start again

---

### 5. MULTIPLE TIMERS

#### Test Case 5.1: Run Multiple Timers Simultaneously
**Steps:**
1. Create 3 timers with different durations (60s, 120s, 180s)
2. Start all timers from their respective pages
3. Navigate to timer list to see all running

**Expected Result:**
- All timers count down independently
- Each timer maintains its own time
- Progress bars update correctly for each
- No interference between timers

#### Test Case 5.2: Complete Multiple Timers
**Steps:**
1. Have 3 running timers with different durations
2. Wait for first timer to complete
3. Handle completion popup
4. Check remaining timers

**Expected Result:**
- First timer completes and shows popup
- Other timers continue running
- Can dismiss/reset without affecting others
- Multiple completion events handled correctly

---

### 6. TIMER LIST MANAGEMENT

#### Test Case 6.1: View Timer List
**Steps:**
1. Create 3 timers
2. Start 2 of them
3. Click "My Timers" button

**Expected Result:**
- Timer list page shows all 3 timers
- Shows timer name, duration, elapsed time
- Progress bars show correct progress
- Stopped timers show 0 progress

#### Test Case 6.2: Select Timer from List
**Steps:**
1. From timer list, click on a timer

**Expected Result:**
- Timer running page opens
- Timer details loaded correctly
- Current time and duration displayed
- Can resume play

#### Test Case 6.3: Add New Timer from List
**Steps:**
1. View timer list
2. Click "+ Add New Timer" button

**Expected Result:**
- New timer created with default duration
- Timer running page opens
- Ready to configure and start

---

### 7. PREDEFINED TIMERS

#### Test Case 7.1: Access Predefined Timers
**Steps:**
1. Default predefined timers available:
   - Quick Timer (60s)
   - 5 Minutes (300s)
   - 10 Minutes (600s)
   - 30 Minutes (1800s)

**Expected Result:**
- All 4 default timers visible on main page
- Can click any to create a timer
- Correct durations applied

#### Test Case 7.2: Save Custom Predefined Timer
**Steps:**
1. Create a timer with custom duration
2. Save as predefined (if such option available)

**Expected Result:**
- Timer saved to predefined list
- Available for future use
- Persists after app restart

---

### 8. DATA PERSISTENCE

#### Test Case 8.1: Persist Predefined Timers
**Steps:**
1. Launch app
2. Verify predefined timers loaded
3. Close app
4. Relaunch app

**Expected Result:**
- Same predefined timers available
- Custom predefined timers preserved
- Data file intact

#### Test Case 8.2: Active Timers on App Close
**Steps:**
1. Create and start a timer
2. Close app
3. Relaunch app

**Expected Result:**
- Timer activity not persisted (timers reset)
- App can be restarted cleanly
- No corrupted timer data

---

### 9. UI/UX TESTING

#### Test Case 9.1: Navigation Flow
**Steps:**
1. Test forward navigation:
   - Main → Create Timer → Running → List
   - List → Timer → Running
2. Test back navigation:
   - Each page should have back button
   - Back button should return to previous page

**Expected Result:**
- Smooth navigation between pages
- Back buttons functional
- No broken navigation paths
- Header navigation clear

#### Test Case 9.2: Responsive Layout
**Steps:**
1. Launch app on various screen sizes (if available)
2. Check element positioning
3. Verify buttons are clickable
4. Check text readability

**Expected Result:**
- UI adapts to screen size
- All buttons accessible
- Text legible
- No overlapping elements

#### Test Case 9.3: Color and Visual Feedback
**Steps:**
1. Click buttons and observe response
2. Check progress bar color changes
3. Verify completion popup appearance
4. Check timer display colors

**Expected Result:**
- Buttons have visual feedback on click
- Progress bar color indicates progress
- Popup clearly visible
- All text readable with good contrast

---

### 10. ERROR HANDLING

#### Test Case 10.1: Invalid Timer Creation
**Steps:**
1. Attempt to create timer with invalid data
2. Exceed maximum timer limit

**Expected Result:**
- Graceful error handling
- Appropriate error messages
- App continues to function
- No crashes or hangs

#### Test Case 10.2: App Robustness
**Steps:**
1. Rapid button clicking
2. Quick navigation between pages
3. Start/stop timers rapidly
4. Try concurrent operations

**Expected Result:**
- App handles rapid input gracefully
- No crashes or freezes
- All operations atomic
- Clean state after rapid operations

---

## Test Results Summary

### Date: ________________
### Tester: ________________
### Device: ________________

| Test Case | Status | Notes |
|-----------|--------|-------|
| 1.1 | ☐ Pass ☐ Fail | |
| 2.1 | ☐ Pass ☐ Fail | |
| 2.2 | ☐ Pass ☐ Fail | |
| 2.3 | ☐ Pass ☐ Fail | |
| 3.1 | ☐ Pass ☐ Fail | |
| 3.2 | ☐ Pass ☐ Fail | |
| 3.3 | ☐ Pass ☐ Fail | |
| 3.4 | ☐ Pass ☐ Fail | |
| 3.5 | ☐ Pass ☐ Fail | |
| 4.1 | ☐ Pass ☐ Fail | |
| 4.2 | ☐ Pass ☐ Fail | |
| 4.3 | ☐ Pass ☐ Fail | |
| 5.1 | ☐ Pass ☐ Fail | |
| 5.2 | ☐ Pass ☐ Fail | |
| 6.1 | ☐ Pass ☐ Fail | |
| 6.2 | ☐ Pass ☐ Fail | |
| 6.3 | ☐ Pass ☐ Fail | |
| 7.1 | ☐ Pass ☐ Fail | |
| 7.2 | ☐ Pass ☐ Fail | |
| 8.1 | ☐ Pass ☐ Fail | |
| 8.2 | ☐ Pass ☐ Fail | |
| 9.1 | ☐ Pass ☐ Fail | |
| 9.2 | ☐ Pass ☐ Fail | |
| 9.3 | ☐ Pass ☐ Fail | |
| 10.1 | ☐ Pass ☐ Fail | |
| 10.2 | ☐ Pass ☐ Fail | |

---

## Bug Report Template

**Bug ID**: ___________
**Severity**: High / Medium / Low
**Status**: Open / In Progress / Fixed / Closed

**Description**: 
[Describe the bug]

**Steps to Reproduce**:
1. [Step 1]
2. [Step 2]
3. [Step 3]

**Expected Result**:
[What should happen]

**Actual Result**:
[What actually happened]

**Screenshots/Logs**:
[Attach relevant logs or screenshots]

**Environment**:
- Device: _____________
- OS Version: _________
- App Version: ________

---

## Regression Testing

After bug fixes, re-test:
- Test Case 3.1 (Start Timer)
- Test Case 4.1 (Timer Completion)
- Test Case 5.1 (Multiple Timers)
- Test Case 10.2 (App Robustness)

---

## Sign Off

Project Manager: ________________ Date: ________
QA Lead: ________________ Date: ________
