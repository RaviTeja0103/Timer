#!/bin/bash

# Tizen Timer App Build Script
# This script builds the Tizen Timer app

set -e

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BUILD_DIR="$PROJECT_DIR/build"
RELEASE_DIR="$PROJECT_DIR/release"

echo "==================================="
echo "Tizen Timer App Build Script"
echo "==================================="
echo "Project Directory: $PROJECT_DIR"

# Create build directory
mkdir -p "$BUILD_DIR"
mkdir -p "$RELEASE_DIR"

echo ""
echo "Step 1: Cleaning previous builds..."
if [ -d "$BUILD_DIR" ]; then
    rm -rf "$BUILD_DIR"/*
fi

echo ""
echo "Step 2: Building C++ Service Component..."
cd "$PROJECT_DIR/TimerService"

# Check if CMakeLists.txt exists
if [ ! -f "CMakeLists.txt" ]; then
    echo "Error: CMakeLists.txt not found in TimerService"
    exit 1
fi

mkdir -p build
cd build
cmake ..
make

echo "C++ Service built successfully!"

echo ""
echo "Step 3: Building C# UI Component..."
cd "$PROJECT_DIR/TimerUI"

# Using Tizen Studio build or dotnet
if command -v dotnet &> /dev/null; then
    echo "Using .NET CLI to build..."
    dotnet build -c Release
else
    echo "Note: dotnet CLI not found. Please build using Tizen Studio IDE."
    echo "File > Open Project > Select TimerApp directory"
fi

echo ""
echo "==================================="
echo "Build Complete!"
echo "==================================="
echo ""
echo "Next Steps:"
echo "1. Package the application with Tizen Studio"
echo "2. Deploy to a Tizen device using 'tizen install' command"
echo "3. Run the application: Launch TimerApp from device home screen"
