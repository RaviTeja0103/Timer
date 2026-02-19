#!/bin/bash

# Tizen Timer App - Project Validation Script
# Validates project structure and files

echo "=================================="
echo "Tizen Timer App - Project Validator"
echo "=================================="
echo ""

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REQUIRED_FILES=(
    "README.md"
    "ARCHITECTURE.md"
    "TEST_PLAN.md"
    "tizen-manifest.xml"
    "CMakeLists.txt"
    "build.sh"
    "TimerApp.sln"
    "TimerService/CMakeLists.txt"
    "TimerService/TimerService.csproj"
    "TimerService/inc/timer_service.h"
    "TimerService/src/timer_service.cpp"
    "TimerService/src/TimerManager.cs"
    "TimerService/src/AssemblyInfo.cs"
    "TimerUI/TimerUI.csproj"
    "TimerUI/src/Program.cs"
    "TimerUI/src/MainWindow.cs"
    "TimerUI/src/MainPage.cs"
    "TimerUI/src/TimerListPage.cs"
    "TimerUI/src/TimerRunningPage.cs"
    "TimerUI/src/TimerFinishPage.cs"
    "TimerUI/src/AssemblyInfo.cs"
)

echo "Checking required files..."
echo ""

MISSING_FILES=0
for file in "${REQUIRED_FILES[@]}"; do
    if [ -f "$PROJECT_DIR/$file" ]; then
        echo "✓ $file"
    else
        echo "✗ MISSING: $file"
        ((MISSING_FILES++))
    fi
done

echo ""
echo "=================================="
if [ $MISSING_FILES -eq 0 ]; then
    echo "✓ All required files present!"
else
    echo "✗ $MISSING_FILES file(s) missing"
    exit 1
fi

echo ""
echo "Checking file sizes..."
echo ""

# Check C# line counts
echo "C# Files:"
find "$PROJECT_DIR" -name "*.cs" ! -path "*build*" -exec wc -l {} + | tail -1

# Check C++ line counts
echo ""
echo "C++ Files:"
find "$PROJECT_DIR" -name "*.cpp" -o -name "*.h" | xargs wc -l 2>/dev/null | tail -1

# Check documentation
echo ""
echo "Documentation Files:"
find "$PROJECT_DIR" -name "*.md" -exec wc -l {} +

echo ""
echo "=================================="
echo "Project Statistics"
echo "=================================="

echo ""
echo "Total Project Files:"
find "$PROJECT_DIR" -type f ! -path "*/.git/*" ! -path "*/build/*" ! -path "*/.*" | wc -l

echo ""
echo "Key Components:"
echo "- C++ Service: timer_service.cpp (~250 lines)"
echo "- C# Service Wrapper: TimerManager.cs (~300 lines)"
echo "- NUI UI Pages: 4 pages (~1000+ lines total)"
echo "- Configuration: CMakeLists.txt, .csproj files"
echo "- Documentation: README, ARCHITECTURE, TEST_PLAN"

echo ""
echo "=================================="
echo "Validation Complete!"
echo "=================================="
echo ""
echo "Next Steps:"
echo "1. Review ARCHITECTURE.md for system design"
echo "2. Review README.md for build instructions"
echo "3. Review TEST_PLAN.md for testing procedures"
echo "4. Run: ./build.sh (requires Tizen SDK)"
echo "5. Deploy to Tizen device"
