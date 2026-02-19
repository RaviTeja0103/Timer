cmake_minimum_required(VERSION 3.0)
project(TimerService CXX)

# Set C++ standard
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Find required packages
pkg_check_modules(DLOG QUIET dlog)

# Add library
add_library(${PROJECT_NAME} SHARED
    src/timer_service.cpp
)

# Include directories
target_include_directories(${PROJECT_NAME} PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}/inc
)

# Link libraries
if(DLOG_FOUND)
    target_link_libraries(${PROJECT_NAME} PUBLIC ${DLOG_LIBRARIES})
    target_compile_definitions(${PROJECT_NAME} PUBLIC DLOG_ENABLED)
endif()

# Installation
install(TARGETS ${PROJECT_NAME} LIBRARY DESTINATION lib)
install(FILES inc/timer_service.h DESTINATION include)
