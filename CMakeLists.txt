cmake_minimum_required(VERSION 3.0)
project(TimerApp CXX)

# Add subdirectories
add_subdirectory(TimerService)

# Set up the main CMake configuration
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Installation of manifest
install(FILES tizen-manifest.xml DESTINATION .)
