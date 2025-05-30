*Hopefully nobody reads READMEs*

# WPF Countdown Timer
A WPF application demonstrating a countdown timer with cancellation support.

## Overview
This project is a WPF desktop application that simulates a long-running operation (10 seconds), provides an animated countdown, and allows the user to cancel the operation mid-way. 

## Features
- **Run a complex operation**: Starts a 10-second simulated operation in the background.
- **Animated Countdown**: A popup dialog displays a countdown timer (10 to 1) while the operation is running.
- **Cancel Support**: Users can cancel the timer, pausing the countdown and prompting for confirmation. 
	- **Note**: Pausing the timer does not pause the complex operation thread. If the timer is running even if the complex operation thread has exited, a warning is displayed
- **Abort or Confirm Cancellation**: 
  - *Abort Cancellation*: Resumes the timer without cancelling the 'Complex Operation' task. 
  - *Confirm Cancellation*: Stops the timer immediately after cancelling the 'Complex Operation' task.
	- **Note**: if cancellation is confirmed, the complex operation thread is **killed** if it is still running

## Requirements
- .NET 8.0 (or compatible version)
- Visual Studio 2022 or newer
- MVVM Framework: CommunityToolkit.Mvvm (didn't really make use of it)
- SeriLog 
- SeriLog.Console

## Miscellaneous
- Output Type of application changed from 'Windows' to 'Console' so that a console window showing logging is displayed.

## References
- PluralSight Videos on WPF for a quick refresher 
- ChatGPT 4.1 Model for an occasional refresher on topics related to event handling and threading and SeriLog Logging
