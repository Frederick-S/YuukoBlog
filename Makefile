PROJECT_NAME = YuukoBlog
PROJECT_FILE = ./$(PROJECT_NAME)/$(PROJECT_NAME).csproj
TEST_PROJECT_FILE = ./$(PROJECT_NAME).Tests/$(PROJECT_NAME).Tests.csproj

build:
	dotnet build $(PROJECT_FILE)
	dotnet build $(TEST_PROJECT_FILE)

test:
	dotnet test $(TEST_PROJECT_FILE)

clean:
	dotnet clean $(PROJECT_FILE)
	dotnet clean $(TEST_PROJECT_FILE)