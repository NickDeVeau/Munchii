# Munchii

## Overview

Munchii is a Xamarin-based mobile application designed to help groups decide on a place to eat. The application uses Firebase for backend services and is available on both iOS and Android platforms.

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Usage](#usage)
- [File Structure](#file-structure)
- [Contributing](#contributing)
- [License](#license)

## Installation

To install the project, you'll need to have Xamarin and Firebase set up. Clone the repository and open it in your preferred IDE.

```bash
git clone https://github.com/NickDeVeau/Munchii.git
```

## Usage

Follow the installation steps and run the application on your emulator or physical device.

## File Structure

### FireBase Functions

#### `index.js`
- This JavaScript file contains Firebase Cloud Functions that handle backend logic.

### Munchii.iOS

#### `AppDelegate.cs`
- The entry point for the iOS application. Initializes the app and sets the root view controller.

#### `Main.cs`
- The main class for the iOS application. It's where the app starts its lifecycle.

### Munchii

#### `App.xaml.cs`
- The main application file that initializes Xamarin Forms.

#### `Models/Place.cs`
- A model representing a place or restaurant.

#### `Views/ClientLobbyPage.xaml.cs`
- The client lobby page where users wait for the host to start the session.

#### `Views/DealBreakerPage.xaml.cs`
- A page where users can set their deal-breakers for choosing a restaurant.

#### `Views/HostPage.xaml.cs`
- The host page where the host can manage the session.

#### `Views/JoinPage.xaml.cs`
- A page where users can join an existing session.

#### `Views/MainPage.xaml.cs`
- The main landing page of the application.

#### `Views/PostQuizWaitingPage.xaml.cs`
- A waiting page displayed after the quiz is completed.

#### `Views/RankRestaurantsPage.xaml.cs`
- A page where users can rank the suggested restaurants.

#### `Views/ResultPage.xaml.cs`
- The final result page displaying the chosen restaurant.

### `README.md`
- This file provides an overview and documentation for the project.

## Contributing

If you'd like to contribute, please fork the repository and create a pull request.

## License

This project is licensed under the MIT License.