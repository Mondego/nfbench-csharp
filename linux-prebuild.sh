#!/bin/bash

# Initialize deeptest submodule --- in the future this will be handled via nuget
git submodule init && git submodule update

# Run build script with build argument (excludes tests)
(cd lib/deeptest && ./build.sh -ScriptArgs "--target=Build")
