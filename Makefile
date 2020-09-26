BUTLER=/Users/blinks/Library/Application\ Support/itch/apps/butler/butler
UNITY=/Applications/Unity/Hub/Editor/2020.1.6f1/Unity.app/Contents/MacOS/Unity

all : win

# Build Windows channel to "Build/windows/under"
win : Build/windows/under/Explorer.exe
	${BUTLER} push Build/windows/under blinks/under:windows-alpha

# TODO: Build Mac channel to "Build/mac/under"

# TODO: Build HTML channel to "Build/html/under"